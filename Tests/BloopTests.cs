using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests; 

/// <summary>
/// Tests for <see cref="Bloop"/>s
/// </summary>
public class BloopTests {
    [Test]
    [TestCase(5)]
    public void RepeatRandom(int numberOfPicks) {
        var random     = new Random();
        var picks      = numberOfPicks.Repeat(() => random.Next());
        var pickGroups = picks.Group();
        Assert.That(pickGroups, Has.Count.EqualTo(numberOfPicks));
    }

    #region Stepping Through a Range

    class RangeTestParameters {
        public float  Min_F { get; }
        public float  Max_F { get; }
        public double Min_D => Min_F;
        public double Max_D => Max_F;

        public int StepCount;

        public List<float>  Expected_F { get; }
        public List<double> Expected_D => Expected_F.Select(it => (double)it).ToList();

        public RangeTestParameters(float min_f, float max_f, int stepCount, List<float> expected_f) {
            Min_F      = min_f;
            Max_F      = max_f;
            StepCount  = stepCount;
            Expected_F = expected_f;
        }
    }

    [Test]
    [TestCase(0, 6, 3, 0, 2,    4)]
    [TestCase(1, 2, 5, 1, 1.2f, 1.4f, 1.6f, 1.8f)]
    public void StepExclusive(float min, float max, int stepCount, params float[] expectedResults) {
        var steps = Bloop.StepExclusive(min, max, stepCount);
        AssertAll.Of(
            () => Assert.That(steps, Is.EqualTo(expectedResults)),
            () => Assert.That(steps, Is.EquivalentTo(expectedResults))
        );
    }

    #endregion
}