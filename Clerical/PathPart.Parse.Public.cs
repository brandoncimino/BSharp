namespace FowlFever.Clerical;

public readonly partial struct PathPart
#if NET7_0_OR_GREATER
    : ISpanParsable<PathPart>
#endif
{
#if NET7_0_OR_GREATER
    [Pure] static PathPart ISpanParsable<PathPart>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                      => Parse(s);
    [Pure] static PathPart IParsable<PathPart>.    Parse(string                s, IFormatProvider? provider)                      => Parse(s);
    [Pure] static bool ISpanParsable<PathPart>.    TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out PathPart result) => TryParse(s, out result);
    [Pure] static bool IParsable<PathPart>.        TryParse(string?            s, IFormatProvider? provider, out PathPart result) => TryParse(s, out result);
#endif

    [Pure] public static PathPart Parse(ReadOnlySpan<char>    s)                      => Parser.Parse_Internal(s, false);
    [Pure] public static bool     TryParse(ReadOnlySpan<char> s, out PathPart result) => Parser.TryParse_Internal(s, false, false, out result);

    [Pure]
    public static PathPart Parse(string s) {
        // Included to match the standard signature of `IParsable<T>.Parse`, which explicitly says it rejects null `string`s
        if (s is null) {
            throw new ArgumentNullException(nameof(s));
        }

        return Parser.Parse_Internal(s, false);
    }

    [Pure] public static bool TryParse(string? s, out PathPart result) => Parser.TryParse_Internal(s, false, false, out result);
}