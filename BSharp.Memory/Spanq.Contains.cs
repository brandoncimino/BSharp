using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Containment

    /// <summary>
    /// Computes whether or not the given value is present in the given span, using <see cref="Vector{T}"/>ization if possible.
    /// <p/>
    /// <b>📎 Note:</b> This method forwards to <see cref="MemoryExtensions.Contains"/> in .NET 5.0+. 
    /// </summary>
    /// <param name="span">this span</param>
    /// <param name="value">the sought-after value</param>
    /// <returns>true if this span contains <paramref name="value"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/></exception>
    /// <seealso cref="PrimitiveMath"/>
    /// <remarks><inheritdoc cref="FastSum{T}(System.ReadOnlySpan{T})"/></remarks>
    [Pure]
    public static bool FastContains<T>(this Span<T> span, T value) where T : unmanaged, IEquatable<T> => span.AsReadOnly().FastContains(value);

    /// <inheritdoc cref="FastContains{T}(System.Span{T},T)"/>
    [Pure]
    public static bool FastContains<T>(this ReadOnlySpan<T> span, T value) where T : unmanaged, IEquatable<T> {
#if NET5_0_OR_GREATER
        return span.Contains(value);
#else
        var index = 0;
        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count) {
            var eqVector = new Vector<T>(value);

            while (index + Vector<T>.Count <= span.Length) {
                var spanSlice = span[index..];
                var vectorSlice = VectorMath.CreateVector(spanSlice);
                index += Vector<T>.Count;

                if (Vector.EqualsAny(vectorSlice, eqVector)) {
                    return true;
                }
            }
            
            // QUESTION: 🙋‍♀️ Since it doesn't matter if we check a span element twice, would it actually be faster to make a span from the end for the last check?
            // ANSWER:   It would be! That's exactly what the implementations of `SpanHelpers.IndexOfAnyValueType` do: https://github.com/dotnet/runtime/blob/3870c07accf4c31d6c473ce24893b97dcde81a6c/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L1711 
            if (index < span.Length - 1) {
                var lastSpanSlice = span[^Vector<T>.Count..];
                var lastVectorSlice = VectorMath.CreateVector(lastSpanSlice);
                return Vector.EqualsAny(lastVectorSlice, eqVector);
            }

            return false;
        }

        for (; index < span.Length; index++) {
            if (PrimitiveMath.EqualTo(span[index], value)) {
                return true;
            }
        }

        return false;
#endif
    }

    #endregion
}