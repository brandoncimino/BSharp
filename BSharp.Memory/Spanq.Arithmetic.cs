using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Arithmetic

    #region Arithmetic - All

    /// <summary>
    /// Adds an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value to each element of this <see cref="Span{T}"/>, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="addend">the value added to each element of the span</param>
    /// <typeparam name="T">an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> is not <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/></exception>
    public static void FastAddAll<T>(this Span<T> span, T addend) where T : unmanaged {
        if (PrimitiveMath.EqualTo(addend, default)) {
            return;
        }

        span.FastOperateAll(addend, PrimitiveMath.ArithmeticOperation.Addition);
    }

    internal static void FastOperateAll<T>(this Span<T> span, T value, PrimitiveMath.ArithmeticOperation operation) where T : unmanaged {
        var index = 0;

        if (Vector.IsHardwareAccelerated) {
            var vectorValue = new Vector<T>(value);

            while (index + Vector<T>.Count <= span.Length) {
                var spanSlice = span[index..];
                index += Vector<T>.Count;
                var sliceVector = new Vector<T>(spanSlice);

                sliceVector = operation.Apply(sliceVector, vectorValue);

                VectorMath.CopyTo(sliceVector, spanSlice);
            }
        }

        for (; index < span.Length; index++) {
            span[index] = operation.Apply(span[index], value);
        }
    }

    /// <summary>
    /// Subtracts an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value from each element of this <see cref="Span{T}"/>, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="subtrahend">the value to subtract from each element of the span</param>
    /// <typeparam name="T"><inheritdoc cref="FastAddAll{T}(System.Span{T},T)"/></typeparam>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastAddAll{T}(System.Span{T},T)"/></exception>
    public static void FastSubtractAll<T>(this Span<T> span, T subtrahend) where T : unmanaged {
        if (PrimitiveMath.EqualTo(subtrahend, default)) {
            return;
        }

        span.FastOperateAll(subtrahend, PrimitiveMath.ArithmeticOperation.Subtraction);
    }

    /// <summary>
    /// Multiplies each element of this <see cref="Span{T}"/> by an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value, using <see cref="Vector"/>ization if possible. 
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="multiplier">the factor by which each element will be multiplied</param>
    /// <typeparam name="T"><inheritdoc cref="FastAddAll{T}(System.Span{T},T)"/></typeparam>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastAddAll{T}(System.Span{T},T)"/></exception>
    public static void FastMultiplyAll<T>(this Span<T> span, T multiplier) where T : unmanaged {
        if (PrimitiveMath.IsOne(multiplier)) {
            return;
        }

        span.FastOperateAll(multiplier, PrimitiveMath.ArithmeticOperation.Multiplication);
    }

    /// <summary>
    /// Divides each element of this <see cref="Span{T}"/> by an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="divisor">the value to divide each element by</param>
    /// <typeparam name="T"><inheritdoc cref="FastAddAll{T}(System.Span{T},T)"/></typeparam>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastAddAll{T}(System.Span{T},T)"/></exception>
    public static void FastDivideAll<T>(this Span<T> span, T divisor) where T : unmanaged {
        if (PrimitiveMath.IsOne(divisor)) {
            return;
        }

        span.FastOperateAll(divisor, PrimitiveMath.ArithmeticOperation.Division);
    }

    #endregion

    #region Arithmetic - All; Vector Pattern

    internal static void FastOperatePattern<T>(this Span<T> span, Vector<T> pattern, PrimitiveMath.ArithmeticOperation operation) where T : unmanaged {
        var index = 0;

        while (index + Vector<T>.Count <= span.Length) {
            var spanSlice = span[index..];
            index += Vector<T>.Count;
            var sliceVector = new Vector<T>(spanSlice);
            sliceVector = operation.Apply(sliceVector, pattern);
            VectorMath.CopyTo(sliceVector, spanSlice);
        }

        for (; index < span.Length; index++) {
            var vectorIndex = index % Vector<T>.Count;
            span[index] = operation.Apply(span[index], pattern[vectorIndex]);
        }
    }

    public static void FastAddPattern<T>(this Span<T> span, Vector<T> pattern) where T : unmanaged {
        if (pattern == Vector<T>.Zero) {
            return;
        }

        span.FastOperatePattern(pattern, PrimitiveMath.ArithmeticOperation.Addition);
    }

    public static void FastSubtractPattern<T>(this Span<T> span, Vector<T> pattern) where T : unmanaged {
        if (pattern == Vector<T>.Zero) {
            return;
        }

        span.FastOperatePattern(pattern, PrimitiveMath.ArithmeticOperation.Subtraction);
    }

    public static void FastMultiplyPattern<T>(this Span<T> span, Vector<T> pattern) where T : unmanaged {
        if (pattern == Vector<T>.One) {
            return;
        }

        span.FastOperatePattern(pattern, PrimitiveMath.ArithmeticOperation.Multiplication);
    }

    public static void FastDividePattern<T>(this Span<T> span, Vector<T> pattern) where T : unmanaged {
        if (pattern == Vector<T>.One) {
            return;
        }

        span.FastOperatePattern(pattern, PrimitiveMath.ArithmeticOperation.Division);
    }

    #endregion

    #region Arithmetic - Each

    internal static void FastZip<T>(this Span<T> span, ReadOnlySpan<T> other, PrimitiveMath.ArithmeticOperation operation) where T : unmanaged {
        var minLen = Math.Min(span.Length, other.Length);
        span  = span[..minLen];
        other = other[..minLen];

        var index = 0;

        if (Vector.IsHardwareAccelerated && minLen >= Vector<T>.Count) {
            var spanSlice  = span[index..];
            var otherSlice = other[index..];

            var spanVec  = new Vector<T>(spanSlice);
            var otherVec = VectorMath.CreateVector(otherSlice);

            var newVec = operation.Apply(spanVec, otherVec);
            VectorMath.CopyTo(newVec, spanSlice);
        }

        for (; index < minLen; index++) {
            span[index] = operation.Apply(span[index], other[index]);
        }
    }

    public static void FastAddEach<T>(this      Span<T> span, ReadOnlySpan<T> addends) where T : unmanaged     => span.FastZip(addends,     PrimitiveMath.ArithmeticOperation.Addition);
    public static void FastSubtractEach<T>(this Span<T> span, ReadOnlySpan<T> subtrahends) where T : unmanaged => span.FastZip(subtrahends, PrimitiveMath.ArithmeticOperation.Subtraction);
    public static void FastMultiplyEach<T>(this Span<T> span, ReadOnlySpan<T> multipliers) where T : unmanaged => span.FastZip(multipliers, PrimitiveMath.ArithmeticOperation.Multiplication);
    public static void FastDivideEach<T>(this   Span<T> span, ReadOnlySpan<T> divisors) where T : unmanaged    => span.FastZip(divisors,    PrimitiveMath.ArithmeticOperation.Division);

    #endregion

    #endregion
}