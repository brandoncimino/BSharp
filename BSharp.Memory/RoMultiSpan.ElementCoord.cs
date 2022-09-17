using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    /// <summary>
    /// Finds the index of the <see cref="ReadOnlySpan{T}"/> that contains the element at the overall index <paramref name="elementIndex"/> <i>and</i> the index of the element <i>within</i> that span.
    /// </summary>
    /// <param name="elementIndex">the "overall" index of an element, treating all of the contained <see cref="ReadOnlySpan{T}"/>s as one continuous collection</param>
    /// <returns>the corresponding <c>(span, element)</c> indices</returns>
    public (int span, int element) GetCoord([NonNegativeValue] int elementIndex) {
        var end = 0;

        Console.WriteLine($"finding index {elementIndex}");

        for (int i = 0; i < SpanCount; i++) {
            var start = end;
            end += this[i].Length;

            if (end > elementIndex) {
                return (i, elementIndex - start);
            }
        }

        if (end == elementIndex) {
            return (SpanCount, end);
        }

        throw SpanHelpers.OutOfBounds(ElementCount, elementIndex);
    }

    /// <inheritdoc cref="GetCoord(int)"/>
    public (int span, int element) GetCoord(Index elementIndex) => GetCoord(elementIndex.GetOffset(ElementCount));

    /// <summary>
    /// The 2D <c>(span, element)</c> indices of the first element of the first non-empty <see cref="GetSpan"/>.
    /// </summary>
    public (int span, int element) StartCoord {
        get {
            for (int i = 0; i < SpanCount; i++) {
                if (this[i].IsEmpty == false) {
                    return (i, 0);
                }
            }

            return default;
        }
    }

    /// <summary>
    /// The 2D <c>(span, element)</c> indices of the last element of the last non-empty <see cref="GetSpan"/>.
    /// </summary>
    public (int span, int element) EndCoord {
        get {
            for (int i = SpanCount - 1; i >= 0; i--) {
                var spanLen = this[i].Length;
                if (spanLen != 0) {
                    return (i, spanLen - 1);
                }
            }

            return default;
        }
    }
}