// ReSharper disable UseDeconstructionOnParameter

using System;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        [Pure]
        public static int LerpInt(
            int               start,
            int               finish,
            double            lerpAmount,
            RoundingDirection roundingDirection = RoundingDirection.Floor
        ) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (lerpAmount * (finish - start)).RoundToInt(roundingDirection),
            };

        [Pure]
        public static long LerpInt(
            long              start,
            long              finish,
            double            lerpAmount,
            RoundingDirection roundingDirection = RoundingDirection.Floor
        ) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + ((finish - start) * lerpAmount).Round(roundingDirection).ToLong(),
            };

        [Pure]
        public static float Lerp(float start, float finish, float lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start) * lerpAmount,
            };

        [Pure]
        public static double Lerp(double start, double finish, double lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start) * lerpAmount,
            };

        public static decimal Lerp(decimal start, decimal finish, decimal lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start) * lerpAmount,
            };

        public static TimeSpan Lerp(TimeSpan start, TimeSpan finish, double lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start).Multiply(lerpAmount),
            };

        public static DateTime Lerp(DateTime start, DateTime finish, double lerpAmount) {
            return lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start).Multiply(lerpAmount),
            };
        }

        public static int      LerpInt(this (int start, int finish)           range, double  lerpAmount) => LerpInt(range.start, range.finish, lerpAmount);
        public static long     LerpInt(this (long start, long finish)         range, double  lerpAmount) => LerpInt(range.start, range.finish, lerpAmount);
        public static float    Lerp(this    (float start, float finish)       range, float   lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
        public static double   Lerp(this    (double start, double finish)     range, double  lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
        public static decimal  Lerp(this    (decimal start, decimal finish)   range, decimal lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
        public static TimeSpan Lerp(this    (TimeSpan start, TimeSpan finish) range, double  lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
        public static DateTime Lerp(this    (DateTime start, DateTime finish) range, double  lerpAmount) => Lerp(range.start, range.finish, lerpAmount);

        #region Inverse Lerp

        public static float InverseLerp(float start, float finish, float lerpedAmount) {
            throw new NotImplementedException("STOOOOOOOOOP!");
        }

        #endregion

        #region Splerp (split lerp)

        /// <summary>
        /// Performs a <see cref="LerpInt(int,int,double,FowlFever.BSharp.Enums.RoundingDirection)"/> operation,
        /// returning a <see cref="Tuple{T1,T2}"/> containing both the result and the "remainder".
        /// </summary>
        /// <example>
        /// This method ensures that all of the "distance" between <paramref name="start"/> and <paramref name="finish"/>)
        /// is accounted for.
        ///
        /// For example, consider whole puppies:
        /// <code><![CDATA[
        /// int 🦈 = 5;
        /// double 🎆 = 0.5;
        ///
        /// int 👈 = Lerp(0, 🦈, 🎆);   // => 0 + Math.Round(5 * .5) => Math.Round(2.5) => 2
        /// int 👉 = Lerp(0, 🦈, 🎆);   // => 0 + Math.Round(5 * .5) => Math.Round(2.5) => 2
        ///
        /// int 👯 = 👈 + 👉;           // => 2 + 2 => 4
        /// ]]></code>
        /// We've lost a 🦈!
        /// <p/>
        /// But no longer!
        /// <code><![CDATA[
        /// int 🦈 = 5;
        /// double 🎆 = 0.5;
        ///
        /// var (👈, 👉) = Sperp(0, 🦈, 🎆);    // => x = Lerp(...); (0 + x, 10 - x) => (0 + 2, 10 - 2) => (👈: 2, 👉: 8)
        ///
        /// int 👯‍ = 👈 + 👉;                  // => (👈: 2, 👉: 8) => 2 + 8 => 10 == 🦈!
        /// ]]></code>
        /// </example>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <param name="lerpAmount"></param>
        /// <param name="leftPortionRounding"></param>
        /// <returns></returns>
        public static (int A, int B) Sperp(
            int               start,
            int               finish,
            double            lerpAmount,
            RoundingDirection leftPortionRounding = default
        ) {
            var leftPortion = LerpInt(start, finish, lerpAmount, leftPortionRounding);
            return (start + leftPortion, finish - leftPortion);
        }

        /// <inheritdoc cref="Sperp(int,int,double,FowlFever.BSharp.Enums.RoundingDirection)"/>
        /// <remarks><see cref="Bisect"/> is equivalent to <see cref="Sperp(int,double,FowlFever.BSharp.Enums.RoundingDirection)"/>-ing with <c>.5</c>.</remarks>
        public static (int A, int B) Bisect(this int total, RoundingDirection leftPartitionRounding = default) => total.Sperp(.5);

        /// <inheritdoc cref="Sperp(int,int,double,FowlFever.BSharp.Enums.RoundingDirection)"/>
        public static (int A, int B) Sperp(this int total, double lerpAmount, RoundingDirection leftPortionRounding = default)
            => Sperp(0, total, lerpAmount, leftPortionRounding);

        /// <inheritdoc cref="Sperp(int,int,double,FowlFever.BSharp.Enums.RoundingDirection)"/>
        public static (long A, long B) Sperp(
            long              start,
            long              finish,
            double            lerpAmount,
            RoundingDirection leftPortionRounding = default
        ) {
            var leftPortion = LerpInt(start, finish, lerpAmount, leftPortionRounding);
            return (start + leftPortion, finish - leftPortion);
        }

        /// <inheritdoc cref="Sperp(int,int,double,FowlFever.BSharp.Enums.RoundingDirection)"/>
        public static (long A, long B) Sperp(this long total, double lerpAmount, RoundingDirection leftPortionRounding = default)
            => Sperp(0, total, lerpAmount, leftPortionRounding);

        #endregion
    }
}