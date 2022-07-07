using System;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Collections.Apportion;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Mathb;

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

    [TestCase(5, 3, new[] { 2, 2, 1 })]
    [TestCase(6, 4, new[] { 2, 2, 1, 1 })]
    public void EvenApportion(int amount, int portions, int[] expectedPortions) {
        var apportion = Apportion.Evenly(amount, portions);
        Assert.That(apportion.ToArray(), Is.EquivalentTo(expectedPortions));
    }

    [Test]
    public static void RangeApportion([ValueSource(nameof(RangeApportionExpectations))] RangeApportionExpectation expectation) {
        var apportion = new RangeApportion(expectation.Amount, expectation.Weights);
        Asserter.Against(apportion)
                .And(Is.EquivalentTo(expectation.ExpectedRanges))
                .Invoke();
    }
}