using System.Diagnostics;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

public readonly partial struct FileExtension
#if NET7_0_OR_GREATER
    : ISpanParsable<FileExtension>,
      IEqualityOperators<FileExtension, FileExtension, bool>
#endif
{
    internal const string ExtensionSeparatorChars = @". \/";

    /// <summary>
    /// Wraps a <see cref="string"/> as a new <see cref="FileExtension"/>, without performing <i>any</i> validation or normalization of the input.
    /// <p/>
    /// If you use this method, then it's up to you to make sure that the input is all-lowercase and starts with a period.
    /// </summary>
    /// <param name="lowercaseExtensionWithPeriod">the file extension, in all-lowercase, with a period</param>
    /// <returns>a new <see cref="FileExtension"/></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static FileExtension CreateUnsafe(StringSegment lowercaseExtensionWithPeriod) {
        return new FileExtension(lowercaseExtensionWithPeriod);
    }

    private static FileExtension CreateFromParsed(SpanOrSegment parsedInput) {
        return TryGetCommonExtensionString(parsedInput, out var str)
                   ? new FileExtension(str)
                   : new FileExtension(parsedInput.ToStringSegment());
    }

    private static FileExtension CreateFromImperfect(ReadOnlySpan<char> inputWithoutPeriod) {
        Debug.Assert(inputWithoutPeriod.LastIndexOfAny(ExtensionSeparatorChars) < 0, $"Expected an extension string without a leading period or any of: `{ExtensionSeparatorChars}` (actual: {inputWithoutPeriod.ToString()})");

        Span<char> buffer = stackalloc char[inputWithoutPeriod.Length + 1];
        buffer[0] = '.';
        var loweredCharacters = inputWithoutPeriod.ToLowerInvariant(buffer[1..]);
        Debug.Assert(loweredCharacters == inputWithoutPeriod.Length, "Didn't allocate a big enough buffer!");

        var str = GetOrCreateExtensionString(buffer);
        return CreateUnsafe(str);
    }

    private static FileExtension Parse_Forgiving(ReadOnlySpan<char> s) {
        return TryParse_Forgiving(s, out var result) ? result : throw new FormatException();
    }

    private static bool TryParse_Forgiving(ReadOnlySpan<char> s, out FileExtension result) {
        s = s.Trim();

        if (s.Length == 0) {
            result = default;
            return true;
        }

        if (s is ".") {
            result = default;
            return false;
        }

        var lastSeparatorIndex = s.LastIndexOfAny(ExtensionSeparatorChars);
        var withoutPeriod = lastSeparatorIndex switch {
            < 0 => s,
            > 0 => default,
            _   => s[0] is '.' ? s[1..] : default
        };

        if (withoutPeriod.IsEmpty) {
            result = default;
            return false;
        }

        result = CreateFromImperfect(withoutPeriod);
        return true;
    }

    internal static bool TryParse_Strict(SpanOrSegment s, out FileExtension result) {
        if (s.Length == 0) {
            result = default;
            return true;
        }

        if (ValidateExtensionSpan(s) is not null) {
            result = default;
            return false;
        }

        result = CreateFromParsed(s);
        return true;
    }

    internal static FileExtension Parse_Strict(SpanOrSegment s) {
        if (s.Length == 0) {
            return default;
        }

        if (ValidateExtensionSpan(s) is { } msg) {
            throw new FormatException(msg);
        }

        return CreateFromParsed(s);
    }

    /// <returns>If the input is <b>valid</b>: <c>null</c>
    /// <br/>If the input is <b>invalid</b>: the reason why</returns>
    /// <remarks>No noticeable difference (to my layman's eye) between returning <see cref="string"/> and returning <see cref="bool"/></remarks>
    private static string? ValidateExtensionSpan(ReadOnlySpan<char> span) {
        if (span.IsEmpty) {
            return null;
        }

        if (span is not ['.', not '.', ..]) {
            return "Must either be empty OR a period + 1-or-more non-periods!";
        }

        for (int i = 1; i < span.Length; i++) {
            if (ValidateChar(span[i]) is { } msg) {
                return msg;
            }
        }

        return null;

        static string? ValidateChar(char c) {
            // The optimistic fast-path: the vast majority of characters we'll be checking will be ASCII lowercase letters or digits.
            if ((uint)(c - 'a') <= (uint)('z' - 'a') || (uint)(c - '0') <= (uint)('9' - '0')) {
                return null;
            }

            if (char.IsUpper(c)) {
                return "Cannot contain uppercase characters!";
            }

            if (char.IsControl(c)) {
                return "Cannot contain control characters!";
            }

            if (char.IsWhiteSpace(c)) {
                return "Cannot contain white space!";
            }

            if (c is '.' or '/' or '\\') {
                return "Cannot contain extra periods or any directory separators!";
            }

            return null;
        }
    }

    /// <remarks>
    /// On a 64-bit machine, a <see cref="Vector{T}"/> of <see cref="char"/>s <i>(i.e. <see cref="ushort"/>s)</i> has 16 values -
    /// the likelihood of a <i>real</i> file extension being that long is nonexistent, so in preference of simplicity and correctness,
    /// this method relies on a "naive" for-each loop and <see cref="char"/> predicates.
    /// </remarks>
    private static bool IsPerfectExtension(ReadOnlySpan<char> s) {
        return ValidateExtensionSpan(s) == null;
    }

#if NET7_0_OR_GREATER
    [Pure] static FileExtension ISpanParsable<FileExtension>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                           => Parse(s);
    [Pure] static FileExtension IParsable<FileExtension>.    Parse(string                s, IFormatProvider? provider)                           => Parse(s);
    [Pure] static bool ISpanParsable<FileExtension>.         TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out FileExtension result) => TryParse(s, out result);
    [Pure] static bool IParsable<FileExtension>.             TryParse(string?            s, IFormatProvider? provider, out FileExtension result) => TryParse(s, out result);
#endif

    [Pure] public static FileExtension Parse(ReadOnlySpan<char>      s) => Parse_Forgiving(s);
    [Pure] public static FileExtension ParseExact(ReadOnlySpan<char> s) => Parse_Strict(s);

    [Pure] public static FileExtension Parse(string      s) => Parse_Forgiving(s);
    [Pure] public static FileExtension ParseExact(string s) => Parse_Strict(s);

    [Pure] public static bool TryParse(ReadOnlySpan<char>      s, out FileExtension result) => TryParse_Forgiving(s, out result);
    [Pure] public static bool TryParseExact(ReadOnlySpan<char> s, out FileExtension result) => TryParse_Strict(s, out result);

    [Pure] public static bool TryParse(string?      s, out FileExtension result) => TryParse_Forgiving(s, out result);
    [Pure] public static bool TryParseExact(string? s, out FileExtension result) => TryParse_Strict(s, out result);
}