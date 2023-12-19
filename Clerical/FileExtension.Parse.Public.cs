namespace FowlFever.Clerical;

// This file contains the public "Parse" methods available for `FileExtension`s.
#if !NET7_0_OR_GREATER
[SuppressMessage("ReSharper", "InheritdocInvalidUsage", Justification = "Versions prior to .NET 7 don't understand static abstract members in interfaces")]
#endif
public readonly partial struct FileExtension : IClericalParsable<FileExtension> {
    #region Explicit implementations of .NET 7 interfaces

    // These are explicit implementations so that we don't clutter up our method signatures with the useless `IFormatProvider` parameters.

#if NET7_0_OR_GREATER
    [Pure] static FileExtension ISpanParsable<FileExtension>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                           => Parse(s);
    [Pure] static FileExtension IParsable<FileExtension>.    Parse(string                s, IFormatProvider? provider)                           => Parse(s);
    [Pure] static bool ISpanParsable<FileExtension>.         TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out FileExtension result) => TryParse(s, out result);
    [Pure] static bool IParsable<FileExtension>.             TryParse(string?            s, IFormatProvider? provider, out FileExtension result) => TryParse(s, out result);
#endif

    #endregion

    #region Public "Parse" methods

    [Pure] public static FileExtension Parse(ReadOnlySpan<char> s, ClericalStyles styles = default) => Parser.Parse_Internal(s, styles);

    [Pure] public static FileExtension Parse(string s, ClericalStyles styles = default) => Parser.Parse_Internal(s, styles);

    [Pure] public static bool TryParse(ReadOnlySpan<char> s, out FileExtension result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;

    [Pure] public static bool TryParse(string? s, out FileExtension result, ClericalStyles styles = default) => Parser.TryParse_Internal(s, styles, out result) is null;

    #endregion
}