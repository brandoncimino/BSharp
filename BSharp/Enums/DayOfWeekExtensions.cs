using System;
using System.Globalization;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums {
    /// <summary>
    /// Silly extension methods for <see cref="DayOfWeek"/>
    /// </summary>
    [PublicAPI]
    public static class DayOfWeekExtensions {
        public static bool IsWeekend(this DayOfWeek dayOfWeek) {
            return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        }

        /**
         * <returns>true if this is a weekday, as extremely poorly defined by <a href="https://en.wikipedia.org/wiki/Workweek_and_weekend#Americas">Workweek and weekend</a></returns>
         * <remarks>
         * TODO: add an optional <see cref="CultureInfo"/> parameter and support for 6-day weeks and...stuff
         * </remarks>
         */
        public static bool IsWeekday(this DayOfWeek dayOfWeek) {
            return !dayOfWeek.IsWeekend();
        }

        /**
         * <returns>true on <see cref="DayOfWeek.Wednesday"/></returns>
         */
        public static bool IsHumpDay(this DayOfWeek dayOfWeek) {
            return dayOfWeek == DayOfWeek.Wednesday;
        }

        /// <returns>true if there is school tomorrow</returns>
        public static bool IsSchoolNight(this DayOfWeek dayOfWeek) {
            return !(dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday);
        }

        public static DayOfWeek Tomorrow(this DayOfWeek today) {
            return today.Next();
        }

        public static DayOfWeek Yesterday(this DayOfWeek today) {
            return today.Previous();
        }
    }
}