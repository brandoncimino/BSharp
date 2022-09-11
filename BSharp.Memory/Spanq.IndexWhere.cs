using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    /// <returns>the first index of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value"/></returns>
    /// <remarks>This is the complement of <see cref="MemoryExtensions.IndexOf{T}(ReadOnlySpan{T},T)"/>.</remarks>
    /// <seealso cref="LastIndexNot{T}"/>
    [Pure]
    public static int IndexNot<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T> {
        for (int i = 0; i < span.Length; i++) {
            if (span[i].Equals(value) == false) {
                return i;
            }
        }

        return -1;
    }

    /// <returns>either:
    /// <list type="table">
    /// <item>
    /// <term><paramref name="positiveMatch"/> = <c>true</c></term>
    /// <description><see cref="MemoryExtensions.IndexOf{T}(System.ReadOnlySpan{T},T)"/></description>
    /// </item>
    /// <item>
    /// <term><paramref name="positiveMatch"/> = <c>false</c></term>
    /// <description><see cref="IndexNot{T}"/></description>
    /// </item>
    /// </list>
    /// </returns>
    [Pure]
    public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, bool positiveMatch)
        where T : IEquatable<T> => positiveMatch switch {
        true  => span.IndexOf(value),
        false => span.IndexNot(value),
    };

    /// <returns>the <b><i>last</i></b> index of <paramref name="span"/> that <b>ISN'T</b> <paramref name="value"/></returns>
    /// <remarks>
    /// This is the complement of <see cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},T)"/>.
    /// </remarks>
    /// <seealso cref="IndexNot{T}"/>
    /// <seealso cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},T)"/>
    [Pure]
    public static int LastIndexNot<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T> {
        for (int i = span.Length - 1; i >= 0; i--) {
            if (span[i].Equals(value) == false) {
                return i;
            }
        }

        return -1;
    }

    /// <returns>either:
    /// <list type="table">
    /// <item>
    /// <term><paramref name="positiveMatch"/> = <c>true</c></term>
    /// <description><see cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},T)"/></description>
    /// </item>
    /// <item>
    /// <term><paramref name="positiveMatch"/> = <c>false</c></term>
    /// <description><see cref="LastIndexNot{T}"/></description>
    /// </item>
    /// </list>
    /// </returns>
    [Pure]
    public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value, bool positiveMatch)
        where T : IEquatable<T> => positiveMatch switch {
        true  => span.IndexOf(value),
        false => span.LastIndexNot(value),
    };

    #region IndexWhere

    /// <summary>
    /// Executes <paramref name="selector"/> against each element of the <paramref name="span"/> and returns the first index where the result
    /// <b>is</b> <i>(if <paramref name="positiveMatch"/> = <c>true</c>)</i> or <b>is not</b> <i>(if <paramref name="positiveMatch"/> = <c>false</c>)</i> equal to <paramref name="expectedValue"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="selector">a <see cref="Func{T,TResult}"/> executed against each entry</param>
    /// <param name="expectedValue">a result of <paramref name="selector"/></param>
    /// <param name="positiveMatch">whether the result of <paramref name="selector"/> <b>should</b> <i>(<c>true</c>)</i> or <b>should not</b> <i>(<c>false</c>)</i> equal <paramref name="expectedValue"/></param>
    /// <typeparam name="T">the <paramref name="span"/> element type</typeparam>
    /// <typeparam name="TExpected">the <paramref name="selector"/> return type</typeparam>
    /// <returns>the first index where <paramref name="selector"/> == <paramref name="expectedValue"/> is equal to <paramref name="positiveMatch"/></returns>
    [MustUseReturnValue]
    public static int IndexWhere<T, TExpected>(
        this                    ReadOnlySpan<T>    span,
        [RequireStaticDelegate] Func<T, TExpected> selector,
        TExpected                                  expectedValue,
        bool                                       positiveMatch = true
    )
        where TExpected : IEquatable<TExpected> {
        for (int i = 0; i < span.Length; i++) {
            if (selector(span[i]).Equals(expectedValue) == positiveMatch) {
                return i;
            }
        }

        return -1;
    }

    /// <inheritdoc cref="IndexWhere{T,TExpected}"/>
    [MustUseReturnValue]
    public static int IndexWhere<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate) => span.IndexWhere(predicate, true);

    [MustUseReturnValue] public static int LastIndexWhere<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate) => span.LastIndexWhere(predicate, true);

    [MustUseReturnValue]
    public static int LastIndexWhere<T, TExpected>(
        this                    ReadOnlySpan<T>    span,
        [RequireStaticDelegate] Func<T, TExpected> selector,
        TExpected                                  expected,
        bool                                       positiveMatch = true
    )
        where TExpected : IEquatable<TExpected> {
        for (int i = span.Length - 1; i >= 0; i--) {
            if (selector(span[i]).Equals(expected) == positiveMatch) {
                return i;
            }
        }

        return -1;
    }

    #endregion
}