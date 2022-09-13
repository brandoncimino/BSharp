using System;
using System.Buffers;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region To{X}

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

    [Pure]
    public static OUT[] ToArray<T, OUT>(this RoMultiSpan<T> spans, [RequireStaticDelegate] ReadOnlySpanFunc<T, OUT> spanConverter) {
        var ar = new OUT[spans.SpanCount];

        for (int i = 0; i < spans.SpanCount; i++) {
            ar[i] = spanConverter(spans[i]);
        }

        return ar;
    }

    #region ToString{X}

    [MustUseReturnValue] public static List<string> ToStringList(this  SpanSpliterator<char> spliterator) => spliterator.ToList(static it => it.ToString());
    [Pure]               public static string[]     ToStringArray(this RoMultiSpan<char>     spans)       => spans.ToArray(static it => it.ToString());

    #endregion

    [Pure] public static T[][] ToJaggedArray<T>(this RoMultiSpan<T> spans) => spans.ToArray(static span => span.ToArray());

    #endregion

    #region Aggregate

    #region Chained Aggregator `Func`

    [MustUseReturnValue]
    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, TOut seed, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator) where TSpan : IEquatable<TSpan> {
        foreach (var span in spliterator) {
            seed = aggregator(span, seed);
        }

        return seed;
    }

    [Pure]
    public static TOut Aggregate<TSpan, TOut>(this RoMultiSpan<TSpan> spans, TOut seed, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator) {
        foreach (var span in spans) {
            seed = aggregator(span, seed);
        }

        return seed;
    }

    #endregion

    #region Unchained Aggregator `Action`

    [MustUseReturnValue]
    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, TOut seed, [RequireStaticDelegate] ReadOnlySpanAction<TSpan, TOut> aggregator) where TSpan : IEquatable<TSpan> {
        foreach (var span in spliterator) {
            aggregator(span, seed);
        }

        return seed;
    }

    [Pure]
    public static TOut Aggregate<TSpan, TOut>(this RoMultiSpan<TSpan> spans, TOut seed, [RequireStaticDelegate] ReadOnlySpanAction<TSpan, TOut> aggregator) {
        foreach (var span in spans) {
            aggregator(span, seed);
        }

        return seed;
    }

    #endregion

    #endregion
}