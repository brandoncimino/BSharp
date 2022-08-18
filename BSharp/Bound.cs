using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// Represents the boundary of a <see cref="Brange{T}"/>.
/// </summary>
/// <param name="Endpoint">the value at the <see cref="Extremum"/></param>
/// <param name="Extremum">whether this <see cref="Bound{T}"/> is a <see cref="Enums.Extremum.Min"/> or <see cref="Enums.Extremum.Max"/></param>
/// <param name="Clusivity">whether the <see cref="Endpoint"/> is considered inside of the <see cref="Bound{T}"/> or not</param>
/// <typeparam name="T"></typeparam>
public readonly record struct Bound<T>(T Endpoint, Extremum Extremum, Clusivity Clusivity)
    where T : notnull, IComparable<T> {
    /// <summary>
    /// Describes how the <see cref="Endpoint"/> should compare to a <typeparamref name="T"/> value.
    /// </summary>
    public ComparisonOperator ComparisonOperator => Bound.GetComparisonOperator(Extremum, Clusivity);

    public Bound(T value, ComparisonOperator                       comparisonOperator) : this(value, Bound.GetAttributes(comparisonOperator)) { }
    public Bound(T value, (Extremum extremum, Clusivity clusivity) attributes) : this(value, attributes.extremum, attributes.clusivity) { }

    public bool Contains(T value) {
        return ComparisonOperator.SatisfiedBy(Endpoint.ComparedWith(value));
    }
}

public static class Bound {
    public static ComparisonOperator GetComparisonOperator(Extremum extremum, Clusivity clusivity) => (extremum, clusivity) switch {
        (Extremum.Min, Clusivity.Inclusive) => ComparisonOperator.GreaterThanOrEqualTo,
        (Extremum.Min, Clusivity.Exclusive) => ComparisonOperator.GreaterThan,
        (Extremum.Max, Clusivity.Inclusive) => ComparisonOperator.LessThanOrEqualTo,
        (Extremum.Max, Clusivity.Exclusive) => ComparisonOperator.LessThan,
        _                                   => throw BEnum.UnhandledSwitch((extremum, clusivity))
    };

    internal static (Extremum extremum, Clusivity clusivity) GetAttributes(ComparisonOperator comparisonOperator) => comparisonOperator switch {
        ComparisonOperator.GreaterThan          => (Extremum.Min, Clusivity.Exclusive),
        ComparisonOperator.GreaterThanOrEqualTo => (Extremum.Min, Clusivity.Inclusive),
        ComparisonOperator.LessThan             => (Extremum.Max, Clusivity.Exclusive),
        ComparisonOperator.LessThanOrEqualTo    => (Extremum.Max, Clusivity.Inclusive),
        ComparisonOperator.EqualTo              => throw BEnum.NotSupported(comparisonOperator),
        ComparisonOperator.NotEqualTo           => throw BEnum.NotSupported(comparisonOperator),
        _                                       => throw BEnum.UnhandledSwitch(comparisonOperator),
    };
}