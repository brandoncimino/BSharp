using System;
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
public static partial class ImmutableExtensions {
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

    /// <inheritdoc cref="Slice{T}(System.Collections.Immutable.ImmutableList{T},int,int)"/>
    public static ImmutableList<T> Slice<T>(this ImmutableList<T> source, Range range) {
        var (off, len) = range.GetOffsetAndLength(source.Count);
        return source.GetRange(off, len);
    }

    /// <summary>
    /// Gets a sub-<see cref="ImmutableArray{T}"/> defined by a <see cref="Range"/>.
    /// </summary>
    /// <remarks>
    /// This will <i>(usually)</i> allocate a <b>new <see cref="ImmutableArray{T}"/></b>!
    /// <i>(The doc comments for <see cref="ImmutableArray.CreateRange{T}"/> claim that it will avoid "tax" when creating <see cref="ImmutableArray{T}"/>s that are sub-sections of existing <see cref="ImmutableArray{T}"/>s,
    /// but I don't see how the logic can actually do that (with the exception of the obvious cases where the <see cref="Range"/> is empty or <see cref="Range.All"/>))</i> 
    /// <p/>
    /// If you want to pass a subsection around without this allocation, use <see cref="ImmutableArray{T}.AsSpan"/>.<see cref="ReadOnlySpan{T}.Slice(int,int)"/> instead
    /// (which can implicitly use [<see cref="Range"/>] syntax).
    /// <p/>
    /// TODO: How could this method <b>not</b> already exist?!
    /// </remarks>
    /// <param name="source">the original <see cref="ImmutableArray{T}"/></param>
    /// <param name="range">the <see cref="Range"/> of items to take from the <paramref name="source"/></param>
    /// <typeparam name="T">the type of elements in <see cref="source"/></typeparam>
    /// <returns>a new <see cref="ImmutableArray{T}"/></returns>
    public static ImmutableArray<T> Slice<T>(this ImmutableArray<T> source, Range range) {
        var (off, len) = range.GetOffsetAndLength(source.Length);
        return ImmutableArray.Create(source, off, len);
    }

    /// <inheritdoc cref="Slice{T}(System.Collections.Immutable.ImmutableArray{T},System.Range)"/>
    public static ImmutableArray<T> Slice<T>(this ImmutableArray<T> source, int start, int length) {
        return source.AsSpan().Slice(start, length).CreateImmutableArray();
    }

    /// <summary>
    /// Calls <see cref="ImmutableArray{T}.Builder.MoveToImmutable"/>, updating the <see cref="ImmutableArray{T}.Builder.Capacity"/> if necessary.
    /// </summary>
    /// <remarks>
    /// Comparison:
    /// <code><![CDATA[
    /// MoveToImmutable         Re-uses the underlying array IF it has the exact same size
    /// ToArray                 ALWAYS creates a new array, even if the size is the same
    /// MoveToImmutableSafely   prefers re-using the array; if it can't, re-sizes it and then uses that one
    /// ]]></code>
    /// </remarks>
    /// <param name="builder">this <see cref="ImmutableArray{T}.Builder"/></param>
    /// <typeparam name="T">the type of entries in the <see cref="ImmutableArray{T}.Builder"/></typeparam>
    /// <returns>an <see cref="ImmutableArray{T}"/> with the entries from the <paramref name="builder"/></returns>
    public static ImmutableArray<T> MoveToImmutableSafely<T>(this ImmutableArray<T>.Builder builder) {
        if (builder.Capacity != builder.Count) {
            builder.Capacity = builder.Count;
        }

        return builder.MoveToImmutable();
    }

    /// <summary>
    /// Creates a new <see cref="ImmutableArray{T}"/> containing the original <paramref name="source"/> repeated <paramref name="repetitions"/> times.
    /// </summary>
    /// <param name="source">the original <see cref="ImmutableArray{T}"/></param>
    /// <param name="repetitions">the number of duplicates of <paramref name="source"/> that you want concatenated together</param>
    /// <typeparam name="T">the array element type</typeparam>
    /// <returns>an <see cref="ImmutableArray{T}"/> containing all of the results</returns>
    public static ImmutableArray<T> Repeat<T>(this ImmutableArray<T> source, int repetitions = 2) {
        ImmutableArray<T> RepeatToImmutable(ImmutableArray<T> immutableArray, int repetitions1) {
            var totalLength = immutableArray.Length * repetitions1;
            var builder     = ImmutableArray.CreateBuilder<T>(totalLength);
            for (int i = 0; i < repetitions1; i++) {
                builder.AddRange(immutableArray);
            }

            return builder.MoveToImmutable();
        }

        return repetitions switch {
            <= 0 => ImmutableArray<T>.Empty,
            1    => source,
            _    => RepeatToImmutable(source, repetitions)
        };
    }

    #region Collecting Spans

    /// <summary>
    /// Creates a new <see cref="ImmutableArray{T}"/> containing the entries of this <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <remarks>
    /// This is named such that it doesn't conflict with the non-.NET-Standard-2.1 method <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray.ToImmutableArray?view=net-7.0">ImmutableArray.Create()</a>.
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the element type of <paramref name="span"/></typeparam>
    /// <returns>a new <see cref="ImmutableArray{T}"/></returns>
    public static ImmutableArray<T> CreateImmutableArray<T>(this ReadOnlySpan<T> span) {
#if NET5_0_OR_GREATER
        return ImmutableArray.Create(span);
#else
        return ImmutableArray.CreateBuilder<T>(span.Length)
                             .AddRange(span)
                             .MoveToImmutable();
#endif
    }

    /// <summary>
    /// <see cref="ImmutableArray{T}.Builder.Add"/>s each element of a <see cref="ReadOnlySpan{T}"/> to this <see cref="ImmutableArray{T}.Builder"/>.
    /// </summary>
    /// <param name="builder">this <see cref="ImmutableArray{T}.Builder"/></param>
    /// <param name="span">the <typeparamref name="T"/> entries to be added</param>
    /// <typeparam name="T">the <see cref="ImmutableArray{T}"/> type</typeparam>
    /// <returns>this <see cref="ImmutableArray{T}.Builder"/></returns>
    public static ImmutableArray<T>.Builder AddRange<T>(this ImmutableArray<T>.Builder builder, ReadOnlySpan<T> span) {
        foreach (var it in span) {
            builder.Add(it);
        }

        return builder;
    }

    /// <summary>
    /// Creates a new <see cref="ImmutableList{T}"/> containing the entries of this <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the element type of <paramref name="span"/></typeparam>
    /// <returns>a new <see cref="ImmutableList{T}"/></returns>
    public static ImmutableList<T> ToImmutableList<T>(this ReadOnlySpan<T> span) {
        return span.ToArray().ToImmutableList();
    }

    #endregion

    /// <summary>
    /// Creates a <see cref="object.GetHashCode"/> value using each entry in this <see cref="ImmutableArray{T}"/>, <i>in order</i>.
    /// </summary>
    /// <param name="source">this <see cref="ImmutableArray{T}"/></param>
    /// <typeparam name="T">the array entry type</typeparam>
    /// <returns>an <see cref="int"/> value usable as a <see cref="object.GetHashCode"/></returns>
    public static int GetSequenceHashCode<T>(this ImmutableArray<T> source) {
        var hc = new HashCode();

        foreach (var t in source) {
            hc.Add(t);
        }

        return hc.ToHashCode();
    }

    /// <summary>
    /// Modifies an existing <see cref="KeyValuePair{TKey,TValue}.value"/> in an <see cref="IImmutableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="dic">this <see cref="IImmutableDictionary{TKey,TValue}"/></param>
    /// <param name="key">the <see cref="KeyValuePair{TKey,TValue}.key"/> whose value will be updated</param>
    /// <param name="update">takes in the old <see cref="KeyValuePair{TKey,TValue}.value"/> and produces the new one</param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.value"/> type</typeparam>
    /// <returns>a new <see cref="IImmutableDictionary{TKey,TValue}"/></returns>
    /// <exception cref="KeyNotFoundException">if the <paramref name="key"/> isn't in the dictionary</exception>
    public static IImmutableDictionary<K, V> Update<K, V>(this IImmutableDictionary<K, V> dic, K key, Func<V, V> update) {
        return dic.SetItem(key, update(dic[key]));
    }

    /// <summary>
    /// <inheritdoc cref="Update{K,V}"/>
    /// </summary>
    /// <param name="dic">this <see cref="IImmutableDictionary{TKey,TValue}"/></param>
    /// <param name="key">the <see cref="KeyValuePair{TKey,TValue}.key"/> whose value will be updated</param>
    /// <param name="arg">an argument to the <paramref name="update"/> function</param>
    /// <param name="update">takes in the old <see cref="KeyValuePair{TKey,TValue}.value"/> and the <paramref name="arg"/> and produces the new value</param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.value"/> type</typeparam>
    /// <typeparam name="A">the <paramref name="arg"/> type</typeparam>
    /// <returns><inheritdoc cref="Update{K,V}"/></returns>
    /// <exception cref="KeyNotFoundException">if the <paramref name="key"/> isn't in the dictionary</exception>
    public static IImmutableDictionary<K, V> Update<K, V, A>(this IImmutableDictionary<K, V> dic, K key, A arg, Func<V, A, V> update) {
        return dic.SetItem(key, update(dic[key], arg));
    }
}