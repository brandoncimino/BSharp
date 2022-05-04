using System;
using System.Collections;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// Represents a <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/comparison-operators">comparison operator</a>.
/// </summary>
public enum ComparisonOperator {
    GreaterThan,
    LessThan,
    EqualTo,
    GreaterThanOrEqualTo,
    LessThanOrEqualTo,
    NotEqualTo,
}

public static class ComparisonOperatorExtensions {
    public static Func<object?, object?, bool> Predicate(this ComparisonOperator comparisonOperator) {
        return (a, b) => comparisonOperator.IntPredicate().Invoke(Comparer.Default.Compare(a, b));
    }

    public static Func<int, bool> IntPredicate(this ComparisonOperator comparisonOperator) {
        return comparisonOperator switch {
            ComparisonOperator.EqualTo              => i => i == 0,
            ComparisonOperator.GreaterThan          => i => i > 0,
            ComparisonOperator.LessThan             => i => i < 0,
            ComparisonOperator.GreaterThanOrEqualTo => i => i >= 0,
            ComparisonOperator.LessThanOrEqualTo    => i => i <= 0,
            ComparisonOperator.NotEqualTo           => i => i != 0,
            _                                       => throw BEnum.UnhandledSwitch(comparisonOperator),
        };
    }

    public static string Symbol(this ComparisonOperator comparisonOperator) {
        return comparisonOperator switch {
            ComparisonOperator.EqualTo              => "==",
            ComparisonOperator.GreaterThan          => ">",
            ComparisonOperator.LessThan             => "<",
            ComparisonOperator.GreaterThanOrEqualTo => ">=",
            ComparisonOperator.LessThanOrEqualTo    => "<=",
            ComparisonOperator.NotEqualTo           => "!=",
            _                                       => throw BEnum.UnhandledSwitch(comparisonOperator),
        };
    }

    public static Comparison<T> Comparing<T, T2>(Func<T, T2> transformation)
        where T2 : IComparable {
        return (a, b) => transformation(a).CompareTo(transformation(b));
    }
}