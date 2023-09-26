using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Skip'n'Take

    #region Take(Range)

    /// <summary>
    /// Roughly equivalent to .NET 6's <a href="https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.take?view=net-6.0#system-linq-enumerable-take-1(system-collections-generic-ienumerable((-0))-system-range)">Enumerable.Take(source, Range)</a>
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="range">the desired <see cref="Range"/> of entries</param>
    /// <typeparam name="T">the type of entries in the <paramref name="span"/></typeparam>
    /// <returns>as much of the <paramref name="span"/> as the <see cref="Range"/> overlaps</returns>
    [Pure]
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, Range range) {
        var start = Math.Clamp(range.Start.GetOffset(span.Length), 0, span.Length);
        var end   = Math.Clamp(range.End.GetOffset(span.Length),   0, span.Length);
        return span[start..end];
    }

    /// <inheritdoc cref="Take{T}(System.ReadOnlySpan{T},System.Range)"/>
    [Pure]
    public static Span<T> Take<T>(this Span<T> span, Range range) {
        var start = Math.Clamp(range.Start.GetOffset(span.Length), 0, span.Length);
        var end   = Math.Clamp(range.End.GetOffset(span.Length),   0, span.Length);
        return span[start..end];
    }

    #endregion

    #region Skip

    /// <param name="span">this span></param>
    /// <param name="toSkip">the number of entries to skip</param>
    /// <typeparam name="T">the span entry type</typeparam>
    /// <returns>all of the entries after the first <paramref name="toSkip"/></returns>
    [Pure]
    public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int toSkip) {
        var startAt = Math.Clamp(toSkip, 0, span.Length);
        return span[startAt..];
    }

    /// <inheritdoc cref="Skip{T}(System.ReadOnlySpan{T},int)"/>
    [Pure]
    public static Span<T> Skip<T>(this Span<T> span, int toSkip) {
        var startAt = Math.Clamp(toSkip, 0, span.Length);
        return span[startAt..];
    }

    #endregion

    #region

    /// <param name="span">this span</param>
    /// <param name="toTake">the number of entries we want</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the first <paramref name="toTake"/> entries</returns>
    [Pure]
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int toTake) {
        var endAt = Math.Clamp(toTake, 0, span.Length);
        return span[..endAt];
    }

    /// <inheritdoc cref="Take{T}(System.ReadOnlySpan{T},System.Range)"/>
    [Pure]
    public static Span<T> Take<T>(this Span<T> span, int toTake) {
        var endAt = Math.Clamp(toTake, 0, span.Length);
        return span[..endAt];
    }

    #endregion

    #region SkipLast

    /// <inheritdoc cref="Skip{T}(System.ReadOnlySpan{T},int)"/>
    [Pure]
    public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int toSkip) => span.Take(span.Length - toSkip);

    /// <inheritdoc cref="Skip{T}(System.Span{T},int)"/>
    [Pure]
    public static Span<T> SkipLast<T>(this Span<T> span, int toSkip) => span.Take(span.Length - toSkip);

    #endregion

    #region TakeLast

    /// <inheritdoc cref="Take{T}(System.ReadOnlySpan{T},int)"/>
    [Pure]
    public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int toTake) => span.Skip(span.Length - toTake);

    /// <inheritdoc cref="Take{T}(System.Span{T},int)"/>
    [Pure]
    public static Span<T> TakeLast<T>(this Span<T> span, int toTake) => span.Skip(span.Length - toTake);

    #endregion

    #endregion
}