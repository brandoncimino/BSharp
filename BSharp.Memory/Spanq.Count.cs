using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Count

    public enum From : byte { Start, End }

    #region Helpers

    private static T Get<T>(this From from, ReadOnlySpan<T> span, int offset) => from == From.Start ? span[offset] : span[^(offset + 1)];

    private static bool _TryExitEarly<T>(ReadOnlySpan<T> span, ref int countLimit) {
        if (span.IsEmpty || countLimit <= 0) {
            return true;
        }

        countLimit = Math.Min(span.Length, countLimit);
        return false;
    }

    #endregion

    [Pure]
    [NonNegativeValue]
    public static int CountWhileAny<T>(
        this ReadOnlySpan<T> span,
        ReadOnlySpan<T>      anyOf,
        int                  countLimit = int.MaxValue,
        From                 from       = From.Start
    ) where T : IEquatable<T> {
        if (_TryExitEarly(span, ref countLimit)) {
            return 0;
        }

        for (int i = 0; i < countLimit; i++) {
            var elem = from.Get(span, i);
            if (anyOf.Contains(elem) == false) {
                return i;
            }
        }

        return countLimit;
    }

    /// <summary>
    /// Counts the number of [leading/trailing] elements in this <paramref name="span"/> for whom <paramref name="selector"/> returns <paramref name="expected"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="arg">an argument passed to the <paramref name="selector"/> function</param>
    /// <param name="selector">transforms <typeparamref name="TSpan"/> into <typeparamref name="TExpected"/></param>
    /// <param name="expected">if <paramref name="selector"/> returns this, we should count that element</param>
    /// <param name="from">whether we should count starting from the first or last entry in the <paramref name="span"/></param>
    /// <param name="countLimit">the maximum number of elements to count</param>
    /// <typeparam name="TSpan">the span element type</typeparam>
    /// <typeparam name="TArg">the type of <paramref name="arg"/></typeparam>
    /// <typeparam name="TExpected">the type of <paramref name="expected"/></typeparam>
    /// <returns>the number of leading elements that satisfy the <paramref name="selector"/> <i>(up to <paramref name="countLimit"/>)</i></returns>
    [Pure]
    [NonNegativeValue]
    public static int CountWhile<TSpan, TArg, TExpected>(
        this ReadOnlySpan<TSpan>                             span,
        TArg                                                 arg,
        [RequireStaticDelegate] Func<TSpan, TArg, TExpected> selector,
        TExpected                                            expected,
        int                                                  countLimit = int.MaxValue,
        From                                                 from       = From.Start
    )
        where TExpected : IEquatable<TExpected> {
        if (_TryExitEarly(span, ref countLimit)) {
            return 0;
        }

        for (int i = 0; i < countLimit; i++) {
            var elem     = from.Get(span, i);
            var selected = selector(elem, arg);
            if (selected.Equals(expected) == false) {
                return i;
            }
        }

        return countLimit;
    }

    /// <inheritdoc cref="CountWhile{TSpan,TArg,TExpected}"/>
    [Pure]
    [NonNegativeValue]
    public static int CountWhile<TSpan, TExpected>(
        this                    ReadOnlySpan<TSpan>    span,
        [RequireStaticDelegate] Func<TSpan, TExpected> selector,
        TExpected                                      expected,
        int                                            countLimit = int.MaxValue,
        From                                           from       = From.Start
    )
        where TExpected : IEquatable<TExpected> {
        return span.CountWhile(
            selector,
            static (elem, del) => del(elem),
            expected,
            countLimit,
            from
        );
    }

    /// <inheritdoc cref="CountWhile{TSpan,TArg,TExpected}"/>
    [Pure]
    [NonNegativeValue]
    public static int CountWhile<T>(
        this                    ReadOnlySpan<T> span,
        [RequireStaticDelegate] Func<T, bool>   predicate,
        int                                     countLimit = int.MaxValue,
        From                                    from       = From.Start
    ) {
        if (_TryExitEarly(span, ref countLimit)) {
            return 0;
        }

        for (int i = 0; i < countLimit; i++) {
            var elem = from.Get(span, i);
            if (predicate(elem) == false) {
                return i;
            }
        }

        return countLimit;
    }

    /// <inheritdoc cref="CountWhile{TSpan,TArg,TExpected}"/>
    [Pure]
    [NonNegativeValue]
    public static int CountWhile<T>(
        this ReadOnlySpan<T> span,
        T                    expected,
        int                  countLimit = int.MaxValue,
        From                 from       = From.Start
    )
        where T : IEquatable<T> {
        if (_TryExitEarly(span, ref countLimit)) {
            return 0;
        }

        for (int i = 0; i < countLimit; i++) {
            var elem = from.Get(span, i);
            if (elem.Equals(expected) == false) {
                return i;
            }
        }

        return countLimit;
    }

    #endregion
}