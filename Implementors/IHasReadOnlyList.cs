using System.Collections;

namespace FowlFever.Implementors;

/// <inheritdoc/>
public interface IHasReadOnlyList<out T> : IReadOnlyList<T> {
    protected IReadOnlyList<T>    AsReadOnlyList  { get; }
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => AsReadOnlyList.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => ((IEnumerable)AsReadOnlyList).GetEnumerator();
    int IReadOnlyCollection<T>.   Count           => AsReadOnlyList.Count;
    T IReadOnlyList<T>.this[int index] => AsReadOnlyList[index];
}