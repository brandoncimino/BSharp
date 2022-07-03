using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// The idea that linq causes extra allocations appears to be a bold-faced lie:
/// <code><![CDATA[
/// |                       Method |     Mean |     Error |    StdDev |    Gen 0 |   Gen 1 | Allocated |
/// |----------------------------- |---------:|----------:|----------:|---------:|--------:|----------:|
/// |    AggregateListBuilder_Linq | 1.905 ms | 0.1163 ms | 0.3412 ms | 121.0938 | 60.5469 |    749 KB |
/// | AggregateListBuilder_ForEach | 2.122 ms | 0.1447 ms | 0.4220 ms | 121.0938 | 60.5469 |    749 KB |
/// ]]></code>
/// </remarks>
public static class ImmutableExtensions {
    #region Plus

    /// <inheritdoc cref="ImmutableList{T}.Builder.AddRange"/>
    /// <remarks>
    /// This is a chain-friendly version of <see cref="ImmutableList{T}.Builder.AddRange"/>.
    /// </remarks>
    public static ImmutableList<T>.Builder Plus<T>(this ImmutableList<T>.Builder source, IEnumerable<T> additional) {
        source.AddRange(additional);
        return source;
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(System.Collections.Generic.IEnumerable{T})"/>
    /// <remarks>
    /// This is a chain-friendly version of <see cref="ImmutableArray{T}.Builder.AddRange(System.Collections.Generic.IEnumerable{T})"/>.
    /// </remarks>
    public static ImmutableArray<T>.Builder Plus<T>(this ImmutableArray<T>.Builder source, IEnumerable<T> additional) {
        source.AddRange(additional);
        return source;
    }

    /// <inheritdoc cref="ImmutableHashSet{T}.Builder.UnionWith"/>
    /// <remarks>
    /// This is a chain-friendly version of <see cref="ImmutableHashSet{T}.Builder.UnionWith"/>.
    /// </remarks>
    public static ImmutableHashSet<T>.Builder Plus<T>(this ImmutableHashSet<T>.Builder source, IEnumerable<T> additional) {
        source.UnionWith(additional);
        return source;
    }

    /// <inheritdoc cref="ImmutableDictionary{TKey,TValue}.Builder.AddRange"/>
    /// <remarks>
    /// This is a chain-friendly version of <see cref="ImmutableDictionary{TKey,TValue}.Builder.AddRange"/>
    /// </remarks>
    public static ImmutableDictionary<K, V>.Builder Plus<K, V>(this ImmutableDictionary<K, V>.Builder source, IEnumerable<KeyValuePair<K, V>> additional)
        where K : notnull {
        source.AddRange(additional);
        return source;
    }

    /// <inheritdoc cref="ImmutableSortedDictionary{TKey,TValue}.Builder.AddRange"/>
    /// <remarks>
    /// This is a chain-friendly version of <see cref="ImmutableSortedDictionary{TKey,TValue}.Builder.AddRange"/>
    /// </remarks>
    public static ImmutableSortedDictionary<K, V>.Builder Plus<K, V>(this ImmutableSortedDictionary<K, V>.Builder source, IEnumerable<KeyValuePair<K, V>> additional)
        where K : notnull {
        source.AddRange(additional);
        return source;
    }

    #endregion

    /// <summary>
    /// Flattens and collects a jagged collection into an <see cref="ImmutableList{T}"/> efficiently via <see cref="ImmutableList{T}.Builder"/>.
    /// </summary>
    /// <param name="source">a jagged collection</param>
    /// <typeparam name="T">the type of individual items</typeparam>
    /// <returns>a new <see cref="ImmutableList{T}"/></returns>
    public static ImmutableList<T> AggregateImmutableList<T>(this IEnumerable<IEnumerable<T>> source) =>
        source.Aggregate(
            ImmutableList.CreateBuilder<T>(),
            Plus,
            builder => builder.ToImmutable()
        );

    /// <summary>
    /// Flattens and collects a jagged collection in an <see cref="ImmutableArray{T}"/> efficiently via <see cref="ImmutableArray{T}.Builder"/>
    /// </summary>
    /// <param name="source">a jagged collection</param>
    /// <typeparam name="T">the type of individual items</typeparam>
    /// <returns>a new <see cref="ImmutableArray{T}"/></returns>
    public static ImmutableArray<T> AggregateImmutableArray<T>(this IEnumerable<IEnumerable<T>> source) =>
        source.Aggregate(
            ImmutableArray.CreateBuilder<T>(),
            Plus,
            builder => builder.ToImmutable()
        );

    /// <summary>
    /// Flattens and collects a jagged collection in an <see cref="ImmutableHashSet{T}"/> efficiently via <see cref="ImmutableHashSet{T}.Builder"/>
    /// </summary>
    /// <param name="source">a jagged collection</param>
    /// <typeparam name="T">the type of individual items</typeparam>
    /// <returns>a new <see cref="ImmutableHashSet{T}"/></returns>
    public static ImmutableHashSet<T> AggregateImmutableHashSet<T>(this IEnumerable<IEnumerable<T>> source) =>
        source.Aggregate(
            ImmutableHashSet.CreateBuilder<T>(),
            Plus,
            builder => builder.ToImmutable()
        );

    public static IImmutableList<T> AsImmutableList<T>(this IEnumerable<T> source) => source as IImmutableList<T> ?? source.ToImmutableList();

    /// <summary>
    /// Delegates to <see cref="ImmutableList{T}.GetRange"/> in a way that places nice with <see cref="System.Range"/>.
    /// </summary>
    public static ImmutableList<T> Slice<T>(this ImmutableList<T> source, int start, int length) {
        return source.GetRange(start, start + length);
    }
}