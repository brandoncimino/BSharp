using System;
using System.Collections.Generic;

namespace BrandonUtils.Standalone.Randomization {
    public class FromWeightedList<T> : Randomized<T> {
        public FromWeightedList(ICollection<T> choices, Func<T, double> weightExtractor, Random? generator = default) : base(gen => Brandom.FromWeightedList(gen, choices, weightExtractor), generator) { }

        public FromWeightedList(IDictionary<T, double> weightedChoices, Random? generator = default) : base(gen => Brandom.FromWeightedList(gen, weightedChoices), generator) { }
        public FromWeightedList(IDictionary<T, int>    weightedChoices, Random? generator = default) : base(gen => Brandom.FromWeightedList(gen, weightedChoices), generator) { }

        public FromWeightedList(
            IEnumerable<(T, double)> weightedChoices,
            Random?                  generator = default
        ) : base(gen => gen.FromWeightedList(weightedChoices), generator) { }

        public FromWeightedList(
            IEnumerable<(T, int)> weightedChoices,
            Random?               generator = default
        ) : base(gen => gen.FromWeightedList(weightedChoices), generator) { }
    }
}