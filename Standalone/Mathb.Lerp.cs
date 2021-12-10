// ReSharper disable UseDeconstructionOnParameter

using System;

using BrandonUtils.Standalone.Chronic;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        public static int LerpInt(int start, int finish, double lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var dist   = finish - start;
                var amount = (lerpAmount * dist).ToInt();
                return start + amount;
            }
        }

        public static long LerpInt(long start, long finish, double lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var dist   = finish - start;
                var amount = (lerpAmount * dist).ToLong();
                return start + amount;
            }
        }

        public static float Lerp(float start, float finish, float lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var diff     = finish - start;
                var lerpDiff = diff * lerpAmount;
                return start + lerpDiff;
            }
        }

        public static double Lerp(double start, double finish, double lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var diff     = finish - start;
                var lerpDiff = diff * lerpAmount;
                return start + lerpDiff;
            }
        }

        public static decimal Lerp(decimal start, decimal finish, decimal lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var diff     = finish - start;
                var lerpDiff = diff * lerpAmount;
                return start + lerpDiff;
            }
        }

        public static TimeSpan Lerp(TimeSpan start, TimeSpan finish, double lerpAmount) {
            var ticks = LerpInt(start.Ticks, finish.Ticks, lerpAmount);
            return TimeSpan.FromTicks(ticks);
        }

        public static DateTime Lerp(DateTime start, DateTime finish, double lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var dist   = finish - start;
                var amount = dist.Multiply(lerpAmount);
                return start + amount;
            }
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