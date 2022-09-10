using System;
using System.Collections.Immutable;

using FowlFever.BSharp.Memory;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class ImmutableExtensions {
    #region Spanq

    [MustUseReturnValue]
    public static ImmutableArray<TOut> ToImmutableArray<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut> spanSelector) where TSpan : IEquatable<TSpan> {
        var builder = ImmutableArray.CreateBuilder<TOut>();

        foreach (var span in spliterator) {
            builder.Add(spanSelector(span));
        }

        return builder.MoveToImmutableSafely();
    }

    #endregion
}