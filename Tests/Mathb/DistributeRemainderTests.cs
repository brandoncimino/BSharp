using System.Linq;

using FowlFever.BSharp.Collections.Apportion;

using NUnit.Framework;

namespace BSharp.Tests.Mathb;

public class DistributeRemainderTests {
    [TestCase(1, Apportion.DistributionStrategy.FromLeft,       new[] { 1, 0, 0, 0, 0 })]
    [TestCase(2, Apportion.DistributionStrategy.FromLeft,       new[] { 1, 1, 0, 0, 0 })]
    [TestCase(3, Apportion.DistributionStrategy.FromLeft,       new[] { 1, 1, 1, 0, 0 })]
    [TestCase(4, Apportion.DistributionStrategy.FromLeft,       new[] { 1, 1, 1, 1, 0 })]
    [TestCase(1, Apportion.DistributionStrategy.FromLeftSpaced, new[] { 1, 0, 0, 0, 0, 0 })]
    [TestCase(2, Apportion.DistributionStrategy.FromLeftSpaced, new[] { 1, 0, 1, 0, 0, 0 })]
    [TestCase(3, Apportion.DistributionStrategy.FromLeftSpaced, new[] { 1, 0, 1, 0, 1, 0 })]
    [TestCase(4, Apportion.DistributionStrategy.FromLeftSpaced, new[] { 1, 0, 1, 0, 1, 1 })]
    [TestCase(5, Apportion.DistributionStrategy.FromLeftSpaced, new[] { 1, 0, 1, 1, 1, 1 })]
    [TestCase(3, Apportion.DistributionStrategy.FromRight,      new[] { 0, 0, 0, 1, 1, 1 })]
    [TestCase(3, Apportion.DistributionStrategy.FromCenter,     new[] { 0, 1, 1, 1, 0, 0 })]
    [TestCase(3, Apportion.DistributionStrategy.FromOutside,    new[] { 1, 1, 0, 0, 0, 1 })]
    public void DistributeTest(
        int                            remainder,
        Apportion.DistributionStrategy strategy,
        int[]                          expected
    ) {
        var portions = new int[expected.Length];
        Apportion.DistributeRemainder(portions, remainder, strategy);
        Assert.That(portions, Is.EquivalentTo(expected));
    }

    [TestCase(1, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 })]
    [TestCase(2, new[] { 1, 0, 1, 0, 0, 0, 0, 0, 0 })]
    [TestCase(3, new[] { 1, 0, 1, 0, 1, 0, 0, 0, 0 })]
    [TestCase(4, new[] { 1, 0, 1, 0, 1, 0, 1, 0, 0 })]
    [TestCase(5, new[] { 1, 0, 1, 0, 1, 0, 1, 0, 1 })]
    [TestCase(6, new[] { 1, 0, 1, 0, 1, 0, 1, 1, 1 })]
    [TestCase(7, new[] { 1, 0, 1, 0, 1, 1, 1, 1, 1 })]
    [TestCase(8, new[] { 1, 0, 1, 1, 1, 1, 1, 1, 1 })]
    public void MiddleweightTest(int remainder, int[] expected) {
        var actual = new int[expected.Length];
        Apportion.DistributeRemainder_FromLeftSpaced(actual, remainder);
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [TestCase(1, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 })]
    [TestCase(2, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 1 })]
    [TestCase(3, new[] { 1, 0, 1, 0, 0, 0, 0, 0, 1 })]
    [TestCase(4, new[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })]
    [TestCase(5, new[] { 1, 0, 1, 0, 1, 0, 1, 0, 1 })]
    [TestCase(6, new[] { 1, 1, 1, 0, 1, 0, 1, 0, 1 })]
    [TestCase(7, new[] { 1, 1, 1, 0, 1, 0, 1, 1, 1 })]
    [TestCase(8, new[] { 1, 1, 1, 1, 1, 0, 1, 1, 1 })]
    public void DistributeSpacedOutside(int remainder, int[] expected) {
        var actual = new int[expected.Length];
        Apportion.DistributeRemainder_FromOutsideSpaced(actual, remainder);
        Assert.That(actual.Sum(), Is.EqualTo(remainder));
        Assert.That(actual,       Is.EquivalentTo(expected));
    }

    [TestCase(1, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 })]
    [TestCase(2, new[] { 1, 1, 0, 0, 0, 0, 0, 0, 0 })]
    [TestCase(3, new[] { 1, 1, 1, 0, 0, 0, 0, 0, 0 })]
    [TestCase(4, new[] { 1, 1, 1, 1, 0, 0, 0, 0, 0 })]
    [TestCase(5, new[] { 1, 1, 1, 1, 1, 0, 0, 0, 0 })]
    [TestCase(6, new[] { 1, 1, 1, 1, 1, 1, 0, 0, 0 })]
    [TestCase(7, new[] { 1, 1, 1, 1, 1, 1, 1, 0, 0 })]
    [TestCase(8, new[] { 1, 1, 1, 1, 1, 1, 1, 1, 0 })]
    [TestCase(2, new[] { 1, 0, 1, 0, 0 })]
    public void DistributeRemainder_SpacedSimple(int remainder, int[] expected) {
        // left
        var actual = new int[expected.Length];
        Apportion.DistributeRemainder_SpacedSimple_Left(actual, remainder);
        Assert.That(actual, Is.EquivalentTo(expected));

        // right
        var actualRight = new int[expected.Length];
        Apportion.DistributeRemainder_SpacedSimple_Right(actualRight, remainder);
        Assert.That(actualRight, Is.EquivalentTo(expected.Reverse().ToArray()));
    }
}