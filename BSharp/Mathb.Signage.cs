using System.Runtime.CompilerServices;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        #region Signage

        #region Math.Sign()

        /// <inheritdoc cref="Math.Sign(short)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this short value) => Math.Sign(value);

        /// <inheritdoc cref="Math.Sign(int)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this int integer) => Math.Sign(integer);

        /// <inheritdoc cref="Math.Sign(long)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this long value) => Math.Sign(value);

        /// <inheritdoc cref="Math.Sign(float)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this float value) => Math.Sign(value);

        /// <inheritdoc cref="Math.Sign(double)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this double value) => Math.Sign(value);

        /// <inheritdoc cref="Math.Sign(decimal)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this decimal value) => Math.Sign(value);

        /// <returns><paramref name="value"/>.<see cref="TimeSpan.CompareTo(TimeSpan)"/>(<see cref="TimeSpan.Zero"/>)</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this TimeSpan value) => value.CompareTo(TimeSpan.Zero);

        #endregion

        #region IsPositive

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this short value) => value >= 0;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this int value) => value >= 0;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this long value) => value >= 0;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this float value) => value >= 0;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this double value) => value >= 0;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this decimal value) => value >= 0;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this TimeSpan value) => value >= TimeSpan.Zero;

        /// <returns><paramref name="value"/> &ge; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this sbyte value) => value >= 0;

        #endregion

        #region IsStrictlyPositive

        /// <returns><paramref name="value"/> &gt; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this short value) => value > 0;

        /// <returns><paramref name="value"/> &gt; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this int value) => value > 0;

        /// <returns><paramref name="value"/> &gt; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this long value) => value > 0;

        /// <returns><paramref name="value"/> &gt; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this float value) => value > 0;

        /// <returns><paramref name="value"/> &gt; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this double value) => value > 0;

        /// <returns><paramref name="value"/> &gt; 0</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this decimal value) => value > 0;

        /// <returns><paramref name="value"/> &gt; <see cref="TimeSpan.Zero"/></returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrictlyPositive(this TimeSpan value) => value > TimeSpan.Zero;

        #endregion

        #region IsNegative

        /// <returns><param name="value"> &lt; 0</param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this short value) => value < 0;

        /// <returns><param name="value"> &lt; 0</param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this int value) => value < 0;

        /// <returns><param name="value"> &lt; 0</param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this long value) => value < 0;

        /// <returns><param name="value"> &lt; 0</param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this float value) => value < 0;

        /// <returns><param name="value"> &lt; 0</param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this double value) => value < 0;

        /// <returns><param name="value"> &lt; 0</param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this decimal value) => value < 0;

        /// <returns><param name="value"> &lt; <see cref="TimeSpan.Zero"/></param></returns> 
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this TimeSpan value) => value < TimeSpan.Zero;

        #endregion

        #endregion
    }
}