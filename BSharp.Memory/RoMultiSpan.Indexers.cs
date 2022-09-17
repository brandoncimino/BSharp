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
    public ReadOnlySpan<T> GetSpan([ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex) {
        SpanCount.RequireIndex(spanIndex);

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
    public bool TryGetSpan([ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex, out ReadOnlySpan<T> span) {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (spanIndex >= 0 && spanIndex < SpanCount) {
            span = this[spanIndex];
            return true;
        }

        span = default;
        return false;
    }

    /// <inheritdoc cref="GetSpan"/>
    public ReadOnlySpan<T> this[[ValueRange(0, RoMultiSpan.MaxSpans)] int spanIndex] {
        get => GetSpan(spanIndex);
        private init {
            switch (spanIndex) {
                case 0:
                    _a = value;
                    break;
                case 1:
                    _b = value;
                    break;
                case 2:
                    _c = value;
                    break;
                case 3:
                    _d = value;
                    break;
                case 4:
                    _e = value;
                    break;
                case 5:
                    _f = value;
                    break;
                case 6:
                    _g = value;
                    break;
                case 7:
                    _h = value;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(spanIndex));
            }
        }
    }

    #endregion

    #region Element Retrieval

    /// <summary>
    /// Retrieves a <typeparamref name="T"/> element by treating this <see cref="RoMultiSpan{T}"/> as if it was a single, flat collection of size <see cref="ElementCount"/>.
    /// </summary>
    /// <remarks>
    /// ⚠ When looping through each element, it is considerably more efficient to use <see cref="EnumerateElements"/> than repeated calls to <see cref="ElementAt(int)"/>,
    /// because <see cref="ElementAt(int)"/> has to traverse each <see cref="EnumerateSpans"/> to find the <see cref="GetContainingSpanIndex"/> of <paramref name="elementIndex"/>.
    /// </remarks>
    /// <param name="elementIndex">the <see cref="Index"/> of the <typeparamref name="T"/> element</param>
    /// <returns>the retrieved <typeparamref name="T"/> element</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="elementIndex"/> is out-of-bounds for <see cref="ElementCount"/></exception>
    [Pure]
    public T ElementAt([NonNegativeValue] int elementIndex) {
        return this[GetCoord(elementIndex)];
    }

    /// <inheritdoc cref="ElementAt(int)"/>
    [Pure]
    public T ElementAt(Index elementIndex) {
        return ElementAt(elementIndex.GetOffset(Count));
    }

    #endregion
}