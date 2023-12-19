namespace FowlFever.Clerical;

public readonly partial record struct DirectoryPath : IClericalParsable<DirectoryPath> {
#if NET7_0_OR_GREATER
    [Pure] static DirectoryPath IParsable<DirectoryPath>.    Parse(string                s, IFormatProvider? provider)                           => Parse(s);
    [Pure] static bool IParsable<DirectoryPath>.             TryParse(string?            s, IFormatProvider? provider, out DirectoryPath result) => TryParse(s, out result);
    [Pure] static DirectoryPath ISpanParsable<DirectoryPath>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                           => Parse(s);
    [Pure] static bool ISpanParsable<DirectoryPath>.         TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out DirectoryPath result) => TryParse(s, out result);
#endif

    [Pure] public static DirectoryPath Parse(string                s, ClericalStyles    styles                        = default) => Parser.Parse_Internal(s, styles);
    [Pure] public static DirectoryPath Parse(ReadOnlySpan<char>    s, ClericalStyles    styles                        = default) => Parser.Parse_Internal(s, styles);
    [Pure] public static bool          TryParse(string?            s, out DirectoryPath result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;
    [Pure] public static bool          TryParse(ReadOnlySpan<char> s, out DirectoryPath result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;
}