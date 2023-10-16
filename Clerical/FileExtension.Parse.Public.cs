namespace FowlFever.Clerical;

// This file contains the public "Parse" methods available for `FileExtension`s.
public readonly partial struct FileExtension
#if NET7_0_OR_GREATER
    : ISpanParsable<FileExtension>
#endif
{
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

    /// <inheritdoc cref="Parser.Parse_Forgiving"/>
    [Pure]
    public static FileExtension Parse(ReadOnlySpan<char> s) => Parser.Parse_Forgiving(s);

    /// <inheritdoc cref="Parser.Parse_Strict"/>
    [Pure]
    public static FileExtension ParseExact(ReadOnlySpan<char> s) => Parser.Parse_Strict(s);

    /// <inheritdoc cref="Parser.Parse_Forgiving"/>
    [Pure]
    public static FileExtension Parse(string s) => Parser.Parse_Forgiving(s);

    /// <inheritdoc cref="Parser.Parse_Strict"/>
    [Pure]
    public static FileExtension ParseExact(string s) => Parser.Parse_Strict(s);

    /// <inheritdoc cref="Parser.Parse_Forgiving"/>
    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out FileExtension result) => Parser.TryParse_Internal(s, false, false, out result);

    /// <inheritdoc cref="Parser.Parse_Strict"/>
    [Pure]
    public static bool TryParseExact(ReadOnlySpan<char> s, out FileExtension result) => Parser.TryParse_Internal(s, true, false, out result);

    /// <inheritdoc cref="Parser.Parse_Forgiving"/>
    [Pure]
    public static bool TryParse(string? s, out FileExtension result) => Parser.TryParse_Internal(s, false, false, out result);

    /// <inheritdoc cref="Parser.Parse_Strict"/>
    [Pure]
    public static bool TryParseExact(string? s, out FileExtension result) => Parser.TryParse_Internal((SpanOrSegment)s, true, false, out result);

    #endregion
}