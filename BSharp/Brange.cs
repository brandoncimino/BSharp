using System;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

[Experimental("Should really hold off on this until Generic Math is available")]
public readonly record struct Brange<T>(MinBound<T>? Min, MaxBound<T>? Max) : IRange<T>
    where T : IComparable<T>;

internal static class BrangeExtensions {
    public static bool Contains<T>(this Bound<T>? bound, T value)
        where T : IComparable<T> => bound?.Contains(value) ?? true;

    public static bool Contains<T>(this Bound<T> bound, T value)
        where T : IComparable<T> {
        return StrictlyContains(bound.Endpoint, bound.Extremum, value) ?? bound.Clusivity == Clusivity.Inclusive;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <example>
    /// <code><![CDATA[
    /// (   => exclusive
    /// [   => inclusive
    ///
    /// = Inclusives =
    /// [5.. contains [5..  => true     ◹   a == b and a is inclusive (b's clusivity doesn't matter)
    /// [5.. contains (5..  => true     ◿ 
    /// [5.. contains [4..  => false    (5 > 4)
    /// [5.. contains (4..  => false    (5 > 4)
    /// [5.. contains [6..  => true     (5 < 6)
    /// [5.. contains (6..  => true     (5 < 6)
    ///
    /// = Exclusives =
    /// (5.. contains [5..  => false    <-  a doesn't contain 5, but b does   
    /// (5.. contains (5..  => true     <-  a == b (including clusivity)
    /// (5.. contains [4..  => false
    /// (5.. contains (4..  => false
    /// (5.. contains [6..  => false
    /// (5.. contains (6..  => false
    /// ]]></code>
    /// </example>
    public static bool Contains<T>(this Bound<T> bound, Bound<T> other)
        where T : IComparable<T> {
        if (bound.Extremum != other.Extremum) {
            return false;
        }

        var strict = StrictlyContains(bound.Endpoint, bound.Extremum, other.Endpoint);
        return strict ?? (bound.Clusivity, other.Clusivity) switch {
            (Clusivity.Inclusive, _)                   => true,
            (Clusivity.Exclusive, Clusivity.Exclusive) => true,
            _                                          => false
        };
    }

    private static bool? StrictlyContains<T>(T bound, Extremum boundExtremum, T value)
        where T : IComparable<T> {
        var valueToBound = value.ComparedWith(bound);
        return (valueToBound, boundExtent: boundExtremum) switch {
            (ComparisonResult.LessThan, Extremum.Min)    => false,
            (ComparisonResult.LessThan, Extremum.Max)    => true,
            (ComparisonResult.GreaterThan, Extremum.Min) => true,
            (ComparisonResult.GreaterThan, Extremum.Max) => false,
            (ComparisonResult.EqualTo, _)                => null,
            _                                            => throw BEnum.UnhandledSwitch((valueToBound, boundExtremum)),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <example>
    /// <code><![CDATA[
    /// (   => exclusive
    /// [   => inclusive
    ///
    /// x.. overlaps y..    => true     📎 Any 2 bounds with the same Extent type will overlap
    ///
    /// ..5] overlaps [5..  => true     
    /// ..5] overlaps (5..  => false
    /// ..5] overlaps [4..  => true     ..5] contains 4
    /// ..5] overlaps (4..  => true
    /// ..5] overlaps [6..  => false    ..5] does not contain 6
    /// ..5] overlaps (6..  => false
    /// 
    /// 
    /// ]]></code>
    /// </example>
    public static bool Overlaps<T>(this Bound<T> self, Bound<T> other)
        where T : IComparable<T> {
        if (self.Extremum == other.Extremum) {
            return true;
        }

        return self.Clusivity                                == Clusivity.Inclusive
               && other.Clusivity                            == Clusivity.Inclusive
               && self.Endpoint.ComparedWith(other.Endpoint) == ComparisonResult.EqualTo;
    }
}