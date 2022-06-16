using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct Indexes : IImmutableList<int> {
    private static readonly ConcurrentDictionary<int, Lazy<ImmutableList<int>>> ListCache = new();
    private                 ImmutableList<int>                                  AsList => ListCache.GetOrAddLazily(Count, ct => Enumerable.Range(0, ct).ToImmutableList());

    #region Implementation of IImmutableList<T>

    IImmutableList<int> IImmutableList<int>.Clear() => AsList.Clear();

    public int IndexOf(
        int                     item,
        int                     index,
        int                     count,
        IEqualityComparer<int>? equalityComparer = null
    ) {
        // TODO: does the basic ImmutableList throw an exception if the index and/or index+count is out of range?
        if (equalityComparer == null) {
            var range = index..(index + count);
            return range.Contains(item) ? item : -1;
        }

        return Enumerable.Range(index, count).FirstIndexOf(item, equalityComparer) ?? -1;
    }

    int IImmutableList<int>.LastIndexOf(
        int                     item,
        int                     index,
        int                     count,
        IEqualityComparer<int>? equalityComparer
    ) => IndexOf(item, index, count, equalityComparer);

    IImmutableList<int> IImmutableList<int>.Add(int                      value)                                          => AsList.Add(value);
    IImmutableList<int> IImmutableList<int>.AddRange(IEnumerable<int>    items)                                          => AsList.AddRange(items);
    IImmutableList<int> IImmutableList<int>.Insert(int                   index, int                    element)          => AsList.Insert(index, element);
    IImmutableList<int> IImmutableList<int>.InsertRange(int              index, IEnumerable<int>       items)            => AsList.InsertRange(index, items);
    IImmutableList<int> IImmutableList<int>.Remove(int                   value, IEqualityComparer<int> equalityComparer) => AsList.Remove(value, equalityComparer);
    IImmutableList<int> IImmutableList<int>.RemoveAll(Predicate<int>     match)                                          => AsList.RemoveAll(match);
    IImmutableList<int> IImmutableList<int>.RemoveRange(IEnumerable<int> items, IEqualityComparer<int> equalityComparer) => AsList.RemoveRange(items, equalityComparer);
    IImmutableList<int> IImmutableList<int>.RemoveRange(int              index, int                    count)            => AsList.RemoveRange(index, count);
    IImmutableList<int> IImmutableList<int>.RemoveAt(int                 index)                                                           => AsList.RemoveAt(index);
    IImmutableList<int> IImmutableList<int>.SetItem(int                  index,    int value)                                             => AsList.SetItem(index, value);
    IImmutableList<int> IImmutableList<int>.Replace(int                  oldValue, int newValue, IEqualityComparer<int> equalityComparer) => AsList.Replace(oldValue, newValue, equalityComparer);

    #endregion
}