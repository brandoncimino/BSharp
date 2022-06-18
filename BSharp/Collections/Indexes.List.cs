using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct Indexes : IImmutableList<int>, IList<int>, IList, IReadOnlyList<int>, IReadOnlyCollection<int> {
    #region Implementation of ICollection<int>

    void ICollection<int>.Add(int item)                       => throw UnsupportedMethodException();
    void ICollection<int>.Clear()                             => throw UnsupportedMethodException();
    void ICollection<int>.CopyTo(int[] array, int arrayIndex) => throw UnsupportedMethodException();
    bool ICollection<int>.Remove(int   item) => throw UnsupportedMethodException();

    #endregion

    #region Implementation of IList

    int IList. Add(object? value)                     => throw UnsupportedMethodException();
    void IList.Clear()                                => throw UnsupportedMethodException();
    bool IList.Contains(object? value)                => AsNonGenericList.Contains(value);
    int IList. IndexOf(object?  value)                => AsNonGenericList.IndexOf(value);
    void IList.Insert(int       index, object? value) => throw UnsupportedMethodException();
    void IList.Remove(object?   value) => throw UnsupportedMethodException();
    bool IList.IsFixedSize             => true;
    object? IList.this[int index] {
        get => AsNonGenericList[index]!;
        set => throw Reject.ReadOnly();
    }

    #endregion

    #region Implementation of IList<int>

    public int  IndexOf(int  item)            => AsList.IndexOf(item);
    public void Insert(int   index, int item) => throw UnsupportedMethodException();
    public void RemoveAt(int index) => throw UnsupportedMethodException();
    int IList<int>.this[int index] {
        get => AsList[index];
        set => throw UnsupportedMethodException();
    }

    #endregion

    #region Implementation of IImmutableList<T>

    IImmutableList<int> IImmutableList<int>.Clear() => AsList.Clear();

    public int              IndexOf(int     item, int index, int count, IEqualityComparer<int>? equalityComparer = null) => AsList.IndexOf(item, index, count, equalityComparer);
    int IImmutableList<int>.LastIndexOf(int item, int index, int count, IEqualityComparer<int>? equalityComparer)        => AsList.LastIndexOf(item, index, count, equalityComparer);

    IImmutableList<int> IImmutableList<int>.Add(int                      value)                                           => AsList.Add(value);
    IImmutableList<int> IImmutableList<int>.AddRange(IEnumerable<int>    items)                                           => AsList.AddRange(items);
    IImmutableList<int> IImmutableList<int>.Insert(int                   index, int                     element)          => AsList.Insert(index, element);
    IImmutableList<int> IImmutableList<int>.InsertRange(int              index, IEnumerable<int>        items)            => AsList.InsertRange(index, items);
    IImmutableList<int> IImmutableList<int>.Remove(int                   value, IEqualityComparer<int>? equalityComparer) => AsList.Remove(value, equalityComparer);
    IImmutableList<int> IImmutableList<int>.RemoveAll(Predicate<int>     match)                                           => AsList.RemoveAll(match);
    IImmutableList<int> IImmutableList<int>.RemoveRange(IEnumerable<int> items, IEqualityComparer<int>? equalityComparer) => AsList.RemoveRange(items, equalityComparer);
    IImmutableList<int> IImmutableList<int>.RemoveRange(int              index, int                     count)            => AsList.RemoveRange(index, count);
    IImmutableList<int> IImmutableList<int>.RemoveAt(int                 index)                                                            => AsList.RemoveAt(index);
    IImmutableList<int> IImmutableList<int>.SetItem(int                  index,    int value)                                              => AsList.SetItem(index, value);
    IImmutableList<int> IImmutableList<int>.Replace(int                  oldValue, int newValue, IEqualityComparer<int>? equalityComparer) => AsList.Replace(oldValue, newValue, equalityComparer);

    #endregion

    #region Implementation of IReadOnlyList<out int>

    public int this[int index] => Must.BeIndexOf(index, this);

    #endregion
}