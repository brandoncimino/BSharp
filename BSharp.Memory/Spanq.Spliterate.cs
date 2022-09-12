using System;
using System.Buffers;
using System.Collections.Generic;

using JetBrains.Annotations;

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

    #region Collecting Spliterators

    [MustUseReturnValue]
    public static RoMultiSpan<T> ToMultiSpan<T>(this SpanSpliterator<T> spliterator) where T : IEquatable<T> {
        var multiSpan = default(RoMultiSpan<T>);

        foreach (var span in spliterator) {
            multiSpan = multiSpan.Add(span);
        }

        return multiSpan;
    }

    [MustUseReturnValue]
    public static List<TOut> ToList<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut> spanConverter) where TSpan : IEquatable<TSpan> {
        var ls    = new List<TOut>();
        var aggro = spliterator.Aggregate((ls, spanConverter), static (span, args) => args.ls.Add(args.spanConverter(span)));
        return aggro.ls;
    }

    [MustUseReturnValue] public static List<string> ToStringList(this SpanSpliterator<char> spliterator) => spliterator.ToList(static it => it.ToString());

    [MustUseReturnValue]
    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, TOut seed, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator) where TSpan : IEquatable<TSpan> {
        foreach (var span in spliterator) {
            seed = aggregator(span, seed);
        }

        return seed;
    }

    [MustUseReturnValue]
    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, TOut seed, [RequireStaticDelegate] ReadOnlySpanAction<TSpan, TOut> aggregator) where TSpan : IEquatable<TSpan> {
        foreach (var span in spliterator) {
            aggregator(span, seed);
        }

        return seed;
    }

    #endregion
}