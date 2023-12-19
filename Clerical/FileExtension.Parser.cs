using System.Buffers;
using System.Diagnostics;

using CommunityToolkit.HighPerformance.Buffers;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

// This file contains the meat and potatoes of parsing FileExtensions 
public readonly partial struct FileExtension {
    [UsedImplicitly]
    internal class Parser : IClericalParser<FileExtension, ClericalStyles, string?> {
        internal static readonly SearchValues<char> ExtensionSeparatorChars = SearchValues.Create(@". \/");
        private static readonly  StringPool         ExtensionPool           = new();

        /// <summary>
        /// This method handles "uncommon" extension strings - i.e. ones that aren't covered by <see cref="TryGetCommonExtensionString(System.ReadOnlySpan{char})"/>.
        /// </summary>
        private static string GetUncommonExtensionString(ReadOnlySpan<char> perfectExtensionSpan) {
            DebugAssert_PerfectExtension(perfectExtensionSpan);
            Debug.Assert(TryGetCommonExtensionString(perfectExtensionSpan, out var common) is false, $"Common extensions like `{common}` should have already been handled!");

            return ExtensionPool.GetOrAdd(perfectExtensionSpan);
        }

        /// <summary>
        /// Wraps a <see cref="string"/> as a new <see cref="FileExtension"/>, without performing <i>any</i> validation or normalization of the input.
        /// <p/>
        /// If you use this method, then it's up to you to make sure that the input is all-lowercase and starts with a period.
        /// </summary>
        /// <param name="lowercaseExtensionWithPeriod">the file extension, in all-lowercase, with a period</param>
        /// <returns>a new <see cref="FileExtension"/></returns>
        [Pure]
        public static FileExtension CreateUnsafe(SpanOrSegment lowercaseExtensionWithPeriod) {
            return new FileExtension(lowercaseExtensionWithPeriod.ToStringSegment());
        }

        /// <summary>
        /// Creates a <see cref="FileExtension"/> out of a "perfect" result - i.e. one whose contents are exactly as we'd like to store them, and therefore,
        /// we might be able to save allocations by re-using the input.
        /// </summary>
        internal static FileExtension CreateFromPerfect(SpanOrSegment perfectExtensionSpan) {
            DebugAssert_PerfectExtension(perfectExtensionSpan);

            var segment = TryGetCommonExtensionString(perfectExtensionSpan[1..])
                          ?? perfectExtensionSpan.Segment
                          ?? GetUncommonExtensionString(perfectExtensionSpan);

            return new FileExtension(segment);
        }

        /// <summary>
        /// Creates a <see cref="FileExtension"/> out of an "imperfect" result - i.e. one that we know will need a new <see cref="string"/> to be created.
        /// </summary>
        internal static FileExtension CreateFromImperfect(SpanOrSegment inputWithoutPeriod) {
            Debug.Assert(inputWithoutPeriod.AsSpan().LastIndexOfAny(ExtensionSeparatorChars) < 0, $"Expected an extension string without a leading period or any of: `{ExtensionSeparatorChars}` (actual: {inputWithoutPeriod.AsSpan().ToString()})");

            Span<char> buffer = stackalloc char[inputWithoutPeriod.Length + 1];
            buffer[0] = '.';
            var loweredCharacters = inputWithoutPeriod.AsSpan().ToLowerInvariant(buffer[1..]);
            Debug.Assert(loweredCharacters == inputWithoutPeriod.Length, "Didn't allocate a big enough buffer!");

            return CreateFromPerfect(new SpanOrSegment(buffer));
        }

        public static FileExtension Parse_Internal(SpanOrSegment s, ClericalStyles styles) {
            if (TryParse_Internal(s, styles, out var result) is { } msg) {
                throw new FormatException(ParseHelpers.CreateFormatExceptionMessage(nameof(FileExtension), s, styles, msg));
            }

            return result;
        }

        private static string GetValidationErrorMessage(ParseHelpers.CharValidationResult singleFlag) {
            Debug.Assert(singleFlag != ParseHelpers.CharValidationResult.Perfect);

            return singleFlag switch {
                ParseHelpers.CharValidationResult.Uppercase          => "Cannot contain uppercase characters!",
                ParseHelpers.CharValidationResult.WhiteSpace         => "Cannot contain whitespace!",
                ParseHelpers.CharValidationResult.Control            => "Cannot contain control characters!",
                ParseHelpers.CharValidationResult.ExtraPeriod        => "Cannot contain non-leading periods!",
                ParseHelpers.CharValidationResult.DirectorySeparator => "Cannot contain directory separators!",
                ParseHelpers.CharValidationResult.Perfect            => throw new ArgumentOutOfRangeException(nameof(singleFlag), singleFlag, null),
                _                                                    => throw new ArgumentOutOfRangeException(nameof(singleFlag), singleFlag, null)
            };
        }

        [Pure]
        public static string? TryParse_Internal(SpanOrSegment input, ClericalStyles styles, out FileExtension result) {
            styles.RemoveUnsupportedStyles(ClericalStyles.AllowLeadingDirectorySeparator, nameof(FileExtension));
            if (styles.TryConsumeBookendTrimming(ref input) is { } failMsg) {
                result = default;
                return failMsg.GetMessage();
            }

            if (input.Length == 0) {
                result = default;
                return null;
            }

            bool isImperfect   = false;
            var  withoutPeriod = input;
            if (input[0] == '.') {
                if (input.Length == 1) {
                    result = default;
                    return "Cannot be a single period!";
                }

                withoutPeriod = input[1..];
            }
            else {
                if ((styles & ClericalStyles.AllowExtensionsWithoutPeriods) == 0) {
                    result = default;
                    return "Must start with a period!";
                }

                isImperfect = true;
            }

            // TODO: This is a potential spot for `TryGetCommonExtension`, or something similar

            foreach (var c in withoutPeriod.AsSpan()) {
                var charValidationResult = ParseHelpers.ValidateChar(c);

                switch (charValidationResult) {
                    case ParseHelpers.CharValidationResult.Perfect:
                        continue;
                    case ParseHelpers.CharValidationResult.Uppercase:
                        if ((styles & ClericalStyles.AllowUpperCaseExtensions) == 0) {
                            goto case default;
                        }

                        isImperfect = true;
                        continue;
                    case ParseHelpers.CharValidationResult.WhiteSpace:
                    case ParseHelpers.CharValidationResult.Control:
                    case ParseHelpers.CharValidationResult.ExtraPeriod:
                    case ParseHelpers.CharValidationResult.DirectorySeparator:
                    default:
                        result = default;
                        return GetValidationErrorMessage(charValidationResult);
                }
            }

            result = isImperfect switch {
                true  => CreateFromImperfect(withoutPeriod),
                false => CreateFromPerfect(input),
            };

            return null;
        }
    }

    [Conditional("DEBUG")]
    private static void DebugAssert_PerfectExtension(ReadOnlySpan<char> s) {
        if (s.IsEmpty) {
            return;
        }

        if (s is not ['.', .. var afterPeriod]) {
            Debug.Fail($"Must be empty or a period followed by 1+ non-periods! (Actual: {s.ToString()})");
            return;
        }

        foreach (var c in afterPeriod) {
            Debug.Assert(ParseHelpers.ValidateChar(c) is ParseHelpers.CharValidationResult.Perfect, $"Bad char: {c}");
        }
    }
}