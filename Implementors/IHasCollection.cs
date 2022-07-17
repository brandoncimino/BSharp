using System.Collections;
using System.Diagnostics.CodeAnalysis;

using FowlFever.Implementors.NonGeneric;

namespace FowlFever.Implementors;

/// <summary>
/// Delegates implementation of <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/> to the <see cref="AsCollection"/> property.
/// </summary>
/// <typeparam name="T">the type of entries in <see cref="AsCollection"/></typeparam>
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public interface IHasCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IHasNonGenericCollection {
    protected ICollection<T> AsCollection { get; }

    #region Implementation

    void ICollection<T>.Add(T item)                       => AsCollection.Add(item);
    void ICollection<T>.Clear()                           => AsCollection.Clear();
    bool ICollection<T>.Contains(T item)                  => AsCollection.Contains(item);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => AsCollection.CopyTo(array, arrayIndex);
    bool ICollection<T>.Remove(T   item) => AsCollection.Remove(item);

    int ICollection<T>.           Count           => AsCollection.Count;
    bool ICollection<T>.          IsReadOnly      => AsCollection.IsReadOnly;
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => AsCollection.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => ((IEnumerable)AsCollection).GetEnumerator();
    int IReadOnlyCollection<T>.   Count           => AsCollection.Count;

    #endregion
}