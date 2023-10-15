using System.Diagnostics;

using CommunityToolkit.HighPerformance.Buffers;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

// This file contains the meat and potatoes of parsing FileExtensions 
public readonly partial struct FileExtension {
    internal const string ExtensionSeparatorChars = @". \/";

    private static readonly StringPool ExtensionPool = new();

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
    internal static FileExtension CreateUnsafe(StringSegment lowercaseExtensionWithPeriod) {
        return new FileExtension(lowercaseExtensionWithPeriod);
    }

    /// <summary>
    /// Creates a <see cref="FileExtension"/> out of a "perfect" result - i.e. one whose contents are exactly as we'd like to store them, and therefore,
    /// we might be able to save allocations by re-using the input.
    /// </summary>
    private static FileExtension CreateFromPerfect(SpanOrSegment perfectExtensionSpan) {
        DebugAssert_PerfectExtension(perfectExtensionSpan);

        var segment = TryGetCommonExtensionString(perfectExtensionSpan[1..])
                      ?? perfectExtensionSpan.Segment
                      ?? GetUncommonExtensionString(perfectExtensionSpan);

        return new FileExtension(segment);
    }

    /// <summary>
    /// Creates a <see cref="FileExtension"/> out of an "imperfect" result - i.e. one that we know will need a new <see cref="string"/> to be created.
    /// </summary>
    private static FileExtension CreateFromImperfect(SpanOrSegment inputWithoutPeriod) {
        Debug.Assert(inputWithoutPeriod.AsSpan().LastIndexOfAny(ExtensionSeparatorChars) < 0, $"Expected an extension string without a leading period or any of: `{ExtensionSeparatorChars}` (actual: {inputWithoutPeriod.AsSpan().ToString()})");

        Span<char> buffer = stackalloc char[inputWithoutPeriod.Length + 1];
        buffer[0] = '.';
        var loweredCharacters = inputWithoutPeriod.AsSpan().ToLowerInvariant(buffer[1..]);
        Debug.Assert(loweredCharacters == inputWithoutPeriod.Length, "Didn't allocate a big enough buffer!");

        return CreateFromPerfect(new SpanOrSegment(buffer));
    }

    /// <summary>
    /// Creates a <see cref="FileExtension"/> using "forgiving" parsing:
    /// <ul>
    /// <li>✅ A leading period is optional <i>(i.e. <c>".json"</c> and <c>"json"</c> are both OK)</i></li>
    /// <li>✅ The input will be <see cref="string.Trim()"/>med</li>
    /// <li>✅ The input will be converted <see cref="string.ToLowerInvariant"/></li>
    /// <li>❌ Non-leading periods are rejected</li>
    /// <li>❌ <see cref="Clerk.DirectorySeparatorChars"/> are rejected</li>
    /// <li>❌ Inner <see cref="char.IsWhiteSpace(char)"/> is rejected</li>
    /// <li>❌ <see cref="char.IsControl(char)"/> characters are rejected</li>
    /// </ul>
    /// </summary>
    /// <param name="s">the potential <see cref="FileExtension"/></param>
    /// <returns>the newly created <see cref="FileExtension"/></returns>
    /// <exception cref="FormatException">The input wasn't a valid <see cref="FileExtension"/></exception>
    private static FileExtension Parse_Forgiving(SpanOrSegment s) {
        TryParse_Internal(s, false, true, out var result);
        return result;
    }

    /// <summary><inheritdoc cref="Parse_Forgiving"/></summary>
    /// <param name="s">the potential <see cref="FileExtension"/></param>
    /// <param name="result">the newly created <see cref="FileExtension"/> <i>(if we succeeded)</i></param>
    /// <returns><c>true</c> if the input was a valid <see cref="FileExtension"/></returns>
    private static bool TryParse_Forgiving(SpanOrSegment s, out FileExtension result) {
        return TryParse_Internal(s, false, false, out result);
    }

    /// <summary>
    /// Creates a <see cref="FileExtension"/> using "strict" parsing.
    /// <p/>
    /// The input must either be empty or a period followed by 1 or more:
    /// <ul>
    /// <li>Non-periods</li>
    /// <li>Non-<see cref="Clerk.DirectorySeparatorChars"/></li>
    /// <li>Non-<see cref="char.IsUpper(char)"/></li>
    /// <li>Non-<see cref="char.IsWhiteSpace(char)"/></li>
    /// <li>Non-<see cref="char.IsControl(char)"/></li>
    /// </ul>
    /// </summary>
    /// <param name="s">the potential <see cref="FileExtension"/></param>
    /// <returns>the new <see cref="FileExtension"/></returns>
    /// <exception cref="FormatException">If the input wasn't a valid <see cref="FileExtension"/></exception>
    /// <remarks>Prefer "strict" parsing <i>(<see cref="ParseExact(string)"/>, etc.)</i> in performance-sensitive contexts, as it should be faster and cause fewer allocations than "forgiving" parsing <i>(<see cref="Parse(System.ReadOnlySpan{char})"/>, etc.)</i>.</remarks>
    private static FileExtension Parse_Strict(SpanOrSegment s) {
        TryParse_Internal(s, true, true, out var result);
        return result;
    }

    /// <summary>
    /// <inheritdoc cref="Parse_Strict"/>
    /// </summary>
    /// <param name="s">the potential <see cref="FileExtension"/></param>
    /// <param name="result">the newly created <see cref="FileExtension"/> <i>(if we succeeded)</i></param>
    /// <returns><c>true</c> if the input was a valid <see cref="FileExtension"/></returns>
    private static bool TryParse_Strict(SpanOrSegment s, out FileExtension result) {
        return TryParse_Internal(s, true, false, out result);
    }

    private enum ValidationResult {
        Perfect,
        Uppercase,
        WhiteSpace,
        Control,
        ExtraPeriod,
        DirectorySeparator,
    }

    private static string GetValidationErrorMessage(ValidationResult singleFlag) {
        Debug.Assert(singleFlag != ValidationResult.Perfect);

        return singleFlag switch {
            ValidationResult.Uppercase          => "Cannot contain uppercase characters!",
            ValidationResult.WhiteSpace         => "Cannot contain whitespace!",
            ValidationResult.Control            => "Cannot contain control characters!",
            ValidationResult.ExtraPeriod        => "Cannot contain non-leading periods!",
            ValidationResult.DirectorySeparator => "Cannot contain directory separators!",
            ValidationResult.Perfect            => throw new ArgumentOutOfRangeException(nameof(singleFlag), singleFlag, null),
            _                                   => throw new ArgumentOutOfRangeException(nameof(singleFlag), singleFlag, null)
        };
    }

    internal static bool TryParse_Internal(SpanOrSegment span, bool strict, bool throwOnFailure, out FileExtension result) {
        if (strict is false) {
            span = span.Trim();
        }

        if (span.Length == 0) {
            result = default;
            return true;
        }

        bool isImperfect   = false;
        var  withoutPeriod = span;
        if (span[0] == '.') {
            if (span.Length == 1) {
                result = default;
                return throwOnFailure switch {
                    true  => throw new FormatException("Cannot be a single period!"),
                    false => false
                };
            }

            withoutPeriod = span[1..];
        }
        else {
            if (strict) {
                result = default;
                return throwOnFailure switch {
                    true  => throw new FormatException("Must start with a period!"),
                    false => false
                };
            }

            isImperfect = true;
        }

        // TODO: This is a potential spot for `TryGetCommonExtension`, or something similar

        foreach (var c in withoutPeriod.AsSpan()) {
            var charValidationResult = ValidateChar(c);

            switch (charValidationResult) {
                case ValidationResult.Perfect:
                    continue;
                case ValidationResult.Uppercase when strict is false:
                    isImperfect = true;
                    continue;
                case ValidationResult.Uppercase:
                case ValidationResult.WhiteSpace:
                case ValidationResult.Control:
                case ValidationResult.ExtraPeriod:
                case ValidationResult.DirectorySeparator:
                default:
                    result = default;
                    return throwOnFailure switch {
                        true  => throw new FormatException(GetValidationErrorMessage(charValidationResult)),
                        false => false
                    };
            }
        }

        Debug.Assert((strict, isImperfect) is not (true, true), $"'strict' validation should never result in an 'imperfect' result!");

        result = isImperfect switch {
            true  => CreateFromImperfect(withoutPeriod),
            false => CreateFromPerfect(span),
        };

        return true;
    }

    private static ValidationResult ValidateChar(char c) {
        // If `c` is ASCII, which it almost always will be, then we can use simpler numeric checks
        // 📎 Check copied from .NET 7+ `char.IsAscii`
        // 🤔 I have no idea why they have the explicit cast to `(uint)`; it seems completely redundant to me.
        if ((uint)c <= '\x007f') {
            return c switch {
                >= 'a' and <= 'z' => ValidationResult.Perfect,
                '.'               => ValidationResult.ExtraPeriod,
                '/' or '\\'       => ValidationResult.DirectorySeparator,
                ' '               => ValidationResult.WhiteSpace,
                >= 'A' and <= 'Z' => ValidationResult.Uppercase,
                < ' '             => ValidationResult.Control,
                _                 => ValidationResult.Perfect
            };
        }

        // The following checks handle all Unicode `char`s.
        if (char.IsUpper(c)) {
            return ValidationResult.Uppercase;
        }

        // TODO: 🙋‍♀️ Should we allow the zero-width joiner?
        if (char.IsWhiteSpace(c)) {
            return ValidationResult.WhiteSpace;
        }

        if (char.IsControl(c)) {
            return ValidationResult.Control;
        }

        return ValidationResult.Perfect;
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
            Debug.Assert(ValidateChar(c) is ValidationResult.Perfect, $"Bad char: {c}");
        }
    }
}