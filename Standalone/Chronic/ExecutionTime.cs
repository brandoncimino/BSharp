using System;
using System.Diagnostics;
using System.Globalization;

using BrandonUtils.Standalone.Optional;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Chronic {
    public class ExecutionTime : IComparable<ExecutionTime> {
        public string?  Nickname  { get; }
        public Failable Execution { get; }
        public TimeSpan Duration  { get; }

        public ExecutionTime(string? nickname, [InstantHandle] Action action) {
            Nickname = nickname;

            var sw = Stopwatch.StartNew();
            Execution = action.Try();
            sw.Stop();
            Duration = sw.Elapsed;
        }

        public ExecutionTime(Action action) : this(default, action) { }

        public ExecutionTime(string? nickname, Failable execution, TimeSpan duration) {
            Execution = execution;
            Duration  = duration;
        }

        public ExecutionTime(Failable execution, TimeSpan duration) : this(default, execution, duration) { }

        public int CompareTo(ExecutionTime? value) {
            return Duration.CompareTo(value?.Duration);
        }


        public override string ToString() {
            var df = new DateTimeFormatInfo();
            df.ShortTimePattern = @"s\.ffff";
            df.ShortDatePattern = @"\y\o\l\o";
            df.LongDatePattern  = @"\l\o\n\g";
            Console.WriteLine("df: " + df);
            Console.WriteLine($"with df: {Duration.ToString("g", df)}");
            var durStr = Duration.ToString(@"mm\:s\.fffffff\s");
            var exeStr = Execution.ToString();
            return $"⏱ {durStr} {exeStr}";
        }
    }
}