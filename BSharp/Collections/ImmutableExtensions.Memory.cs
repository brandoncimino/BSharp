using System.Collections.Immutable;

using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Memory.Enumerators;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class ImmutableExtensions {
    #region Spanq

    [MustUseReturnValue]
    public static ImmutableArray<TOut> ToImmutableArray<TSpan, TArg, TOut>(this SpanSpliterator<TSpan> spliterator, TArg arg, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TArg, TOut> spanSelector) where TSpan : IEquatable<TSpan> {
        var builder = ImmutableArray.CreateBuilder<TOut>();

        foreach (var span in spliterator) {
            builder.Add(spanSelector(span, arg));
        }

        return builder.MoveToImmutableSafely();
    }

    [MustUseReturnValue]
    public static ImmutableArray<TOut> ToImmutableArray<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut> spanSelector) where TSpan : IEquatable<TSpan> {
        //TODO: Benchmark this double-delegate version vs. duplicating the code from the other ToImmutableArray() overload
        return spliterator.ToImmutableArray<TSpan, ReadOnlySpanFunc<TSpan, TOut>, TOut>(spanSelector, static (span, func) => func(span));
    }

    [MustUseReturnValue]
    public static ImmutableArray<string> ToImmutableStringArray(this SpanSpliterator<char> spliterator) {
        return spliterator.ToImmutableArray(static span => span.ToString());
    }

    #endregion
}