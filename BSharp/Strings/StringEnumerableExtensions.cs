using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/>s of <see cref="string"/>s.
/// </summary>
/// <remarks>
/// These methods extend <c>T</c> instances constrained to <c><![CDATA[where T : IEnumerable<string?>?]]></c>.
/// <p/>
/// While this is more roundabout than extending <see cref="IEnumerable{T}"/> directly, it prevents <c>struct</c>s that extend <see cref="IEnumerable{T}"/> from being boxed,
/// like <see cref="System.Collections.Immutable.ImmutableArray{T}"/>.
/// </remarks>
public static class StringEnumerableExtensions {
    /// <summary>
    /// Filters out all of the <see cref="string.IsNullOrEmpty"/> strings.
    /// </summary>
    /// <remarks>
    /// Using the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving">null-forgiving operator</a> is ugly, but a limitation of
    /// linq.
    /// <ul>
    /// <li><a href="https://stackoverflow.com/a/60205019">Using Linq's Where/Select to filter out null and convert the type to non nullable cannot be made into an extension method</a></li>
    /// </ul>
    /// </remarks>
    /// <param name="strings">a collection of <see cref="string"/>s</param>
    /// <returns>the non-<see cref="string.IsNullOrEmpty"/> <paramref name="strings"/></returns>
    /// <seealso cref="string.IsNullOrEmpty"/>
    public static IEnumerable<string> NonEmpty<T>(this T strings)
        where T : IEnumerable<string?>? {
        return strings.OrEmpty().Where(static str => string.IsNullOrEmpty(str) == false)!;
    }

    /// <summary>
    /// Filters out all of the <see cref="string.IsNullOrWhiteSpace"/> strings.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="NonEmpty{T}"/>
    /// </remarks>
    /// <param name="strings">a collection of <see cref="string"/>s</param>
    /// <returns>the non=<see cref="string.IsNullOrWhiteSpace"/> <paramref name="strings"/></returns>
    /// <seealso cref="string.IsNullOrWhiteSpace"/>
    public static IEnumerable<string> NonBlank<T>(this T strings)
        where T : IEnumerable<string?>? {
        return strings.OrEmpty().Where(static str => string.IsNullOrWhiteSpace(str) == false)!;
    }

    /// <summary>
    /// <see cref="Enumerable.Append{TSource}"/>s <paramref name="str"/> if it <see cref="StringUtils.IsNotBlank(string?)"/>.
    /// </summary>
    /// <param name="strings">a sequence of <see cref="string"/>s</param>
    /// <param name="str">the <see cref="string"/> to be added</param>
    /// <param name="moreStrings">more <see cref="string"/>s to be added</param>
    /// <returns>the <see cref="Enumerable.Append{TSource}"/>ed sequence, if we went through with it; otherwise, the original <paramref name="strings"/></returns>
    [LinqTunnel]
    public static IEnumerable<string> AppendNonBlank(this IEnumerable<string> strings, string? str, params string?[] moreStrings) => strings.AppendNonBlank(moreStrings.Prepend(str));

    /// <summary>
    /// <see cref="Enumerable.Concat{TSource}"/>s all of the <see cref="NonBlank{T}"/> entries from <paramref name="moreStrings"/>.
    /// </summary>
    /// <param name="strings"></param>
    /// <param name="moreStrings"></param>
    /// <returns></returns>
    public static IEnumerable<string> AppendNonBlank(this IEnumerable<string> strings, IEnumerable<string?>? moreStrings) => moreStrings == null ? strings : strings.Concat(moreStrings.NonBlank());
}