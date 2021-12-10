using System;
using System.Diagnostics;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A variation on <see cref="IFailable"/> that also includes timing information, e.g. <see cref="StartedAt"/> and <see cref="Duration"/>.
    /// </summary>
    public readonly struct Timeable : IFailable {
        private readonly Failable  Execution;
        public           Exception Excuse    => Execution.Excuse;
        public           bool      Failed    => Execution.Failed;
        public           DateTime  StartedAt { get; }
        public           DateTime  EndedAt   => StartedAt + Duration;
        public           TimeSpan  Duration  { get; }

        public Timeable(Action action) {
            StartedAt = DateTime.UtcNow;
            var sw = Stopwatch.StartNew();
            Execution = action.Try();
            sw.Stop();
            Duration = sw.Elapsed;
        }
    }
}