using System.Numerics;

using FowlFever.BSharp.Memory;

namespace BSharp.Memory.Tests;

public class VectorMath_ContainsTests {
    private static bool ReallyContainsAny<T>(Vector<T> vector, params T[] values) where T : unmanaged {
        for (int i = 0; i < Vector<T>.Count; i++) {
            if (values.Contains(vector[i])) {
                return true;
            }
        }

        return false;
    }

    private static readonly int[]       SoughtAfterValues = { 0, 10, 99, -1, 2, -64 };
    private static readonly Vector<int> SearchingInVector = new(Enumerable.Range(-5, Vector<int>.Count).ToArray());

    [Test]
    public void VectorMath_Contains_OneValue([ValueSource(nameof(SoughtAfterValues))] int a) {
        var expected = ReallyContainsAny(SearchingInVector, a);
        Assert.That(VectorMath.Contains(SearchingInVector, a), Is.EqualTo(expected));
    }

    [Test]
    public void VectorMath_ContainsAny_TwoValues([ValueSource(nameof(SoughtAfterValues))] int a, [ValueSource(nameof(SoughtAfterValues))] int b) {
        var expected = ReallyContainsAny(SearchingInVector, a, b);
        Assert.That(VectorMath.ContainsAny(SearchingInVector, a, b), Is.EqualTo(expected));
    }

    [Test]
    [Pairwise]
    public void VectorMath_ContainsAny_ThreeValues([ValueSource(nameof(SoughtAfterValues))] int a, [ValueSource(nameof(SoughtAfterValues))] int b, [ValueSource(nameof(SoughtAfterValues))] int c) {
        var expected = ReallyContainsAny(SearchingInVector, a, b, c);
        Assert.That(VectorMath.ContainsAny(SearchingInVector, a, b, c), Is.EqualTo(expected));
    }
}