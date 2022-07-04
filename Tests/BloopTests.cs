using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests;

/// <summary>
/// Tests for <see cref="Bloop"/>s
/// </summary>
[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
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

        public RangeTestParameters(
            float       min_f,
            float       max_f,
            int         stepCount,
            List<float> expected_f
        ) {
            Min_F      = min_f;
            Max_F      = max_f;
            StepCount  = stepCount;
            Expected_F = expected_f;
        }
    }

    [Test]
    [TestCase(0, 6, 3, 0, 2,    4)]
    [TestCase(1, 2, 5, 1, 1.2f, 1.4f, 1.6f, 1.8f)]
    public void StepExclusive(
        float          min,
        float          max,
        int            stepCount,
        params float[] expectedResults
    ) {
        var steps = Bloop.StepExclusive(min, max, stepCount);
        Asserter.Against(steps)
                .And(steps, Is.EqualTo(expectedResults))
                .And(steps, Is.EquivalentTo(expectedResults))
                .Invoke();
    }

    #endregion

    #region Wrapping

    public record WrapExpectation(int SourceCount, int IterationCount, int[] ExpectedResults) {
        public Bloop.RepetitionHandling RepetitionHandling;
        public int ExpectedInvocations => RepetitionHandling switch {
            Bloop.RepetitionHandling.CacheResult => Math.Min(SourceCount, IterationCount),
            Bloop.RepetitionHandling.ReEvaluate  => IterationCount,
            _                                    => throw BEnum.UnhandledSwitch(RepetitionHandling)
        };
    }

    public static WrapExpectation[] WrapExpectations = {
        new(5, 3, new[] { 0, 1, 2 }),
        new(3, 5, new[] { 0, 1, 2, 0, 1 }),
        new(1, 5, new[] { 0, 0, 0, 0, 0 }),
        new(2, 5, new[] { 0, 1, 0, 1, 0 }),
    };

    [Test]
    public void WrapAround_Cached([ValueSource(nameof(WrapExpectations))] WrapExpectation expectation) {
        _WrapAround(expectation, Bloop.RepetitionHandling.CacheResult);
    }

    [Test]
    public void WrapAround_ReEvaluate([ValueSource(nameof(WrapExpectations))] WrapExpectation expectation) {
        _WrapAround(expectation, Bloop.RepetitionHandling.ReEvaluate);
    }

    private void _WrapAround(WrapExpectation expectation, Bloop.RepetitionHandling repetitionHandling) {
        expectation = expectation with { RepetitionHandling = repetitionHandling };
        var list = Enumerable.Range(0, expectation.SourceCount)
                             .WithCounter(out var counter)
                             .AssertCounter(counter, 0)
                             .WrapAround(expectation.IterationCount, expectation.RepetitionHandling)
                             .AssertCounter(counter, 0)
                             .ToList()
                             .AssertCounter(counter, expectation.ExpectedInvocations);

        Assert.That(list,    Has.Exactly(expectation.IterationCount).Items);
        Assert.That(counter, Has.Exactly(expectation.ExpectedInvocations).Items);
        Assert.That(list,    Is.EqualTo(expectation.ExpectedResults));
    }

    #endregion
}