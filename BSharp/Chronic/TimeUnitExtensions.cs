using System;
using System.Collections.Generic;

using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Chronic {
    /// <summary>
    /// Extension methods for the <see cref="TimeUnit"/> enum.
    /// </summary>
    [PublicAPI]
    public static class TimeUnitExtensions {
        /// <summary>
        /// The number of <see cref="TimeUnit.Ticks"/> for some amount of the <paramref name="originalUnit"/>.
        /// </summary>
        /// <remarks>This is the inverse of <see cref="FromTicks"/>.</remarks>
        /// <param name="originalUnit">the original <see cref="TimeUnit"/></param>
        /// <param name="originalAmount">the amount of the original <see cref="TimeUnit"/></param>
        /// <returns>a number of <see cref="TimeUnit.Ticks"/></returns>
        /// <seealso cref="FromTicks"/>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unsupported <see cref="TimeUnit"/> is provided</exception>
        public static long ToTicks(this TimeUnit originalUnit, double originalAmount = 1) {
            return (long)(originalUnit.TicksPer() * originalAmount);
        }

        private static Dictionary<TimeUnit, long> TicksPerUnit = new Dictionary<TimeUnit, long>() {
            { TimeUnit.Ticks, 1 },
            { TimeUnit.Milliseconds, TimeSpan.TicksPerMillisecond },
            { TimeUnit.Seconds, TimeSpan.TicksPerSecond },
            { TimeUnit.Minutes, TimeSpan.TicksPerMinute },
            { TimeUnit.Hours, TimeSpan.TicksPerHour },
            { TimeUnit.Days, TimeSpan.TicksPerDay },
            { TimeUnit.Weeks, TimeSpan.TicksPerDay                * 7 },
            { TimeUnit.ShortRests, TimeSpan.TicksPerHour          * 1 },
            { TimeUnit.LongRests, TimeSpan.TicksPerHour           * 8 },
            { TimeUnit.QueensberryRounds, TimeSpan.TicksPerMinute * 3 },
        };

        /// <summary>
        /// The number of <see cref="TimeUnit.Ticks"/> for a <b>single</b> instance of <paramref name="timeUnit"/>.
        /// </summary>
        /// <param name="timeUnit">the original <see cref="TimeUnit"/></param>
        /// <returns>the number of <see cref="TimeUnit.Ticks"/> per <paramref name="timeUnit"/></returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unsupported <see cref="TimeUnit"/> is provided</exception>
        public static long TicksPer(this TimeUnit timeUnit) {
            return TicksPerUnit.ContainsKey(timeUnit) ? TicksPerUnit[timeUnit] : throw BEnum.InvalidEnumArgumentException(nameof(timeUnit), timeUnit);
        }

        /// <summary>
        /// Converts a number of <see cref="TimeUnit.Ticks"/> to the <paramref name="targetUnit"/>.
        /// </summary>
        /// <remarks>This is the inverse of <see cref="ToTicks"/>.</remarks>
        /// <param name="targetUnit">the target <see cref="TimeUnit"/></param>
        /// <param name="originalTicks">the original <see cref="TimeUnit.Ticks"/></param>
        /// <returns>an amount of the <paramref name="targetUnit"/></returns>
        /// <seealso cref="ToTicks"/>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unsupported <see cref="TimeUnit"/> is provided</exception>
        public static double FromTicks(this TimeUnit targetUnit, long originalTicks) {
            return (double)originalTicks / targetUnit.ToTicks();
        }

        /// <summary>
        /// Creates a new <see cref="TimeSpan"/> from an amount of the given <see cref="TimeUnit"/>.
        /// </summary>
        /// <param name="timeUnit">a <see cref="TimeUnit"/></param>
        /// <param name="amount">the amount of the <paramref name="timeUnit"/></param>
        /// <returns>a new <see cref="TimeSpan"/></returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unsupported <see cref="TimeUnit"/> is provided</exception>
        public static TimeSpan SpanOf(this TimeUnit timeUnit, double amount = 1) {
            return timeUnit switch {
                TimeUnit.Milliseconds => TimeSpan.FromMilliseconds(amount),
                TimeUnit.Seconds      => TimeSpan.FromSeconds(amount),
                TimeUnit.Minutes      => TimeSpan.FromMinutes(amount),
                TimeUnit.Hours        => TimeSpan.FromHours(amount),
                TimeUnit.Days         => TimeSpan.FromDays(amount),
                TimeUnit.Ticks        => TimeSpan.FromTicks((long)amount),
                _                     => TimeSpan.FromTicks(timeUnit.ToTicks(amount)) // others should default to whatever their conversion ToTicks is
            };
        }
    }
}