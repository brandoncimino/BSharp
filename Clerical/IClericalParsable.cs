using System.Diagnostics.CodeAnalysis;

namespace FowlFever.Clerical;

/// <summary>
/// (Ideally) Explicitly implements <see cref="IParsable{TSelf}"/> and <see cref="ISpanParsable{TSelf}"/> while providing similar methods without the annoying <see cref="IFormatProvider"/> arguments.
/// <p/>
/// Benefits of this type:
/// <ul>
/// <li> Conditionally implements <see cref="ISpanParsable{TSelf}"/>, reducing the number of annoying <c>#if</c> directives needed in child classes</li>
/// <li> Defines equivalent methods to <see cref="IParsable{TSelf}"/> / <see cref="ISpanParsable{TSelf}"/> without the annoying <see cref="IFormatProvider"/> argument</li>
/// <li><s>Explicitly implements <see cref="IParsable{TSelf}"/> and <see cref="ISpanParsable{TSelf}"/></s> <b>❌ For some reason, this causes <see cref="StackOverflowException"/>s when the class is statically loaded 😡</b></li>
/// </ul>
/// </summary>
/// <typeparam name="TSelf"><inheritdoc cref="ISpanParsable{TSelf}"/></typeparam>
internal interface IClericalParsable<TSelf>
#if NET7_0_OR_GREATER
    : ISpanParsable<TSelf> where TSelf : IClericalParsable<TSelf>
#endif
{
#if NET7_0_OR_GREATER
    // ⚠ For some reason, having any explicit implementations here causes a super-early `StackOverflowException` 😤
    // Maybe it's related to https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1033?

    // static TSelf ISpanParsable<TSelf>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => TSelf.Parse(s);
    // static bool ISpanParsable<TSelf>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result) => TSelf.TryParse(s, out result);
    // static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider) => TSelf.Parse(s);
    // static bool IParsable<TSelf>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result) => TSelf.TryParse(s, out result);

    /// <summary>
    /// <inheritdoc cref="IParsable{TSelf}.Parse"/>
    /// </summary>
    /// <param name="s"><inheritdoc cref="IParsable{TSelf}.Parse"/></param>
    /// <param name="styles">the <see cref="ClericalStyles"/> that control how parsing should happen</param>
    /// <returns>The newly created <typeparamref name="TSelf"/>.</returns>
    public static abstract TSelf Parse(string s, ClericalStyles styles = default);

    /// <summary>
    /// <inheritdoc cref="ISpanParsable{TSelf}.Parse(System.ReadOnlySpan{char},System.IFormatProvider?)"/>
    /// </summary>
    /// <param name="s"><inheritdoc cref="ISpanParsable{TSelf}.Parse(System.ReadOnlySpan{char},System.IFormatProvider?)"/></param>
    /// <param name="styles">the <see cref="ClericalStyles"/> that control how parsing should happen</param>
    /// <returns>The newly created <typeparamref name="TSelf"/>.</returns>
    public static abstract TSelf Parse(ReadOnlySpan<char> s, ClericalStyles styles = default);

    /// <summary>
    /// <inheritdoc cref="IParsable{TSelf}.TryParse"/>
    /// </summary>
    /// <param name="s"><inheritdoc cref="IParsable{TSelf}.TryParse"/></param>
    /// <param name="result">The newly created <typeparamref name="TSelf"/>.</param>
    /// <param name="styles">The <see cref="ClericalStyles"/> that control how parsing should happen.</param>
    /// <returns><inheritdoc cref="IParsable{TSelf}.TryParse"/></returns>
    public static abstract bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out TSelf result, ClericalStyles styles = default);

    /// <summary>
    /// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(System.ReadOnlySpan{char},System.IFormatProvider?,out TSelf)"/>
    /// </summary>
    /// <param name="s"><inheritdoc cref="ISpanParsable{TSelf}.TryParse(System.ReadOnlySpan{char},System.IFormatProvider?,out TSelf)"/></param>
    /// <param name="result">The newly created <typeparamref name="TSelf"/>.</param>
    /// <param name="styles">The <see cref="ClericalStyles"/> that control how parsing should happen.</param>
    /// <returns><inheritdoc cref="ISpanParsable{TSelf}.TryParse(System.ReadOnlySpan{char},System.IFormatProvider?,out TSelf)"/></returns>
    public static abstract bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out TSelf result, ClericalStyles styles = default);
#endif
}