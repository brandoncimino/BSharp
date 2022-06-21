using System.Collections.Generic;

namespace FowlFever.BSharp;

public static class ComparerExtensions {
    /// <summary>
    /// Falls back on <see cref="EqualityComparer{T}.Default"/> if this <see cref="IEqualityComparer{T}"/> is <c>null</c>.
    /// </summary>
    /// <param name="comparer">this <see cref="IEqualityComparer{T}"/></param>
    /// <typeparam name="T">the type being compared</typeparam>
    /// <returns>this <see cref="IEqualityComparer{T}"/>, if it was non-null; otherwise, <see cref="EqualityComparer{T}.Default"/></returns>
    public static IEqualityComparer<T> OrDefault<T>(this IEqualityComparer<T>? comparer) => comparer ?? EqualityComparer<T>.Default;

    /// <summary>
    /// Falls back on <see cref="Comparer{T}.Default"/> if this <see cref="IComparer{T}"/> is <c>null</c>.
    /// </summary>
    /// <param name="comparer">this <see cref="IComparer{T}"/></param>
    /// <typeparam name="T">the type being compared</typeparam>
    /// <returns>this <see cref="IComparer{T}"/>, if it was non-null; otherwise, <see cref="Comparer{T}.Default"/></returns>
    public static IComparer<T> OrDefault<T>(this IComparer<T>? comparer) => comparer ?? Comparer<T>.Default;
}