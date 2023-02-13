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
internal static partial class PrimitiveMath {
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
}