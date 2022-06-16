using System;
using System.Collections;
using System.Collections.Generic;

namespace FowlFever.BSharp;

/// <summary>
/// Similar to <see cref="Wrapped{T}"/>, but automatically delegates <see cref="IList{T}"/> operations to the underlying <typeparamref name="TCollection"/>. 
/// </summary>
/// <param name="Value">the wrapped <see cref="ICollection{T}"/></param>
/// <typeparam name="TElement">the type of the elements in the underlying <see cref="ICollection{T}"/></typeparam>
/// <typeparam name="TCollection">the type of the underlying <see cref="ICollection{T}"/></typeparam>
public abstract record WrappedCollection<TElement, TCollection>(TCollection Value) : Wrapped<TCollection>,
                                                                                     IList<TElement>,
                                                                                     IReadOnlyList<TElement>,
                                                                                     IList
    where TCollection : IEnumerable<TElement> {
    public override    TCollection           Value            { get; } = Value;
    protected abstract IList<TElement>       AsList           { get; }
    protected abstract IList                 AsNonGenericList { get; }
    protected abstract ICollection<TElement> AsCollection     { get; }

    #region Implementation of IEnumerable

    public IEnumerator<TElement> GetEnumerator() {
        var ls = new List<int>();
        return Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)Value).GetEnumerator();
    }

    #endregion

    #region Implementation of ICollection<ELEMENT>

    public void Add(TElement item) => AsList.Add(item);

    int IList.Add(object value) => AsNonGenericList.Add(value);

    public void Clear() => AsCollection.Clear();

    bool IList.Contains(object value) => AsNonGenericList.Contains(value);

    int IList.IndexOf(object value) => AsNonGenericList.IndexOf(value);

    public void Insert(int index, object value) => AsNonGenericList.Insert(index, value);

    void IList.Remove(object value) => AsNonGenericList.Remove(value);

    public bool Contains(TElement item) => AsCollection.Contains(item);

    public void CopyTo(TElement[] array, int arrayIndex) => AsCollection.CopyTo(array, arrayIndex);

    public bool Remove(TElement item) => AsCollection.Remove(item);

    public void CopyTo(Array array, int index) => AsNonGenericList.CopyTo(array, index);

    public int         Count          => AsCollection.Count;
    public bool        IsSynchronized => AsNonGenericList.IsSynchronized;
    object ICollection.SyncRoot       => AsNonGenericList.SyncRoot;
    public bool        IsReadOnly     => AsCollection.IsReadOnly;
    object IList.this[int index] {
        get => AsNonGenericList[index];
        set => AsNonGenericList[index] = value;
    }

    #endregion

    #region Implementation of IList<ELEMENT>

    public int IndexOf(TElement item) {
        return AsList.IndexOf(item);
    }

    public void Insert(int index, TElement item) {
        AsList.Insert(index, item);
    }

    public void RemoveAt(int index) {
        AsList.RemoveAt(index);
    }

    public bool IsFixedSize => AsNonGenericList.IsFixedSize;

    public TElement this[int index] {
        get => AsList[index];
        set => AsList[index] = value;
    }

    #endregion
}