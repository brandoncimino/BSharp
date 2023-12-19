namespace FowlFever.Clerical;

// This file contains the public `{Try}Parse` methods, which delegate to the logic defined in `FileName.Parser`.
public readonly partial record struct FileName : IClericalParsable<FileName> {
#if NET7_0_OR_GREATER
    static FileName IParsable<FileName>.    Parse(string                s, IFormatProvider? provider)                      => Parse(s);
    static bool IParsable<FileName>.        TryParse(string?            s, IFormatProvider? provider, out FileName result) => TryParse(s, out result);
    static FileName ISpanParsable<FileName>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                      => Parse(s);
    static bool ISpanParsable<FileName>.    TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out FileName result) => TryParse(s, out result);
#endif

    [Pure]
    public static FileName Parse(string s, ClericalStyles styles = default) {
        if (s == null) {
            throw new ArgumentNullException(nameof(s));
        }

        return Parser.Parse_Internal(s, styles);
    }

    [Pure] public static bool     TryParse(string?            s, out FileName   result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;
    [Pure] public static FileName Parse(ReadOnlySpan<char>    s, ClericalStyles styles                        = default) => Parser.Parse_Internal(s, styles);
    [Pure] public static bool     TryParse(ReadOnlySpan<char> s, out FileName   result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;
}