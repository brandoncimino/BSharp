using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

/// <inheritdoc cref="WrappedImmutableCollection{TElement,TCollection,TSlice}"/>
public abstract record WrappedImmutableCollection<TElement, TCollection> : WrappedImmutableCollection<TElement, TCollection, TCollection>
    where TCollection : IReadOnlyCollection<TElement>;

/// <summary>
/// Similar to <see cref="WrappedCollection{TElement,TCollection}"/> but also delegates to <see cref="IImmutableList{T}"/> methods.
/// </summary>
/// <inheritdoc cref="WrappedCollection{TElement,TCollection}"/>
public abstract record WrappedImmutableCollection<TElement, TCollection, TSlice> : WrappedCollection<TElement, TCollection, TSlice>, IImmutableList<TElement>
    where TCollection : IReadOnlyCollection<TElement>
    where TSlice : IEnumerable<TElement> {
    /// <summary>
    /// A representation of the underlying <see cref="Wrapped{TCollection}.Value"/> as an <see cref="IImmutableList{T}"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">if the <see cref="Wrapped{T}.Value"/> isn't an <see cref="IImmutableList{T}"/></exception>
    protected virtual IImmutableList<TElement> AsImmutableList => Value.MustBe<IImmutableList<TElement>>();

    #region Implementation of IImmutableList<T>

    public new virtual IImmutableList<TElement> Clear()                                                                                                     => AsImmutableList.Clear();
    public virtual     int                      IndexOf(TElement                  item, int index, int count, IEqualityComparer<TElement> equalityComparer) => AsImmutableList.IndexOf(item, index, count, equalityComparer);
    public virtual     int                      LastIndexOf(TElement              item, int index, int count, IEqualityComparer<TElement> equalityComparer) => AsImmutableList.LastIndexOf(item, index, count, equalityComparer);
    public new virtual IImmutableList<TElement> Add(TElement                      value)                                               => AsImmutableList.Add(value);
    public virtual     IImmutableList<TElement> AddRange(IEnumerable<TElement>    items)                                               => AsImmutableList.AddRange(items);
    public new virtual IImmutableList<TElement> Insert(int                        index, TElement                    element)          => AsImmutableList.Insert(index, element);
    public virtual     IImmutableList<TElement> InsertRange(int                   index, IEnumerable<TElement>       items)            => AsImmutableList.InsertRange(index, items);
    public virtual     IImmutableList<TElement> Remove(TElement                   value, IEqualityComparer<TElement> equalityComparer) => AsImmutableList.Remove(value, equalityComparer);
    public virtual     IImmutableList<TElement> RemoveAll(Predicate<TElement>     match)                                               => AsImmutableList.RemoveAll(match);
    public virtual     IImmutableList<TElement> RemoveRange(IEnumerable<TElement> items, IEqualityComparer<TElement> equalityComparer) => AsImmutableList.RemoveRange(items, equalityComparer);
    public virtual     IImmutableList<TElement> RemoveRange(int                   index, int                         count)            => AsImmutableList.RemoveRange(index, count);
    public new virtual IImmutableList<TElement> RemoveAt(int                      index)                                                                     => AsImmutableList.RemoveAt(index);
    public virtual     IImmutableList<TElement> SetItem(int                       index,    TElement value)                                                  => AsImmutableList.SetItem(index, value);
    public virtual     IImmutableList<TElement> Replace(TElement                  oldValue, TElement newValue, IEqualityComparer<TElement> equalityComparer) => AsImmutableList.Replace(oldValue, newValue, equalityComparer);

    #endregion
}

/// <summary>
/// A slightly stricter version of <see cref="WrappedImmutableCollection{TElement,TCollection}"/> that:
/// <ul>
/// <li>Makes <see cref="Value"/> <c>sealed</c></li>
/// <li>Requires that <see cref="TCollection"/> extends all of the requisite interfaces</li>
/// <li>Requires the <see cref="Value"/> passed to the primary <c>record</c> constructor</li>
/// </ul>
/// </summary>
/// <inheritdoc cref="WrappedImmutableCollection{TElement,TCollection}"/>
public abstract record StrictImmutableCollection<TElement, TCollection>(TCollection Value) : WrappedImmutableCollection<TElement, TCollection>
    where TCollection :
    IImmutableList<TElement>,
    IList<TElement>,
    ICollection<TElement>,
    IList,
    IReadOnlyCollection<TElement>,
    IReadOnlyList<TElement>,
    ICollection {
    public sealed override TCollection Value { get; } = Value;
}