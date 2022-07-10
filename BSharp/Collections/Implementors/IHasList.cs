using System.Collections;
using System.Collections.Generic;

namespace FowlFever.BSharp.Collections.Implementors;

/// <summary>
/// Delegates the <see cref="IList{T}"/> implementation to the <see cref="AsList"/> property.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHasList<T> : IList<T> {
    public IList<T>               AsList                            { get; }
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
}