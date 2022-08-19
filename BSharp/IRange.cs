using System;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.BSharp;

public interface IRange<T>
    where T : IComparable<T> {
    MinBound<T>? Min { get; }
    MaxBound<T>? Max { get; }
}

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Conflict with System.Range RangeExtensions")]
public static class IRangeExtensions {
    public static bool Contains<R, V>(this R range, V value)
        where R : IRange<V>
        where V : IComparable<V> {
        return (range.Min?.Contains(value) ?? true) && (range.Max?.Contains(value) ?? true);
    }

    public static int? Diameter<R>(this R range)
        where R : IRange<int> {
        return range.Max?.Endpoint - range.Min?.Endpoint;
    }
}