using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Various utilities for working with <see cref="Vector"/>s and <see cref="Type.IsPrimitive"/> types.
/// </summary>
public static class VectorMath {
    /// <summary>
    /// Provides backwards-compatible support to create a <see cref="Vector{T}"/> from a <see cref="ReadOnlySpan{T}"/>, which wasn't added until .NET 5.
    /// </summary>
    /// <param name="span">a <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a new <see cref="Vector{T}"/> containing the first <see cref="Vector{T}.Count"/> elements of the span</returns>
    /// <exception cref="IndexOutOfRangeException">if the input span doesn't contain at least <see cref="Vector{T}.Count"/> elements</exception>
    [Pure]
    public static Vector<T> CreateVector<T>(ReadOnlySpan<T> span) where T : unmanaged {
#if NET5_0_OR_GREATER
        return new Vector<T>(span);
#else
        if (span.Length < Vector<T>.Count) {
            throw new IndexOutOfRangeException($"Input span didn't contain at least {Vector<T>.Count} elements!");
        }

        return System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Vector<T>>(ref System.Runtime.CompilerServices.Unsafe.As<T, byte>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span)));
#endif
    }

    /// <inheritdoc cref="Vector.ConvertToInt32"/>
    /// <remarks>
    /// This will drop the non-integer portion of the <see cref="float"/>s in the same way that an explicit cast would, i.e.:
    /// <code><![CDATA[
    /// -1.5 => -1
    /// - .5 => 0
    ///   .5 => 0
    ///  1.5 => 1
    ///  2.5 => 2
    ///  3.5 => 3 
    /// ]]></code>
    /// </remarks>
    [Pure]
    internal static Vector<int> ToInts(this Vector<float> floats) => Vector.ConvertToInt32(floats);

    /// <inheritdoc cref="Vector.ConvertToInt64"/>
    [Pure]
    internal static Vector<long> ToLongs(this Vector<double> doubles) => Vector.ConvertToInt64(doubles);

    /// <inheritdoc cref="Vector.ConvertToSingle(System.Numerics.Vector{int})"/>
    [Pure]
    internal static Vector<float> ToFloats(this Vector<int> ints) => Vector.ConvertToSingle(ints);

    /// <inheritdoc cref="Vector.ConvertToDouble(System.Numerics.Vector{long})"/>
    [Pure]
    internal static Vector<double> ToDoubles(this Vector<long> longs) => Vector.ConvertToDouble(longs);

    /// <summary>
    /// Copies the contents of a <see cref="Vector{T}"/> into a <see cref="Span{T}"/>.
    /// </summary>
    /// <remarks>
    /// Provides backwards-compatible support for .NET Core 3+'s <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector-1.copyto?view=net-7.0#system-numerics-vector-1-copyto(system-span((-0)))">Vector&lt;T&gt;.CopyTo(Span&lt;T&gt;)</a>.
    /// </remarks>
    /// <param name="source">the input <see cref="Vector{T}"/></param>
    /// <param name="destination">the <see cref="Span{T}"/> that will be modified</param>
    /// <typeparam name="T">a <see cref="PrimitiveMath.IsPrimitiveNumeric{T}"/></typeparam>
    public static void CopyTo<T>(Vector<T> source, Span<T> destination) where T : unmanaged {
#if NETCOREAPP3_0_OR_GREATER
        source.CopyTo(destination);
#else
        if ((uint)destination.Length < (uint)Vector<T>.Count) {
            throw new ArgumentException($"Destination span of length {destination.Length} can't fit a full {nameof(Vector<T>)}<{typeof(T).Name}>'s {Vector<T>.Count} elements!", nameof(destination));
        }

        System.Runtime.CompilerServices.Unsafe.WriteUnaligned<Vector<T>>(ref System.Runtime.CompilerServices.Unsafe.As<T, byte>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(destination)), source);
#endif
    }

    /// <summary>
    /// Gets the <see cref="PrimitiveMath.Min{T}"/> element from this <see cref="Vector"/>.
    /// </summary>
    /// <param name="vector">this <see cref="Vector{T}"/></param>
    /// <typeparam name="T"><inheritdoc cref="PrimitiveMath.Add{T}"/></typeparam>
    /// <returns>the <see cref="PrimitiveMath.Min{T}"/> value in this vector</returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="PrimitiveMath.Add{T}"/></exception>
    [Pure]
    public static T Min<T>(this Vector<T> vector) where T : unmanaged {
        T min = default;

        for (int i = 0; i < Vector<T>.Count; i++) {
            min = PrimitiveMath.Min(min, vector[i]);
        }

        return min;
    }

    /// <summary>
    /// Gets the <see cref="PrimitiveMath.Max{T}"/> element from this <see cref="Vector{T}"/>.
    /// </summary>
    /// <inheritdoc cref="PrimitiveMath.Min{T}"/>
    /// <returns>the <see cref="PrimitiveMath.Max{T}"/> value in this vector</returns>
    [Pure]
    public static T Max<T>(this Vector<T> vector) where T : unmanaged {
        T max = default;

        for (int i = 0; i < Vector<T>.Count; i++) {
            max = PrimitiveMath.Max(max, vector[i]);
        }

        return max;
    }

    /// <summary>
    /// <see cref="PrimitiveMath.Add{T}"/>s the elements of this <see cref="Vector{T}"/> together.
    /// </summary>
    /// <param name="vector">this <see cref="Vector{T}"/></param>
    /// <typeparam name="T"><inheritdoc cref="PrimitiveMath.Add{T}"/></typeparam>
    /// <returns>the total of all of the elements from this <see cref="Vector{T}"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="PrimitiveMath.Add{T}"/></exception>
    /// <remarks>TODO: link to online docs for future Vector.Sum() method</remarks>
    [Pure]
    public static T Sum<T>(this Vector<T> vector) where T : unmanaged {
#if NET6_0_OR_GREATER
        return Vector.Sum(vector);
#else
        T sum = default;
        for (int i = 0; i < Vector<T>.Count; i++) {
            sum = PrimitiveMath.Add(sum, vector[i]);
        }

        return sum;
#endif
    }

    #region "Bool"-like interpretations

    /// <returns>the <see cref="bool"/> equivalent of a value returned by a boolean-like <see cref="Vector"/>, such as <see cref="Vector.GreaterThan(System.Numerics.Vector{double},System.Numerics.Vector{double})"/></returns>
    /// <remarks>For some insane reason, when you do <see cref="Vector"/> operations like <see cref="Vector.GreaterThan(System.Numerics.Vector{double},System.Numerics.Vector{double})"/>, the result is returned as <typeparamref name="T"/> values where 0 means "false" and -1 means "true".</remarks>
    [Pure]
    private static bool ToBool<T>(T vectorValue) where T : unmanaged {
        return PrimitiveMath.IsZero(vectorValue) == false;
    }

    /// <returns>the first index of this <see cref="Vector{T}"/> that has the desired <see cref="ToBool{T}"/>, if found; otherwise, -1</returns>
    [Pure]
    internal static int FirstBool<T>(this Vector<T> vector, bool desired) where T : unmanaged {
        // There's definitely a fancy way to do this like they do in `SpanHelpers.ComputeFirstIndex<>()`, but that's scary
        for (int i = 0; i < Vector<T>.Count; i++) {
            var v = vector[i];
            if (ToBool(v) == desired) {
                return i;
            }
        }

        return -1;
    }

    /// <returns>the first index that has the desired <see cref="ToBool{T}"/> in <b>both</b> of the given <see cref="Vector{T}"/>s, if found; otherwise, -1</returns>
    [Pure]
    internal static int FirstBoolInBoth<T>(Vector<T> a, Vector<T> b, bool desired) where T : unmanaged {
        for (int i = 0; i < Vector<T>.Count; i++) {
            if (ToBool(a[i]) == desired && ToBool(b[i]) == desired) {
                return i;
            }
        }

        return -1;
    }

    /// <returns><see cref="FirstBool{T}"/>, but from the end</returns>
    [Pure]
    internal static int LastBool<T>(this Vector<T> vector, bool desired) where T : unmanaged {
        for (int i = Vector<T>.Count - 1; i >= 0; i--) {
            var v = vector[i];
            if (ToBool(v) == desired) {
                return i;
            }
        }

        return -1;
    }

    #endregion
}