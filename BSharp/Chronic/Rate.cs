using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Chronic {
    /// <summary>
    /// Represents an arbitrary quantity over time.
    /// </summary>
    /// <remarks>
    /// The primary purpose of this is to avoid the ambiguity between manipulating a value as "X (per second)" vs. "X seconds per each".
    /// </remarks>
    [PublicAPI]
    [Serializable]
    public struct Rate : IComparable<Rate> {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The amount per 1 <see cref="TimeUnit.Seconds"/>.
        /// </summary>
        [JsonProperty]
        public double Hertz { get; set; }

        /// <summary>
        /// The <see cref="TimeSpan"/> of 1 "event" (e.g. 2 <see cref="Hertz"/> = 0.5 <see cref="TimeUnit.Seconds"/>)
        /// </summary>
        [JsonIgnore]
        public TimeSpan Interval {
            get => HertzToInterval(Hertz);
            set => Hertz = IntervalToHertz(value);
        }

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Rate"/> instance with the specified <see cref="Hertz"/> value.
        /// </summary>
        /// <param name="hertz">the quantity per <see cref="TimeUnit.Seconds"/></param>
        public Rate(double hertz) {
            Hertz = hertz;
        }

        /// <summary>
        /// Creates a new <see cref="Rate"/> instance with the specified <see cref="Interval"/> value.
        /// </summary>
        /// <param name="interval">the elapsed time to complete a single "event"</param>
        public Rate(TimeSpan interval) : this() {
            Interval = interval;
        }

        /// <summary>
        /// Creates a new <see cref="Rate"/> instance from an arbitrary <paramref name="amount"/> over a <paramref name="duration"/>
        /// </summary>
        /// <param name="amount">the number of "events" that occured during the <paramref name="duration"/></param>
        /// <param name="duration">the elapsed <see cref="TimeSpan"/> during which all of the "events" occurred</param>
        public Rate(double amount, TimeSpan duration) : this() {
            Interval = duration.Divide(amount);
        }

        /// <summary>
        /// Creates a new <see cref="Rate"/> instance of an arbitrary <paramref name="amount"/> per <see cref="TimeUnit"/>.
        /// </summary>
        /// <param name="amount">the number of "events" that occur <paramref name="perTimeUnit"/></param>
        /// <param name="perTimeUnit">the <see cref="TimeUnit"/> of the rate</param>
        public Rate(double amount, TimeUnit perTimeUnit) : this(amount, perTimeUnit.SpanOf()) { }

        #endregion

        public double Per(TimeSpan timeSpan) {
            return timeSpan.Divide(Interval);
        }

        public double Per(TimeUnit timeUnit) {
            return Per(timeUnit.SpanOf());
        }

        private static double IntervalToHertz(TimeSpan interval) {
            return interval == TimeSpan.Zero ? double.PositiveInfinity : OneSecond.Divide(interval);
        }

        private static TimeSpan HertzToInterval(double hertz) {
            return hertz == 0 ? default : OneSecond.Divide(hertz);
        }

        public int CompareTo(Rate other) {
            return Hertz.CompareTo(other.Hertz);
        }

        #region Operators

        #region Arithmetic

        public static Rate operator +(Rate  a, Rate b) => new Rate(a.Hertz + b.Hertz);
        public static Rate operator -(Rate  a, Rate b) => new Rate(a.Hertz - b.Hertz);
        public static Rate operator *(Rate  a, Rate b) => new Rate(a.Hertz * b.Hertz);
        public static Rate operator /(Rate  a, Rate b) => new Rate(a.Hertz / b.Hertz);
        public static bool operator ==(Rate a, Rate b) => a.Equals(b);
        public static bool operator !=(Rate a, Rate b) => !(a == b);
        public static bool operator >=(Rate a, Rate b) => a.Hertz >= b.Hertz;
        public static bool operator <=(Rate a, Rate b) => a.Hertz <= b.Hertz;
        public static bool operator >(Rate  a, Rate b) => a.Hertz > b.Hertz;
        public static bool operator <(Rate  a, Rate b) => a.Hertz < b.Hertz;

        #endregion

        #region Equality

        public bool Equals(Rate other) {
            return Hertz.Equals(other.Hertz);
        }

        public override bool Equals(object obj) {
            return obj is Rate other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() {
            return Hertz.GetHashCode();
        }

        #endregion

        #endregion
    }
}