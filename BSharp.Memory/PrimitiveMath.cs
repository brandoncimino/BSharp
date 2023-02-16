using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Math operations that work on <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types">unmanaged</a> numeric types.
/// </summary>
/// <remarks>
/// This bulk of this class was stolen from <a href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Scalar.cs">Scalar.cs</a> from <c>System.Private.CoreLib</c>.
///
/// The weirdest thing about it is that it casts via <see cref="object"/> as a middleman all over the place - which seems like it would cause allocations, but somehow, doesn't!
/// Turns out this One Weird Trick does have the limitation, though, that it requires <c>T</c> to be <b><i>exactly</i></b> the type you cast the <see cref="object"/> to -
/// it won't use any conversion operators.
///
/// This also won't work with <see cref="Type.IsByRefLike"/> types like <see cref="Span{T}"/>, because, even though the boxing doesn't <i>actually</i> happen, the syntax implies it, and by-ref types are very strict.
/// </remarks>
public static partial class PrimitiveMath {
    /// <inheritdoc cref="Scalar{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Add<T>(T a, T b) where T : unmanaged => Scalar<T>.Add(a, b);

    /// <inheritdoc cref="Scalar{T}.Subtract"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Subtract<T>(T a, T b) where T : unmanaged => Scalar<T>.Subtract(a, b);

    /// <inheritdoc cref="Scalar{T}.Multiply"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(T a, T b) where T : unmanaged => Scalar<T>.Multiply(a, b);

    /// <inheritdoc cref="Scalar{T}.Divide"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Divide<T>(T a, T b) where T : unmanaged => Scalar<T>.Divide(a, b);

    /// <inheritdoc cref="Scalar{T}.Min"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b) where T : unmanaged => Scalar<T>.Min(a, b);

    /// <inheritdoc cref="Scalar{T}.Max"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(T a, T b) where T : unmanaged => Scalar<T>.Max(a, b);

    /// <summary>
    /// Gets the <see cref="Scalar{T}.Min"/> element from a single <see cref="Vector{T}"/>.
    /// </summary>
    /// <param name="vector">this <see cref="Vector{T}"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Min<T>(this Vector<T> vector) where T : unmanaged {
        T min = default;

        for (int i = 0; i < Vector<T>.Count; i++) {
            min = Scalar<T>.Min(min, vector[i]);
        }

        return min;
    }

    public static T Max<T>(this Vector<T> vector) where T : unmanaged {
        T max = default;

        for (int i = 0; i < Vector<T>.Count; i++) {
            max = Scalar<T>.Max(max, vector[i]);
        }

        return max;
    }

    public static T Sum<T>(this Vector<T> vector) where T : unmanaged {
#if NET6_0_OR_GREATER
        return Vector.Sum(vector);
#else
        T sum = default;
        for (int i = 0; i < Vector<T>.Count; i++) {
            sum = Scalar<T>.Add(sum, vector[i]);
        }

        return sum;
#endif
    }

    /// <summary>
    /// Provides backwards-compatible support to create a <see cref="Vector{T}"/> from a <see cref="ReadOnlySpan{T}"/>, which wasn't added until .NET 5.
    /// </summary>
    /// <param name="span">a <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a new <see cref="Vector{T}"/> containing the first <see cref="Vector{T}.Count"/> elements of the span</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static Vector<T> CreateVector<T>(ReadOnlySpan<T> span) where T : unmanaged {
#if NET5_0_OR_GREATER
        return new Vector<T>(span);
#else
        if (span.Length < Vector<T>.Count) {
            throw new IndexOutOfRangeException($"Input span didn't contain at least {Vector<T>.Count} elements!");
        }

        return Unsafe.ReadUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)));
#endif
    }

    public static Vector<T> NextVector<T>(this ReadOnlySpan<T> span, ref int index) where T : unmanaged {
        var vector = CreateVector(span[index..]);
        index += Vector<T>.Count;
        return vector;
    }

    /// <inheritdoc cref="Vector.ConvertToInt32"/>
    public static Vector<int> ToInts(this Vector<float> floats) => Vector.ConvertToInt32(floats);

    /// <inheritdoc cref="Vector.ConvertToInt64"/>
    public static Vector<long> ToLongs(this Vector<double> doubles) => Vector.ConvertToInt64(doubles);

    /// <inheritdoc cref="Vector.ConvertToSingle(System.Numerics.Vector{int})"/>
    public static Vector<float> ToFloats(this Vector<int> ints) => Vector.ConvertToSingle(ints);

    /// <inheritdoc cref="Vector.ConvertToDouble(System.Numerics.Vector{long})"/>
    public static Vector<double> ToDoubles(this Vector<long> longs) => Vector.ConvertToDouble(longs);

    // public static Vector<int> RoundToInts(this Vector<float> floats, MidpointRounding rounding = default) {
    //     if(rounding == )
    // } 

    /// <summary>
    /// Provides backwards-compatible support to copy a <see cref="Vector{T}"/> into a <see cref="Span{T}"/>, which wasn't added until .NET 5.
    /// </summary>
    /// <param name="source">the input <see cref="Vector{T}"/></param>
    /// <param name="destination">the <see cref="Span{T}"/> that will be modified</param>
    /// <typeparam name="T">a <see cref="PrimitiveMath.PrimitiveNumberType"/></typeparam>
    public static void CopyTo<T>(Vector<T> source, Span<T> destination) where T : unmanaged {
#if NET5_0_OR_GREATER
        source.CopyTo(destination);
#else
        if ((uint)destination.Length < (uint)Vector<T>.Count) {
            throw new ArgumentException($"Destination span of length {destination.Length} can't fit a full {nameof(Vector<T>)}<{typeof(T).Name}>'s {Vector<T>.Count} elements!", nameof(destination));
        }

        Unsafe.WriteUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), source);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSupported<T>() where T : unmanaged => Scalar<T>.IsSupported;
}