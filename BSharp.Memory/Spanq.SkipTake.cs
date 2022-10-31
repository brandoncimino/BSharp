using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Skip'n'Take

    /// <summary>
    /// Roughly equivalent to .NET 6's <a href="https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.take?view=net-6.0#system-linq-enumerable-take-1(system-collections-generic-ienumerable((-0))-system-range)">Enumerable.Take(source, Range)</a>
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="range">the desired <see cref="Range"/> of entries</param>
    /// <typeparam name="T">the type of entries in the <paramref name="span"/></typeparam>
    /// <returns>as much of the <paramref name="span"/> as the <see cref="Range"/> overlaps</returns>
    [Pure]
    public static ReadOnlySpan<T> SafeSlice<T>(this ReadOnlySpan<T> span, Range range) {
        var start = Math.Clamp(range.Start.GetOffset(span.Length), 0, span.Length);
        var end   = Math.Clamp(range.End.GetOffset(span.Length),   0, span.Length);
        return span[start..end];
    }

    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="toSkip">the number of entries to skip</param>
    /// <typeparam name="T">the span entry type</typeparam>
    /// <returns>all of the entries after the first <paramref name="toSkip"/></returns>
    [Pure]
    public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int toSkip) => toSkip switch {
        <= 0                         => span,
        _ when toSkip >= span.Length => default,
        _                            => span[toSkip..]
    };

    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="toTake">the number of entries we want</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>the first <paramref name="toTake"/> entries</returns>
    [Pure]
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int toTake) => toTake switch {
        <= 0                         => default,
        _ when toTake >= span.Length => span,
        _                            => span[..toTake]
    };

    public static RoSpanTuple<T, T> TakeLeftovers<T>(this ReadOnlySpan<T> span, int toTake) =>
        new() {
            A = span.Take(toTake),
            B = span.Skip(toTake),
        };

    #region {x}Last

    /// <inheritdoc cref="Skip{T}"/>
    [Pure]
    public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int toSkip) => span.Take(span.Length - toSkip);

    /// <inheritdoc cref="Take{T}"/>
    [Pure]
    public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int toTake) => span.Skip(span.Length - toTake);

    #endregion

    #region SkipWhile

    /// <summary>
    /// [Skip/Take]s up to <paramref name="limit"/> entries where <paramref name="selector"/> returns <paramref name="expected"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="selector">transforms entries in the <paramref name="span"/></param>
    /// <param name="expected">if <paramref name="selector"/> returns this, we [skip/take] that entry</param>
    /// <param name="limit">the maximum number of entries we can [skip/take]</param>
    /// <param name="from">whether to skip from the start or end of the <paramref name="span"/></param>
    /// <typeparam name="T">the span entry type</typeparam>
    /// <typeparam name="TExpected">the <paramref name="selector"/> output type</typeparam>
    /// <returns>this <paramref name="span"/>, possibly with some leading entries [skipped/taken]</returns>
    [Pure]
    public static ReadOnlySpan<T> SkipWhile<T, TExpected>(
        this                    ReadOnlySpan<T>    span,
        [RequireStaticDelegate] Func<T, TExpected> selector,
        TExpected                                  expected,
        int                                        limit = int.MaxValue,
        From                                       from  = From.Start
    )
        where TExpected : IEquatable<TExpected> {
        return span.Skip(span.CountWhile(selector, expected, limit, from));
    }

    /// <summary>
    /// [Skip/Take]s up to <paramref name="limit"/> entries where <paramref name="predicate"/> returns <c>true</c>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="predicate">if this returns <c>true</c>, we should [skip/take] this entry</param>
    /// <param name="limit">the maximum number of entries to [skip/take]</param>
    /// <param name="from">whether to skip from the start or end of the <paramref name="span"/></param>
    /// <typeparam name="T">the span entry type</typeparam>
    /// <returns>this <paramref name="span"/>, possibly with some leading entries removed</returns>
    [Pure]
    public static ReadOnlySpan<T> SkipWhile<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, int limit = int.MaxValue, From from = From.Start) {
        return span.SkipWhile(predicate, true, limit, from);
    }

    #endregion

    #endregion
}