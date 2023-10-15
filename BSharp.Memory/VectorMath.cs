using System;
using System.Numerics;
using System.Runtime.CompilerServices;

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

    /// <summary>
    /// If possible, create a <see cref="Vector{T}(ReadOnlySpan{T})"/> from and <see cref="ReadOnlySpan{T}.Slice(int)"/>s this <paramref name="span"/>. 
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="slice">the newly-created <see cref="Vector{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>true if we created a new <see cref="Vector{T}"/></returns>
    /// <example>
    /// This method simplifies the code needed to looping through each <see cref="Vector{T}"/> of a <see cref="ReadOnlySpan{T}"/>, reducing the amount of code and the number of local variables to manage.
    /// <p/>
    /// <b>Traditional "inline" version:</b> 
    /// <code><![CDATA[
    ///     int index;
    ///     while(index + Vector<T>.Count < span.Length){
    ///         var slice = new Vector<T>(span[index..]);
    ///         index += Vector<T>.Count;
    ///     
    ///         // Process the Vector<T> here
    ///     }
    ///
    ///     for(; index < span.Length; index++){
    ///         // Process the remaining items
    ///     }
    /// ]]></code>
    /// <p/>
    /// <b>Using <see cref="TryTakeNextVector{T}"/>:</b>
    /// <code><![CDATA[
    ///     while(span.TryTakeNextVector()){
    ///         // Process the Vector<T> here
    ///     }
    ///
    ///     foreach(var it in span){
    ///         // Process the remaining items
    ///     }
    /// ]]></code>
    ///
    /// <p/>
    /// In terms of performance, using <see cref="TryTakeNextVector{T}"/> is just as efficient as the "inline" version - or possibly faster:
    ///
    /// | Method                       | NumberOfVectors | Mean        | Error      | StdDev     | Median      | Ratio | RatioSD | Allocated | Alloc Ratio |
    /// |----------------------------- |---------------- |------------:|-----------:|-----------:|------------:|------:|--------:|----------:|------------:|
    /// | 'Linq's Enumerable.Sum(T[])' | 10              |    47.98 ns |   1.871 ns |   5.457 ns |    45.97 ns |  1.00 |    0.00 |         - |          NA |
    /// | FastSum_Inline               | 10              |    17.73 ns |   0.385 ns |   0.564 ns |    17.52 ns |  0.40 |    0.02 |         - |          NA |
    /// | FastSum_TryTakeNextVector    | 10              |    14.34 ns |   0.299 ns |   0.603 ns |    14.24 ns |  0.32 |    0.02 |         - |          NA |
    /// |                              |                 |             |            |            |             |       |         |           |             |
    /// | 'Linq's Enumerable.Sum(T[])' | 100             |   379.32 ns |   8.385 ns |  23.512 ns |   377.86 ns |  1.00 |    0.00 |         - |          NA |
    /// | FastSum_Inline               | 100             |   178.77 ns |   4.005 ns |  11.229 ns |   176.38 ns |  0.47 |    0.04 |         - |          NA |
    /// | FastSum_TryTakeNextVector    | 100             |   104.08 ns |   2.080 ns |   5.695 ns |   102.44 ns |  0.28 |    0.02 |         - |          NA |
    /// |                              |                 |             |            |            |             |       |         |           |             |
    /// | 'Linq's Enumerable.Sum(T[])' | 1000            | 3,322.92 ns |  65.558 ns |  94.021 ns | 3,324.55 ns |  1.00 |    0.00 |         - |          NA |
    /// | FastSum_Inline               | 1000            | 1,538.78 ns |  29.752 ns |  31.834 ns | 1,544.83 ns |  0.46 |    0.01 |         - |          NA |
    /// | FastSum_TryTakeNextVector    | 1000            |   901.71 ns |  14.751 ns |  13.798 ns |   902.75 ns |  0.27 |    0.01 |         - |          NA |
    ///  
    /// </example>
    public static bool TryTakeNextVector<T>(ref this ReadOnlySpan<T> span, out Vector<T> slice) where T : unmanaged {
        Unsafe.SkipInit(out slice);
        if (span.Length < Vector<T>.Count) {
            return false;
        }

        slice = CreateVector(span);
        span  = span[Vector<T>.Count..];
        return true;
    }

    #region Contains

    public static bool Contains<T>(Vector<T> vector, T a) where T : unmanaged {
        return Vector.EqualsAny(vector, new Vector<T>(a));
    }

    /// <summary>
    /// Uses 🧙‍♀️ to check if the given <see cref="Vector{T}"/> contains any of multiple <typeparamref name="T"/> values.
    /// </summary>
    /// <remarks>
    /// This One Weird Trick was stolen from <a href="https://github.com/dotnet/runtime/blob/3870c07accf4c31d6c473ce24893b97dcde81a6c/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L1680">SpanHelpers.IndexOfAnyValueType</a>.
    /// The trick also words for 3 sough-after values, etc.
    /// </remarks>
    public static bool ContainsAny<T>(Vector<T> vector, T a, T b) where T : unmanaged {
        var aVec   = new Vector<T>(a);
        var bVec   = new Vector<T>(b);
        var equals = Vector.Equals(vector, aVec) | Vector.Equals(vector, bVec);
        return equals != default;
    }

    /// <inheritdoc cref="ContainsAny{T}(System.Numerics.Vector{T},T,T)"/>
    public static bool ContainsAny<T>(Vector<T> vector, T a, T b, T c) where T : unmanaged {
        var aVec   = new Vector<T>(a);
        var bVec   = new Vector<T>(b);
        var cVec   = new Vector<T>(c);
        var equals = Vector.Equals(vector, aVec) | Vector.Equals(vector, bVec) | Vector.Equals(vector, cVec);
        return equals != default;
    }

    #endregion
}