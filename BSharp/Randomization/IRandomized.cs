using System;

namespace FowlFever.BSharp.Randomization {
    public interface IRandomized<out T> {
        public Func<Random, T> Randomizer { get; }
        public Random          Generator  { get; }

        public T Value { get; }
    }
}