// ReSharper disable UseDeconstructionOnParameter

using System;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Chronic;
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
    }
}