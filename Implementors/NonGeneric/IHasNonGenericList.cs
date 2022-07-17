using System.Collections;

namespace FowlFever.Implementors.NonGeneric;

/// <summary>
/// Delegates the implementation of <see cref="IList"/> to the <see cref="AsNonGenericList"/> property.
/// </summary>
public interface IHasNonGenericList : IList {
    protected IList AsNonGenericList { get; }

    #region Implementation

    IEnumerator IEnumerable.GetEnumerator()                => AsNonGenericList.GetEnumerator();
    void ICollection.       CopyTo(Array array, int index) => AsNonGenericList.CopyTo(array, index);
    int ICollection.        Count                                  => AsNonGenericList.Count;
    bool ICollection.       IsSynchronized                         => AsNonGenericList.IsSynchronized;
    object ICollection.     SyncRoot                               => AsNonGenericList.SyncRoot;
    int IList.              Add(object? value)                     => AsNonGenericList.Add(value);
    void IList.             Clear()                                => AsNonGenericList.Clear();
    bool IList.             Contains(object? value)                => AsNonGenericList.Contains(value);
    int IList.              IndexOf(object?  value)                => AsNonGenericList.IndexOf(value);
    void IList.             Insert(int       index, object? value) => AsNonGenericList.Insert(index, value);
    void IList.             Remove(object?   value) => AsNonGenericList.Remove(value);
    void IList.             RemoveAt(int     index) => AsNonGenericList.RemoveAt(index);
    bool IList.             IsFixedSize             => AsNonGenericList.IsFixedSize;
    bool IList.             IsReadOnly              => AsNonGenericList.IsReadOnly;
    object? IList.this[int index] {
        get => AsNonGenericList[index];
        set => AsNonGenericList[index] = value;
    }

    #endregion
}