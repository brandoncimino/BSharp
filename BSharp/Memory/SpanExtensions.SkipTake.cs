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

    #region Skip

    [Pure]
    public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int toSkip) => toSkip switch {
        < 0 => span,
        _   => span.SafeSlice(toSkip..)
    };

    [Pure] public static ReadOnlySpan<T> SkipWhile<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate)                => span.Skip(span.IndexWhere(predicate, false));
    [Pure] public static ReadOnlySpan<T> SkipWhile<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, int skipLimit) => span.Skip(span.IndexWhere(predicate, false).Limit(skipLimit));

    [Pure]
    public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int toSkip) {
        return toSkip switch {
            < 0 => span,
            _   => span.SafeSlice(..^toSkip)
        };
    }

    [Pure] public static ReadOnlySpan<T> SkipLastWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)                => span.SkipLast(span.LastIndexWhere(predicate, false));
    [Pure] public static ReadOnlySpan<T> SkipLastWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, int skipLimit) => span.SkipLast(span.LastIndexWhere(predicate, false).Limit(skipLimit));

    #endregion

    #region Skip

    [Pure]
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int toTake) => toTake switch {
        < 0 => default,
        _   => span.SafeSlice(..toTake)
    };

    [Pure] public static ReadOnlySpan<T> TakeWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)                => span.Take(span.IndexWhere(predicate, false));
    [Pure] public static ReadOnlySpan<T> TakeWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, int skipLimit) => span.Take(span.IndexWhere(predicate, false).Limit(skipLimit));

    [Pure]
    public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int toTake) => toTake switch {
        < 0 => default,
        _   => span.SafeSlice(^toTake..)
    };

    [Pure] public static ReadOnlySpan<T> TakeLastWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)                => span.Take(span.LastIndexWhere(predicate, false));
    [Pure] public static ReadOnlySpan<T> TakeLastWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, int skipLimit) => span.Take(span.LastIndexWhere(predicate, false).Limit(skipLimit));

    #endregion

    [Pure]
    public static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> span, T toTrim, int trimLimit)
        where T : IEquatable<T> {
        return span.Skip(span.IndexNot(toTrim).Limit(trimLimit));
    }

    [Pure]
    public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> span, T toTrim, int numberToTrim)
        where T : IEquatable<T> {
        return span.SkipLast(span.LastIndexNot(toTrim).Limit(numberToTrim));
    }

    #endregion
}