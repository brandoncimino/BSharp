using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Chronic {
    /// <summary>
    /// Extension methods for <see cref="TimeSpan"/>.
    ///
    /// TODO: Decide if this can / should coexist with <see cref="TimeUtils"/>...
    /// </summary>
    [PublicAPI]
    public static class TimeSpanExtensions {
        /// <summary>
        /// Returns the total number of some <see cref="TimeUnit"/> contained within a <see cref="TimeSpan"/>.
        /// </summary>
        /// <remarks>
        /// Uses the dedicated <see cref="TimeSpan"/> property, e.g. <see cref="TimeSpan.TotalHours"/>, wherever possible.
        /// </remarks>
        public static double TotalOf(this TimeSpan timeSpan, TimeUnit timeUnit) {
            return timeUnit switch {
                TimeUnit.Milliseconds      => timeSpan.TotalMilliseconds,
                TimeUnit.Seconds           => timeSpan.TotalSeconds,
                TimeUnit.Minutes           => timeSpan.TotalMinutes,
                TimeUnit.Hours             => timeSpan.TotalHours,
                TimeUnit.Days              => timeSpan.TotalDays,
                TimeUnit.Weeks             => timeSpan.TotalDays / 7,
                TimeUnit.Ticks             => timeSpan.Ticks,
                TimeUnit.ShortRests        => timeSpan.TotalHours,
                TimeUnit.LongRests         => timeSpan.TotalHours   / 8,
                TimeUnit.QueensberryRounds => timeSpan.TotalMinutes / 3,
                _                          => throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null)
            };
        }
    }
}