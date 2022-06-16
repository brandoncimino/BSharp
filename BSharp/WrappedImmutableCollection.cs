using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FowlFever.BSharp;

/// <summary>
/// Similar to <see cref="WrappedCollection{TElement,TCollection}"/> but also delegates to <see cref="IImmutableList{T}"/> methods.
/// </summary>
/// <inheritdoc cref="WrappedCollection{TElement,TCollection}"/>
public abstract record WrappedImmutableCollection<TElement, TCollection>(TCollection Value) : WrappedCollection<TElement, TCollection>(Value), IImmutableList<TElement>
    where TCollection : IImmutableList<TElement> {
    #region Implementation of IImmutableList<T>

    public new virtual IImmutableList<TElement> Clear() {
        return Value.Clear();
    }

    public virtual int IndexOf(
        TElement                    item,
        int                         index,
        int                         count,
        IEqualityComparer<TElement> equalityComparer
    ) {
        return Value.IndexOf(item, index, count, equalityComparer);
    }

    public virtual int LastIndexOf(
        TElement                    item,
        int                         index,
        int                         count,
        IEqualityComparer<TElement> equalityComparer
    ) {
        return Value.LastIndexOf(item, index, count, equalityComparer);
    }

    public new virtual IImmutableList<TElement> Add(TElement value) {
        return Value.Add(value);
    }

    public virtual IImmutableList<TElement> AddRange(IEnumerable<TElement> items) {
        return Value.AddRange(items);
    }

    public new virtual IImmutableList<TElement> Insert(int index, TElement element) {
        return Value.Insert(index, element);
    }

    public virtual IImmutableList<TElement> InsertRange(int index, IEnumerable<TElement> items) {
        return Value.InsertRange(index, items);
    }

    public virtual IImmutableList<TElement> Remove(TElement value, IEqualityComparer<TElement> equalityComparer) {
        return Value.Remove(value, equalityComparer);
    }

    public virtual IImmutableList<TElement> RemoveAll(Predicate<TElement> match) {
        return Value.RemoveAll(match);
    }

    public virtual IImmutableList<TElement> RemoveRange(IEnumerable<TElement> items, IEqualityComparer<TElement> equalityComparer) {
        return Value.RemoveRange(items, equalityComparer);
    }

    public virtual IImmutableList<TElement> RemoveRange(int index, int count) {
        return Value.RemoveRange(index, count);
    }

    public new virtual IImmutableList<TElement> RemoveAt(int index) {
        return Value.RemoveAt(index);
    }

    public virtual IImmutableList<TElement> SetItem(int index, TElement value) {
        return Value.SetItem(index, value);
    }

    public virtual IImmutableList<TElement> Replace(TElement oldValue, TElement newValue, IEqualityComparer<TElement> equalityComparer) {
        return Value.Replace(oldValue, newValue, equalityComparer);
    }

    #endregion
}