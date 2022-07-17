using System.Collections;

namespace FowlFever.Implementors.NonGeneric;

/// <summary>
/// Delegates the implementation of <see cref="ICollection"/> to the <see cref="AsNonGenericCollection"/> property.
/// </summary>
public interface IHasNonGenericCollection : ICollection {
    protected ICollection   AsNonGenericCollection         { get; }
    IEnumerator IEnumerable.GetEnumerator()                => AsNonGenericCollection.GetEnumerator();
    void ICollection.       CopyTo(Array array, int index) => AsNonGenericCollection.CopyTo(array, index);
    int ICollection.        Count          => AsNonGenericCollection.Count;
    bool ICollection.       IsSynchronized => AsNonGenericCollection.IsSynchronized;
    object ICollection.     SyncRoot       => AsNonGenericCollection.SyncRoot;
}