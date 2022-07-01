using System;
using System.Collections.Generic;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// A nice representation of an <see cref="IComparable{T}.CompareTo"/> operation.
/// </summary>
public enum ComparisonResult : sbyte {
    LessThan = -1, EqualTo = 0, GreaterThan = 1,
}

/// <summary>
/// Equivalent to <see cref="IComparer{T}"/> but returns the results as <see cref="ComparisonResult"/> instead of ugly integers.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Comparator<T> : IComparer<T> {
    public static readonly Comparator<T> Default = new();
    private                IComparer<T>  Comparer { get; init; } = Comparer<T>.Default;

    public static Comparator<T> Create(Comparison<T> comparison) {
        return new Comparator<T>() {
            Comparer = Comparer<T>.Create(comparison)
        };
    }

    public static Comparator<T> Comparing<TInnerSelf>(Converter<T, TInnerSelf> converter, Comparison<TInnerSelf>? comparison = default) {
        return comparison switch {
            null => Create((x, y) => Comparer<TInnerSelf>.Default.Compare(converter(x), converter(y))),
            _    => Create((x, y) => comparison(converter(x), converter(y)))
        };
    }

    public ComparisonResult Compare(T? x, T? y) => _compare(x, y).ToComparisonResult();

    public ComparisonResult Compare(T? x, IComparable<T?>? y) {
        return (x, y) switch {
            (null, null) => ComparisonResult.EqualTo,
            (null, _)    => ComparisonResult.LessThan,
            (_, null)    => ComparisonResult.GreaterThan,
            (_, _)       => y.CompareTo(x).ToComparisonResult(),
        };
    }

    private int      _compare(T? x, T? y) => Comparer.Compare(x, y);
    int IComparer<T>.Compare(T?  x, T? y) => _compare(x, y);
}

public static class ComparisonExtensions {
    /// <summary>
    /// Performs an <see cref="IComparer{T}.Compare"/>-ison against 2 objects, returning the result as a nice <see cref="ComparisonResult"/>. 
    /// </summary>
    /// <param name="self">this <typeparamref name="T"/></param>
    /// <param name="other">another <typeparamref name="T"/></param>
    /// <param name="comparer">an alternative <see cref="IComparer{T}"/> to <see cref="Comparer{T}.Default"/></param>
    /// <typeparam name="T">the type of the objects being compared</typeparam>
    /// <returns>a nice <see cref="ComparisonResult"/></returns>
    /// <remarks>
    /// The <see cref="IComparable{T}"/> interface is automatically used by <see cref="Comparer{T}.Default"/>: <a href="https://source.dot.net/#System.Private.CoreLib/src/System/Collections/Generic/ComparerHelpers.cs,35">CompareHelpers.CreateDefaultComparer</a>.
    /// </remarks>
    public static ComparisonResult ComparedWith<T>(this T? self, T? other, IComparer<T?>? comparer = default)
        => (comparer ?? Comparer<T?>.Default).Compare(self, other).ToComparisonResult();

    /// <inheritdoc cref="ComparedWith{T}(T?,T?,System.Collections.Generic.IComparer{T?}?)"/>
    public static ComparisonResult ComparedWith<T>(this T? self, T? other, Comparison<T?> comparison)
        where T : IComparable<T> => Comparator<T>.Create(comparison).Compare(self, other);

    internal static ComparisonResult ToComparisonResult(this int sign) {
        return sign switch {
            < 0 => ComparisonResult.LessThan,
            0   => ComparisonResult.EqualTo,
            > 0 => ComparisonResult.GreaterThan,
        };
    }

    internal static ComparisonResult Invert(this ComparisonResult result) => result switch {
        ComparisonResult.LessThan    => ComparisonResult.GreaterThan,
        ComparisonResult.EqualTo     => ComparisonResult.EqualTo,
        ComparisonResult.GreaterThan => ComparisonResult.LessThan,
        _                            => throw BEnum.UnhandledSwitch(result),
    };

    public static IComparer<T> ToComparer<T>(this Comparison<T> comparison) => Comparer<T>.Create(comparison);
}