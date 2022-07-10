using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FowlFever.BSharp.Collections.Implementors;

public interface IHasImmutableList<T> : IImmutableList<T> {
    public IImmutableList<T>      AsList          { get; }
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => AsList.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => ((IEnumerable)AsList).GetEnumerator();
    int IReadOnlyCollection<T>.   Count           => AsList.Count;
    T IReadOnlyList<T>.this[int                                 index] => AsList[index];
    IImmutableList<T> IImmutableList<T>.Add(T                   value)                                                                                    => AsList.Add(value);
    IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items)                                                                                    => AsList.AddRange(items);
    IImmutableList<T> IImmutableList<T>.Clear()                                                                                                           => AsList.Clear();
    int IImmutableList<T>.              IndexOf(T                  item,  int                   index, int count, IEqualityComparer<T>? equalityComparer) => AsList.IndexOf(item, index, count, equalityComparer);
    IImmutableList<T> IImmutableList<T>.Insert(int                 index, T                     element)                                                  => AsList.Insert(index, element);
    IImmutableList<T> IImmutableList<T>.InsertRange(int            index, IEnumerable<T>        items)                                                    => AsList.InsertRange(index, items);
    int IImmutableList<T>.              LastIndexOf(T              item,  int                   index, int count, IEqualityComparer<T>? equalityComparer) => AsList.LastIndexOf(item, index, count, equalityComparer);
    IImmutableList<T> IImmutableList<T>.Remove(T                   value, IEqualityComparer<T>? equalityComparer) => AsList.Remove(value, equalityComparer);
    IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T>     match)                                                                            => AsList.RemoveAll(match);
    IImmutableList<T> IImmutableList<T>.RemoveAt(int               index)                                                                            => AsList.RemoveAt(index);
    IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items,    IEqualityComparer<T>? equalityComparer)                                 => AsList.RemoveRange(items, equalityComparer);
    IImmutableList<T> IImmutableList<T>.RemoveRange(int            index,    int                   count)                                            => AsList.RemoveRange(index, count);
    IImmutableList<T> IImmutableList<T>.Replace(T                  oldValue, T                     newValue, IEqualityComparer<T>? equalityComparer) => AsList.Replace(oldValue, newValue, equalityComparer);
    IImmutableList<T> IImmutableList<T>.SetItem(int                index,    T                     value) => AsList.SetItem(index, value);
}