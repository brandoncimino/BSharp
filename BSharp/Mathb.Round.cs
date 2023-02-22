using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

public static partial class Mathb {
    #region Rounding

    #region Round

    #region No params

    /// <inheritdoc cref="Math.Round(double)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double a) => Math.Round(a);

    /// <inheritdoc cref="Math.Round(decimal)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this decimal d) => decimal.Round(d);

    /// <inheritdoc cref="MathF.Round(float)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x) => MathF.Round(x);

    #endregion

    #region digits

    /// <inheritdoc cref="Math.Round(double, int)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double a, int digits) => Math.Round(a, digits);

    /// <inheritdoc cref="decimal.Round(decimal, int)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this decimal d, int decimals) => decimal.Round(d, decimals);

    /// <inheritdoc cref="MathF.Round(float, int)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x, int digits) => MathF.Round(x, digits);

    #endregion

    #region RoundingMode

    /// <inheritdoc cref="Math.Round(double, MidpointRounding)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double a, RoundingMode mode) => mode.Round(a);

    /// <inheritdoc cref="decimal.Round(decimal, MidpointRounding)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this decimal d, RoundingMode mode) => mode.Round(d);

    /// <inheritdoc cref="MathF.Round(float)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x, RoundingMode mode) => mode.Round(x);

    #region digits, mode

    /// <inheritdoc cref="Math.Round(double, int, MidpointRounding)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double a, int digits, RoundingMode mode) => mode.Round(a, digits);

    /// <inheritdoc cref="decimal.Round(decimal, MidpointRounding)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this decimal d, int digits, RoundingMode mode) => mode.Round(d, digits);

    /// <inheritdoc cref="MathF.Round(float, int, MidpointRounding"/> 
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x, int digits, RoundingMode mode) => mode.Round(x, digits);

    #endregion

    #endregion

    #endregion

    /// <inheritdoc cref="Round(float)"/>
    /// <returns>the closest <see cref="int"/> to this <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this float value) => (int)MathF.Round(value);

    /// <inheritdoc cref="Round(double)"/>
    /// <returns>the closest <see cref="int"/> to this <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this double value) => (int)Math.Round(value);

    /// <inheritdoc cref="Round(decimal)"/>
    /// <returns>the closest <see cref="int"/> to this <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this decimal value) => (int)decimal.Round(value);

    /// <inheritdoc cref="Round(float, RoundingMode)"/>
    /// <returns>the closest <see cref="int"/> to this <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this float value, RoundingMode mode) => (int)mode.Round(value);

    /// <inheritdoc cref="Round(double, RoundingMode)"/>
    /// <returns>the closest <see cref="int"/> to this <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this double value, RoundingMode mode) => (int)mode.Round(value);

    /// <inheritdoc cref="Round(decimal, RoundingMode)"/>
    /// <returns>the closest <see cref="int"/> to this <paramref name="value"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this decimal value, RoundingMode mode) => (int)mode.Round(value);

    #endregion
}