using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct Indexes : IImmutableList<int>, IList<int>, IList {
    #region Implementation of ICollection<int>

    bool IList.           IsReadOnly                          => true;
    bool ICollection<int>.IsReadOnly                          => true;
    void ICollection<int>.Add(int item)                       => throw Reject.ReadOnly();
    void ICollection<int>.Clear()                             => throw Reject.ReadOnly();
    void ICollection<int>.CopyTo(int[] array, int arrayIndex) => throw UnsupportedMethodException();
    void ICollection.     CopyTo(Array array, int index)      => throw UnsupportedMethodException();
    bool ICollection<int>.Remove(int   item) => throw Reject.ReadOnly();

    #endregion

    #region Implementation of IList

    int IList. Add(object? value)                     => throw Reject.ReadOnly();
    void IList.Clear()                                => throw Reject.ReadOnly();
    bool IList.Contains(object? value)                => value is int i && Contains(i);
    int IList. IndexOf(object?  value)                => value is int i && Contains(i) ? i : -1;
    void IList.Insert(int       index, object? value) => throw Reject.ReadOnly();
    void IList.Remove(object?   value) => throw Reject.ReadOnly();
    bool IList.IsFixedSize             => true;
    object? IList.this[int index] {
        get => RequireIndex(index);
        set => throw Reject.ReadOnly();
    }

    #endregion

    #region Implementation of IList<int>

    int IList<int>. IndexOf(int  item)            => Contains(item) ? item : -1;
    void IList<int>.Insert(int   index, int item) => throw Reject.ReadOnly();
    void IList.     RemoveAt(int index) => throw Reject.ReadOnly();
    void IList<int>.RemoveAt(int index) => throw Reject.ReadOnly();

    int IList<int>.this[int index] {
        get => RequireIndex(index);
        set => throw Reject.ReadOnly();
    }

    #endregion

    #region Implementation of IImmutableList<T>

    IImmutableList<int> IImmutableList<int>.Clear() => ImmutableList<int>.Empty;

    private int _IndexOfImmutable_Directional(
        int                     item,
        int                     index,
        int                     count,
        IEqualityComparer<int>? equalityComparer,
        bool                    isForward
    ) {
        var endPoint = index + count;
        RequireIndex(index);
        RequireIndex(endPoint);

        if (equalityComparer == null) {
            return item >= index && item < endPoint ? item : -1;
        }

        for (int i = index; i < endPoint; i++) {
            var checkIndex = isForward switch {
                true  => i,
                false => endPoint - i + 1,
            };

            if (equalityComparer.Equals(checkIndex, item)) {
                return checkIndex;
            }
        }

        return -1;
    }

    int IImmutableList<int>.IndexOf(int item, int index, int count, IEqualityComparer<int>? equalityComparer) => _IndexOfImmutable_Directional(item, index, count, equalityComparer, true);

    public int LastIndexOf(int item, int index, int count, IEqualityComparer<int>? equalityComparer) => _IndexOfImmutable_Directional(item, index, count, equalityComparer, false);

    IImmutableList<int> IImmutableList<int>.Add(int                   value) => this.Append(value).ToImmutableList();
    IImmutableList<int> IImmutableList<int>.AddRange(IEnumerable<int> items) => this.Concat(items).ToImmutableList();

    IImmutableList<int> IImmutableList<int>.Insert(int index, int element) {
        RequireIndex(index);
        var (taken, leftovers) = this.TakeLeftovers(index);
        return taken.Append(element).Concat(leftovers).ToImmutableList();
    }

    IImmutableList<int> IImmutableList<int>.InsertRange(int index, IEnumerable<int> items) {
        RequireIndex(index);
        var (taken, leftovers) = this.TakeLeftovers(index);
        return taken.Concat(items).Concat(leftovers).ToImmutableList();
    }

    IImmutableList<int> IImmutableList<int>.Remove(int value, IEqualityComparer<int>? equalityComparer) {
        RequireIndex(value);
        //todo: could be more efficient 🤷‍
        return this.ToImmutableList().Remove(value, equalityComparer);
    }

    IImmutableList<int> IImmutableList<int>.RemoveAll(Predicate<int>     match)                                           => this.Where(it => match(it) == false).ToImmutableList();
    IImmutableList<int> IImmutableList<int>.RemoveRange(IEnumerable<int> items, IEqualityComparer<int>? equalityComparer) => this.Except(items, equalityComparer).ToImmutableList();

    IImmutableList<int> IImmutableList<int>.RemoveRange(int index, int count) {
        var endPoint = index + count;
        RequireIndex(index);
        RequireIndex(endPoint);
        var before = Enumerable.Range(0,        index);
        var after  = Enumerable.Range(endPoint, Count - endPoint);
        return before.Concat(after).ToImmutableList();
    }

    IImmutableList<int> IImmutableList<int>.RemoveAt(int index) {
        RequireIndex(index);
        var before = Enumerable.Range(0,         index);
        var after  = Enumerable.Range(index + 1, Count - index - 1);
        return before.Concat(after).ToImmutableList();
    }

    IImmutableList<int> IImmutableList<int>.SetItem(int index, int value) => this.Select(it => it == index ? value : it).ToImmutableList();

    IImmutableList<int> IImmutableList<int>.Replace(int oldValue, int newValue, IEqualityComparer<int>? equalityComparer) {
        RequireIndex(oldValue);
        //todo: could be improved with Indexes-specific logic, but like, gross
        return this.ToImmutableList().Replace(oldValue, newValue, equalityComparer);
    }

    #endregion

    #region Implementation of IReadOnlyList<out int>

    public int this[int index] => RequireIndex(index);

    #endregion
}