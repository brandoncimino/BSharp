using System;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Exceptions;

using Pure = System.Diagnostics.Contracts.PureAttribute;
using JetBrains.Annotations;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        #region Signage

        #region Math.Sign()

        /**
         * <seealso cref="Math.Sign(short)"/>
         */
        [Pure] public static int Sign(this short value) {
            return Math.Sign(value);
        }

        /**
         * <seealso cref="Math.Sign(int)"/>
         */
        [Pure] public static int Sign(this int integer) {
            return Math.Sign(integer);
        }

        /**
         * <seealso cref="Math.Sign(long)"/>
         */
        [Pure] public static int Sign(this long value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(float)"/>
         */
        [Pure] public static int Sign(this float value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(double)"/>
         */
        [Pure] public static int Sign(this double value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(decimal)"/>
         */
        [Pure] public static int Sign(this decimal value) {
            return Math.Sign(value);
        }

        /// <returns><paramref name="value"/>.<see cref="TimeSpan.CompareTo(object)"/>(<see cref="TimeSpan.Zero"/>)</returns>
        [Pure] public static int Sign(this TimeSpan value) {
            return value.CompareTo(TimeSpan.Zero);
        }

        #endregion

        #region IsPositive

        [Pure] public static bool IsPositive(this short    value) => value >= 0;
        [Pure] public static bool IsPositive(this int      value) => value >= 0;
        [Pure] public static bool IsPositive(this long     value) => value >= 0;
        [Pure] public static bool IsPositive(this float    value) => value >= 0;
        [Pure] public static bool IsPositive(this double   value) => value >= 0;
        [Pure] public static bool IsPositive(this decimal  value) => value >= 0;
        [Pure] public static bool IsPositive(this TimeSpan value) => value >= TimeSpan.Zero;

        /// <summary>
        /// Attempts to call the appropriate <see cref="IsPositive(short)"/> overload.
        /// </summary>
        /// <param name="value">a value supported by <see cref="IsPositive(short)"/>, etc.</param>
        /// <returns>true if <paramref name="value"/> ≥ 0</returns>
        /// <exception cref="ArgumentException">if <paramref name="value"/> isn't a type with an appropriate <see cref="IsPositive(short)"/> method</exception>
        [Pure]
        public static bool IsPositive<T>(T value) {
            return value switch {
                null => false,
                short s    => s.IsPositive(),
                int i      => i.IsPositive(),
                long l     => l.IsPositive(),
                float f    => f.IsPositive(),
                double d   => d.IsPositive(),
                decimal d  => d.IsPositive(),
                TimeSpan t => t.IsPositive(),
                _ => throw RejectArgument.UnhandledSwitchType(value, nameof(value), nameof(IsPositive)),
            };
        }

        #endregion

        #region IsStrictlyPositive

        [Pure] public static bool IsStrictlyPositive(this short    value) => value > 0;
        [Pure] public static bool IsStrictlyPositive(this int      value) => value > 0;
        [Pure] public static bool IsStrictlyPositive(this long     value) => value > 0;
        [Pure] public static bool IsStrictlyPositive(this float    value) => value > 0;
        [Pure] public static bool IsStrictlyPositive(this double   value) => value > 0;
        [Pure] public static bool IsStrictlyPositive(this decimal  value) => value > 0;
        [Pure] public static bool IsStrictlyPositive(this TimeSpan value) => value > TimeSpan.Zero;

        /// <summary>
        /// Attempts to call the appropriate <see cref="IsStrictlyPositive(short)"/> overload.
        /// </summary>
        /// <param name="value">a value supported by <see cref="IsStrictlyPositive(short)"/>, etc.</param>
        /// <returns>true if <paramref name="value"/> > 0</returns>
        /// <exception cref="ArgumentException">if <paramref name="value"/> isn't a type with an appropriate <see cref="IsStrictlyPositive(short)"/> method</exception>
        [Pure]
        public static bool IsStrictlyPositive<T>(T value) {
            return value switch {
                null       => false,
                short s    => s.IsStrictlyPositive(),
                int i      => i.IsStrictlyPositive(),
                long l     => l.IsStrictlyPositive(),
                float f    => f.IsStrictlyPositive(),
                double d   => d.IsStrictlyPositive(),
                decimal d  => d.IsStrictlyPositive(),
                TimeSpan t => t.IsStrictlyPositive(),
                _          => throw RejectArgument.UnhandledSwitchType(value, nameof(value), nameof(IsStrictlyPositive)),
            };
        }

        #endregion

        #region IsNegative

        [Pure] public static bool IsNegative(this short    value) => value < 0;
        [Pure] public static bool IsNegative(this int      value) => value < 0;
        [Pure] public static bool IsNegative(this long     value) => value < 0;
        [Pure] public static bool IsNegative(this float    value) => value < 0;
        [Pure] public static bool IsNegative(this double   value) => value < 0;
        [Pure] public static bool IsNegative(this decimal  value) => value < 0;
        [Pure] public static bool IsNegative(this TimeSpan value) => value < TimeSpan.Zero;

        /// <summary>
        /// Attempts to call the appropriate <see cref="IsNegative(short)"/> overload.
        /// </summary>
        /// <param name="value">a value supported by <see cref="IsNegative(short)"/>, etc.</param>
        /// <returns>true if <paramref name="value"/> <![CDATA[<]]> 0</returns>
        /// <exception cref="ArgumentException">if <paramref name="value"/> isn't a type with an appropriate <see cref="IsNegative(short)"/> method</exception>
        [Pure]
        public static bool IsNegative<T>(T value) {
            return value switch {
                null => false,
                short s    => s.IsNegative(),
                int i      => i.IsNegative(),
                long l     => l.IsNegative(),
                float f    => f.IsNegative(),
                double d   => d.IsNegative(),
                decimal d  => d.IsNegative(),
                TimeSpan t => t.IsNegative(),
                _          => throw RejectArgument.UnhandledSwitchType(value, nameof(value), nameof(IsNegative)),
            };
        }

        #endregion

        #endregion
    }
}