using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Math operations that work on <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types">unmanaged</a>, <see cref="Type.IsPrimitive"/> numeric types.
/// </summary>
/// <remarks>
/// The type parameters in this class use the <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters#unmanaged-constraint"><c>unmanaged</c> type constraint</a>.
/// However, they also must be <see cref="Type.IsPrimitive"/>, which unfortunately does not have a corresponding type constraint.
/// In practice, this excludes types such as <see cref="decimal"/>, <see cref="Enum"/>, and custom <see cref="ValueType"/>s.
/// <p/>
/// This bulk of this class was stolen from <a href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Scalar.cs">Scalar.cs</a> from <c>System.Private.CoreLib</c>.
/// <p/>
/// <b>Casting via <see cref="object"/></b><br/>
/// <a href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Scalar.cs">Scalar.cs</a> from <c>System.Private.CoreLib</c> does a bunch of casting
/// using <see cref="object"/> as a "middleman", i.e.:
/// <code><![CDATA[
///     int i = (int)(object)tVal;
/// ]]></code>
/// This seems like it would cause allocations, but somehow, doesn't!
/// Turns out this One Weird Trick does have the limitation, though, that it requires <c>T</c> to be <b><i>exactly</i></b> the type you cast the <see cref="object"/> to -
/// it won't use any conversion operators.
/// <p/>
/// This also won't work with <see cref="Type.IsByRefLike"/> types like <see cref="Span{T}"/>, because, even though the boxing doesn't <i>actually</i> happen, the syntax implies it, and by-ref types are very strict.
/// <p/>
/// <b>Ways to cast <c>T</c> (which we <i>know</i> is an <see cref="int"/>) to <see cref="int"/>:</b>
/// <code><![CDATA[
///     int i = (int)tVal;                      // ❌ Won't compile, because `tVal` doesn't have a conversion operator defined
///     int i = Convert.ToInt32(tVal);          // ❌ Technically works, but boxes `tVal` in an `object`, which is bad
///     int i = Unsafe.As<T, int>(ref tVal);    // 〰 Works, but requires `ref tVal`, which is goofy and sometimes requires `Unsafe.AsRef(tVal)` first
///     int i = (int)(object)tVal;              // ✅ Always works, doesn't box `tVal`, and is _slightly_ faster than `Unsafe.As<T, int>(ref tVal)` 
/// ]]></code> 
/// </remarks>
public static partial class PrimitiveMath {
    /// <summary>
    /// Computes the sum of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the augend</param>
    /// <param name="b">the addend</param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns><paramref name="a"/> ➕ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> is not <see cref="IsPrimitiveNumeric{T}"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Add<T>(T a, T b) where T : unmanaged => Scalar<T>.Add(a, b);

    /// <summary>
    /// Computes the difference between two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the minuend</param>
    /// <param name="b">the subtrahend</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns><paramref name="a"/> ➖ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Subtract<T>(T a, T b) where T : unmanaged => Scalar<T>.Subtract(a, b);

    /// <summary>
    /// Computes the product of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the multiplicand</param>
    /// <param name="b">the multiplier</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns><paramref name="a"/> ✖ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(T a, T b) where T : unmanaged => Scalar<T>.Multiply(a, b);

    /// <summary>
    /// Computes the quotient of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the dividend</param>
    /// <param name="b">the divisor</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns><paramref name="a"/> ➗ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Divide<T>(T a, T b) where T : unmanaged => Scalar<T>.Divide(a, b);

    /// <summary>
    /// Computes the lesser of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the first value</param>
    /// <param name="b">the second value</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns>the lesser of <paramref name="a"/> and <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b) where T : unmanaged => Scalar<T>.Min(a, b);

    /// <summary>
    /// Computes the greater of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <inheritdoc cref="Min{T}(T,T)"/>
    /// <returns>the greater of <paramref name="a"/> and <paramref name="b"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(T a, T b) where T : unmanaged => Scalar<T>.Max(a, b);

    #region Vectors

    /// <summary>
    /// Gets the <see cref="Min{T}(T,T)"/> element from this <see cref="Vector{T}"/>.
    /// </summary>
    /// <param name="vector">this <see cref="Vector{T}"/></param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns>the <see cref="Min{T}(T,T)"/> value in this vector</returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    public static T Min<T>(this Vector<T> vector) where T : unmanaged {
        T min = default;

        for (int i = 0; i < Vector<T>.Count; i++) {
            min = Scalar<T>.Min(min, vector[i]);
        }

        return min;
    }

    /// <summary>
    /// Gets the <see cref="Max{T}(T,T)"/> element from this <see cref="Vector{T}"/>.
    /// </summary>
    /// <inheritdoc cref="Min{T}(T,T)"/>
    /// <returns>the <see cref="Max{T}(T,T)"/> value in this vector</returns>
    public static T Max<T>(this Vector<T> vector) where T : unmanaged {
        T max = default;

        for (int i = 0; i < Vector<T>.Count; i++) {
            max = Scalar<T>.Max(max, vector[i]);
        }

        return max;
    }

    /// <summary>
    /// <see cref="Add{T}"/>s the elements of this <see cref="Vector{T}"/> together.
    /// </summary>
    /// <param name="vector">this <see cref="Vector{T}"/></param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns>the total of all of the elements from this <see cref="Vector{T}"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    /// <remarks>TODO: link to online docs for future Vector.Sum() method</remarks>
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
    /// <exception cref="IndexOutOfRangeException">if the input span doesn't contain at least <see cref="Vector{T}.Count"/> elements</exception>
    public static Vector<T> CreateVector<T>(ReadOnlySpan<T> span) where T : unmanaged {
#if NET5_0_OR_GREATER
        return new Vector<T>(span);
#else
        if (span.Length < Vector<T>.Count) {
            throw new IndexOutOfRangeException($"Input span didn't contain at least {Vector<T>.Count} elements!");
        }

        return Unsafe.ReadUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span)));
#endif
    }

    /// <summary>
    /// Creates a <see cref="Vector{T}"/> from this span that starts at the the given <paramref name="index"/> and contains <see cref="Vector{T}.Count"/> items.
    /// The <paramref name="index"/> is then incremented by <see cref="Vector{T}.Count"/>.
    /// </summary>
    /// <param name="span">this span</param>
    /// <param name="index">the index in the span that will become the first element of the <see cref="Vector{T}"/></param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns>a new <see cref="Vector{T}"/></returns>
    /// <exception cref="IndexOutOfRangeException">if the (<paramref name="index"/>) or (<paramref name="index"/> + <see cref="Vector{T}.Count"/>) is out-of-bounds for this span</exception>
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

    /// <summary>
    /// Copies the contents of a <see cref="Vector{T}"/> into a <see cref="Span{T}"/>.
    /// </summary>
    /// <remarks>
    /// Provides backwards-compatible support for .NET Core 3+'s <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector-1.copyto?view=net-7.0#system-numerics-vector-1-copyto(system-span((-0)))">Vector&lt;T&gt;.CopyTo(Span&lt;T&gt;)</a>.
    /// </remarks>
    /// <param name="source">the input <see cref="Vector{T}"/></param>
    /// <param name="destination">the <see cref="Span{T}"/> that will be modified</param>
    /// <typeparam name="T">a <see cref="IsPrimitiveNumeric{T}"/></typeparam>
    public static void CopyTo<T>(Vector<T> source, Span<T> destination) where T : unmanaged {
#if NETCOREAPP3_0_OR_GREATER
        source.CopyTo(destination);
#else
        if ((uint)destination.Length < (uint)Vector<T>.Count) {
            throw new ArgumentException($"Destination span of length {destination.Length} can't fit a full {nameof(Vector<T>)}<{typeof(T).Name}>'s {Vector<T>.Count} elements!", nameof(destination));
        }

        Unsafe.WriteUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(destination)), source);
#endif
    }

    #endregion

    /// <summary>
    /// Returns <c>true</c> if <typeparamref name="T"/> is an <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types">unmanaged</a>, <see cref="Type.IsPrimitive"/>, numeric type.
    /// </summary>
    /// <typeparam name="T">the <see cref="Type"/> under scrutiny</typeparam>
    /// <inheritdoc cref="Scalar{T}.IsSupported"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitiveNumeric<T>() where T : unmanaged {
#if NET7_0_OR_GREATER
        return Vector<T>.IsSupported;
#else
        return Scalar<T>.IsSupported;
#endif
    }
}