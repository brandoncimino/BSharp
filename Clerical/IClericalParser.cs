using JetBrains.Annotations;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// An internal contract to keep different "parser" implementations consistent, e.g. <see cref="FileExtension.Parser.TryParse_Internal"/>, <see cref="PathPart.Parser.TryParse_Internal"/>.
/// </summary>
/// <typeparam name="TParsed">the type that I create</typeparam>
/// <typeparam name="TStyle">controls how parsing should be done á la <see cref="System.Globalization.NumberStyles"/></typeparam>
/// <typeparam name="TTryResult">the type that I return from <c>Try{x}</c> methods
/// <br/>
/// <br/><i>(📎 As of October 16, 2023, this is always <see cref="bool"/>, but there's a good chance I'm going to change it to some fancy enum like <see cref="ParseHelpers.CharValidationResult"/>. The benefit of that would be to provide specific exception messages in "molecular" types.)</i>
/// <br/><i>(📎 As of October 19, 2023, I've already changed them all to <see cref="string"/>?)</i>
/// </typeparam>
internal interface IClericalParser<TParsed, TStyle, TTryResult> {
#if NET7_0_OR_GREATER
    /// <summary>
    /// This is the "workhorse" parsing method.
    /// <p/>
    /// An equivalent method to this should be implemented in each of the <see cref="Clerical"/> validated types, e.g.:
    /// <ul>
    /// <li><see cref="TryParse_Internal"/></li>
    /// <li><see cref="FileName.Parser.TryParse_Internal"/></li>
    /// </ul>
    /// <b>⚠ WARNING:</b> Implementations of this method should call each other (as opposed to <see cref="IParsable{TSelf}"/> methods, for example) so that the efficient <see cref="SpanOrSegment"/> type can be passed around internally.
    /// </summary>
    /// <param name="input">the stuff being parsed, which is probably one of:
    /// <ul>
    /// <li>A string from <see cref="IParsable{TSelf}.Parse"/>, etc.</li>
    /// <li>A <see cref="ReadOnlySpan{T}"/> from <see cref="ISpanParsable{TSelf}.Parse(System.ReadOnlySpan{char},System.IFormatProvider?)"/>, etc.</li>
    /// <li>A <see cref="StringSegment"/> from another <see cref="Clerical"/> parsing method</li>
    /// </ul></param>
    /// <param name="style">controls how parsing should be done á la <see cref="System.Globalization.NumberStyles"/></param>
    /// <param name="result">the parsed value, if we were successful</param>
    /// <returns>whether the parsing operation succeeded or not</returns>
    [Pure]
    [UsedImplicitly]
    static abstract TTryResult TryParse_Internal(SpanOrSegment input, TStyle style, out TParsed result);

    /// <summary>
    /// Similar to <see cref="TryParse_Internal"/>, but should throw a <see cref="FormatException"/> if the resulting <typeparamref name="TTryResult"/> is unsatisfactory.
    /// </summary>
    [Pure]
    [UsedImplicitly]
    static abstract TParsed Parse_Internal(SpanOrSegment input, TStyle style);

    /// <summary>
    /// Creates an instance of <typeparamref name="TParsed"/> <b><i>without any validation</i></b>.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>In the case of an "atomic" type, such as <see cref="PathPart"/> or <see cref="FileExtension"/>, this method should just wrap the input as-is.</li>
    /// <li>In the case of a "molecular" type, such as <see cref="FileName"/> or <see cref="DirectoryPath"/>, it should perform as little work as possible to determine the individual "atoms", and then they should be <see cref="CreateUnsafe"/>d.</li>
    /// </ul>
    /// <p/>
    /// While this is sorta redundant with the basic constructor, it's included for clarity and explicitness.
    /// Basically, it's easy to call a constructor willy-nilly, but the word "Unsafe" should make you pause.
    /// </remarks>
    [Pure]
    [UsedImplicitly]
    static abstract TParsed CreateUnsafe(SpanOrSegment input);
#endif
}