using System.Collections;
using System.Collections.Immutable;

namespace FowlFever.Implementors;

/// <summary>
/// Delegates the implementation of <see cref="IImmutableList{T}"/> to the <see cref="AsImmutableList"/> property.
/// </summary>
/// <typeparam name="T">the type of the entries in <see cref="AsImmutableList"/></typeparam>
public interface IHasImmutableList<T> : IImmutableList<T> {
    protected IImmutableList<T> AsImmutableList { get; }

    #region Implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => AsImmutableList.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => ((IEnumerable)AsImmutableList).GetEnumerator();
    int IReadOnlyCollection<T>.   Count           => AsImmutableList.Count;
    T IReadOnlyList<T>.this[int                                 index] => AsImmutableList[index];
    IImmutableList<T> IImmutableList<T>.Add(T                   value)                                                                                    => AsImmutableList.Add(value);
    IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items)                                                                                    => AsImmutableList.AddRange(items);
    IImmutableList<T> IImmutableList<T>.Clear()                                                                                                           => AsImmutableList.Clear();
    int IImmutableList<T>.              IndexOf(T                  item,  int                   index, int count, IEqualityComparer<T>? equalityComparer) => AsImmutableList.IndexOf(item, index, count, equalityComparer);
    IImmutableList<T> IImmutableList<T>.Insert(int                 index, T                     element)                                                  => AsImmutableList.Insert(index, element);
    IImmutableList<T> IImmutableList<T>.InsertRange(int            index, IEnumerable<T>        items)                                                    => AsImmutableList.InsertRange(index, items);
    int IImmutableList<T>.              LastIndexOf(T              item,  int                   index, int count, IEqualityComparer<T>? equalityComparer) => AsImmutableList.LastIndexOf(item, index, count, equalityComparer);
    IImmutableList<T> IImmutableList<T>.Remove(T                   value, IEqualityComparer<T>? equalityComparer) => AsImmutableList.Remove(value, equalityComparer);
    IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T>     match)                                                                            => AsImmutableList.RemoveAll(match);
    IImmutableList<T> IImmutableList<T>.RemoveAt(int               index)                                                                            => AsImmutableList.RemoveAt(index);
    IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items,    IEqualityComparer<T>? equalityComparer)                                 => AsImmutableList.RemoveRange(items, equalityComparer);
    IImmutableList<T> IImmutableList<T>.RemoveRange(int            index,    int                   count)                                            => AsImmutableList.RemoveRange(index, count);
    IImmutableList<T> IImmutableList<T>.Replace(T                  oldValue, T                     newValue, IEqualityComparer<T>? equalityComparer) => AsImmutableList.Replace(oldValue, newValue, equalityComparer);
    IImmutableList<T> IImmutableList<T>.SetItem(int                index,    T                     value) => AsImmutableList.SetItem(index, value);

    #endregion
}