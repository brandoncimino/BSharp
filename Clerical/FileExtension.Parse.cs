using System.Diagnostics;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

public readonly partial struct FileExtension
#if NET7_0_OR_GREATER
    : System.ISpanParsable<FileExtension>,
      System.Numerics.IEqualityOperators<FileExtension, FileExtension, bool>
#endif
{
    internal const string ExtensionSeparatorChars = @". \/";

    private static ReadOnlySpan<char> GetWithoutPeriod(ReadOnlySpan<char> rawInput) {
        var lastSeparatorIndex = rawInput.LastIndexOfAny(ExtensionSeparatorChars);
        return lastSeparatorIndex switch {
            < 0 => rawInput,
            > 0 => default,
            _   => rawInput is ['.', _] ? rawInput[1..] : default
        };
    }

    /// <summary>
    /// Wraps a <see cref="string"/> as a new <see cref="FileExtension"/>, without performing <i>any</i> validation or normalization of the input.
    /// <p/>
    /// If you use this method, then it's up to you to make sure that the input is all-lowercase and starts with a period.
    /// </summary>
    /// <param name="lowercaseExtensionWithPeriod">the file extension, in all-lowercase, with a period</param>
    /// <returns>a new <see cref="FileExtension"/></returns>
    [Pure]
    public static FileExtension CreateUnsafe(StringSegment lowercaseExtensionWithPeriod) {
        return new FileExtension(lowercaseExtensionWithPeriod);
    }

    /// <inheritdoc cref="TryParse(System.ReadOnlySpan{char}, IFormatProvider, out FowlFever.Clerical.FileExtension)"/>
    /// <remarks>
    /// In the <b>vast majority of cases</b>, <paramref name="s"/> will be one of the <see cref="TryGetCommonExtensionString"/>s.
    /// However, it might come with OR without a leading period, and it might come with goofy casing.
    /// <p/>
    /// If we're trying to create a <see cref="FileExtension"/> from a <see cref="string"/>, then it's most likely a hard-coded literal.
    /// </remarks>
    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out FileExtension result) {
        var withoutPeriod = GetWithoutPeriod(s);

        if (withoutPeriod.IsEmpty) {
            result = default;
            return false;
        }

        Span<char> buffer = stackalloc char[withoutPeriod.Length + 1];
        buffer[0] = '.';
        var loweredChars = withoutPeriod.ToLowerInvariant(buffer[1..]);

        Debug.Assert(loweredChars == s.Length, "Didn't allocate a properly sized buffer!");

        var extensionString = GetOrCreateExtensionString(buffer);
        result = CreateUnsafe(extensionString);
        return true;
    }

    private static bool IsPerfectExtension(ReadOnlySpan<char> s) => s is [] or ['.', not '.', ..] && CharHelpers.ContainsAsciiLetterUpper(s) == false;

    [Pure]
    public static FileExtension Parse(ReadOnlySpan<char> s, IFormatProvider? provider = default) {
        return TryParse(s, null, out var result) ? result : throw new FormatException();
    }

    [Pure]
    public static FileExtension Parse(string s, IFormatProvider? provider = default) {
        return TryParse(s, provider, out var result) ? result : throw new FormatException();
    }

    [Pure]
    public static bool TryParse(string? s, IFormatProvider? provider, out FileExtension result) {
        if (s != null && IsPerfectExtension(s)) {
            result = CreateUnsafe(s);
            return true;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }
}