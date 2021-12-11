using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        #region Signage

        #region Math.Sign()

        /**
         * <seealso cref="Math.Sign(short)"/>
         */
        [Pure]
        public static int Sign(this short value) {
            return Math.Sign(value);
        }

        /**
         * <seealso cref="Math.Sign(int)"/>
         */
        [Pure]
        public static int Sign(this int integer) {
            return Math.Sign(integer);
        }

        /**
         * <seealso cref="Math.Sign(long)"/>
         */
        [Pure]
        public static int Sign(this long value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(float)"/>
         */
        [Pure]
        public static int Sign(this float value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(double)"/>
         */
        [Pure]
        public static int Sign(this double value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(decimal)"/>
         */
        [Pure]
        public static int Sign(this decimal value) {
            return Math.Sign(value);
        }

        /// <returns><paramref name="value"/>.<see cref="TimeSpan.CompareTo(object)"/>(<see cref="TimeSpan.Zero"/>)</returns>
        [Pure]
        public static int Sign(this TimeSpan value) {
            return value.CompareTo(TimeSpan.Zero);
        }

        #endregion

        #region IsPositive

        [Pure]
        public static bool IsPositive(this short value) => value >= 0;

        [Pure]
        public static bool IsPositive(this int value) => value >= 0;

        [Pure]
        public static bool IsPositive(this long value) => value >= 0;

        [Pure]
        public static bool IsPositive(this float value) => value >= 0;

        [Pure]
        public static bool IsPositive(this double value) => value >= 0;

        [Pure]
        public static bool IsPositive(this decimal value) => value >= 0;

        [Pure]
        public static bool IsPositive(this TimeSpan value) => value >= TimeSpan.Zero;

        #endregion

        #region IsStrictlyPositive

        [Pure]
        public static bool IsStrictlyPositive(this int value) => value > 0;

        [Pure]
        public static bool IsStrictlyPositive(this short value) => value > 0;

        [Pure]
        public static bool IsStrictlyPositive(this long value) => value > 0;

        [Pure]
        public static bool IsStrictlyPositive(this float value) => value > 0;

        [Pure]
        public static bool IsStrictlyPositive(this double value) => value > 0;

        [Pure]
        public static bool IsStrictlyPositive(this decimal value) => value > 0;

        [Pure]
        public static bool IsStrictlyPositive(this TimeSpan value) => value > TimeSpan.Zero;

        #endregion

        #region IsNegative

        [Pure]
        public static bool IsNegative(this short value) => value < 0;

        [Pure]
        public static bool IsNegative(this int value) => value < 0;

        [Pure]
        public static bool IsNegative(this long value) => value < 0;

        [Pure]
        public static bool IsNegative(this float value) => value < 0;

        [Pure]
        public static bool IsNegative(this double value) => value < 0;

        [Pure]
        public static bool IsNegative(this decimal value) => value < 0;

        [Pure]
        public static bool IsNegative(this TimeSpan value) => value < TimeSpan.Zero;

        #endregion

        #region MustBePositive

        [Pure, NonNegativeValue]
        public static short MustBePositive(this short value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        [Pure, NonNegativeValue]
        public static int MustBePositive(this int value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        [Pure, NonNegativeValue]
        public static long MustBePositive(this long value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        [Pure, NonNegativeValue]
        public static float MustBePositive(this float value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        [Pure, NonNegativeValue]
        public static double MustBePositive(this double value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        [Pure, NonNegativeValue]
        public static decimal MustBePositive(this decimal value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        [Pure, NonNegativeValue]
        public static TimeSpan MustBePositive(this TimeSpan value) => value.IsPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be positive (x ⩾ 0)!");

        #endregion

        #region MustBeStrictlyPositive

        [Pure, NonNegativeValue]
        public static short MustBeStrictlyPositive(this short value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        [Pure, NonNegativeValue]
        public static int MustBeStrictlyPositive(this int value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        [Pure, NonNegativeValue]
        public static long MustBeStrictlyPositive(this long value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        [Pure, NonNegativeValue]
        public static float MustBeStrictlyPositive(this float value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        [Pure, NonNegativeValue]
        public static double MustBeStrictlyPositive(this double value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        [Pure, NonNegativeValue]
        public static decimal MustBeStrictlyPositive(this decimal value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        [Pure, NonNegativeValue]
        public static TimeSpan MustBeStrictlyPositive(this TimeSpan value) => value.IsStrictlyPositive() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be strictly positive (x > 0)!");

        #endregion

        #endregion
    }
}