using System;
using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Spliterate

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, StringSplitOptions options, params T[] splitters)
        where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitters, SplitterStyle.AnyEntry, options);
    }

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, params T[] splitters)
        where T : IEquatable<T> {
        return span.Spliterate(StringSplitOptions.None, splitters);
    }

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters, SplitterStyle splitterStyle = SplitterStyle.AnyEntry, StringSplitOptions options = StringSplitOptions.None)
        where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitters, splitterStyle, options);
    }

    [MustUseReturnValue]
    public static ImmutableArray<string> ToStringArray(this SpanSpliterator<char> spliterator) {
        var arr = ImmutableArray.CreateBuilder<string>();

        foreach (var span in spliterator) {
            arr.Add(span.ToString());
        }

        return arr.MoveToImmutableSafely();
    }

    #endregion

    #region Collecting Spliterators

    [MustUseReturnValue]
    public static ImmutableArray<TOut> ToImmutableArray<TIn, TArg, TOut>(this SpanSpliterator<TIn> spliterator, ReadOnlySpanFunc<TIn, TArg, TOut> factory, TArg arg)
        where TIn : IEquatable<TIn> {
        var arr          = ImmutableArray.CreateBuilder<TOut>();
        int currentIndex = 0;
        foreach (var span in spliterator) {
            arr[currentIndex] =  factory(span, arg);
            currentIndex      += 1;
        }

        return arr.MoveToImmutable();
    }

    [MustUseReturnValue]
    public static ImmutableArray<TOut> ToImmutableArray<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut> factory)
        where TSpan : IEquatable<TSpan> {
        var arr = ImmutableArray.CreateBuilder<TOut>();
        foreach (var span in spliterator) {
            arr.Add(factory(span));
        }

        return arr.MoveToImmutableSafely();
    }

    [MustUseReturnValue]
    public static RoMultiSpan<T> ToMultiSpan<T>(this SpanSpliterator<T> spliterator) where T : IEquatable<T> {
        var multiSpan = default(RoMultiSpan<T>);

        foreach (var span in spliterator) {
            multiSpan = multiSpan.Add(span);
        }

        return multiSpan;
    }

    [MustUseReturnValue]
    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] Func<TOut> initialFactory, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator)
        where TSpan : IEquatable<TSpan> {
        var soFar = initialFactory();
        foreach (var span in spliterator) {
            soFar = aggregator(span, soFar);
        }

        return soFar;
    }

    #endregion
}