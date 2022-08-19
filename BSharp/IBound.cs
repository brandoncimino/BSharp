using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

public interface IBound<out T>
    where T : IComparable<T> {
    /// <summary>
    /// Describes how the <see cref="Endpoint"/> should compare to a <typeparamref name="T"/> value.
    /// </summary>
    public ComparisonOperator ComparisonOperator { get; }
    public T         Endpoint  { get; }
    public Extremum  Extremum  { get; }
    public Clusivity Clusivity { get; }
}

public interface IMinBound<out T> : IBound<T>
    where T : IComparable<T> {
    Extremum IBound<T>.Extremum => Extremum.Min;
}

public interface IMaxBound<out T> : IBound<T>
    where T : IComparable<T> {
    Extremum IBound<T>.Extremum => Extremum.Max;
}