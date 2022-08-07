using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class SpanExtensions {
    #region Skip'n'Take

    /// <summary>
    /// Roughly equivalent to .NET 6's <a href="https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.take?view=net-6.0#system-linq-enumerable-take-1(system-collections-generic-ienumerable((-0))-system-range)">Enumerable.Take(source, Range)</a>
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="range">the desired <see cref="Range"/> of entries</param>
    /// <typeparam name="T">the type of entries in the <paramref name="span"/></typeparam>
    /// <returns>as much of the <paramref name="span"/> as the <see cref="Range"/> overlaps</returns>
    [Pure]
    public static ReadOnlySpan<T> SafeSlice<T>(this ReadOnlySpan<T> span, Range range) => span[range.Clamp(span.Length)];

    [Pure]
    public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int toSkip) => toSkip switch {
        <= 0                         => span,
        _ when toSkip >= span.Length => default,
        _                            => span[toSkip..]
    };

    [Pure]
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int toTake) => toTake switch {
        <= 0                         => default,
        _ when toTake >= span.Length => span,
        _                            => span[..^toTake]
    };

    #region {x}Last

    [Pure] public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int toSkip) => span.Take(span.Length - toSkip);
    [Pure] public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int toTake) => span.Skip(span.Length - toTake);

    #endregion

    #region SkipWhile

    [Pure]
    public static ReadOnlySpan<T> SkipWhile<T, T2>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, T2> selector, T2 expected, int skipLimit = int.MaxValue)
        where T2 : IEquatable<T2> => span.Skip(span.CountWhile(selector, expected));

    [Pure] public static ReadOnlySpan<T> SkipWhile<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, int skipLimit = int.MaxValue) => span.SkipWhile(predicate, true, skipLimit);

    [Pure]
    public static ReadOnlySpan<T> SkipLastWhile<T, T2>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, T2> selector, T2 equals, int skipLimit = int.MaxValue)
        where T2 : IEquatable<T2> =>
        span.SkipLast(span.CountLastWhile(selector, equals, skipLimit));

    [Pure] public static ReadOnlySpan<T> SkipLastWhile<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, int skipLimit = int.MaxValue) => span.SkipLastWhile(predicate, true, skipLimit);

    #endregion

    #region TakeWhile

    [Pure]
    public static ReadOnlySpan<T> TakeWhile<T, T2>(this ReadOnlySpan<T> span, Func<T, T2> selector, T2 expected, int takeLimit = int.MaxValue)
        where T2 : IEquatable<T2> => span.Take(span.CountWhile(selector, expected).Min(takeLimit));

    [Pure] public static ReadOnlySpan<T> TakeWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, int takeLimit = int.MaxValue) => span.TakeWhile(predicate, true, takeLimit);

    [Pure]
    public static ReadOnlySpan<T> TakeLastWhile<T, T2>(this ReadOnlySpan<T> span, Func<T, T2> selector, T2 expected, int takeLimit = int.MaxValue)
        where T2 : IEquatable<T2> => span.Take(span.CountLastWhile(selector, expected).Min(takeLimit));

    [Pure] public static ReadOnlySpan<T> TakeLastWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, int takeLimit = int.MaxValue) => span.TakeLastWhile(predicate, true);

    #endregion

    #endregion
}