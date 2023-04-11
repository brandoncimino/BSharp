using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Spliterate

    /// <summary>
    /// Splits this <paramref name="span"/> by the <see cref="MemoryExtensions.IndexOf"/> the given <paramref name="splitter"/>.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// "1-2-3".Spliterate('-')     => ["1", "2", "3"]
    /// "1--2".Spliterate('-')      => ["1", "", "2"]   📎 Note the empty entry in the middle!
    /// "abc".Spliterate('-')       => ["abc"]
    /// "".Spliterate('-')          => []               📎 Note that this does not produce ANY iterations!
    /// "1-2-3".Spliterate('-', 2)  => ["1", "2-3"]     
    /// ]]></code>
    /// </example>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitter">this <paramref name="span"/> will be split by any element that <see cref="IEquatable{T}.Equals(T)"/> an element from this array</param>
    /// <param name="partitionLimit">the maximum number of sub-spans that this span will be split into</param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <returns>a new <see cref="SpanSpliterator{T}"/></returns>
    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, T splitter, int partitionLimit = int.MaxValue) where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitter, partitionLimit);
    }

    /// <summary>
    /// Splits this span by a "sub-sequence," similar to <see cref="MemoryExtensions.IndexOf{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="Spliterate{T}(System.ReadOnlySpan{T},T,int)"/>
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitSequence">the sequence of <typeparamref name="T"/> values that will be used as the splitter</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a new <see cref="SpanSpliterator{T}"/></returns>
    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitSequence) where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitSequence);
    }

    /// <summary>
    /// Splits this span by <i>any</i> of the given values, similarly to <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>. 
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="possibleSplitters"><b>any</b> of these <typeparam name="T"> values can be used to split the span</typeparam></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a new <see cref="SpanSpliterator{T}"/></returns>
    [Pure]
    public static SpanSpliterator<T> SpliterateAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> possibleSplitters) where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, possibleSplitters, SplitterMatchStyle.AnyEntry);
    }

    #endregion
}