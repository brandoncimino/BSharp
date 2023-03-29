using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    /// <summary>
    /// Calculates the total of a span of <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> values, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this span</param>
    /// <typeparam name="T">an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns>the total of all the elements</returns>
    /// <remarks>The methods prefixed with <c>Fast{X}</c> only work with <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> types.</remarks>
    /// <seealso cref="PrimitiveMath"/>
    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> is not <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/></exception>
    [Pure]
    public static T FastSum<T>(this ReadOnlySpan<T> span) where T : unmanaged {
        if (span.IsEmpty) {
            return default;
        }

        T   sum   = default;
        int index = 0;
        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count * 2) {
            var sums = PrimitiveMath.CreateVector(span);
            index = Vector<T>.Count;

            while (index + Vector<T>.Count <= span.Length) {
                var nextSlice = PrimitiveMath.CreateVector(span[index..]);
                sums  += nextSlice;
                index += Vector<T>.Count;
            }

            sum = sums.Sum();
        }

        for (; index < span.Length; index++) {
            sum = PrimitiveMath.Add(sum, span[index]);
        }

        return sum;
    }

    /// <inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static T FastSum<T>(this Span<T> span) where T : unmanaged => span.AsReadOnly().FastSum();

    /// <summary>
    /// Retrieves the <see cref="PrimitiveMath.Max{T}(T,T)"/> value of this span's elements, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <returns>the <see cref="PrimitiveMath.Max{T}(T,T)"/></returns>
    /// <inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static T FastMax<T>(this ReadOnlySpan<T> span) where T : unmanaged {
        if (span.IsEmpty) {
            return default;
        }

        T   max   = default;
        int index = 0;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count * 2) {
            var maxes = PrimitiveMath.CreateVector(span);
            index = Vector<T>.Count;

            while (index + Vector<T>.Count <= span.Length) {
                var nextSlice = PrimitiveMath.CreateVector(span[index..]);
                index += Vector<T>.Count;
                maxes =  Vector.Max(maxes, nextSlice);
            }

            max = maxes.Max();
        }

        for (; index < span.Length; index++) {
            max = PrimitiveMath.Max(max, span[index]);
        }

        return max;
    }

    /// <inheritdoc cref="FastMax{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static T FastMax<T>(this Span<T> span) where T : unmanaged => span.AsReadOnly().FastMax();

    /// <summary>
    /// Retrieves the <see cref="PrimitiveMath.Min{T}(T,T)"/> value of this span's elements.
    /// </summary>
    /// <returns>the <see cref="PrimitiveMath.Min{T}(T,T)"/></returns>
    /// <inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static T FastMin<T>(this ReadOnlySpan<T> span) where T : unmanaged {
        if (span.IsEmpty) {
            return default;
        }

        T   min   = default;
        int index = 0;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count * 2) {
            var mins = PrimitiveMath.CreateVector(span);
            index = Vector<T>.Count;

            while (index + Vector<T>.Count <= span.Length) {
                var nextSlice = PrimitiveMath.CreateVector(span[index..]);
                index += Vector<T>.Count;
                mins  =  Vector.Min(mins, nextSlice);
            }

            min = mins.Min();
        }

        for (; index < span.Length; index++) {
            min = PrimitiveMath.Min(min, span[index]);
        }

        return min;
    }

    /// <inheritdoc cref="FastMin{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static T FastMin<T>(this Span<T> span) where T : unmanaged => span.AsReadOnly().FastMin();

    /// <summary>
    /// Retrieves this span's <see cref="FastMin{T}(System.ReadOnlySpan{T})">min</see> and <see cref="FastMax{T}(System.ReadOnlySpan{T})">max</see> using a single <see cref="Vector"/>ized iteration.
    /// </summary>
    /// <returns>(<see cref="PrimitiveMath.Min{T}(T,T)">min</see>, <see cref="PrimitiveMath.Max{T}(T,T)"> max</see>)</returns>
    /// <inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static (T min, T max) FastMinMax<T>(this ReadOnlySpan<T> span) where T : unmanaged {
        if (span.IsEmpty) {
            return default;
        }

        T   min   = default;
        T   max   = default;
        int index = 0;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count * 2) {
            var mins  = PrimitiveMath.CreateVector(span);
            var maxes = mins;
            index = Vector<T>.Count;

            while (index + Vector<T>.Count <= span.Length) {
                var nextSlice = PrimitiveMath.CreateVector(span[index..]);
                index += Vector<T>.Count;
                mins  =  Vector.Min(mins, nextSlice);
                maxes =  Vector.Max(maxes, nextSlice);
            }

            min = mins.Min();
            max = maxes.Max();
        }

        for (; index < span.Length; index++) {
            min = PrimitiveMath.Min(min, span[index]);
            max = PrimitiveMath.Max(max, span[index]);
        }

        return (min, max);
    }

    /// <inheritdoc cref="FastMinMax{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static (T min, T max) FastMinMax<T>(this Span<T> span) where T : unmanaged => span.AsReadOnly().FastMinMax();

    //region Measure

    /// <summary>
    /// Retrieves the <see cref="FastMin{T}(System.ReadOnlySpan{T})">min</see>, <see cref="FastMax{T}(System.ReadOnlySpan{T})">max</see>, and <see cref="FastSum{T}(System.ReadOnlySpan{T})">sum</see> using a single <see cref="Vector"/>ized iteration of a span.
    /// </summary>
    /// <returns>(<see cref="PrimitiveMath.Min{T}(T,T)">min</see>, <see cref="PrimitiveMath.Max{T}(T,T)">max</see>, <see cref="PrimitiveMath.Add{T}">sum</see>)</returns>
    /// <inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static (T min, T max, T sum) FastMeasure<T>(this ReadOnlySpan<T> span) where T : unmanaged {
        if (span.IsEmpty) {
            return default;
        }

        T   min   = default;
        T   max   = default;
        T   sum   = default;
        int index = 0;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count * 2) {
            var mins  = PrimitiveMath.CreateVector(span);
            var maxes = mins;
            var sums  = mins;
            index = Vector<T>.Count;

            while (index + Vector<T>.Count <= span.Length) {
                var nextSlice = PrimitiveMath.CreateVector(span);
                index += Vector<T>.Count;

                mins  =  Vector.Min(mins, nextSlice);
                maxes =  Vector.Max(maxes, nextSlice);
                sums  += nextSlice;
            }

            min = mins.Min();
            max = maxes.Max();
            sum = sums.Sum();
        }

        for (; index < span.Length; index++) {
            min = PrimitiveMath.Min(min, span[index]);
            max = PrimitiveMath.Max(max, span[index]);
            sum = PrimitiveMath.Add(sum, span[index]);
        }

        return (min, max, sum);
    }

    /// <inheritdoc cref="FastMeasure{T}(System.ReadOnlySpan{T})"/>
    [Pure]
    public static (T min, T max, T sum) FastMeasure<T>(this Span<T> span) where T : unmanaged => span.AsReadOnly().FastMeasure();

    //endregion
}