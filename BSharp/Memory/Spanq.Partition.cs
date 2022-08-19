using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Partition

    /// <summary>
    /// Splits a <paramref name="span"/> by the first instance of <see cref="splitSequence"/>, returning the parts before and after.
    /// <p/>
    /// If <paramref name="splitSequence"/> isn't found, <see cref="SpanPartition{T}.IsSuccessful"/> will be <c>false</c> and <see cref="SpanPartition{T}.Before"/> and <see cref="SpanPartition{T}.After"/> will be <see cref="ReadOnlySpan{T}.IsEmpty"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="splitSequence">the inner <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns>the <see cref="ReadOnlySpan{T}"/> before and after <paramref name="splitSequence"/>, if found; otherwise, <c>default</c></returns>
    public static SpanPartition<T> Partition<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitSequence)
        where T : IEquatable<T> {
        return new SpanPartition<T>(span, splitSequence);
    }

    /// <inheritdoc cref="Partition{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>
    public static bool Partition<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitSequence, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after)
        where T : IEquatable<T> {
        span.Partition(splitSequence).Deconstruct(out var successful, out before, out after);
        return successful;
    }

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, T)"/>
    public static SpanPartition<T> Partition<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> {
        return new SpanPartition<T>(span, splitter);
    }

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, T)"/>
    public static bool Partition<T>(this ReadOnlySpan<T> span, T splitter, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after)
        where T : IEquatable<T> {
        span.Partition(splitter).Deconstruct(out var successful, out before, out after);
        return successful;
    }

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, Range)"/>
    public static SpanPartition<T> Partition<T>(this ReadOnlySpan<T> span, Range range)
        where T : IEquatable<T> {
        return new SpanPartition<T>(span, range);
    }

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, Range)"/>
    public static bool Partition<T>(this ReadOnlySpan<T> span, Range range, out ReadOnlySpan<T> before, out ReadOnlySpan<T> after)
        where T : IEquatable<T> {
        span.Partition(range).Deconstruct(out var successful, out before, out after);
        return successful;
    }

    #endregion
}