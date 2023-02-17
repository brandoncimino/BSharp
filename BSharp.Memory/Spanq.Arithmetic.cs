using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Arithmetic

    /// <summary>
    /// Adds an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value to each element of this <see cref="Span{T}"/>, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="addend">the value added to each element of the span</param>
    /// <typeparam name="T">an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> is not <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/></exception>
    public static void FastAddAll<T>(this Span<T> span, T addend) where T : unmanaged {
        var index        = 0;
        var vectorAddend = new Vector<T>(addend);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count) {
            var spanSlice = span[index..];
            index += Vector<T>.Count;
            var vectorSlice = new Vector<T>(spanSlice) + vectorAddend;
            PrimitiveMath.CopyTo(vectorSlice, spanSlice);
        }

        for (; index < Vector<T>.Count; index++) {
            span[index] = PrimitiveMath.Add(span[index], addend);
        }
    }

    /// <summary>
    /// Subtracts an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value from each element of this <see cref="Span{T}"/>, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="subtrahend">the value to subtract from each element of the span</param>
    /// <typeparam name="T"><inheritdoc cref="FastAddAll{T}"/></typeparam>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastAddAll{T}"/></exception>
    public static void FastSubtractAll<T>(this Span<T> span, T subtrahend) where T : unmanaged {
        var index        = 0;
        var vectorAmount = new Vector<T>(subtrahend);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count) {
            var spanSlice   = span[index..];
            var vectorSlice = new Vector<T>(spanSlice) - vectorAmount;
            PrimitiveMath.CopyTo(vectorSlice, spanSlice);
            index += Vector<T>.Count;
        }

        for (; index < Vector<T>.Count; index++) {
            span[index] = PrimitiveMath.Subtract(span[index], subtrahend);
        }
    }

    /// <summary>
    /// Multiplies each element of this <see cref="Span{T}"/> by an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value, using <see cref="Vector"/>ization if possible. 
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="multiplier">the factor by which each element will be multiplied</param>
    /// <typeparam name="T"><inheritdoc cref="FastAddAll{T}"/></typeparam>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastAddAll{T}"/></exception>
    public static void FastMultiplyAll<T>(this Span<T> span, T multiplier) where T : unmanaged {
        var index        = 0;
        var vectorFactor = new Vector<T>(multiplier);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count) {
            var spanSlice   = span[index..];
            var vectorSlice = new Vector<T>(spanSlice) * vectorFactor;
            PrimitiveMath.CopyTo(vectorSlice, spanSlice);

            index += Vector<T>.Count;
        }

        for (; index < Vector<T>.Count; index++) {
            span[index] = PrimitiveMath.Multiply(span[index], multiplier);
        }
    }

    /// <summary>
    /// Divides each element of this <see cref="Span{T}"/> by an <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/> value, using <see cref="Vector"/>ization if possible.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <param name="divisor">the value to divide each element by</param>
    /// <typeparam name="T"><inheritdoc cref="FastAddAll{T}"/></typeparam>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastAddAll{T}"/></exception>
    public static void FastDivideAll<T>(this Span<T> span, T divisor) where T : unmanaged {
        var index         = 0;
        var vectorDivisor = new Vector<T>(divisor);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count) {
            var spanSlice = span[index..];
            index += Vector<T>.Count;
            var vectorSlice = new Vector<T>(span) / vectorDivisor;
            PrimitiveMath.CopyTo(vectorSlice, spanSlice);
        }

        for (; index < span.Length; index++) {
            span[index] = PrimitiveMath.Divide(span[index], divisor);
        }
    }

    #endregion
}