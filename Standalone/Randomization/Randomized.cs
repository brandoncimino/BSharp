using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Standalone.Randomization {
    public class Randomized<T> : IRandomized<T> {
        public Func<Random, T> Randomizer { get; protected set; }
        public Random          Generator  { get; }
        public T               Value      => Randomizer.Invoke(Generator);

        protected Randomized() { }

        public Randomized(Func<Random, T> randomizer, Random? generator = default) {
            Generator  = generator ?? Brandom.Gen;
            Randomizer = randomizer;
        }

        public static Randomized<T> FromList(ICollection<T> choices) {
            return new Randomized<T>(choices.Random);
        }

        public T Get() => Value;
    }

    public static class Randomized {
        public static FromWeightedList<T> FromWeightedList<T>(ICollection<T> choices, Func<T, double> weightSelector) {
            return new FromWeightedList<T>(choices, weightSelector);
        }

        public static FromWeightedList<T> FromWeightedList<T>(IDictionary<T, double> weightedChoices) {
            return new FromWeightedList<T>(weightedChoices);
        }

        public static FromWeightedList<T> FromWeightedList<T>(IEnumerable<(T, double)> weightedChoices) {
            return new FromWeightedList<T>(weightedChoices);
        }

        public static FromWeightedList<T> FromWeightedList<T>(IEnumerable<(T, int)> weightedChoices) {
            return new FromWeightedList<T>(weightedChoices);
        }
    }
}