using System.Diagnostics;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

public readonly partial struct PathPart {
    [UsedImplicitly]
    internal class Parser : IClericalParser<PathPart, ClericalStyles, string?> {
        public static string? TryParse_Internal(SpanOrSegment s, ClericalStyles styles, out PathPart result) {
            if (styles.TryConsumeBookendTrimming(ref s) is { } failMsg) {
                result = default;
                return failMsg.GetMessage();
            }

            Debug.Assert((styles & (ClericalStyles.AllowBookendDirectorySeparators | ClericalStyles.AllowBookendWhiteSpace)) == 0, "Should have already handled bookend directory separators or white space!");

            if (s.Length == 0) {
                result = default;
                return (styles & ClericalStyles.AllowEmptyPathParts) switch {
                    0 => "Cannot contain empty path parts!",
                    _ => null
                };
            }

            if (s.AsSpan().IndexOfAny('/', '\\') >= 0) {
                result = default;
                return "Cannot contain directory separators!";
            }

            result = CreateUnsafe(s);
            return null;
        }

        public static PathPart Parse_Internal(SpanOrSegment s, ClericalStyles styles) {
            return TryParse_Internal(s, styles, out var result) switch {
                null    => result,
                var msg => throw new FormatException(msg),
            };
        }

        public static PathPart CreateUnsafe(SpanOrSegment input) {
            return new PathPart(input.ToStringSegment());
        }
    }
}