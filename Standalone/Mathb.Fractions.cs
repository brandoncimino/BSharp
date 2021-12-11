using System;
using System.Diagnostics.Contracts;

using BrandonUtils.Standalone.Chronic;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        [Pure]
        public static double Inverse(this double value) => 1 / value;

        [Pure]
        public static float Inverse(this float value) => 1 / value;

        [Pure]
        public static double Reciprocal(this double value) => value.Inverse();

        [Pure]
        public static float Reciprocal(this float value) => value.Inverse();

        [Pure]
        public static double DeltaRatio(double initial, double final) {
            return (initial, final) switch {
                (0, 0)                                             => 0,
                (0, _)                                             => double.PositiveInfinity * final.Sign(),
                (double.PositiveInfinity, double.PositiveInfinity) => double.NaN,
                (double.PositiveInfinity, _)                       => double.NegativeInfinity,
                (double.NegativeInfinity, double.NegativeInfinity) => double.NaN,
                (double.NegativeInfinity, _)                       => double.NegativeInfinity,
                _                                                  => (initial - final) / initial
            };
        }

        [Pure]
        public static double DeltaRatio(Rate initial, Rate final) => DeltaRatio(initial.Hertz, final.Hertz);

        [Pure]
        public static double DeltaRatio(TimeSpan initial, TimeSpan final) => DeltaRatio(initial.Ticks, final.Ticks);

        #region Fraction (i.e. the "fractional part")

        public static float   Fraction(this float   value) => value % 1;
        public static double  Fraction(this double  value) => value % 1;
        public static decimal Fraction(this decimal value) => value % 1;

        #endregion

        #region Apportion

        public static (double a, double b) Apportion(double a, double b, double total = 1) {
            var sum      = a + b;
            var aRatio   = a      / sum;
            var aPortion = aRatio * total;
            var bRatio   = total - aPortion;
            return (aRatio, bRatio);
        }

        public static (int a, int b) Apportion(double a, double b, int total) {
            var sum      = a + b;
            var aRatio   = a      / sum;
            var aPortion = aRatio * total;
            var aRounded = Math.Round(aPortion).ToInt();
            var bRounded = total - aRounded;
            return (aRounded, bRounded);
        }

        #endregion
    }
}