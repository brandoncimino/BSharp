using System;

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
    public static Func<T, T, bool> Predicate<T>(this ComparisonOperator op)
        where T : IComparable<T> {
        return op switch {
            ComparisonOperator.EqualTo              => static (a, b) => a.ComparedWith(b).Satisfies(ComparisonOperator.EqualTo),
            ComparisonOperator.NotEqualTo           => static (a, b) => a.ComparedWith(b).Satisfies(ComparisonOperator.NotEqualTo),
            ComparisonOperator.GreaterThan          => static (a, b) => a.ComparedWith(b).Satisfies(ComparisonOperator.GreaterThan),
            ComparisonOperator.GreaterThanOrEqualTo => static (a, b) => a.ComparedWith(b).Satisfies(ComparisonOperator.GreaterThanOrEqualTo),
            ComparisonOperator.LessThan             => static (a, b) => a.ComparedWith(b).Satisfies(ComparisonOperator.LessThan),
            ComparisonOperator.LessThanOrEqualTo    => static (a, b) => a.ComparedWith(b).Satisfies(ComparisonOperator.LessThanOrEqualTo),
            _                                       => throw BEnum.UnhandledSwitch(op),
        };
    }

    public static Func<ComparisonResult, bool> ResultPredicate(this ComparisonOperator op) =>
        op switch {
            ComparisonOperator.EqualTo              => static r => r.Satisfies(ComparisonOperator.EqualTo),
            ComparisonOperator.NotEqualTo           => static r => r.Satisfies(ComparisonOperator.NotEqualTo),
            ComparisonOperator.GreaterThan          => static r => r.Satisfies(ComparisonOperator.GreaterThan),
            ComparisonOperator.GreaterThanOrEqualTo => static r => r.Satisfies(ComparisonOperator.GreaterThanOrEqualTo),
            ComparisonOperator.LessThan             => static r => r.Satisfies(ComparisonOperator.LessThan),
            ComparisonOperator.LessThanOrEqualTo    => static r => r.Satisfies(ComparisonOperator.LessThanOrEqualTo),
            _                                       => throw BEnum.UnhandledSwitch(op)
        };

    public static Func<int, bool> IntPredicate(this ComparisonOperator op) {
        return op switch {
            ComparisonOperator.EqualTo              => static i => i == 0,
            ComparisonOperator.GreaterThan          => static i => i > 0,
            ComparisonOperator.LessThan             => static i => i < 0,
            ComparisonOperator.GreaterThanOrEqualTo => static i => i >= 0,
            ComparisonOperator.LessThanOrEqualTo    => static i => i <= 0,
            ComparisonOperator.NotEqualTo           => static i => i != 0,
            _                                       => throw BEnum.UnhandledSwitch(op),
        };
    }

    public static char Symbol(this ComparisonOperator op) => op switch {
        ComparisonOperator.EqualTo              => '=',
        ComparisonOperator.NotEqualTo           => '≠',
        ComparisonOperator.GreaterThan          => '>',
        ComparisonOperator.GreaterThanOrEqualTo => '≥',
        ComparisonOperator.LessThan             => '<',
        ComparisonOperator.LessThanOrEqualTo    => '≤',
        _                                       => throw BEnum.UnhandledSwitch(op)
    };

    public static Clusivity Clusivity(this ComparisonOperator op) => op switch {
        ComparisonOperator.EqualTo              => Enums.Clusivity.Inclusive,
        ComparisonOperator.NotEqualTo           => Enums.Clusivity.Exclusive,
        ComparisonOperator.GreaterThan          => Enums.Clusivity.Exclusive,
        ComparisonOperator.GreaterThanOrEqualTo => Enums.Clusivity.Inclusive,
        ComparisonOperator.LessThan             => Enums.Clusivity.Exclusive,
        ComparisonOperator.LessThanOrEqualTo    => Enums.Clusivity.Inclusive,
        _                                       => throw BEnum.UnhandledSwitch(op)
    };

    public static bool SatisfiedBy(this ComparisonOperator op, int              comparisonResult) => Comparisons.ToComparisonResult(comparisonResult).Satisfies(op);
    public static bool SatisfiedBy(this ComparisonOperator op, ComparisonResult result)           => result.Satisfies(op);

    public static bool SatisfiedBy<T>(this ComparisonOperator op, T a, T b)
        where T : IComparable<T> => op.SatisfiedBy(a.ComparedWith(b));
}