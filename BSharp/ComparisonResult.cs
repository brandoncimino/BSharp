using System;
using System.ComponentModel;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// A nice representation of a <see cref="IComparable{T}.CompareTo"/> operation.
/// </summary>
public enum ComparisonResult {
    LessThan = -1, EqualTo = 0, GreaterThan = 1,
}

public static class ComparisonResultExtensions {
    /// <param name="result">this <see cref="ComparisonResult"/></param>
    /// <param name="op">the <see cref="ComparisonOperator"/></param>
    /// <returns><c>true</c> if this <see cref="ComparisonResult"/> satisfies the given <see cref="ComparisonOperator"/></returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown (<see cref="ComparisonResult"/>, <see cref="ComparisonOperator"/>) combination is provided</exception>
    public static bool Satisfies(this ComparisonResult result, ComparisonOperator op) => (result, op) switch {
        (ComparisonResult.EqualTo, _)                                                                             => op.Clusivity() == Clusivity.Inclusive,
        (_, ComparisonOperator.NotEqualTo)                                                                        => true,
        (ComparisonResult.GreaterThan, ComparisonOperator.GreaterThan or ComparisonOperator.GreaterThanOrEqualTo) => true,
        (ComparisonResult.LessThan, ComparisonOperator.LessThan or ComparisonOperator.LessThanOrEqualTo)          => true,
        _                                                                                                         => throw BEnum.UnhandledSwitch((result, op))
    };
}