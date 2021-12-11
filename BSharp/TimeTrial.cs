using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FowlFever.BSharp.Chronic;

namespace FowlFever.BSharp {
    /// <summary>
    /// A convenient way to record multi-lap <see cref="System.Diagnostics.Stopwatch"/>es
    /// </summary>
    public class TimeTrial {
        private readonly Lazy<Stopwatch> _stopwatch = new Lazy<Stopwatch>(Stopwatch.StartNew);
        private          Stopwatch       Stopwatch => _stopwatch.Value;
        public           TimeSpan        Total     => Laps.Sum();
        public readonly  List<TimeSpan>  Laps = new List<TimeSpan>();
        public           TimeSpan        Average  => TimeSpan.FromTicks(Total.Ticks / Laps.Count);
        public           TimeSpan        Shortest => Laps.Min();
        public           TimeSpan        Longest  => Laps.Max();

        public void Start() {
            Stopwatch.Restart();
        }

        public void Stop() {
            Stopwatch.Stop();
            Laps.Add(Stopwatch.Elapsed);
        }

        public void StartLap() {
            Stop();
            Start();
        }

        public void Lap(Action action) {
            StartLap();
            action.Invoke();
            Stop();
        }

        public static TimeTrial Attack(Action action, int numberOfLaps) {
            var timeTrial = new TimeTrial();
            for (int lap = 0; lap < numberOfLaps; lap++) {
                timeTrial.Lap(action);
            }

            return timeTrial;
        }
    }
}