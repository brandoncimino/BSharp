using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    #region Span Retrieval

    /// <summary>
    /// Retrieves one of the contained <see cref="ReadOnlySpan{T}"/>s by its zero-based index.
    /// </summary>
    /// <param name="spanIndex">the index of one of the <see cref="ReadOnlySpan{T}"/>s in this <see cref="RoMultiSpan{T}"/></param>
    /// <returns>the corresponding <see cref="ReadOnlySpan{T}"/></returns>
    /// <exception cref="IndexOutOfRangeException">if <see cref="spanIndex"/> is &lt; 0 or &gt;= <see cref="SpanCount"/></exception>
    public ReadOnlySpan<T> GetSpan([ValueRange(0, MaxSpans)] int spanIndex) {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (spanIndex < 0 || spanIndex > SpanCount) {
            throw new ArgumentOutOfRangeException(nameof(spanIndex), $"{nameof(spanIndex)} {spanIndex} is out-of-bounds: this {nameof(RoMultiSpan<T>)} contains {SpanCount} spans!");
        }

        return spanIndex switch {
            0 => _a,
            1 => _b,
            2 => _c,
            3 => _d,
            4 => _e,
            5 => _f,
            6 => _g,
            7 => _h,
            _ => throw new InvalidOperationException("This code should have been unreachable!"),
        };
    }

    /// <summary>
    /// <inheritdoc cref="GetSpan"/>
    /// </summary>
    /// <param name="spanIndex">the index of one of the <see cref="ReadOnlySpan{T}"/>s in this <see cref="RoMultiSpan{T}"/></param>
    /// <param name="span">the retrieved <see cref="ReadOnlySpan{T}"/></param>
    /// <returns><c>true</c> if <paramref name="spanIndex"/> was within the <see cref="SpanCount"/>; otherwise, <c>false</c></returns>
    public bool TryGetSpan([ValueRange(0, MaxSpans)] int spanIndex, out ReadOnlySpan<T> span) {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (spanIndex >= 0 && spanIndex < SpanCount) {
            span = this[spanIndex];
            return true;
        }

        span = default;
        return false;
    }

    /// <inheritdoc cref="GetSpan"/>
    public ReadOnlySpan<T> this[[ValueRange(0, MaxSpans)] int spanIndex] => GetSpan(spanIndex);

    #endregion

    #region Element Retrieval

    /// <summary>
    /// Retrieves a <typeparamref name="T"/> element by treating this <see cref="RoMultiSpan{T}"/> as if it was a single, flat collection of size <see cref="ElementCount"/>.
    /// </summary>
    /// <remarks>
    /// <b>⚠ Warning:</b> When looping through each element, it is considerably more efficient to use <see cref="EnumerateElements"/> than repeated calls to <see cref="GetElement(int)"/>.
    /// </remarks>
    /// <param name="elementIndex">the <see cref="Index"/> of the <typeparamref name="T"/> element</param>
    /// <returns>the retrieved <typeparamref name="T"/> element</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="elementIndex"/> is out-of-bounds for <see cref="ElementCount"/></exception>
    [Pure]
    public T GetElement(int elementIndex) {
        var ec = ElementCount;
        if (elementIndex < 0 || elementIndex >= ec) {
            throw new ArgumentOutOfRangeException(nameof(elementIndex), elementIndex, $"{elementIndex} is out-of-bounds: this {nameof(RoMultiSpan<T>)} contains {ec} elements!");
        }

        var currentStart = 0;

        foreach (var span in this) {
            var currentEnd = currentStart + span.Length;

            if (currentEnd > elementIndex) {
                return span[elementIndex - currentStart];
            }

            currentStart = currentEnd;
        }

        throw new InvalidOperationException("This code should have been unreachable!");
    }

    /// <inheritdoc cref="GetElement(int)"/>
    [Pure]
    public T GetElement(Index elementIndex) {
        return GetElement(elementIndex.GetOffset(Count));
    }

    #endregion
}