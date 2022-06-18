using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct IntRange : IImmutableList<int>, IList<int>, IList, ICollection, ICollection<int> {
    bool IList.IsReadOnly => true;
    object? IList.this[int index] {
        get => AsNonGenericList[index];
        set => throw Reject.ReadOnly();
    }
    public int this[int index] {
        get => Contains(index) ? index : throw new IndexOutOfRangeException($"{nameof(index)}: {index}");
        set => throw Reject.ReadOnly();
    }

    public bool Contains(int item) => item >= Start && item < End;
    public int  IndexOf(int  item) => Contains(item) ? item - Offset : -1;

    void ICollection<int>.     Add(int     item)                      => throw Reject.ReadOnly();
    int IList.                 Add(object? value)                     => throw Reject.ReadOnly();
    void IList.                Clear()                                => throw Reject.ReadOnly();
    bool IList.                Contains(object? value)                => AsNonGenericList.Contains(value);
    int IList.                 IndexOf(object?  value)                => AsNonGenericList.IndexOf(value);
    void IList.                Insert(int       index, object? value) => throw Reject.ReadOnly();
    void IList.                Remove(object?   value)             => throw Reject.ReadOnly();
    void IList.                RemoveAt(int     index)             => throw Reject.ReadOnly();
    bool IList.                IsFixedSize                         => true;
    void ICollection<int>.     Clear()                             => throw Reject.ReadOnly();
    void ICollection<int>.     CopyTo(int[] array, int arrayIndex) => throw Reject.Unsupported();
    bool ICollection<int>.     Remove(int   item)            => throw Reject.ReadOnly();
    bool ICollection<int>.     IsReadOnly                    => true;
    void IList<int>.           Insert(int   index, int item) => throw Reject.ReadOnly();
    void IList<int>.           RemoveAt(int index)                                                                                => throw Reject.ReadOnly();
    public IImmutableList<int> Clear()                                                                                            => ImmutableList<int>.Empty;
    public int                 IndexOf(int                  item, int index, int count, IEqualityComparer<int>? equalityComparer) => AsList.IndexOf(item, index, count, equalityComparer);
    public int                 LastIndexOf(int              item, int index, int count, IEqualityComparer<int>? equalityComparer) => AsList.LastIndexOf(item, index, count, equalityComparer);
    public IImmutableList<int> Add(int                      value)                                           => AsList.Add(value);
    public IImmutableList<int> AddRange(IEnumerable<int>    items)                                           => AsList.AddRange(items);
    public IImmutableList<int> Insert(int                   index, int                     element)          => AsList.Insert(index, element);
    public IImmutableList<int> InsertRange(int              index, IEnumerable<int>        items)            => AsList.InsertRange(index, items);
    public IImmutableList<int> Remove(int                   value, IEqualityComparer<int>? equalityComparer) => AsList.Remove(value, equalityComparer);
    public IImmutableList<int> RemoveAll(Predicate<int>     match)                                           => AsList.RemoveAll(match);
    public IImmutableList<int> RemoveRange(IEnumerable<int> items, IEqualityComparer<int>? equalityComparer) => AsList.RemoveRange(items, equalityComparer);
    public IImmutableList<int> RemoveRange(int              index, int                     count)            => AsList.RemoveRange(index, count);
    public IImmutableList<int> RemoveAt(int                 index)                                                            => AsList.RemoveAt(index);
    public IImmutableList<int> SetItem(int                  index,    int value)                                              => AsList.SetItem(index, value);
    public IImmutableList<int> Replace(int                  oldValue, int newValue, IEqualityComparer<int>? equalityComparer) => AsList.Replace(oldValue, newValue, equalityComparer);
    void ICollection.          CopyTo(Array                 array,    int index) => AsNonGenericList.CopyTo(array, index);

    bool ICollection.  IsSynchronized => AsNonGenericList.IsSynchronized;
    object ICollection.SyncRoot       => AsNonGenericList.SyncRoot;
}