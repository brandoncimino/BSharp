using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Testing;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Randomization;

using JetBrains.Annotations;

using NUnit.Framework;

using Is = BrandonUtils.Testing.Is;

namespace BrandonUtils.Tests.Standalone {
    public class BrandomTests {
        [Test]
        [TestCase(7, 1000)]
        public void FromList(int numberOfChoices, int numberOfPicks) {
            var ls = Enumerable.Range(0, numberOfChoices).ToList();
            var v2 = new FromList<int>(ls);

            var picks = numberOfPicks.Repeat<int>(() => v2).ToList();

            Assert.That(picks, Is.SupersetOf(ls));

            var groups                = picks.Group();
            var expectedHitsPerChoice = (double)numberOfPicks / numberOfChoices;

            Assert.That(groups, Has.All.Values().Approximately(expectedHitsPerChoice, expectedHitsPerChoice / 2));
        }

        private static IEnumerable<(string choice, int weight)> BuildWeighted(params int[] weights) {
            return weights.Select((it, i) => (i.ToString(), it));
        }

        private static IDictionary<T, double> BuildWeightedPortion<T, T2>(
            [InstantHandle]
            IEnumerable<(T choice, T2 weight)> weighted
        ) {
            weighted = weighted.ToList();
            var totalWeight = weighted.Select(it => it.Item2)
                                      .Sum(it => it.ToDouble());
            return weighted.ToDictionary(it => it.choice, it => it.weight.ToDouble() / totalWeight);
        }

        [TestCase(1000, new int[] { 1, 1, 1, 5, 2, 3, 7, 8 })]
        public void FromWeightedList(int numberOfPicks, params int[] weights) {
            var weighted = BuildWeighted(weights);

            var picks = numberOfPicks.Repeat(it => Brandom.Gen.FromWeightedList(weighted)).ToList();

            AssertPicks(picks, weighted);
        }

        private static void AssertPicks(
            IReadOnlyCollection<string?>? picks,
            [InstantHandle]
            IEnumerable<(string choice, int weight)> weighted
        ) {
            weighted = weighted?.ToArray();
            var choices = weighted.Select(it => it.choice);
            Assert.That(picks?.Distinct(), Is.EquivalentTo(choices.Distinct()), "All of the choices were picked at least once, and all of the picks were choices");

            var groups = picks.Group();
            var expectedHits = BuildWeightedPortion(weighted)
                .ToDictionary(
                    it => it.Key,
                    it => picks?.Count * it.Value
                );

            foreach (var g in groups) {
                Assert.That(g, Has.Property(nameof(KeyValuePair<int, int>.Value)).Approximately(expectedHits[g.Key], picks.Count / 10f));
            }
        }

        [Test]
        [TestCase(1000, new[] { 1, 2, 3, 4, 15, 3 })]
        public void RandomizedFromWeightedList(int numberOfPicks, params int[] weights) {
            var weighted = BuildWeighted(weights).ToArray();

            var fromWeightedList = Randomized.FromWeightedList(weighted);

            var picks = numberOfPicks.Repeat(fromWeightedList.Get).ToList();

            AssertPicks(picks, weighted);
        }
    }
}