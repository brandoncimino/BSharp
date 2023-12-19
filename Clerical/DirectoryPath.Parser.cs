using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

public readonly partial record struct DirectoryPath {
    [UsedImplicitly]
    internal class Parser : IClericalParser<DirectoryPath, ClericalStyles, string?> {
        private ref struct PathPartSegmentEnumerator {
            private SpanOrSegment _remaining;
            public  SpanOrSegment Current { get; private set; }

            public PathPartSegmentEnumerator(SpanOrSegment source) {
                _remaining = source;
            }

            public PathPartSegmentEnumerator GetEnumerator() => this;

            public bool MoveNext() {
                if (_remaining.IsEmpty) {
                    return false;
                }

                var next = _remaining.AsSpan().IndexOfAny('/', '\\');
                if (next < 0) {
                    Current    = _remaining;
                    _remaining = default;
                    return true;
                }

                Current    = _remaining[..next];
                _remaining = _remaining[next..];
                return true;
            }
        }

        public static DirectoryPath CreateUnsafe(SpanOrSegment input) {
            if (input.IsEmpty) {
                return default;
            }

            var partBuilder = ImmutableArray.CreateBuilder<PathPart>();
            foreach (var part in new PathPartSegmentEnumerator(input)) {
                var pathPart = PathPart.Parser.CreateUnsafe(part);
                partBuilder.Add(pathPart);
            }

            return new DirectoryPath(partBuilder.MoveToImmutableSafely());
        }

        // Strict: Rejects any empty parts (not counting the first)
        public static string? TryParse_Internal(SpanOrSegment input, ClericalStyles styles, out DirectoryPath result) {
            if (input.Length == 0) {
                result = default;
                return null;
            }

            if (styles.TryConsumeBookendTrimming(ref input) is { } failMsg) {
                result = default;
                return failMsg.GetMessage();
            }

            var arrayBuilder = ImmutableArray.CreateBuilder<PathPart>();
            var allowEmpty   = (styles & ClericalStyles.AllowEmptyPathParts) != 0;
            foreach (var segment in new PathPartSegmentEnumerator(input)) {
                if (segment.IsEmpty && allowEmpty == false) {
                    result = default;
                    // return $"Cannot contain empty {nameof(PathPart)}s!";
                    return ClericalValidationMessage.EmptyPathPart.GetMessage();
                }

                // TODO: `TryParse_Internal` will allocate if necessary, because it originally assumed it would be the last step - but it might not be! It should return a "ready to use" `SpanOrSegment` instead, which can then be `CreateUnsafe`d.
                if (PathPart.Parser.TryParse_Internal(segment, styles, out var part) is { } partMsg) {
                    result = default;
                    // TODO: Return a fancy message here *that doesn't allocate anything*!
                    return partMsg;
                }

                arrayBuilder.Add(part);
            }

            var array = arrayBuilder.MoveToImmutableSafely();
            result = new DirectoryPath(array);
            return null;
        }

        public static DirectoryPath Parse_Internal(SpanOrSegment input, ClericalStyles styles) {
            return TryParse_Internal(input, styles, out var result) switch {
                null    => result,
                var msg => throw new FormatException(ParseHelpers.CreateFormatExceptionMessage(nameof(DirectoryPath), input, styles, msg))
            };
        }
    }
}