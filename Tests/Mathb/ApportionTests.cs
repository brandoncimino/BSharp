using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Collections.Apportion;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Mathb;

[Obsolete]
public class ApportionTests {
    public readonly record struct RangeApportionExpectation(int Amount, double[] Weights, Range[] ExpectedRanges) {
        public int[] ExpectedSizes => ExpectedRanges.Select(it => (it.End.Value - it.Start.Value)).ToArray();

        #region Overrides of ValueType

        public override string ToString() {
            return $"(💰 {Amount}) (🏋️ {Weights.JoinString(", ", "[", "]")}) ❓ (🍕 {ExpectedRanges.JoinString(", ", "[", "]")} // {ExpectedSizes.JoinString(", ", "[", "]")})";
        }

        #endregion
    }

    public static RangeApportionExpectation[] RangeApportionExpectations = {
        new() {
            Amount         = 6,
            Weights        = new[] { 1d, 1 },
            ExpectedRanges = new[] { ..3, 3..6 },
        },
        new() {
            Amount         = 5,
            Weights        = new[] { 1d, 3 },
            ExpectedRanges = new[] { ..1, 1..5 },
        },
        new() {
            Amount         = 5,
            Weights        = new[] { 3d, 1 },
            ExpectedRanges = new[] { ..4, 4..5 },
        },
        new() {
            Amount         = 6,
            Weights        = new[] { 1d, 1, 1, 1 },
            ExpectedRanges = new[] { ..2, 2..3, 3..4, 4..6 },
        },
        new() {
            Amount         = 2,
            Weights        = new[] { 1d, 1, 1 },
            ExpectedRanges = new[] { ..1, 1..1, 1..2 },
        },
    };

    [Test]
    public void SimpleApportion([ValueSource(nameof(RangeApportionExpectations))] RangeApportionExpectation expectation) {
        var apportion = new SimpleApportion(expectation.Amount, expectation.Weights);
        Asserter.Against(apportion)
                .WithHeading(expectation.ToString())
                .And(it => it.Portions, Is.EquivalentTo(expectation.ExpectedSizes))
                .Invoke();
    }

    [TestCase(5, 3, Apportion.DistributionStrategy.FromLeft,          new[] { 2, 2, 1 })]
    [TestCase(5, 3, Apportion.DistributionStrategy.FromLeftSpaced,    new[] { 2, 1, 2 })]
    [TestCase(6, 4, Apportion.DistributionStrategy.FromLeft,          new[] { 2, 2, 1, 1 })]
    [TestCase(7, 4, Apportion.DistributionStrategy.FromLeft,          new[] { 2, 2, 2, 1 })]
    [TestCase(6, 4, Apportion.DistributionStrategy.FromLeftSpaced,    new[] { 2, 1, 2, 1 })]
    [TestCase(7, 4, Apportion.DistributionStrategy.FromLeftSpaced,    new[] { 2, 1, 2, 2 })]
    [TestCase(3, 5, Apportion.DistributionStrategy.FromLeft,          new[] { 1, 1, 1, 0, 0 })]
    [TestCase(3, 5, Apportion.DistributionStrategy.FromLeftSpaced,    new[] { 1, 0, 1, 0, 1 })]
    [TestCase(1, 7, Apportion.DistributionStrategy.FromOutsideSpaced, new[] { 1, 0, 0, 0, 0, 0, 0 })]
    [TestCase(2, 7, Apportion.DistributionStrategy.FromOutsideSpaced, new[] { 1, 0, 0, 0, 0, 0, 1 })]
    [TestCase(3, 7, Apportion.DistributionStrategy.FromOutsideSpaced, new[] { 1, 0, 1, 0, 0, 0, 1 })]
    [TestCase(4, 7, Apportion.DistributionStrategy.FromOutsideSpaced, new[] { 1, 0, 1, 0, 1, 0, 1 })]
    [TestCase(5, 7, Apportion.DistributionStrategy.FromOutsideSpaced, new[] { 1, 1, 1, 0, 1, 0, 1 })]
    [TestCase(6, 7, Apportion.DistributionStrategy.FromOutsideSpaced, new[] { 1, 1, 1, 0, 1, 1, 1 })]
    public void EvenApportion(int amount, int portions, Apportion.DistributionStrategy strategy, int[] expectedPortions) {
        var apportion = Apportion.Evenly(amount, portions, strategy).AsArray();
        Assert.Multiple(
            () => {
                Assert.That(apportion.Sum(), Is.EqualTo(amount));
                Assert.That(apportion,       Is.EquivalentTo(expectedPortions));
            }
        );
    }

    [Test]
    public static void RangeApportion([ValueSource(nameof(RangeApportionExpectations))] RangeApportionExpectation expectation) {
        var apportion = new RangeApportion(expectation.Amount, expectation.Weights);
        Asserter.Against(apportion)
                .And(Is.EquivalentTo(expectation.ExpectedRanges))
                .Invoke();
    }

    private static ImmutableArray<double> GetNormalizedWeights(IEnumerable<double> weights) {
        var results = weights.ToArray();
        var sum     = results.Sum();
        for (int i = 0; i < results.Length; i++) {
            results[i] /= sum;
        }

        return results.ToImmutableArray();
    }

    [TestCase(.5, .5, .5)]
    [TestCase(2,  8,  99)]
    [TestCase(0,  2,  7)]
    public void NormalizeWeights_InPlace(params double[] weights) {
        var expected = GetNormalizedWeights(weights);
        var actual   = weights.AsSpan();
        Apportion.NormalizeWeightsInPlace(actual);
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [TestCase(new[] { .5, .5 },     new[] { .5, .5 })]
    [TestCase(new[] { 1.5, 1, .5 }, new[] { .5, 1.0 / 3, .5 / 3 })]
    public void NormalizeWeights_InPlace_ManualExpectation(double[] weights, double[] normalized) {
        var actual = weights.AsSpan();
        Apportion.NormalizeWeightsInPlace(actual);
        Assert.That(actual.ToArray(), Is.EquivalentTo(normalized));
    }

    [Test]
    [TestCase(
        10,
        new[] { 2f, 2f },
        new[] { 5, 5 }
    )]
    public void DistributeInto(int amount, float[] weights, int[] expected) {
        Span<int> actual = stackalloc int[expected.Length];
        Apportion.Weighted(amount, weights, actual);
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }
}