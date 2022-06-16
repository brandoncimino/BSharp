using System.Collections;
using System.Collections.Generic;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp;

/// <summary>
/// Similar to <see cref="Wrapped{T}"/>, but automatically delegates <see cref="IList{T}"/> operations to the underlying <typeparamref name="TCollection"/>. 
/// </summary>
/// <param name="Value">the wrapped <see cref="ICollection{T}"/></param>
/// <typeparam name="TElement">the type of the elements in the underlying <see cref="ICollection{T}"/></typeparam>
/// <typeparam name="TCollection">the type of the underlying <see cref="ICollection{T}"/></typeparam>
public abstract record WrappedCollection<TElement, TCollection>(TCollection Value) : Wrapped<TCollection>,
                                                                                     IList<TElement>,
                                                                                     IReadOnlyList<TElement>
    where TCollection : ICollection<TElement> {
    public override TCollection      Value { get; } = Value;
    private         IList<TElement>? _asList;
    private         IList<TElement>  AsList => _asList ??= Value.AsList();

    #region Implementation of IEnumerable

    public IEnumerator<TElement> GetEnumerator() {
        return Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)Value).GetEnumerator();
    }

    #endregion

    #region Implementation of ICollection<ELEMENT>

    public void Add(TElement item) {
        Value.Add(item);
    }

    public void Clear() {
        Value.Clear();
    }

    public bool Contains(TElement item) {
        return Value.Contains(item);
    }

    public void CopyTo(TElement[] array, int arrayIndex) {
        Value.CopyTo(array, arrayIndex);
    }

    public bool Remove(TElement item) {
        return Value.Remove(item);
    }

    public int  Count      => Value.Count;
    public bool IsReadOnly => Value.IsReadOnly;

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

    public TElement this[int index] {
        get => AsList[index];
        set => AsList[index] = value;
    }

    #endregion
}