using System;

using BrandonUtils.Standalone.Exceptions;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Chronic {
    /// <summary>
    /// Represents the <see cref="TimeSpan"/> between two specific points in time.
    /// </summary>
    [PublicAPI]
    public readonly struct TimePeriod : IEquatable<TimePeriod>, IEquatable<TimeSpan> {
        [JsonProperty]
        public readonly DateTime StartTime;
        [JsonIgnore]
        public DateTime EndTime => StartTime + Duration;
        [JsonProperty]
        public readonly TimeSpan Duration;

        public TimePeriod(DateTime startTime, DateTime endTime) {
            Validate_StartIsBeforeEnd(startTime, endTime);
            StartTime = startTime;
            Duration  = endTime - startTime;
        }

        public TimePeriod(DateTime startTime, TimeSpan duration) {
            StartTime = startTime;
            Duration  = duration;
        }

        [Pure]
        public string ToString(Func<DateTime, string> formatter) {
            return $"{formatter.Invoke(StartTime)} → {formatter.Invoke(EndTime)}";
        }

        [Pure]
        public override string ToString() {
            return ToString(Convert.ToString);
        }

        #region Exceptions & Validations

        private static void Validate_StartIsBeforeEnd(DateTime startTime, DateTime endTime) {
            if (startTime > endTime) {
                throw new TimeParadoxException(
                    $"The {nameof(startTime)} must be BEFORE the {nameof(endTime)}!" +
                    $"\n\t{nameof(startTime)}: {startTime}"                          +
                    $"\n\t{nameof(endTime)}:   {endTime}"
                );
            }
        }

        #endregion

        #region Equality

        public        bool Equals(TimePeriod      other)           => StartTime.Equals(other.StartTime) && Duration.Equals(other.Duration);
        public static bool operator ==(TimePeriod a, TimePeriod b) => a.Equals(b);
        public static bool operator !=(TimePeriod a, TimePeriod b) => !(a == b);
        public        bool Equals(TimeSpan        other)           => Duration.Equals(other);
        public static bool operator ==(TimePeriod a, TimeSpan   b) => a.Equals(b);
        public static bool operator !=(TimePeriod a, TimeSpan   b) => !(a == b);
        public static bool operator ==(TimeSpan   a, TimePeriod b) => b.Equals(a);
        public static bool operator !=(TimeSpan   a, TimePeriod b) => !(a == b);

        public override bool Equals(object other) {
            return other switch {
                TimePeriod period => Equals(period),
                TimeSpan span     => Equals(span),
                _                 => false
            };
        }

        public override int GetHashCode() {
            unchecked {
                return (StartTime.GetHashCode() * 397) ^ Duration.GetHashCode();
            }
        }

        #endregion
    }
}