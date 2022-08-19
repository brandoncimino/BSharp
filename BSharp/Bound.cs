using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// Represents the boundary of a <see cref="Brange{T}"/>.
/// </summary>
/// <param name="Endpoint">the value at the <see cref="Extremum"/></param>
/// <param name="Extremum">whether this <see cref="Bound{T}"/> is a <see cref="Enums.Extremum.Min"/> or <see cref="Enums.Extremum.Max"/></param>
/// <param name="Clusivity">whether the <see cref="Endpoint"/> is considered inside of the <see cref="Bound{T}"/> or not</param>
/// <typeparam name="T">the <see cref="IComparable{T}"/> type that this bound constrains</typeparam>
public readonly record struct Bound<T>(T Endpoint, Extremum Extremum, Clusivity Clusivity) : IBound<T>
    where T : IComparable<T> {
    public ComparisonOperator ComparisonOperator => Bound.GetComparisonOperator(Extremum, Clusivity);
}

/// <inheritdoc cref="Bound{T}"/>
public readonly record struct MinBound<T>(T Endpoint, Clusivity Clusivity) : IMinBound<T>
    where T : IComparable<T> {
    public ComparisonOperator ComparisonOperator => Bound.GetComparisonOperator(Extremum.Min, Clusivity);
    public Extremum           Extremum           => Extremum.Min;
}

/// <inheritdoc cref="Bound{T}"/>
public readonly record struct MaxBound<T>(T Endpoint, Clusivity Clusivity) : IMinBound<T>
    where T : IComparable<T> {
    public ComparisonOperator ComparisonOperator => Bound.GetComparisonOperator(Extremum.Max, Clusivity);
    public Extremum           Extremum           => Extremum.Max;
}

public static class BoundExtensions {
    /// <summary>
    /// 📎 a <c>null</c> <see cref="IBound{T}"/> is satisfied by any value.
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="value"></param>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public static bool Contains<B, V>(this B? bound, V value)
        where B : IBound<V>?
        where V : IComparable<V> {
        return bound?.ComparisonOperator.SatisfiedBy(bound.Endpoint.ComparedWith(value)) ?? true;
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

    public static MinBound<T> Min<T>(T value, Clusivity clusivity = Clusivity.Inclusive)
        where T : IComparable<T> => new(value, clusivity);

    public static MaxBound<T> Max<T>(T value, Clusivity clusivity = Clusivity.Exclusive)
        where T : IComparable<T> => new(value, clusivity);
}