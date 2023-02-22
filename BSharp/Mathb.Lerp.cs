// ReSharper disable UseDeconstructionOnParameter

using System.Runtime.CompilerServices;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        /// <summary>
        /// Returns the value <paramref name="lerpAmount"/> of the way from <paramref name="start"/> to <paramref name="finish"/>. 
        /// </summary>
        /// <param name="start">the starting point, returned when <paramref name="lerpAmount"/> is ≤ 0</param>
        /// <param name="finish">the ending point, returned when <paramref name="lerpAmount"/> is ≥ 1</param>
        /// <param name="lerpAmount">how far we want to travel from <paramref name="start"/> (0) towards <paramref name="finish"/> (1)</param>
        /// <param name="roundingMode">the <see cref="RoundingMode"/> that should be applied to the result. Defaults to <see cref="RoundingMode.HalfEven"/></param>
        /// <returns>a value between <see cref="start"/> and <see cref="finish"/>, inclusive</returns>
        /// <remarks>
        /// 📎 <paramref name="lerpAmount"/> will be "clamped" between 0 and 1, meaning that the result will always be between <paramref name="start"/> and <paramref name="finish"/>.
        /// </remarks>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LerpInt(
            int          start,
            int          finish,
            double       lerpAmount,
            RoundingMode roundingMode = default
        ) {
            return lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (lerpAmount * (finish - start)).RoundToInt(roundingMode)
            };
        }

        /// <inheritdoc cref="LerpInt(int,int,double,RoundingMode)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LerpInt(
            long         start,
            long         finish,
            double       lerpAmount,
            RoundingMode roundingMode = default
        ) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + ((finish - start) * lerpAmount).Round(roundingMode).ToLong(),
            };

        /// <summary>
        /// <inheritdoc cref="LerpInt(int,int,double,RoundingMode)"/>
        /// </summary>
        /// <param name="start">the starting point, returned when <paramref name="lerpAmount"/> is ≤ 0</param>
        /// <param name="finish">the ending point, returned when <paramref name="lerpAmount"/> is ≥ 0</param>
        /// <param name="lerpAmount">how far we want to move from <paramref name="start"/> (0) towards <paramref name="finish"/> (1)</param>
        /// <returns>a value between <see cref="start"/> and <see cref="finish"/>, inclusive</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float start, float finish, float lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start) * lerpAmount,
            };

        /// <inheritdoc cref="Lerp(float,float,float)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double start, double finish, double lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start) * lerpAmount,
            };

        /// <inheritdoc cref="Lerp(float,float,float)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Lerp(decimal start, decimal finish, decimal lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start) * lerpAmount,
            };

        /// <inheritdoc cref="Lerp(float,float,float)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Lerp(TimeSpan start, TimeSpan finish, double lerpAmount) =>
            lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start).Multiply(lerpAmount),
            };

        /// <inheritdoc cref="Lerp(float,float,float)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime Lerp(DateTime start, DateTime finish, double lerpAmount) {
            return lerpAmount switch {
                <= 0 => start,
                >= 1 => finish,
                _    => start + (finish - start).Multiply(lerpAmount),
            };
        }

        #region Tuple extension methods

        /// <inheritdoc cref="LerpInt(int,int,double,FowlFever.BSharp.RoundingMode)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LerpInt(this (int start, int finish) range, double lerpAmount, RoundingMode roundingMode = default) => LerpInt(range.start, range.finish, lerpAmount, roundingMode);

        /// <inheritdoc cref="LerpInt(long, long,double,FowlFever.BSharp.RoundingMode)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LerpInt(this (long start, long finish) range, double lerpAmount, RoundingMode roundingMode = default) => LerpInt(range.start, range.finish, lerpAmount, roundingMode);

        /// <inheritdoc cref="Lerp(float,float,float)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(this (float start, float finish) range, float lerpAmount) => Lerp(range.start, range.finish, lerpAmount);

        /// <inheritdoc cref="Lerp(double, double, double)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(this (double start, double finish) range, double lerpAmount) => Lerp(range.start, range.finish, lerpAmount);

        /// <inheritdoc cref="Lerp(decimal, decimal, decimal)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Lerp(this (decimal start, decimal finish) range, decimal lerpAmount) => Lerp(range.start, range.finish, lerpAmount);

        /// <inheritdoc cref="Lerp(TimeSpan, TimeSpan, double)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Lerp(this (TimeSpan start, TimeSpan finish) range, double lerpAmount) => Lerp(range.start, range.finish, lerpAmount);

        /// <inheritdoc cref="Lerp(DateTime, DateTime, double)"/>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime Lerp(this (DateTime start, DateTime finish) range, double lerpAmount) => Lerp(range.start, range.finish, lerpAmount);

        #endregion

        #region Inverse Lerp (Reverp?)

        public static float InverseLerp(float start, float finish, float lerpedAmount) {
            throw new NotImplementedException("STOOOOOOOOOP!");
        }

        #endregion

        #region Terp (take lerp) (formally "Splerp" / "Sperp" ("split lerp"), but that's too similar to "slerp" ("spherical lerp")

        /// <summary>
        /// Performs a "take lerp" operation, returning a <see cref="ValueTuple{T1,T2}"/> containing:
        /// <ul>
        /// <li><b><c>taken</c></b>: The amount <paramref name="lerpAmount"/> of the way from <paramref name="start"/> to <paramref name="finish"/></li>
        /// <li><b><c>leftovers</c></b>: The leftovers from <b><c>taken</c></b> to <paramref name="finish"/></li>
        /// </ul>
        /// </summary>
        /// <example>
        /// This method ensures that all of the "distance" between <paramref name="start"/> and <paramref name="finish"/>
        /// is accounted for, which can be easy to mess up when doing multiple discrete <see cref="LerpInt(int,int,double,RoundingMode)"/> operations.
        /// <p/>
        /// For example, consider whole puppies, which we want to divide 70/30:
        /// <code><![CDATA[
        /// int 🐶    = 5;
        /// double 🍕 = .3;
        ///
        /// int 👈    = LerpInt(0, 🐶, 🍕);         // => 0 + Math.Round(5 * .3) => Math.Round(1.5) => 2  
        /// int 👉    = LerpInt(0, 🐶, (1 - 🍕));   // => 0 + Math.Round(5 * .7) => Math.Round(3.5) => 4
        ///   
        /// int 👯    = 👈 + 👉;                    // => 2 + 4 => 6
        /// ]]></code>
        /// We've <i>gained</i> a 🐶!
        /// <p/>
        /// But no longer!
        /// <code><![CDATA[
        /// int 🐶       = 5;
        /// double 🍕    = 0.3;
        ///
        /// var (👈, 👉) = Splerp(0, 🐶, 🍕);    // => (👈: LerpInt(0, 5, .3) => 2, 👉: 5 - 👈 => 3)
        /// int 👯‍       = 👈 + 👉;              // => (👈: 2, 👉: 3) => 2 + 3 => 5 == 🐶!
        /// ]]></code>
        /// </example>
        /// <param name="start">the lowest value</param>
        /// <param name="finish">the highest value</param>
        /// <param name="lerpAmount">the amount between <paramref name="start"/> and <paramref name="finish"/> that we want to take</param>
        /// <param name="takePortionRounding">how we should apply rounding to the <b><i>left</i></b> portion of the results. Defaults to <see cref="RoundingMode.HalfEven"/></param>
        /// <returns>(<see cref="int"/> taken, <see cref="int"/> leftovers)</returns>
        /// <remarks>
        /// This was previously named <c>"Splerp"</c> / <c>"Sperp"</c>, for "split lerp", but that causes confusion with <c>"slerp"</c>, for <a href="https://en.wikipedia.org/wiki/Slerp">"spherical lerp"</a>. 
        /// </remarks>
        public static (int taken, int leftovers) Terp(
            int          start,
            int          finish,
            double       lerpAmount,
            RoundingMode takePortionRounding = default
        ) {
            var leftPortion = LerpInt(start, finish, lerpAmount, takePortionRounding);
            return (start + leftPortion, finish - leftPortion);
        }

        /// <inheritdoc cref="Terp(int,int,double,RoundingMode)"/>
        /// <remarks><see cref="Bisect"/> is equivalent to <see cref="Terp(int,int,double,RoundingMode)"/>-ing with <c>.5</c>.</remarks>
        public static (int taken, int leftovers) Bisect(this int total, RoundingMode leftPartitionRounding = default) => total.Terp(.5, leftPartitionRounding);

        /// <summary>
        /// Performs a <see cref="Terp(int,int,double,RoundingMode)"/> from <c>0</c> to <paramref name="total"/>.
        /// </summary>
        /// <param name="total">the amount of stuff we're divvying up</param>
        /// <param name="lerpAmount">the amount of the <paramref name="total"/> that we're going to take</param>
        /// <param name="takePortionRounding">the <see cref="RoundingMode"/> that should be applied to the taken amount. Defaults to <see cref="RoundingMode.HalfEven"/></param>
        /// <example>
        /// <inheritdoc cref="Terp(int,int,double,RoundingMode)"/>
        /// </example>
        /// <returns>
        /// (<see cref="int"/> taken, <see cref="int"/> leftovers)
        /// </returns>
        public static (int taken, int leftovers) Terp(this int total, double lerpAmount, RoundingMode takePortionRounding = default)
            => Terp(0, total, lerpAmount, takePortionRounding);

        /// <inheritdoc cref="Terp(int,int,double,RoundingMode)"/>
        public static (long A, long B) Terp(
            long         start,
            long         finish,
            double       lerpAmount,
            RoundingMode takePortionRounding = default
        ) {
            var leftPortion = LerpInt(start, finish, lerpAmount, takePortionRounding);
            return (start + leftPortion, finish - leftPortion);
        }

        /// <inheritdoc cref="Terp(int,double,RoundingMode)"/>
        public static (long A, long B) Terp(this long total, double lerpAmount, RoundingMode takePortionRounding = default)
            => Terp(0, total, lerpAmount, takePortionRounding);

        #endregion
    }
}