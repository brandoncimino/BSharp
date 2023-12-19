namespace FowlFever.Clerical;

public readonly partial struct PathPart : IClericalParsable<PathPart> {
#if NET7_0_OR_GREATER
    [Pure] static PathPart ISpanParsable<PathPart>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                      => Parse(s);
    [Pure] static PathPart IParsable<PathPart>.    Parse(string                s, IFormatProvider? provider)                      => Parse(s);
    [Pure] static bool ISpanParsable<PathPart>.    TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out PathPart result) => TryParse(s, out result);
    [Pure] static bool IParsable<PathPart>.        TryParse(string?            s, IFormatProvider? provider, out PathPart result) => TryParse(s, out result);
#endif

    [Pure] public static PathPart Parse(ReadOnlySpan<char>    s, ClericalStyles styles                        = default) => Parser.Parse_Internal(s, styles);
    [Pure] public static bool     TryParse(ReadOnlySpan<char> s, out PathPart   result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;
    [Pure] public static PathPart Parse(string                s, ClericalStyles styles                        = default) => Parser.Parse_Internal(s.RequireNonNull(), styles);
    [Pure] public static bool     TryParse(string?            s, out PathPart   result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;
}