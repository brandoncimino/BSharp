using System.Collections.Immutable;

using FowlFever.BSharp.Memory;

namespace BSharp.Memory.Tests;

public class Spanq_ComparisonsTests {
    private static int GetExpectedIndex<T>(IList<T> stuff, Func<T, bool> predicate) {
        for (int i = 0; i < stuff.Count; i++) {
            if (predicate(stuff[i])) {
                return i;
            }
        }

        return -1;
    }

    private static bool CheckComparison(int left, int right, PrimitiveMath.ComparisonOperator comparisonOperator) {
        return comparisonOperator switch {
            PrimitiveMath.ComparisonOperator.GreaterThan          => left > right,
            PrimitiveMath.ComparisonOperator.LessThan             => left < right,
            PrimitiveMath.ComparisonOperator.EqualTo              => left == right,
            PrimitiveMath.ComparisonOperator.GreaterThanOrEqualTo => left >= right,
            PrimitiveMath.ComparisonOperator.LessThanOrEqualTo    => left <= right,
            PrimitiveMath.ComparisonOperator.NotEqualTo           => left != right,
            _                                                     => throw new ArgumentOutOfRangeException(nameof(comparisonOperator), comparisonOperator, null)
        };
    }

    private static int GetExpectedIndex(IList<int> stuff, int endpoint, PrimitiveMath.ComparisonOperator comparisonOperator) {
        return GetExpectedIndex(stuff, i => CheckComparison(i, endpoint, comparisonOperator));
    }

    public static readonly ImmutableArray<int> ValueCounts = ImmutableArray.Create(0, 1, 4, 8, 15, 16, 17, 31, 32, 33, 100);

    [Test]
    public void FirstIndexSatisfyingComparison_Randomized([ValueSource(nameof(ValueCounts))] int numberOfItems, [Values] PrimitiveMath.ComparisonOperator comparisonOperator) {
        var rng      = new Random(99);
        var endpoint = rng.Next();
        var numbers = Enumerable.Range(0, numberOfItems)
                                .Select(it => rng.Next())
                                .ToImmutableArray();
        var expected = GetExpectedIndex(numbers, endpoint, comparisonOperator);
        var actual   = numbers.AsSpan().FirstIndexSatisfyingComparison(endpoint, comparisonOperator);
        Assert.That(
            actual,
            Is.EqualTo(expected),
            () => $"""
                   {
                       nameof(Spanq.FirstIndexSatisfyingComparison)
                   }:
                     First index of: [{
                         string.Join(", ", numbers)
                     }]
                     {
                         comparisonOperator
                     } {
                         endpoint
                     }
                   """
        );
    }

    [Test]
    public void FirstIndexInRange_Randomized(
        [ValueSource(nameof(ValueCounts))] int  numberOfItems,
        [Values]                           bool minInclusive,
        [Values]                           bool maxInclusive
    ) {
        var rng = new Random(99);
        var min = rng.Next();
        var max = rng.Next();
        var numbers = Enumerable.Range(0, numberOfItems)
                                .Select(it => rng.Next())
                                .ToImmutableArray();
        var expected = GetExpectedIndex(
            numbers,
            it => (minInclusive ? it    >= min : it > min)
                  && (maxInclusive ? it <= max : it < max)
        );

        var actual = numbers.AsSpan().FirstIndexInRange(min, minInclusive, max, maxInclusive);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void FirstIndexInRange_RejectsOutOfOrderBounds([Values] bool minInclusive, [Values] bool maxInclusive) {
        const int min   = 5;
        const int max   = -5;
        int[]     stuff = Array.Empty<int>();

        Assert.That(() => Spanq.FirstIndexInRange(stuff, min, minInclusive, max, maxInclusive), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}