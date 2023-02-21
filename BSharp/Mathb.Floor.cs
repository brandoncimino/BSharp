using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

public static partial class Mathb {
    #region Rounding

    #region Floor

#if NET7_0_OR_GREATER

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Floor"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F Floor<F>(this F x) where F : IFloatingPoint<F> => F.Floor(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Floor"/>
    public static int FloorToInt<F>(this F x) where F : IFloatingPoint<F> => int.CreateChecked(x.Floor());

#else
    /// <inheritdoc cref="MathF.Floor"/> 
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(this float x) => MathF.Floor(x);

    /// <inheritdoc cref="Math.Floor(double)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Floor(this double a) => Math.Floor(a);

    /// <inheritdoc cref="decimal.Floor"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Floor(this decimal d) => decimal.Floor(d);

    /// <inheritdoc cref="MathF.Floor"/>
    /// <returns><see cref="MathF.Floor"/> as an <see cref="int"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(this float x) => (int)x.Floor();

    /// <inheritdoc cref="Math.Floor(double)"/>
    /// <returns><see cref="Math.Floor(double)"/> as an <see cref="int"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(this double a) => (int)a.Floor();

    /// <inheritdoc cref="decimal.Floor"/>
    /// <returns><see cref="decimal.Floor"/> as an <see cref="int"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(this decimal d) => (int)d.Floor();
#endif

    #endregion

    #endregion
}