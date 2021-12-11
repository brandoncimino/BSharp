using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Chronic {
    /// <summary>
    /// Contains utility methods that manipulate or extend <see cref="DateTime" />, <see cref="TimeSpan" />, etc.
    ///
    /// TODO: Figure out if I want "{X}Extensions" or "{X}Utils"..."{X}Extensions" seems to be the standard practice, but that feels arbitrary - after all, it makes sense to call methods with the syntax TimeUtils.Min(a,b) or TimeUtils.Divide(n), but also maybe a.Min(b)...
    /// </summary>
    [PublicAPI]
    public static class TimeUtils {
        /// <summary>
        /// Creates a <see cref="TimeSpan"/> of some amount of the given <see cref="TimeUnit"/>.
        /// </summary>
        /// <param name="amount">the number of <see cref="TimeUnit"/>s</param>
        /// <param name="timeUnit">the <see cref="TimeUnit"/> of <paramref name="amount"/>"/></param>
        /// <returns>a new <see cref="TimeSpan"/></returns>
        [Pure]
        public static TimeSpan SpanOf(double amount, TimeUnit timeUnit) {
            return timeUnit.SpanOf(amount);
        }

        #region Arithmetic

        #region Addition

        [Pure]
        public static TimeSpan Add(this TimeSpan span, double amount, TimeUnit unit) {
            return span + SpanOf(amount, unit);
        }

        [Pure]
        public static DateTime Add(this DateTime date, double amount, TimeUnit unit) {
            return date + SpanOf(amount, unit);
        }

        #endregion

        #region Subtraction

        [Pure]
        public static TimeSpan Subtract(this TimeSpan span, double amount, TimeUnit unit) {
            return span - SpanOf(amount, unit);
        }

        [Pure]
        public static DateTime Subtract(this DateTime date, double amount, TimeUnit unit) {
            return date - SpanOf(amount, unit);
        }

        #endregion

        #region Division

        /// <summary>
        /// Mimics .NET Core's <a href="https://docs.microsoft.com/en-us/dotnet/api/system.timespan.divide">TimeSpan.Divide</a>.
        /// Does this by converting the given <see cref="TimeSpan" />s into <see cref="TimeSpan.TotalSeconds" /> and performing the division on those.
        /// </summary>
        /// <remarks>
        /// This originally performed the division on <see cref="TimeSpan.Ticks" /> rather than <see cref="TimeSpan.TotalSeconds" />, but this was actually slightly inaccurate due to the number of ticks being so large.
        ///
        /// Update from Brandon on 8/15/2021: What did I mean? How was using <see cref="TimeSpan.Ticks"/> less accurate than <see cref="TimeSpan.TotalSeconds"/>...?
        /// <br/>
        /// I would prefer to name this "DividedBy", by I named it "Divide" for parity with .NET Core's <a href="https://docs.microsoft.com/en-us/dotnet/api/system.timespan.divide">TimeSpan.Divide</a>.
        /// </remarks>
        /// <param name="dividend">the <see cref="TimeSpan"/> to be divided (i.e. top of the fraction)</param>
        /// <param name="divisor">the <see cref="TimeSpan"/> by which the <paramref name="dividend" /> will be divided (i.e. the bottom of the fraction)</param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException">if the <see cref="divisor"/> is <see cref="TimeSpan.Zero"/></exception>
        [Pure]
        public static double Divide(this TimeSpan dividend, TimeSpan divisor) {
            ValidateDivisor(divisor);
            return (double)dividend.Ticks / divisor.Ticks;
        }

        /// <summary>
        /// Divides <paramref name="dividend"/> by <paramref name="divisor"/>, returning a new <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="dividend">the <see cref="TimeSpan"/> to be divided (i.e. the top of the fraction)</param>
        /// <param name="divisor">the number to divide the <see cref="dividend"/> by (i.e. the bottom of the fraction)</param>
        /// <returns></returns>
        [Pure]
        public static TimeSpan Divide(this TimeSpan dividend, double divisor) {
            return TimeSpan.FromTicks((long)(dividend.Ticks / divisor));
        }


        /// <inheritdoc cref="Divide(System.TimeSpan,double)"/>
        [Pure]
        public static TimeSpan Divide(this DateTime dividend, double divisor) {
            return dividend.AsTimeSpan().Divide(divisor);
        }

        /// <summary>
        ///     Divides <paramref name="dividend" /> by <paramref name="divisor" />, returning the integer quotient.
        /// </summary>
        /// <param name="dividend">The number to be divided (i.e. top of the fraction)</param>
        /// <param name="divisor">The number by which <paramref name="dividend" /> will be divided (i.e. the bottom of the fraction)</param>
        /// <returns></returns>
        [Pure]
        public static double Quotient(this TimeSpan dividend, TimeSpan divisor) {
            ValidateDivisor(divisor);
            return Math.Floor(Divide(dividend, divisor));
        }

        /// <summary>
        ///     Returns the <see cref="TimeSpan" /> remainder after <paramref name="dividend" /> is divided by <paramref name="divisor" />.
        /// </summary>
        /// <param name="dividend">The number to be divided (i.e. top of the fraction)</param>
        /// <param name="divisor">The number by which <paramref name="dividend" /> will be divided (i.e. the bottom of the fraction)</param>
        /// <returns></returns>
        [Pure]
        public static TimeSpan Modulus(this TimeSpan dividend, TimeSpan divisor) {
            ValidateDivisor(divisor);
            return TimeSpan.FromTicks(dividend.Ticks % divisor.Ticks);
        }

        private static void ValidateDivisor(TimeSpan divisor) {
            if (divisor == TimeSpan.Zero) {
                throw new DivideByZeroException($"Cannot divide by a zero {nameof(TimeSpan)}!");
            }
        }

        #endregion

        #region Multiplication

        /// <summary>
        ///     Multiplies <paramref name="timeSpan" /> by <paramref name="factor" />, returning a new <see cref="TimeSpan" />.
        /// </summary>
        /// <remarks>
        /// This is taken as verbatim as possible from the source code of <a href="https://docs.microsoft.com/en-us/dotnet/api/system.timespan.op_multiply?view=net-6.0">TimeSpan.Multiply()</a>
        /// </remarks>
        /// <param name="timeSpan"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        [Pure]
        public static TimeSpan Multiply(this TimeSpan timeSpan, double factor) {
            if (double.IsNaN(factor)) {
                throw new ArgumentException($"Cannot multiply a {nameof(TimeSpan)} by {double.NaN}", nameof(factor));
            }

            // (The following comment is taken from the .NET source code)
            //
            // Rounding to the nearest tick is as close to the result we would have with unlimited
            // precision as possible, and so likely to have the least potential to surprise.
            double ticks = Math.Round(timeSpan.Ticks * factor);
            return IntervalFromDoubleTicks(ticks);
        }

        /// <remarks>
        /// This is taken as verbatim as possible from the source code of <a href="https://docs.microsoft.com/en-us/dotnet/api/system.timespan.op_multiply?view=net-6.0">TimeSpan.Multiply()</a>.
        ///
        /// I do not know why they have an explicit check for <see cref="long.MaxValue"/>, but not <see cref="long.MinValue"/>...
        /// </remarks>
        [SuppressMessage("ReSharper", "All")]
        internal static TimeSpan IntervalFromDoubleTicks(double ticks) {
            if (ticks is > long.MaxValue or < long.MinValue or double.NaN)
                throw new OverflowException($"Tick count [{ticks}] was out of bounds for a long, so it can't fit inside of a {nameof(TimeSpan)}!");
            if (ticks == long.MaxValue)
                return TimeSpan.MaxValue;
            return new TimeSpan((long)ticks);
        }

        #endregion

        #endregion

        #region Precision Normalization

        /// <inheritdoc cref="NormalizePrecision" />
        public static double NormalizeMinutes(double minutes) {
            return TimeSpan.FromMinutes(minutes).TotalMinutes;
        }

        /// <inheritdoc cref="NormalizePrecision" />
        public static double NormalizeSeconds(double seconds) {
            return TimeSpan.FromSeconds(seconds).TotalSeconds;
        }

        /// <inheritdoc cref="NormalizePrecision" />
        public static double NormalizeHours(double hours) {
            return TimeSpan.FromHours(hours).TotalHours;
        }

        /// <inheritdoc cref="NormalizePrecision" />
        public static double NormalizeDays(double days) {
            return TimeSpan.FromDays(days).TotalDays;
        }

        /// <inheritdoc cref="NormalizePrecision" />
        public static double NormalizeMilliseconds(double milliseconds) {
            return TimeSpan.FromMilliseconds(milliseconds).TotalMilliseconds;
        }

        /// <summary>
        ///     Reduces the given <paramref name="value" /> so that it matches the appropriate precision for the given <paramref name="unit" /> component of a <see cref="TimeSpan" />.
        /// </summary>
        /// <remarks>
        ///     <li>Converts <paramref name="value" /> into a <see cref="TimeSpan" /> via the given <paramref name="unit" />, then returns the total <paramref name="unit" />s of the new <see cref="TimeSpan" />.</li>
        ///     <li>
        ///         Joins together the multiple "Normalize" methods, e.g. <see cref="NormalizeMinutes" />, into one method, via <see cref="TimeUnit" />.
        ///         <ul>
        ///             <li>
        ///                 The individual methods such as <see cref="NormalizeDays" /> are maintained for parity with <see cref="TimeSpan" /> methods such as <see cref="TimeSpan.FromDays" />.
        ///             </li>
        ///         </ul>
        ///     </li>
        /// </remarks>
        /// <example>
        ///     TODO: Add an example, because this is kinda hard to explain without one.
        ///     TODO: Future Brandon, on 8/16/2021, can confirm past Brandon's assessment from 9/22/2020.
        /// </example>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double NormalizePrecision(double value, TimeUnit unit) {
            return unit switch {
                TimeUnit.Milliseconds => NormalizeMilliseconds(value),
                TimeUnit.Seconds      => NormalizeSeconds(value),
                TimeUnit.Minutes      => NormalizeMinutes(value),
                TimeUnit.Hours        => NormalizeHours(value),
                TimeUnit.Days         => NormalizeDays(value),
                _                     => throw BEnum.InvalidEnumArgumentException(nameof(unit), unit)
            };
        }

        #endregion

        /// <summary>
        /// Corresponds to <see cref="Math.Min(int, int)"/>, etc.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [Pure]
        public static DateTime Min(this DateTime a, DateTime b, params DateTime[] c) {
            return c.Append(a).Append(b).Min();
        }

        /// <summary>
        /// Corresponds to <see cref="Math.Max(byte,byte)"/>, etc.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [Pure]
        public static DateTime Max(this DateTime a, DateTime b, params DateTime[] c) {
            return c.Append(a).Append(b).Max();
        }

        [Pure]
        public static TimeSpan Min(this TimeSpan a, TimeSpan b, params TimeSpan[] c) {
            return c.Append(a).Append(b).Min();
        }

        [Pure]
        public static TimeSpan Max(this TimeSpan a, TimeSpan b, params TimeSpan[] c) {
            return c.Append(a).Append(b).Max();
        }

        /// <summary>
        /// Converts <see cref="DateTime"/> <paramref name="dateTime"/> into a <see cref="TimeSpan"/> representing the elapsed time since <see cref="DateTime.MinValue"/>.
        /// </summary>
        /// <remarks>
        /// A bunch of people on the stackoverflow that shows up as the first search result, <a href="https://stackoverflow.com/questions/17959440/convert-datetime-to-timespan">Convert DateTime to TimeSpan</a>, suggest using <see cref="DateTime.TimeOfDay"/> - which is an absolutely bafflingly incorrect answer because <see cref="DateTime.TimeOfDay"/> gives you the time elapsed <b><i>today</i></b>, discarding almost all of the information in the <see cref="DateTime"/>...
        /// <p/>Sidenote - "stackoverflow" and "stackexchange" might be different websites...?
        /// </remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [Pure]
        public static TimeSpan AsTimeSpan(this DateTime dateTime) {
            return TimeSpan.FromTicks(dateTime.Ticks);
        }

        /// <summary>
        /// Converts <see cref="TimeSpan"/> <paramref name="timeSpan"/> into a <see cref="DateTime"/> representing the date if <paramref name="timeSpan"/> had elapsed since <see cref="DateTime.MinValue"/>.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        [Pure]
        public static DateTime AsDateTime(this TimeSpan timeSpan) {
            return new DateTime(timeSpan.Ticks);
        }

        /// <summary>
        /// Equivalent to calling <see cref="Enumerable.Sum(System.Collections.Generic.IEnumerable{decimal})"/> against a <see cref="TimeSpan"/>.
        /// </summary>
        /// <remarks>
        /// As of 8/26/2020, despite methods like <see cref="Enumerable.Min(System.Collections.Generic.IEnumerable{decimal})"/> having genericized versions (that I can't seem to create a direct link doc comment link to), <a href="https://stackoverflow.com/questions/4703046/sum-of-timespans-in-c-sharp">.Sum() does not</a>.
        /// </remarks>
        /// <param name="timeSpans"></param>
        /// <returns></returns>
        [Pure]
        public static TimeSpan Sum(this IEnumerable<TimeSpan> timeSpans) {
            return new TimeSpan(timeSpans.Sum(it => it.Ticks));
        }

        /// <summary>
        /// Attempts to convert <paramref name="value"/> to a <see cref="TimeSpan"/>, either by:
        /// <li>Directly casting <paramref name="value"/>, i.e. <c>(TimeSpan)value</c></li>
        /// <li>Casting <paramref name="value"/> to a number type (int, long, etc.; casting that to a <c>long</c> if necessary) and passing it to <see cref="TimeSpan.FromTicks"/></li>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static TimeSpan? TimeSpanFromObject(object value) {
            switch (value) {
                case TimeSpan timeSpan:
                    return timeSpan;
                case DateTime dateTime:
                    return dateTime.AsTimeSpan();
                case int i:
                    return TimeSpan.FromTicks(i);
                case long l:
                    return TimeSpan.FromTicks(l);
                case float f:
                    return TimeSpan.FromTicks((long)f);
                case double d:
                    return TimeSpan.FromTicks((long)d);
                case decimal d:
                    return TimeSpan.FromTicks((long)d);
                case string s:
                    return TimeSpan.Parse(s);
                default:
                    try {
                        return (TimeSpan)Convert.ChangeType(value, typeof(TimeSpan));
                    }
                    catch {
                        return null;
                    }
            }
        }

        /// <summary>
        /// <inheritdoc cref="TimeSpanFromObject"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">If the <see cref="value"/> could not be converted to a <see cref="TimeSpan"/></exception>
        [Pure]
        public static TimeSpan TimeSpanOf(object value) {
            return TimeSpanFromObject(value) ?? throw new InvalidCastException($"Could not convert {nameof(value)} [{value?.GetType().Name}]{value} to a {nameof(TimeSpan)}!");
        }

        [Obsolete("Please call " + nameof(MethodTimer) + "." + nameof(MethodTimer.MeasureExecution) + " directly", true)]
        public static AggregateExecutionTime AverageExecutionTime(Action action, int iterations = 1) {
            return MethodTimer.MeasureExecution(action, iterations);
        }
    }
}