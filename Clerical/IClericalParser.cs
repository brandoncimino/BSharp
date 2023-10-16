using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// An internal contract to keep different "parser" implementations consistent, e.g. <see cref="FileExtension.Parser.TryParse_Internal"/>, <see cref="PathPart.Parser.TryParse_Internal"/>.
/// </summary>
/// <typeparam name="TSelf">the type that I create</typeparam>
/// <typeparam name="TTryResult">the type that I return from <c>Try{x}</c> methods
/// <br/>
/// <i>(📎 As of October 16, 2023, this is always <see cref="bool"/>, but there's a good chance I'm going to change it to some fancy enum like <see cref="FileExtension.Parser.ValidationResult"/>. The benefit of that would be to provide specific exception messages in "molecular" types.)</i></typeparam>
internal interface IClericalParser<TSelf, TTryResult> {
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
    /// <param name="strict">if <c>true</c>, parsing will be unforgiving - ideally so that a successful parsing operation produces 0 allocations
    /// <p/>
    /// TODO: Should this be called "exact" to match <see cref="DateTime.ParseExact(System.ReadOnlySpan{char},System.ReadOnlySpan{char},System.IFormatProvider?,System.Globalization.DateTimeStyles)"/>?
    /// TODO: Should this be called "requireNoAllocations" to make it easier to hold accountable?
    /// </param>
    /// <param name="throwOnFailure">if <c>true</c>, instead of returning <c>false</c>, we'll throw a <see cref="FormatException"/></param>
    /// <param name="result">the parsed value, if we were successful</param>
    /// <returns>whether the parsing operation succeeded or not</returns>
    /// <exception cref="FormatException">if we would've returned <c>false</c> but <paramref name="throwOnFailure"/> was <c>true</c></exception>
    [Pure]
    static abstract TTryResult TryParse_Internal(SpanOrSegment input, bool strict, bool throwOnFailure, out TSelf result);

    /// <summary>
    /// A convenience on <see cref="TryParse_Internal"/> that should always contain the following implementation:
    /// <code><![CDATA[
    /// var success = TResult.TryParseInternal(input, strict, true, out result);
    /// Debug.Assert(success, "Failures should've already thrown an exception!");
    /// return result;
    /// ]]></code>
    /// </summary>
    [Pure]
    static abstract TSelf Parse_Internal(SpanOrSegment input, bool strict);
#endif
}