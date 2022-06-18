using System;

using FowlFever.BSharp.Chronic;

namespace FowlFever.BSharp;

public static partial class Mathb {
    [Pure] public static double Inverse(this    double value) => 1 / value;
    [Pure] public static float  Inverse(this    float  value) => 1 / value;
    [Pure] public static double Reciprocal(this double value) => value.Inverse();
    [Pure] public static float  Reciprocal(this float  value) => value.Inverse();

    /// <summary>
    /// Returns the difference between <paramref name="initial"/> and <paramref name="final"/> as a ratio of <paramref name="initial"/>.
    /// <br/>
    /// In other words, "how many <paramref name="initial"/>s does it take to get to <paramref name="final"/>?"
    /// <br/>
    /// In other other words, "by how much percent did <paramref name="initial"/> change?"
    /// </summary>
    /// <remarks>
    /// This method can be thought of as "percent change", where a <see cref="double"/> of 1 is equivalent to 100%, i.e.:
    /// <code><![CDATA[
    /// var q1 = 4_000;                 // 4k in Q1
    /// var q2 = 6_000;                 // 6k in Q2
    /// var growth = q1.DeltaRaio(q2);  // => +2k => .5 change => 50% growth
    /// ]]></code>
    /// </remarks>
    /// <param name="initial">the starting amount</param>
    /// <param name="final">the ending amount</param>
    /// <returns>how many <paramref name="initial"/>s we changed by</returns>
    [Pure]
    public static double DeltaRatio(double initial, double final) {
        return (initial, final) switch {
            (0, 0)                                             => 0,
            (0, _)                                             => double.PositiveInfinity * final.Sign(),
            (double.PositiveInfinity, double.PositiveInfinity) => double.NaN,
            (double.PositiveInfinity, _)                       => double.NegativeInfinity,
            (double.NegativeInfinity, double.NegativeInfinity) => double.NaN,
            (double.NegativeInfinity, _)                       => double.PositiveInfinity,
            _                                                  => (final - initial) / initial
        };
    }

    /// <inheritdoc cref="DeltaRatio(double,double)"/>
    [Pure]
    public static double DeltaRatio(Rate initial, Rate final) => DeltaRatio(initial.Hertz, final.Hertz);

    /// <inheritdoc cref="DeltaRatio(double,double)"/>
    [Pure]
    public static double DeltaRatio(TimeSpan initial, TimeSpan final) => DeltaRatio(initial.Ticks, final.Ticks);

    #region Fraction (i.e. the "fractional part")

    [Pure] public static float   Fraction(this float   value) => value % 1;
    [Pure] public static double  Fraction(this double  value) => value % 1;
    [Pure] public static decimal Fraction(this decimal value) => value % 1;

    #endregion
}