using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class CollectionUtils {
    #region OrEmpty

    /// <param name="source">the <see cref="IEnumerable{T}"/> that might be null</param>
    /// <typeparam name="T">the type of the elements in <paramref name="source"/></typeparam>
    /// <returns><paramref name="source"/>, or an <see cref="Enumerable.Empty{TResult}"/> if <paramref name="source"/> was null</returns>
    /// <remarks>
    /// To facilitate easier work with <see cref="ImmutableArray{T}"/>, this will also replace <see cref="ImmutableArray{T}.IsDefault"/> with <see cref="ImmutableArray{T}.Empty"/>.
    /// <p/>
    /// <see cref="ImmutableArray{T}"/> also requires a special <c>switch</c> branch because of its nature as a <see cref="ValueType"/>.
    /// <p/>
    /// Other overloads shouldn't require dedicated <c>switch</c> branches because, if their type is known, they should already be passed to the correct overload.
    /// <p/>
    /// Note that there can be conflicts / confusion if another extension method named <c>OrEmpty</c> exists in a different class. For this reason, all overloads should be kept here,
    /// including ones like <see cref="string"/>. 
    /// </remarks>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> OrEmpty<T>([NoEnumeration] this IEnumerable<T>? source) {
        return source switch {
            ImmutableArray<T> immer => immer.OrEmpty(),
            _                       => source ?? Enumerable.Empty<T>(),
        };
    }

    /// <summary>
    /// Returns <see cref="string.Empty">""</see> if <paramref name="str"/> is <c>null</c>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns><paramref name="str"/>, if it was non-<c>null</c>; otherwise, <see cref="string.Empty">""</see></returns>
    /// <remarks>
    /// An explicit overload for <see cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/> is required to make sure that we don't accidentally cast
    /// <see cref="string"/>s to <see cref="IEnumerable{T}"/>s.
    /// </remarks>
    public static string OrEmpty(this string? str) => str ?? "";

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static ImmutableArray<T> OrEmpty<T>(this ImmutableArray<T> source) => source.IsDefault ? ImmutableArray<T>.Empty : source;

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static ImmutableArray<T> OrEmpty<T>(this ImmutableArray<T>? source) => source.GetValueOrDefault().OrEmpty();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static T[] OrEmpty<T>(this T[]? source) => source ?? Array.Empty<T>();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static IList<T> OrEmpty<T>(this IList<T>? source) => source ?? new List<T>();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static List<T> OrEmpty<T>(this List<T>? source) => source ?? new List<T>();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static IDictionary<TKey, TValue> OrEmpty<TKey, TValue>(this IDictionary<TKey, TValue>? source) where TKey : notnull => source ?? new Dictionary<TKey, TValue>();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static Dictionary<TKey, TValue> OrEmpty<TKey, TValue>(this Dictionary<TKey, TValue>? source) where TKey : notnull => source ?? new Dictionary<TKey, TValue>();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static IImmutableList<T> OrEmpty<T>(this IImmutableList<T>? source) => source ?? ImmutableList.Create<T>();

    /// <inheritdoc cref="OrEmpty{T}(System.Collections.Generic.IEnumerable{T}?)"/>
    [Pure]
    public static ImmutableList<T> OrEmpty<T>(this ImmutableList<T>? source) => source ?? ImmutableList.Create<T>();

    #endregion
}