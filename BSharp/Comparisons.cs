using System;
using System.Collections.Generic;

namespace FowlFever.BSharp;

public static partial class Comparisons {
    /// <summary>
    /// Performs an <see cref="IComparer{T}.Compare"/>-ison against 2 objects, returning the result as a nice <see cref="ComparisonResult"/>. 
    /// </summary>
    /// <param name="self">this <typeparamref name="T"/></param>
    /// <param name="other">another <typeparamref name="T"/></param>
    /// <param name="comparer">an alternative <see cref="IComparer{T}"/> to <see cref="Comparer{T}.Default"/></param>
    /// <typeparam name="T">the type of the objects being compared</typeparam>
    /// <returns>a nice <see cref="ComparisonResult"/></returns>
    /// <remarks>
    /// The <see cref="IComparable"/> interface is automatically used by <see cref="Comparer{T}.Default"/>: <a href="https://source.dot.net/#System.Private.CoreLib/src/System/Collections/Generic/ComparerHelpers.cs,35">CompareHelpers.CreateDefaultComparer</a>.
    /// </remarks>
    public static ComparisonResult ComparedWith<T>(this T? self, T? other, IComparer<T?>? comparer) {
        comparer ??= Comparer<T?>.Default;
        var sign = comparer.Compare(self, other);
        return ToComparisonResult(sign);
    }

    /// <inheritdoc cref="ComparedWith{T}(T?,T?,System.Collections.Generic.IComparer{T?}?)"/>
    public static ComparisonResult ComparedWith<T>(this IComparable<T>? self, T? other) {
        return (self, other) switch {
            (null, null) => ComparisonResult.EqualTo,
            (null, _)    => ComparisonResult.LessThan,
            (_, null)    => ComparisonResult.GreaterThan,
            _            => ToComparisonResult(self.CompareTo(other))
        };
    }

    /// <inheritdoc cref="ComparedWith{T}(T?,T?,System.Collections.Generic.IComparer{T?}?)"/>
    public static ComparisonResult ComparedWith<T>(this T? self, T? other, Comparison<T> comparison)
        => comparison.GetCompared(self, other);

    /// <inheritdoc cref="Comparison{T}"/>
    /// <returns>a <see cref="ComparisonResult"/> describing the relationship between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public static ComparisonResult GetCompared<T>(this Comparison<T> comparison, T? x, T? y) {
        return (x, y) switch {
            (null, null) => ComparisonResult.EqualTo,
            (null, _)    => ComparisonResult.LessThan,
            (_, null)    => ComparisonResult.GreaterThan,
            (_, _)       => ToComparisonResult(comparison(x, y)),
        };
    }

    /// <inheritdoc cref="IComparer{T}.Compare"/>
    /// <returns>a <see cref="ComparisonResult"/> describing the relationship between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public static ComparisonResult GetCompared<T>(this IComparer<T> comparer, T? x, T? y) => ToComparisonResult(comparer.Compare(x!, y!));

    /// <summary>
    /// Converts a <see cref="IComparable{T}.CompareTo"/>-style result to a <see cref="ComparisonResult"/>:
    /// <ul>
    /// <li><c>-1</c> => <see cref="ComparisonResult.LessThan"/></li>
    /// <li><c>0</c> => <see cref="ComparisonResult.EqualTo"/></li>
    /// <li><c>1</c> => <see cref="ComparisonResult.GreaterThan"/></li>
    /// </ul>
    /// </summary>
    /// <param name="sign">the output of a <see cref="IComparable{T}.CompareTo"/>-style method</param>
    /// <returns></returns>
    /// <remarks>
    /// This is not an extension method because:
    /// <ul>
    /// <li>We don't want to clutter up primitive types like <see cref="int"/> with extension methods</li>
    /// <li>Callers should prefer methods that return <see cref="ComparisonResult"/> directly, such as <see cref="ComparedWith{T}(T?,T?,System.Collections.Generic.IComparer{T?}?)"/></li>
    /// </ul> 
    /// </remarks>
    public static ComparisonResult ToComparisonResult(int sign) {
        return sign switch {
            < 0 => ComparisonResult.LessThan,
            0   => ComparisonResult.EqualTo,
            > 0 => ComparisonResult.GreaterThan,
        };
    }
}