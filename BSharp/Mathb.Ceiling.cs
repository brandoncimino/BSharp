using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

public static partial class Mathb {
    #region Rounding

    #region Ceiling

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="IFloatingPoint{F}.Ceiling"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F Ceiling<F>(this F x) where F : IFloatingPoint<F> => F.Floor(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling"/>
    public static int CeilingToInt<F>(this F x) where F : IFloatingPoint<F> => int.CreateChecked(F.Floor(x));
#else
    /// <inheritdoc cref="MathF.Ceiling"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Ceiling(this float x) => MathF.Ceiling(x);

    /// <inheritdoc cref="Math.Ceiling(double)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Ceiling(this double a) => Math.Ceiling(a);

    /// <inheritdoc cref="decimal.Ceiling"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Ceiling(this decimal d) => decimal.Ceiling(d);

    /// <inheritdoc cref="MathF.Ceiling"/>
    /// <returns><see cref="MathF.Ceiling"/> as an <see cref="int"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilingToInt(this float x) => (int)x.Ceiling();

    /// <inheritdoc cref="Math.Ceiling(double)"/>
    /// <returns><see cref="Math.Ceiling(double)"/> as an <see cref="int"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilingToInt(this double a) => (int)a.Ceiling();

    /// <inheritdoc cref="decimal.Ceiling"/>
    /// <returns><see cref="decimal.Ceiling"/> as an <see cref="int"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilingToInt(this decimal d) => (int)d.Ceiling();
#endif

    #endregion

    #endregion
}