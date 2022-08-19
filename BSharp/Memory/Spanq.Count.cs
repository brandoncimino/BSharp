using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Count

    [Pure]
    [NonNegativeValue]
    public static int Count<T, T2>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, T2> selector, T2 expected, int countLimit = int.MaxValue)
        where T2 : IEquatable<T2> {
        if (span.IsEmpty || countLimit <= 0) {
            return 0;
        }

        countLimit = Math.Max(span.Length, countLimit);

        int hits = 0;

        foreach (var t in span) {
            if (selector(t).Equals(expected) == false) {
                return hits;
            }

            hits += 1;

            if (hits >= countLimit) {
                return countLimit;
            }
        }

        return span.Length;
    }

    [Pure]
    [NonNegativeValue]
    public static int Count<T>(
        this                    ReadOnlySpan<T> span,
        [RequireStaticDelegate] Func<T, bool>   predicate,
        int                                     countLimit = int.MaxValue
    ) =>
        span.Count(
            predicate,
            true,
            countLimit
        );

    [Pure]
    [NonNegativeValue]
    public static int CountWhile<T, T2>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, T2> selector, T2 expected, int countLimit = int.MaxValue)
        where T2 : IEquatable<T2> {
        if (span.IsEmpty || countLimit <= 0) {
            return 0;
        }

        countLimit = Math.Min(span.Length, countLimit);
        Brandon.Print(countLimit);
        Brandon.Print(selector.Method.Name);

        for (int i = 0; i < countLimit; i++) {
            Brandon.Print(i);
            Brandon.Print(span[i]);
            Brandon.Print(selector(span[i]));
            if (selector(span[i]).Equals(expected) == false) {
                return i;
            }
        }

        return countLimit;
    }

    [Pure]
    [NonNegativeValue]
    public static int CountWhile<T>(
        this                    ReadOnlySpan<T> span,
        [RequireStaticDelegate] Func<T, bool>   predicate,
        int                                     countLimit = int.MaxValue
    ) =>
        span.CountWhile(
            predicate,
            true,
            countLimit
        );

    [Pure]
    [NonNegativeValue]
    public static int CountLastWhile<T, T2>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, T2> selector, T2 expected, int countLimit = int.MaxValue)
        where T2 : IEquatable<T2> {
        if (span.IsEmpty || countLimit <= 0) {
            return 0;
        }

        countLimit = Math.Min(span.Length, countLimit);

        for (int i = 0; i < span.Length; i++) {
            if (selector(span[^(i + 1)]).Equals(expected) == false) {
                return i;
            }
        }

        return countLimit;
    }

    [Pure]
    [NonNegativeValue]
    public static int CountLastWhile<T>(
        this                    ReadOnlySpan<T> span,
        [RequireStaticDelegate] Func<T, bool>   predicate,
        int                                     countLimit = int.MaxValue
    ) =>
        span.CountLastWhile(predicate, true, countLimit);

    #endregion
}