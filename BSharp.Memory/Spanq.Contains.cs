using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Containment

    /// <summary>
    /// Computes whether or not the given value is present in the given span, using <see cref="Vector{T}"/>ization if possible.
    /// </summary>
    /// <param name="span">this span</param>
    /// <param name="value">the sought-after value</param>
    /// <returns>true if this span contains <paramref name="value"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/></exception>
    /// <seealso cref="PrimitiveMath"/>
    /// <remarks><inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/></remarks>
    [Pure]
    public static bool FastContains<T>(this Span<T> span, T value) where T : unmanaged => span.AsReadOnly().FastContains(value);

    /// <inheritdoc cref="FastContains{T}(System.Span{T},T)"/>
    [Pure]
    public static bool FastContains<T>(this ReadOnlySpan<T> span, T value) where T : unmanaged {
        if (span.IsEmpty) {
            return false;
        }

        var index = 0;
        if (Vector.IsHardwareAccelerated) {
            var eqVector = new Vector<T>(value);

            while (index + Vector<T>.Count <= span.Length) {
                var spanSlice   = span[index..];
                var vectorSlice = PrimitiveMath.CreateVector(spanSlice);
                index += Vector<T>.Count;

                if (Vector.EqualsAny(vectorSlice, eqVector)) {
                    return true;
                }
            }

            // TODO: 🙋‍♀️ Since it doesn't matter it we check a span element twice, would it actually be faster to make a span from the end for the last check?
            // var lastSpanSlice   = span[^Vector<T>.Count..];
            // var lastVectorSlice = PrimitiveMath.CreateVector(lastSpanSlice);
            // return Vector.EqualsAll(lastVectorSlice, eqVector);
        }

        for (; index < span.Length; index++) {
            if (PrimitiveMath.EqualTo(span[index], value)) {
                return true;
            }
        }

        return false;
    }

    #endregion
}