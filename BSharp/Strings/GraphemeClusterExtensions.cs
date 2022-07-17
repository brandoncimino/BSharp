using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Methods for interacting with <a href="https://www.unicode.org/glossary/#grapheme_cluster">grapheme clusters</a>,
/// which C# refers to as 
/// <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-encoding-introduction#grapheme-clusters">"text elements"</a>.
/// </summary>
public static class GraphemeClusterExtensions {
    /// <summary>
    /// Creates an <see cref="IEnumerable{T}"/> for each of the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-encoding-introduction#grapheme-clusters">"text elements"</a> in a <see cref="string"/>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns>a new <see cref="IEnumerable{T}"/></returns>
    public static IEnumerable<GraphemeCluster> EnumerateTextElements(this string? str) {
        if (str == null) {
            yield break;
        }

        var enumerator = StringInfo.GetTextElementEnumerator(str);
        while (enumerator.MoveNext()) {
            yield return GraphemeCluster.CreateRisky(enumerator.GetTextElement());
        }
    }

    /// <inheritdoc cref="EnumerateTextElements(string?)"/>
    public static IEnumerable<GraphemeCluster> EnumerateTextElements(this IHas<string?>? str) => str.GetValueOrDefault().EnumerateTextElements();

    /// <summary>
    /// Gets the "visible" length of a <see cref="string"/> by counting the number of <a href="https://www.unicode.org/glossary/#grapheme_cluster">grapheme clusters</a>,
    /// which C# refers to as 
    /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-encoding-introduction#grapheme-clusters">"text elements"</a>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns><inheritdoc cref="StringInfo.LengthInTextElements"/></returns>
    public static int VisibleLength(this string? str) => str == null ? 0 : new StringInfo(str).LengthInTextElements;

    /// <inheritdoc cref="VisibleLength(string?)"/>
    public static int VisibleLength(this IHas<string?>? str) => str.GetValueOrDefault().VisibleLength();

    /// <summary>
    /// <see cref="Bloop.WrapAround{T}"/>s <paramref name="source"/> until we reach <paramref name="desiredLength"/>.
    /// </summary>
    /// <param name="source">this <see cref="IEnumerable{T}"/> of <see cref="GraphemeCluster"/>s</param>
    /// <param name="desiredLength">the desired number of <see cref="GraphemeCluster"/>s</param>
    /// <returns>a sequence of <paramref name="desiredLength"/> <see cref="GraphemeCluster"/>s</returns>
    public static IEnumerable<GraphemeCluster> RepeatToLength(this IEnumerable<GraphemeCluster> source, int desiredLength) {
        return source switch {
            OneLine line => line.RepeatToLength(desiredLength),
            _            => source.WrapAround(desiredLength),
        };
    }

    /// <inheritdoc cref="RepeatToLength(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.GraphemeCluster},int)"/>
    public static OneLine RepeatToLength(this OneLine line, int desiredLength) => OneLine.CreateRisky(line.AsEnumerable().RepeatToLength(desiredLength));
}