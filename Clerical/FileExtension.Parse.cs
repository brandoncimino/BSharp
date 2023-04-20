using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace FowlFever.Clerical;

public readonly partial struct FileExtension {
    internal const string IllegalChars = ". \n\\/";

    /// <summary>
    /// The maximum <see cref="ReadOnlySpan{T}.Length"/> of a file extension's <see cref="_value"/>.
    ///
    /// If you want to create a <see cref="FileExtension"/>
    /// </summary>
    /// <remarks>
    /// The only file extension I've ever seen longer than 6 characters is <c>.DotSettings</c>.
    /// <br/>
    /// I've never seen a (reasonable) extension longer than 6 characters <i>(<c>.csproj</c>, <c>.asmdef</c>)</i>.
    /// The value of <see cref="MaxExtensionLengthIncludingPeriod"/> was chosen to give some extra space and because computers like powers of 2.
    /// </remarks>
    internal const int MaxExtensionLengthIncludingPeriod = 16;

    private static ReadOnlySpan<char> GetWithoutPeriod(ReadOnlySpan<char> goodSizedString) {
        Debug.Assert(goodSizedString.Length <= MaxExtensionLengthIncludingPeriod);

        var lastBadChar = goodSizedString.LastIndexOfAny(IllegalChars);
        return lastBadChar switch {
            < 0 => goodSizedString,
            0   => goodSizedString[0] == '.' ? goodSizedString[1..] : default,
            > 0 => default
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
    public static FileExtension CreateUnsafe(string lowercaseExtensionWithPeriod) {
        return new FileExtension(lowercaseExtensionWithPeriod);
    }

    public static bool TryParse(ReadOnlySpan<char> s, out FileExtension result) {
        if (s.Length > MaxExtensionLengthIncludingPeriod) {
            result = default;
            return false;
        }

        var withoutPeriod = GetWithoutPeriod(s);
        if (withoutPeriod.IsEmpty || withoutPeriod.Length == MaxExtensionLengthIncludingPeriod) {
            result = default;
            return false;
        }

        Debug.Assert(withoutPeriod.Length is > 0 and < MaxExtensionLengthIncludingPeriod);
        //🙋‍♀️ Would it be better to first check if the input isn't actually in the right case?
        Span<char> buffer = stackalloc char[withoutPeriod.Length + 1];
        buffer[0] = '.';
        var loweredChars = withoutPeriod.ToLowerInvariant(buffer[1..]);
        Debug.Assert(loweredChars == withoutPeriod.Length);

        result = new FileExtension(FinalizeExtensionString(buffer));
        return true;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out FileExtension result) {
        return TryParse(s.AsSpan(), out result);
    }

    public static FileExtension Parse(ReadOnlySpan<char> s) {
        return TryParse(s, out var result) ? result : throw new FormatException();
    }

    /// <summary>
    /// Creates a new <see cref="FileExtension"/>.
    /// </summary>
    /// <param name="s">the input string, which can be <see cref="WithPeriod"/> or <see cref="WithoutPeriod"/></param>
    /// <returns>a new <see cref="FileExtension"/></returns>
    /// <exception cref="FormatException">if the <see cref="string"/> wasn't a valid <see cref="FileExtension"/></exception>
    public static FileExtension Parse(string s) {
        return TryParse(s, out var result) ? result : throw new FormatException();
    }
}