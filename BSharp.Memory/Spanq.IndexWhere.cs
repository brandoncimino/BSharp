using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    /// <param name="span">the span to search</param>
    /// <param name="value">the value to avoid</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the first index of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value"/></returns>
    /// <remarks>This is the complement of <see cref="MemoryExtensions.IndexOf{T}(ReadOnlySpan{T},T)"/>, and forwards to the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/System.MemoryExtensions.IndexOfAnyExcept">IndexOfAnyExcept</a> if possible.</remarks>
    /// <seealso cref="LastIndexNot{T}(System.ReadOnlySpan{T},T)"/> 
    [Pure]
    public static int IndexNot<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.IndexOfAnyExcept(value);
#else
        for (int i = 0; i < span.Length; i++) {
            if (span[i].Equals(value) == false) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="IndexNot{T}(System.ReadOnlySpan{T},T)"/>
    [Pure]
    public static int IndexNot<T>(this Span<T> span, T value) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).IndexNot(value);

    /// <param name="span">the span to search</param>
    /// <param name="value0">the first unwanted value</param>
    /// <param name="value1">the second unwanted value</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the first index of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value0"/> or <paramref name="value1"/></returns>
    /// <remarks>This is the complement of <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},T,T)"/>, and forwards to the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.indexofanyexcept#system-memoryextensions-indexofanyexcept-1(system-readonlyspan((-0))-0-0)">IndexOfAnyExcept</a> if possible.</remarks>
    [Pure]
    public static int IndexNot<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.IndexOfAnyExcept(value0, value1);
#else
        for (int i = 0; i < span.Length; i++) {
            if (span[i].Equals(value0) == false && span[i].Equals(value1) == false) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="IndexNot{T}(System.ReadOnlySpan{T},T,T)"/>
    [Pure]
    public static int IndexNot<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).IndexNot(value0, value1);

    /// <inheritdoc cref="IndexNot{T}(System.ReadOnlySpan{T},T,T)"/>
    /// <param name="value2">the third unwanted value</param>
    /// <returns>the first index of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value0"/>, <paramref name="value1"/> or <paramref name="value2"/></returns>
    /// <remarks>This is the complement of <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},T,T,T)"/>, and forwards to the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.indexofanyexcept#system-memoryextensions-indexofanyexcept-1(system-readonlyspan((-0))-0-0-0)">IndexOfAnyExcept</a> if possible.</remarks>
    [Pure]
    [SuppressMessage("ReSharper", "InvalidXmlDocComment", Justification = "Inherited docs")]
    public static int IndexNot<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.IndexOfAnyExcept(value0, value1, value2);
#else

        for (int i = 0; i < span.Length; i++) {
            // TODO: Benchmark -
            //  if:
            //      a == b == c
            //      x != a
            //  which is faster:
            //      x == a || x == b || x == c
            //  or
            //      a == b == c ?
            //          x == a  :
            //          x == a || x == b || x == c
            var it = span[i];
            if (it.Equals(value0) == false && it.Equals(value1) == false && it.Equals(value2) == false) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="IndexNot{T}(System.ReadOnlySpan{T},T,T,T)"/>
    [Pure]
    public static int IndexNot<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).IndexNot(value0, value1, value2);

    /// <param name="span">the span to search</param>
    /// <param name="values">the unwanted values</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the first index of the <paramref name="span"/> that <b>ISN'T</b> one of the unwanted <paramref name="values"/></returns>
    /// <remarks>This is the complement of <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>, and forwards to the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.indexofanyexcept#system-memoryextensions-indexofanyexcept-1(system-readonlyspan((-0))-system-readonlyspan((-0)))">IndexOfAnyExcept</a> if possible.</remarks>
    [Pure]
    public static int IndexNot<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.IndexOfAnyExcept(values);
#else
        for (int i = 0; i < span.Length; i++) {
            if (values.IndexOf(span[i]) < 0) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="IndexNot{T}(System.ReadOnlySpan{T},ReadOnlySpan{T})"/>
    [Pure]
    public static int IndexNot<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).IndexNot(values);

    /// <param name="span">the span to search</param>
    /// <param name="value">the unwanted value</param>
    /// <returns>the <b><i>last</i></b> index of <paramref name="span"/> that <b>ISN'T</b> <paramref name="value"/></returns>
    /// <remarks>
    /// This is the complement of <see cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},T)"/>, and forwards to the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.lastindexofanyexcept#system-memoryextensions-lastindexofanyexcept-1(system-readonlyspan((-0))-0)">LastIndexOfAnyExcept</a> if possible.
    /// </remarks>
    /// <seealso cref="IndexNot{T}(System.ReadOnlySpan{T},T)"/>
    /// <seealso cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},T)"/>
    [Pure]
    public static int LastIndexNot<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.LastIndexOfAnyExcept(value);
#else
        for (int i = span.Length - 1; i >= 0; i--) {
            if (span[i].Equals(value) == false) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="LastIndexNot{T}(System.ReadOnlySpan{T},T)"/>
    [Pure]
    public static int LastIndexNot<T>(this Span<T> span, T value) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).LastIndexNot(value);

    /// <param name="span">the span to search</param>
    /// <param name="value0">the first unwanted value</param>
    /// <param name="value1">the second unwanted value</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the <b><i>last</i></b> index of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value0"/> or <paramref name="value1"/></returns>
    [Pure]
    public static int LastIndexNot<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.LastIndexOfAnyExcept(value0, value1);
#else
        for (int i = span.Length - 1; i >= 0; i--) {
            var it = span[i];
            if ((it.Equals(value0) || it.Equals(value1)) == false) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="LastIndexNot{T}(System.ReadOnlySpan{T},T,T)"/>
    [Pure]
    public static int LastIndexNot<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).LastIndexNot(value0, value1);

    /// <param name="span">the span to search</param>
    /// <param name="value0">the first unwanted value</param>
    /// <param name="value1">the second unwanted value</param>
    /// <param name="value2">the third unwanted value</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the <b><i>last</i></b> element of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value0"/>, <paramref name="value1"/> or <paramref name="value2"/></returns>
    [Pure]
    public static int LastIndexNot<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.LastIndexOfAnyExcept(value0, value1, value2);
#else
        for (int i = span.Length - 1; i >= 0; i--) {
            var it = span[i];
            if ((it.Equals(value0) || it.Equals(value1) || it.Equals(value2)) == false) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="LastIndexNot{T}(System.ReadOnlySpan{T},T,T,T)"/>
    [Pure]
    public static int LastIndexNot<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).LastIndexNot(value0, value1, value2);

    /// <param name="span">the span to search</param>
    /// <param name="values">the unwanted values</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the <b><i>last</i></b> index of the <paramref name="span"/> that <b>ISN'T</b> one of the unwanted <paramref name="values"/></returns>
    [Pure]
    public static int LastIndexNot<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T> {
#if NET7_0_OR_GREATER
        return span.LastIndexOfAnyExcept(values);
#else
        for (int i = span.Length - 1; i >= 0; i--) {
            if (values.IndexOf(span[i]) < 0) {
                return i;
            }
        }

        return -1;
#endif
    }

    /// <inheritdoc cref="LastIndexNot{T}(System.ReadOnlySpan{T},ReadOnlySpan{T})"/>
    [Pure]
    public static int LastIndexNot<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T> => ((ReadOnlySpan<T>)span).LastIndexNot(values);

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