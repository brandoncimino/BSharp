using System;

namespace BrandonUtils.Standalone.Randomization {
    public interface IRandomized<out T> {
        public Func<Random, T> Randomizer { get; }
        public Random          Generator  { get; }

        public T Value { get; }
    }
}