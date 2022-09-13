using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Spliterate

    /// <summary>
    /// Splits this <paramref name="span"/> by <see cref="SplitterMatchStyle.AnyEntry"/> of <paramref name="splitters"/>.
    /// </summary>
    /// <remarks>
    /// Additional options for the <see cref="SpanSpliterator{T}"/> can be provided using a <c>with</c> expression.
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitters">this <paramref name="span"/> will be split by any element that <see cref="IEquatable{T}.Equals(T)"/> an element from this array</param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <returns>a new <see cref="SpanSpliterator{T}"/></returns>
    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, params T[] splitters) where T : IEquatable<T> => new(span, splitters) { MatchStyle = SplitterMatchStyle.AnyEntry };

    /// <summary>
    /// Splits this <paramref name="span"/> by a <see cref="SplitterMatchStyle.SubSequence"/>.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref=""/>
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitSequence"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitSequence) where T : IEquatable<T> => new(span, splitSequence);

    #endregion
}