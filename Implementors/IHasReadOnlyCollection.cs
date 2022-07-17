using System.Collections;

namespace FowlFever.Implementors;

/// <inheritdoc/>
public interface IHasReadOnlyCollection<out T> : IReadOnlyCollection<T> {
    protected IReadOnlyCollection<T> AsReadOnlyCollection { get; }
    int IReadOnlyCollection<T>.      Count                => AsReadOnlyCollection.Count;
    IEnumerator<T> IEnumerable<T>.   GetEnumerator()      => AsReadOnlyCollection.GetEnumerator();
    IEnumerator IEnumerable.         GetEnumerator()      => ((IEnumerable)AsReadOnlyCollection).GetEnumerator();
}