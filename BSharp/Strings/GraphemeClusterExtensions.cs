using System;
using System.Collections.Generic;
using System.Globalization;

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
    public static IEnumerable<string> EnumerateTextElements(this string? str) {
        if (str == null) {
            yield break;
        }

        var enumerator = StringInfo.GetTextElementEnumerator(str);
        while (enumerator.MoveNext()) {
            yield return enumerator.Current.ToString()!;
        }
    }

    /// <inheritdoc cref="EnumerateTextElements(string?)"/>
    public static IEnumerable<string> EnumerateTextElements(this IHas<string?>? str) => str.GetValueOrDefault().EnumerateTextElements();

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
    /// Retrieves a <see cref="GraphemeCluster"/> by its index within a <see cref="StringInfo"/>.
    /// </summary>
    /// <param name="stringInfo">this <see cref="StringInfo"/></param>
    /// <param name="index">the index of the <see cref="StringInfo.SubstringByTextElements(int)"/> of length 1</param>
    /// <returns>the corresponding <see cref="GraphemeCluster"/></returns>
    public static GraphemeCluster ElementAt(this StringInfo stringInfo, int index) {
        return GraphemeCluster.CreateRisky(stringInfo.SubstringByTextElements(index, 1));
    }

    /// <inheritdoc cref="StringInfo.SubstringByTextElements(int)"/>
    public static string SubstringByTextElements(this StringInfo stringInfo, Range range) {
        var (off, len) = range.GetOffsetAndLength(stringInfo.LengthInTextElements);
        return stringInfo.SubstringByTextElements(off, len);
    }
}