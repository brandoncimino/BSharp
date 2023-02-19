using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using FowlFever.BSharp.Memory;
using FowlFever.Testing.Extensions;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public class FastMathTests {
    public static readonly int[] IntSpanSizes = {
        0,
        Vector<int>.Count / 2,
        Vector<int>.Count,
        Vector<int>.Count + Vector<int>.Count / 2,
        Vector<int>.Count * 2,
        Vector<int>.Count * 2 + Vector<int>.Count / 2
    };

    public static readonly int[] AddAmounts = { 0, 1, 10, -3 };

    private static IEnumerable<T> ZipWithExtras<T>(IReadOnlyList<T> a, IReadOnlyList<T> b, Func<T, T, T> combiner) {
        var results = new T[a.Count];
        for (int i = 0; i < a.Count; i++) {
            if (b.Count > i) {
                results[i] = combiner(a[i], b[i]);
            }
            else {
                results[i] = a[i];
            }
        }

        return results;
    }

    #region All

    [Test]
    public void FastAddAll([ValueSource(nameof(IntSpanSizes))] int numbers, [ValueSource(nameof(AddAmounts))] int amount) {
        var ints     = TestData.RandomInts(numbers).ToArray();
        var expected = ints.Select(it => it + amount).ToArray();
        var span     = ints.AsSpan();
        span.FastAddAll(amount);
        span.SequenceEqual(expected).AssertTrue();
    }

    [Test]
    public void FastSubtractAll([ValueSource(nameof(IntSpanSizes))] int numbers, [ValueSource(nameof(AddAmounts))] int amount) {
        var ints     = TestData.RandomInts(numbers).ToArray();
        var expected = ints.Select(it => it - amount).ToArray();
        var span     = ints.AsSpan();
        span.FastSubtractAll(amount);
        span.SequenceEqual(expected).AssertTrue();
    }

    [Test]
    public void FastMultiplyAll([ValueSource(nameof(IntSpanSizes))] int numbers, [ValueSource(nameof(AddAmounts))] int amount) {
        var ints     = TestData.RandomInts(numbers).ToArray();
        var expected = ints.Select(it => it * amount).ToArray();
        var span     = ints.AsSpan();
        span.FastMultiplyAll(amount);
        span.SequenceEqual(expected).AssertTrue();
    }

    [Test]
    public void FastDivideAll([ValueSource(nameof(IntSpanSizes))] int numbers, [ValueSource(nameof(AddAmounts))] int amount) {
        var ints = TestData.RandomInts(numbers, 20).ToArray();

        if (amount == 0 && numbers > 0) {
            Assert.Throws<DivideByZeroException>(
                () => { ints.AsSpan().FastDivideAll(amount); }
            );
            return;
        }

        var expected = ints.Select(it => it / amount).ToArray();
        var span     = ints.AsSpan();
        span.FastDivideAll(amount);

        Assert.That(span.ToArray(), Is.EquivalentTo(expected));
    }

    #endregion

    #region Each

    [Test]
    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5, 6 }
    )]
    [TestCase(
        new[] { 1, 2 },
        new[] { 3, 4, 5 }
    )]
    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5 }
    )]
    public void FastAddEach(int[] a, int[] b) {
        var expected = ZipWithExtras(a, b, (x, y) => x + y);
        a.AsSpan().FastAddEach(b);
        Assert.That(a, Is.EquivalentTo(expected));
    }

    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5, 6 }
    )]
    [TestCase(
        new[] { 1, 2 },
        new[] { 3, 4, 5 }
    )]
    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5 }
    )]
    public void FastSubtractEach(int[] a, int[] b) {
        var expected = ZipWithExtras(a, b, (x, y) => x - y);
        a.AsSpan().FastSubtractEach(b);
        Assert.That(a, Is.EquivalentTo(expected));
    }

    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5, 6 }
    )]
    [TestCase(
        new[] { 1, 2 },
        new[] { 3, 4, 5 }
    )]
    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5 }
    )]
    public void FastMultiplyEach(int[] a, int[] b) {
        var expected = ZipWithExtras(a, b, (x, y) => x * y);
        a.AsSpan().FastMultiplyEach(b);
        Assert.That(a, Is.EquivalentTo(expected));
    }

    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5, 6 }
    )]
    [TestCase(
        new[] { 1, 2 },
        new[] { 3, 4, 5 }
    )]
    [TestCase(
        new[] { 1, 2, 3 },
        new[] { 4, 5 }
    )]
    public void FastDivideEach(int[] a, int[] b) {
        var expected = ZipWithExtras(a, b, (x, y) => x / y);
        a.AsSpan().FastDivideEach(b);
        Assert.That(a, Is.EquivalentTo(expected));
    }

    #endregion
}