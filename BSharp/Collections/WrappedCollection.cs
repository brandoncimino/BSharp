using System;
using System.Collections;
using System.Collections.Generic;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

/// <inheritdoc cref="WrappedCollection{TElement,TCollection,TSlice}"/>
public abstract record WrappedCollection<TElement, TCollection> : WrappedCollection<TElement, TCollection, TElement>
    where TCollection : IEnumerable<TElement>
    where TElement : IEnumerable<TElement>;

/// <summary>
/// Similar to <see cref="Wrapped{T}"/>, but automatically delegates <see cref="IList{T}"/> operations to the underlying <typeparamref name="TCollection"/>. 
/// </summary>
/// <typeparam name="TElement">the type of the elements in the underlying <see cref="ICollection{T}"/></typeparam>
/// <typeparam name="TCollection">the type of the underlying <see cref="ICollection{T}"/></typeparam>
/// <typeparam name="TSlice">the type of sub-<see cref="Range"/>s of <typeparamref name="TCollection"/></typeparam>
public abstract record WrappedCollection<TElement, TCollection, TSlice> : Wrapped<TCollection>,
                                                                          IList<TElement>,
                                                                          IReadOnlyList<TElement>,
                                                                          IList
    where TCollection : IEnumerable<TElement>
    where TSlice : IEnumerable<TElement> {
    protected virtual IList<TElement>       AsList           => Value.MustBe<IList<TElement>>();
    protected virtual IList                 AsNonGenericList => Value.MustBe<IList>();
    protected virtual ICollection<TElement> AsCollection     => Value.MustBe<ICollection<TElement>>();

    protected abstract TSlice CreateSlice(IEnumerable<TElement> elements);

    public virtual TSlice this[Range range] => CreateSlice(range.EnumerateSlice(this));

    #region Implementation of IEnumerable

    public IEnumerator<TElement> GetEnumerator() => Value.GetEnumerator();
    IEnumerator IEnumerable.     GetEnumerator() => ((IEnumerable)Value).GetEnumerator();

    #endregion

    #region Implementation of ICollection<ELEMENT>

    public virtual void        Add(TElement item)                       => AsList.Add(item);
    public virtual void        Clear()                                  => AsCollection.Clear();
    public virtual bool        Contains(TElement item)                  => AsCollection.Contains(item);
    void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex) => AsCollection.CopyTo(array, arrayIndex);
    public virtual bool        Remove(TElement   item) => AsCollection.Remove(item);
    public virtual int         Count                   => AsCollection.Count;
    public virtual bool        IsReadOnly              => AsCollection.IsReadOnly;

    #endregion

    #region Non-Generic Implementations

    object? IList.this[int index] {
        get => AsNonGenericList[index];
        set => AsNonGenericList[index] = value;
    }

    bool ICollection.  IsSynchronized                         => AsNonGenericList.IsSynchronized;
    object ICollection.SyncRoot                               => AsNonGenericList.SyncRoot;
    int IList.         Add(object?      value)                => AsNonGenericList.Add(value);
    bool IList.        Contains(object? value)                => AsNonGenericList.Contains(value);
    int IList.         IndexOf(object?  value)                => AsNonGenericList.IndexOf(value);
    void IList.        Insert(int       index, object? value) => AsNonGenericList.Insert(index, value);
    void IList.        Remove(object?   value)            => AsNonGenericList.Remove(value);
    void ICollection.  CopyTo(Array     array, int index) => AsNonGenericList.CopyTo(array, index);

    #endregion

    #region Implementation of IList<ELEMENT>

    public virtual int  IndexOf(TElement item)                 => AsList.IndexOf(item);
    public virtual void Insert(int       index, TElement item) => AsList.Insert(index, item);
    public virtual void RemoveAt(int     index) => AsList.RemoveAt(index);

    public bool IsFixedSize => AsNonGenericList.IsFixedSize;

    public TElement this[int index] {
        get => AsList[index];
        set => AsList[index] = value;
    }

    #endregion

    #region Deconstructors

    public void Deconstruct(out TElement a, out TElement b)                 => (a, b) = (this[0], this[1]);
    public void Deconstruct(out TElement a, out TElement b, out TElement c) => (a, b, c) = (this[0], this[1], this[2]);

    public void Deconstruct(
        out TElement a,
        out TElement b,
        out TElement c,
        out TElement d
    ) => (a, b, c, d) = (this[0], this[1], this[2], this[3]);

    public void Deconstruct(
        out TElement a,
        out TElement b,
        out TElement c,
        out TElement d,
        out TElement e
    ) => (a, b, c, d, e) = (this[0], this[1], this[2], this[3], this[4]);

    public void Deconstruct(
        out TElement a,
        out TElement b,
        out TElement c,
        out TElement d,
        out TElement e,
        out TElement f
    ) => (a, b, c, d, e, f) = (this[0], this[1], this[2], this[3], this[4], this[5]);

    #endregion
}