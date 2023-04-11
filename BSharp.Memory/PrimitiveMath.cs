using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

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
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Add<T>(T a, T b) where T : unmanaged => Scalar<T>.Add(a, b);

    /// <summary>
    /// Computes the difference between two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the minuend</param>
    /// <param name="b">the subtrahend</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns><paramref name="a"/> ➖ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Subtract<T>(T a, T b) where T : unmanaged => Scalar<T>.Subtract(a, b);

    /// <summary>
    /// Computes the product of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the multiplicand</param>
    /// <param name="b">the multiplier</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns><paramref name="a"/> ✖ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(T a, T b) where T : unmanaged => Scalar<T>.Multiply(a, b);

    /// <summary>
    /// Computes the quotient of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the dividend</param>
    /// <param name="b">the divisor</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns><paramref name="a"/> ➗ <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Divide<T>(T a, T b) where T : unmanaged => Scalar<T>.Divide(a, b);

    /// <summary>
    /// Computes the lesser of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <param name="a">the first value</param>
    /// <param name="b">the second value</param>
    /// <typeparam name="T"><inheritdoc cref="Add{T}"/></typeparam>
    /// <returns>the lesser of <paramref name="a"/> and <paramref name="b"/></returns>
    /// <exception cref="NotSupportedException"><inheritdoc cref="Add{T}"/></exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b) where T : unmanaged => Scalar<T>.Min(a, b);

    /// <summary>
    /// Computes the greater of two <see cref="IsPrimitiveNumeric{T}"/> values.
    /// </summary>
    /// <inheritdoc cref="Min{T}(T,T)"/>
    /// <returns>the greater of <paramref name="a"/> and <paramref name="b"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(T a, T b) where T : unmanaged => Scalar<T>.Max(a, b);

    /// <summary>
    /// Returns <c>true</c> if <typeparamref name="T"/> is an <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types">unmanaged</a>, <see cref="Type.IsPrimitive"/>, numeric type.
    /// </summary>
    /// <typeparam name="T">the <see cref="Type"/> under scrutiny</typeparam>
    /// <inheritdoc cref="Scalar{T}.IsSupported"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitiveNumeric<T>() where T : unmanaged {
#if NET7_0_OR_GREATER
        return Vector<T>.IsSupported;
#else
        return Scalar<T>.IsSupported;
#endif
    }

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Abs<T>(T value) where T : unmanaged => Scalar<T>.Abs(value);

    /// <summary>
    /// Compares two <see cref="IsPrimitiveNumeric{T}"/> values for equality.
    /// </summary>
    /// <param name="a">the first value</param>
    /// <param name="b">the second value</param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns>true if <paramref name="a"/> and <paramref name="b"/> are equal</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualTo<T>(T a, T b) where T : unmanaged => Scalar<T>.Equals(a, b);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualTo<T>(T a, T b, T tolerance) where T : unmanaged => GreaterThan(Abs(Subtract(a, b)), tolerance);

    private const string NoEqualsMessage = $"Do not use {nameof(PrimitiveMath)}.{nameof(Equals)}(). If you wanted mathematical equality, use {nameof(PrimitiveMath)}.{nameof(EqualTo)}(). If you wanted object equality, use object.{nameof(object.Equals)}() directly.";

    [Obsolete(NoEqualsMessage, error: true), DoesNotReturn]
    public new static bool Equals(object? objA, object? objB) => throw new NotSupportedException(NoEqualsMessage);

    /// <summary>
    /// Gets an <see cref="IsPrimitiveNumeric{T}"/> value equivalent to 1.
    /// </summary>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns>the <typeparamref name="T"/> value equivalent to 1</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T One<T>() where T : unmanaged => Scalar<T>.One;

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Zero<T>() where T : unmanaged => default;

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero<T>(T value) where T : unmanaged => EqualTo(value, default);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOne<T>(T value) where T : unmanaged => EqualTo(value, One<T>());

    #region Comparisons

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<T>(T left, T right) where T : unmanaged => Scalar<T>.LessThan(left, right);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqualTo<T>(T left, T right) where T : unmanaged => Scalar<T>.LessThanOrEqual(left, right);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<T>(T left, T right) where T : unmanaged => Scalar<T>.GreaterThan(left, right);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqualTo<T>(T left, T right) where T : unmanaged => Scalar<T>.GreaterThanOrEqual(left, right);

    #endregion

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Increment<T>(T value) where T : unmanaged => Add(value, One<T>());

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Decrement<T>(T value) where T : unmanaged => Subtract(value, One<T>());

    /// <returns>true if <typeparamref name="T"/> is an integer type <b>AND <see cref="IsPrimitiveNumeric{T}"/></b></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBinaryInteger<T>() where T : unmanaged =>
        typeof(T) == typeof(int)    ||
        typeof(T) == typeof(uint)   ||
        typeof(T) == typeof(short)  ||
        typeof(T) == typeof(ushort) ||
        typeof(T) == typeof(long)   ||
        typeof(T) == typeof(ulong)  ||
        typeof(T) == typeof(byte)   ||
        typeof(T) == typeof(sbyte);

    /// <returns>true if <typeparamref name="T"/> is a floating-point type <b>AND <see cref="IsPrimitiveNumeric{T}"/></b></returns>
    /// <remarks>Notably, this excludes <see cref="decimal"/> and <see cref="T:System.Half"/>.</remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsFloatingPoint<T>() where T : unmanaged =>
        typeof(T) == typeof(float) ||
        typeof(T) == typeof(double);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static bool IsInteger<T>(T value) where T : unmanaged {
        if (IsBinaryInteger<T>()) {
            return true;
        }

        if (typeof(T) == typeof(float)) {
            var f = (float)(object)value;
            return float.IsFinite(f) && f == MathF.Truncate(f);
        }

        // ReSharper disable once InvertIf
        if (typeof(T) == typeof(double)) {
            var d = (double)(object)value;
            return double.IsFinite(d) && d == Math.Truncate(d);
        }

        throw NotPrimitiveType<T>();
    }

    #region Rounding

    /// <summary>
    /// Rounds a <paramref name="value"/> <b>down</b> to the closest integral value.
    /// </summary>
    /// <param name="value">a <see cref="float"/> or <see cref="double"/></param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/>, floating-point type</typeparam>
    /// <inheritdoc cref="NotFloatingPointType{T}"/>
    /// <returns>the closest integer that is less than <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Floor<T>(T value) where T : unmanaged => Scalar<T>.Floor(value);

    /// <summary>
    /// Rounds a <paramref name="value"/> <b>up</b> to the closest integral value.
    /// </summary>
    /// <param name="value">a <see cref="float"/> or <see cref="double"/></param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/>, floating-point type</typeparam>
    /// <returns>the closest integer that is greater than <paramref name="value"/></returns>
    /// <inheritdoc cref="NotFloatingPointType{T}"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Ceiling<T>(T value) where T : unmanaged => Scalar<T>.Ceiling(value);

    /// <inheritdoc cref="Math.Truncate(double)"/>
    /// <inheritdoc cref="NotFloatingPointType{T}"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Truncate<T>(T value) where T : unmanaged {
        if (typeof(T) == typeof(float)) {
            return (T)(object)MathF.Truncate((float)(object)value);
        }
        else if (typeof(T) == typeof(double)) {
            return (T)(object)Math.Truncate((double)(object)value);
        }
        else {
            throw NotFloatingPointType<T>();
        }
    }

    /// <summary>
    /// Similar to <see cref="Truncate{T}"/>, but allows <typeparamref name="T"/> to be an <see cref="IsBinaryInteger{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T TruncateSafely<T>(T value) where T : unmanaged {
        return IsBinaryInteger<T>() ? value : Truncate(value);
    }

    #endregion

    [ValueRange(-1, 1)]
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign<T>(T value) where T : unmanaged {
        // byte, ushort, uint, and ulong should have already been handled

        if (typeof(T) == typeof(double)) {
            return Math.Sign((double)(object)value);
        }
        else if (typeof(T) == typeof(short)) {
            return Math.Sign((short)(object)value);
        }
        else if (typeof(T) == typeof(int)) {
            return Math.Sign((int)(object)value);
        }
        else if (typeof(T) == typeof(long)) {
            return Math.Sign((long)(object)value);
        }
        else if (typeof(T) == typeof(nint)) {
            return Math.Sign((nint)(object)value);
        }
        else if (typeof(T) == typeof(sbyte)) {
            return Math.Sign((sbyte)(object)value);
        }
        else if (typeof(T) == typeof(float)) {
            return Math.Sign((float)(object)value);
        }
        else {
            throw NotSignedType<T>();
        }
    }

    /// <param name="value">an <see cref="IsPrimitiveNumeric{T}"/> value</param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns>true if <paramref name="value"/> ≥ 0</returns>
    /// <remarks>
    /// This should match the logic of the .NET 7+ <see cref="M:System.Numerics.INumberBase`1.IsPositive(`0)"/> method.
    /// </remarks>
    /// <seealso cref="IsStrictlyPositive{T}"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive<T>(T value) where T : unmanaged => GreaterThanOrEqualTo(value, Zero<T>());

    /// <param name="value">an <see cref="IsPrimitiveNumeric{T}"/> value</param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns>true if <paramref name="value"/> > 0</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStrictlyPositive<T>(T value) where T : unmanaged => GreaterThan(value, Zero<T>());

    /// <param name="value">an <see cref="IsPrimitiveNumeric{T}"/> value</param>
    /// <typeparam name="T">an <see cref="IsPrimitiveNumeric{T}"/> type</typeparam>
    /// <returns>true if <paramref name="value"/> &lt; 0 <i>(strictly negative)</i></returns>
    /// <remarks>
    /// This should match the logic of the .NET 7+ <see cref="M:System.Numerics.INumberBase`1.IsNegative(`0)"/> method.
    /// </remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative<T>(T value) where T : unmanaged => LessThan(value, Zero<T>());

    #region Exceptions

    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> isn't an <see cref="IsPrimitiveNumeric{T}"/> type</exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static NotSupportedException NotPrimitiveType<T>() => RejectType<T>("not a primitive numeric type!");

    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> isn't an <see cref="IsPrimitiveNumeric{T}"/>, signed type</exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static NotSupportedException NotSignedType<T>() => RejectType<T>("not a primitive, signed numeric type!");

    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> isn't an <see cref="IsPrimitiveNumeric{T}"/>, integer type <i>(<see cref="int"/>, <see cref="long"/>, etc.)</i></exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static NotSupportedException NotIntegerType<T>() => RejectType<T>("not a primitive integer type!");

    /// <exception cref="NotSupportedException">if <typeparamref name="T"/> isn't an <see cref="IsPrimitiveNumeric{T}"/>, floating-point type <i>(<see cref="float"/> or <see cref="double"/>)</i></exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static NotSupportedException NotFloatingPointType<T>() => RejectType<T>("not a primitive floating-point type!");

    private static NotSupportedException RejectType<T>(string reason, [CallerMemberName] string? caller = default) {
        return caller != null
                   ? new NotSupportedException($"{caller}() type {typeof(T).Name} is invalid: {reason}")
                   : new NotSupportedException($"type {typeof(T).Name} is invalid: {reason}");
    }

    #endregion
}