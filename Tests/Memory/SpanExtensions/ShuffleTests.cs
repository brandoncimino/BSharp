using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public class ShuffleTests {
    private static void AssertAllAreUnique<T>(Span<T> stuff) {
        var uniques = new HashSet<T>();
        foreach (var it in stuff) {
            Assert.That(uniques, Does.Not.Contain(it));
            uniques.Add(it);
        }
    }

    [Test]
    [TestCase(10)]
    [TestCase(100)]
    public void Shuffle_DoesNotProduceDuplicates(int range) {
        var rng   = new Random(range);
        var stuff = Enumerable.Range(0, range).ToArray().AsSpan();
        stuff.Shuffle(rng);
        AssertAllAreUnique(stuff);
    }

    [Test]
    [TestCase(10)]
    [TestCase(100)]
    public void Shuffle_UsesAllValues(int range) {
        var rng    = new Random(range);
        var og     = Enumerable.Range(0, range).ToImmutableArray();
        var copied = og.ToArray().AsSpan();
        copied.Shuffle(rng);
        copied.Sort();
        Assert.That(copied.SequenceEqual(og.AsSpan()));
    }
}