using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

/// <summary>
/// Extension methods for <see cref="IConvertible"/> "<c>To{X}</c>" methods.
/// </summary>
/// <remarks>
/// These methods use the "normal" keywords for the types, i.e. <see cref="ToInt{T}"/> instead of <see cref="IConvertible.ToInt32"/>.
/// </remarks>
public static class ConvertibleExtensions {
    #region Convert.To{x}

    /// <inheritdoc cref="IConvertible.ToInt16"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToShort<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToInt16(provider);

    /// <inheritdoc cref="IConvertible.ToUInt16"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToUShort<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToUInt16(provider);

    /// <inheritdoc cref="IConvertible.ToInt32"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToInt32(provider);

    /// <inheritdoc cref="IConvertible.ToUInt32"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUInt<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToUInt32(provider);

    /// <inheritdoc cref="IConvertible.ToInt64"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToLong<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToInt64(provider);

    /// <inheritdoc cref="IConvertible.ToUInt64"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToULong<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToUInt64(provider);

    /// <inheritdoc cref="IConvertible.ToSingle"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToSingle(provider);

    /// <inheritdoc cref="IConvertible.ToDouble"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToDouble(provider);

    /// <inheritdoc cref="IConvertible.ToDecimal"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToDecimal(provider);

    /// <inheritdoc cref="IConvertible.ToByte"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ToByte<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToByte(provider);

    /// <inheritdoc cref="IConvertible.ToSByte"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ToSByte<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToSByte(provider);

    /// <inheritdoc cref="IConvertible.ToBoolean"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ToBool<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToBoolean(provider);

    /// <inheritdoc cref="IConvertible.ToChar"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToChar<T>(this T obj, IFormatProvider? provider = default) where T : IConvertible => obj.ToChar(provider);

    #endregion
}