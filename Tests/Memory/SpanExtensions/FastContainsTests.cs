using System;
using System.Linq;
using System.Numerics;

using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public class FastContainsTests {
    public static int[] GetVectorSizes<T>() where T : unmanaged {
        return new[] {
                0,
                .5,
                1,
                1.5,
                2,
                2.5
            }.Select(it => it * Vector<T>.Count)
             .Select(it => (int)it)
             .Append(1)
             .ToArray();
    }

    public static T Multiple<T>(int factor) where T : INumber<T> {
        var sum = T.One;

        for (int i = 0; i < factor; i++) {
            sum += T.One;
        }

        return sum;
    }

    public static int[] IntSizes  => GetVectorSizes<int>();
    public static int[] LongSizes => GetVectorSizes<long>();
    public static int[] ByteSizes => GetVectorSizes<byte>();

    private static void _TestFastContains<T>(int size) where T : unmanaged, INumber<T> {
        var values = Enumerable.Range(0, size)
                               .Select(it => Multiple<T>(it % 3))
                               .ToArray();

        var toFind = -T.One;

        if (size > 0) {
            var toFindIndex = Random.Shared.Next(values.Length);
            values[toFindIndex] = toFind;
        }

        var actual   = values.AsSpan().FastContains(toFind);
        var expected = values.Contains(toFind);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void FastContains_Int([ValueSource(nameof(IntSizes))] int size) {
        _TestFastContains<int>(size);
    }

    [Test]
    public void FastContains_Long([ValueSource(nameof(LongSizes))] int size) {
        _TestFastContains<long>(size);
    }

    [Test]
    public void FastContains_Byte([ValueSource(nameof(ByteSizes))] int size) {
        _TestFastContains<byte>(size);
    }

    [Test]
    public void FastContains_RejectsNonPrimitive() {
        var halves = new Half[] { Half.One, Half.Pi, Half.Tau };
        Assert.That(() => halves.AsSpan().FastContains(Half.One), Throws.NotSupportedException);
    }
}