using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Implementors;

/// <summary>
/// Delegates the implementation of <see cref="IList{T}"/> to the <see cref="AsList"/> property.
/// </summary>
/// <typeparam name="T">the type of the entries in <see cref="IList{T}"/></typeparam>
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public interface IHasList<T> : IList<T>, IReadOnlyList<T> {
    protected IList<T> AsList { get; }

    #region Implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()                   => AsList.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator()                   => ((IEnumerable)AsList).GetEnumerator();
    void ICollection<T>.          Add(T item)                       => AsList.Add(item);
    void ICollection<T>.          Clear()                           => AsList.Clear();
    bool ICollection<T>.          Contains(T item)                  => AsList.Contains(item);
    void ICollection<T>.          CopyTo(T[] array, int arrayIndex) => AsList.CopyTo(array, arrayIndex);
    bool ICollection<T>.          Remove(T   item) => AsList.Remove(item);

    int ICollection<T>. Count      => AsList.Count;
    bool ICollection<T>.IsReadOnly => AsList.IsReadOnly;

    int IList<T>. IndexOf(T    item)          => AsList.IndexOf(item);
    void IList<T>.Insert(int   index, T item) => AsList.Insert(index, item);
    void IList<T>.RemoveAt(int index) => AsList.RemoveAt(index);

    T IList<T>.this[int index] {
        get => AsList[index];
        set => AsList[index] = value;
    }
    int IReadOnlyCollection<T>.Count => AsList.Count;
    T IReadOnlyList<T>.this[int index] => AsList[index];

    #endregion
}