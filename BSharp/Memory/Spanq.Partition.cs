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

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, T)"/>
    public static SpanPartition<T> Partition<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> {
        return new SpanPartition<T>(span, splitter);
    }

    /// <inheritdoc cref="SpanPartition{T}(ReadOnlySpan{T}, Range)"/>
    public static SpanPartition<T> Partition<T>(this ReadOnlySpan<T> span, Range range)
        where T : IEquatable<T> {
        return new SpanPartition<T>(span, range);
    }

    #endregion
}