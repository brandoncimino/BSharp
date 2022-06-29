using System;
using System.Collections.Generic;

namespace FowlFever.BSharp;

/// <summary>
/// A nice representation of an <see cref="IComparable{T}.CompareTo"/> operation.
/// </summary>
public enum ComparisonResult : sbyte {
    LessThan = -1, EqualTo = 0, GreaterThan = 1,
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
        where T : IComparable<T> => ComparedWith(self, other, Comparer<T?>.Create(comparison));

    internal static ComparisonResult ToComparisonResult(this int sign) {
        return sign switch {
            < 0 => ComparisonResult.LessThan,
            0   => ComparisonResult.EqualTo,
            > 0 => ComparisonResult.GreaterThan,
        };
    }
}