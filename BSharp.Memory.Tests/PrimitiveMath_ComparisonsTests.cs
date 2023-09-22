using System.Collections.Immutable;

using FowlFever.BSharp.Memory;

namespace BSharp.Memory.Tests;

public class PrimitiveMath_ComparisonsTests {
    private static bool GetExpectedResult<T>(T left, T right, PrimitiveMath.ComparisonOperator comparisonOperator) where T : IComparable<T> {
        var comparison = Comparer<T>.Default.Compare(left, right);
        return comparisonOperator switch {
            PrimitiveMath.ComparisonOperator.GreaterThan          => comparison > 0,
            PrimitiveMath.ComparisonOperator.LessThan             => comparison < 0,
            PrimitiveMath.ComparisonOperator.EqualTo              => comparison == 0,
            PrimitiveMath.ComparisonOperator.GreaterThanOrEqualTo => comparison >= 0,
            PrimitiveMath.ComparisonOperator.LessThanOrEqualTo    => comparison <= 0,
            PrimitiveMath.ComparisonOperator.NotEqualTo           => comparison != 0,
            _                                                     => throw new ArgumentOutOfRangeException(nameof(comparisonOperator), comparisonOperator, null)
        };
    }

    public static readonly ImmutableArray<int> Ints = ImmutableArray.Create(1, -1, 0, int.MaxValue, int.MinValue, 99, -5_000);

    [Test]
    public void ComparisonOperator_Apply_Ints([ValueSource(nameof(Ints))] int left, [ValueSource(nameof(Ints))] int right, [Values] PrimitiveMath.ComparisonOperator comparisonOperator) {
        Console.WriteLine($"{left}.CompareTo({right}): {left.CompareTo(right)}");
        var expected = GetExpectedResult(left, right, comparisonOperator);
        var actual   = comparisonOperator.Apply(left, right);
        Assert.That(actual, Is.EqualTo(expected), $"{left} {comparisonOperator} {right}");
    }
}