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

    public static abstract TSelf Parse(string                         s);
    public static abstract TSelf Parse(ReadOnlySpan<char>             s);
    public static abstract bool  TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out TSelf result);
    public static abstract bool  TryParse(ReadOnlySpan<char>          s, [MaybeNullWhen(false)] out TSelf result);
#endif
}