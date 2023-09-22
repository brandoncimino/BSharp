using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    /// <summary>
    /// Finds the first index of this <see cref="Span{T}"/> that satisfies the given <see cref="PrimitiveMath.ComparisonOperator"/> relative to the <paramref name="endpoint"/>. 
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// var ints = new int[] { 0, 1, 2, 3, 4, 5 };
    /// FirstIndexSatisfyingComparison(ints, PrimitiveMath.ComparisonOperator.GreaterThan, 2); // => 3 
    /// ]]></code>
    /// </example>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="endpoint">the value that my <typeparamref name="T"/> elements will be compared to</param>
    /// <param name="comparisonOperator">how my <typeparamref name="T"/> elements should be compared to the <paramref name="endpoint"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the index of the first element that satisfies <paramref name="comparisonOperator"/> relative to <paramref name="endpoint"/>, if found; otherwise, -1</returns>
    [Pure]
    public static int FirstIndexSatisfyingComparison<T>(this ReadOnlySpan<T> span, T endpoint, PrimitiveMath.ComparisonOperator comparisonOperator) where T : unmanaged {
        var index = 0;

        if (Vector.IsHardwareAccelerated) {
            var endpointVector = new Vector<T>(endpoint);

            while (index + Vector<T>.Count <= span.Length) {
                var spanSlice   = span[index..];
                var sliceVector = VectorMath.CreateVector(spanSlice);

                var resultVector = comparisonOperator.Apply(sliceVector, endpointVector);
                if (resultVector == Vector<T>.Zero) {
                    index += Vector<T>.Count;
                    continue;
                }

                return index + resultVector.FirstBool(true);
            }
        }

        for (; index < span.Length; index++) {
            if (comparisonOperator.Apply(span[index], endpoint)) {
                return index;
            }
        }

        return -1;
    }

    [Pure]
    public static int LastIndexSatisfyingComparison<T>(this ReadOnlySpan<T> span, T endpoint, PrimitiveMath.ComparisonOperator comparisonOperator) where T : unmanaged {
        throw new NotImplementedException($"Should mirror {nameof(FirstIndexSatisfyingComparison)}");
    }

    /// <summary>
    /// Finds the first index of this <see cref="Span{T}"/> that is between <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="min">the lower boundary of the range</param>
    /// <param name="minInclusive">whether <paramref name="min"/> itself should be considered to be inside of the range</param>
    /// <param name="max">the upper boundary of the range</param>
    /// <param name="maxInclusive">whether <paramref name="max"/> itself should be considered to be inside of the range</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the index of the first element between <paramref name="min"/> and <paramref name="max"/>, if found; otherwise, -1</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="min"/> is greater than <paramref name="max"/></exception>
    [Pure]
    public static int FirstIndexInRange<T>(
        this ReadOnlySpan<T> span,
        T                    min,
        bool                 minInclusive,
        T                    max,
        bool                 maxInclusive
    ) where T : unmanaged {
        if (PrimitiveMath.GreaterThan(min, max)) {
            throw new ArgumentOutOfRangeException(nameof(min), min, $"`{nameof(min)}` cannot be greater than `{nameof(max)}` ({max})");
        }

        var index       = 0;
        var minOperator = minInclusive ? PrimitiveMath.ComparisonOperator.GreaterThanOrEqualTo : PrimitiveMath.ComparisonOperator.GreaterThan;
        var maxOperator = maxInclusive ? PrimitiveMath.ComparisonOperator.LessThanOrEqualTo : PrimitiveMath.ComparisonOperator.LessThan;

        if (Vector.IsHardwareAccelerated) {
            var minVector = new Vector<T>(min);
            var maxVector = new Vector<T>(max);

            while (index + Vector<T>.Count <= span.Length) {
                var spanSlice   = span[index..];
                var vectorSlice = VectorMath.CreateVector(spanSlice);

                var minResult = minOperator.Apply(vectorSlice, minVector);
                if (minResult == default) {
                    continue;
                }

                var maxResult = maxOperator.Apply(vectorSlice, maxVector);

                var firstInBoth = VectorMath.FirstBoolInBoth(minResult, maxResult, true);
                if (firstInBoth < 0) {
                    index += Vector<T>.Count;
                    continue;
                }

                return index + firstInBoth;
            }
        }

        for (; index < span.Length; index++) {
            var element = span[index];
            if (minOperator.Apply(element, min) && maxOperator.Apply(element, max)) {
                return index;
            }
        }

        return -1;
    }
}