using System.Diagnostics;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

public readonly partial struct PathPart {
    [UsedImplicitly]
    internal class Parser : IClericalParser<PathPart, bool> {
        public static bool TryParse_Internal(SpanOrSegment s, bool strict, bool throwOnFailure, out PathPart result) {
            if (strict == false) {
                s = s.Trim();
                if (s is ['/' or '\\', .. var afterSeparator]) {
                    s = afterSeparator;
                }

                if (s is [.. var beforeSeparator, '/' or '\\']) {
                    s = beforeSeparator;
                }
            }

            if (s.Length == 0) {
                result = default;
                return true;
            }

            if (char.IsWhiteSpace(s[0]) || char.IsWhiteSpace(s[^1])) {
                result = default;
                return throwOnFailure switch {
                    true  => throw new FormatException("Cannot start or end with whitespace!"),
                    false => false
                };
            }

            if (s.AsSpan().IndexOfAny(Clerk.DirectorySeparatorChars) >= 0) {
                result = default;
                return throwOnFailure switch {
                    true  => throw new FormatException("Cannot contain directory separators!"),
                    false => false
                };
            }

            result = CreateUnsafe(s.ToStringSegment());
            return true;
        }

        public static PathPart Parse_Internal(SpanOrSegment s, bool strict) {
            var success = TryParse_Internal(s, strict, true, out var result);
            Debug.Assert(success);
            return result;
        }
    }
}