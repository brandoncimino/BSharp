using System.Collections;

namespace FowlFever.Implementors.NonGeneric;

/// <summary>
/// Delegates implementation of <see cref="IDictionary"/> to the <see cref="AsNonGenericDictionary"/> property.
/// </summary>
public interface IHasNonGenericDictionary : IDictionary {
    protected IDictionary AsNonGenericDictionary { get; }

    #region Implementation

    void IDictionary.                 Add(object key, object? value) => AsNonGenericDictionary.Add(key, value);
    void IDictionary.                 Clear()              => AsNonGenericDictionary.Clear();
    bool IDictionary.                 Contains(object key) => AsNonGenericDictionary.Contains(key);
    IDictionaryEnumerator IDictionary.GetEnumerator()      => AsNonGenericDictionary.GetEnumerator();
    void IDictionary.                 Remove(object key)   => AsNonGenericDictionary.Remove(key);
    bool IDictionary.                 IsFixedSize          => AsNonGenericDictionary.IsFixedSize;
    bool IDictionary.                 IsReadOnly           => AsNonGenericDictionary.IsReadOnly;
    object? IDictionary.this[object key] {
        get => AsNonGenericDictionary[key];
        set => AsNonGenericDictionary[key] = value;
    }
    ICollection IDictionary.Keys                           => AsNonGenericDictionary.Keys;
    ICollection IDictionary.Values                         => AsNonGenericDictionary.Values;
    IEnumerator IEnumerable.GetEnumerator()                => ((IEnumerable)AsNonGenericDictionary).GetEnumerator();
    void ICollection.       CopyTo(Array array, int index) => AsNonGenericDictionary.CopyTo(array, index);
    int ICollection.        Count          => AsNonGenericDictionary.Count;
    bool ICollection.       IsSynchronized => AsNonGenericDictionary.IsSynchronized;
    object ICollection.     SyncRoot       => AsNonGenericDictionary.SyncRoot;

    #endregion
}