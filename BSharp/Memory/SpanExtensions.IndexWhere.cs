using System;

using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class SpanExtensions {
    #region IndexWhere

    [MustUseReturnValue]
    public static int IndexWhere<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, Polarity polarity = Polarity.Positive)
        => span.IndexWhere(predicate, true, polarity);

    [MustUseReturnValue]
    public static int IndexWhere<T, TExpected>(
        this                    ReadOnlySpan<T>    span,
        [RequireStaticDelegate] Func<T, TExpected> selector,
        TExpected                                  expectedValue,
        Polarity                                   polarity = Polarity.Positive
    )
        where TExpected : IEquatable<TExpected> {
        for (int i = 0; i < span.Length; i++) {
            if (selector(span[i]).Equals(expectedValue) == polarity is Polarity.Positive) {
                return i;
            }
        }

        return -1;
    }

    [MustUseReturnValue]
    public static int LastIndexWhere<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, Polarity polarity = Polarity.Positive)
        => span.LastIndexWhere(predicate, true, polarity);

    [MustUseReturnValue]
    public static int LastIndexWhere<T, TExpected>(
        this                    ReadOnlySpan<T>    span,
        [RequireStaticDelegate] Func<T, TExpected> selector,
        TExpected                                  expected,
        Polarity                                   polarity = Polarity.Positive
    )
        where TExpected : IEquatable<TExpected> {
        for (int i = span.Length - 1; i >= 0; i--) {
            if (selector(span[i]).Equals(expected) == polarity is Polarity.Positive) {
                return i;
            }
        }

        return -1;
    }

    #endregion
}