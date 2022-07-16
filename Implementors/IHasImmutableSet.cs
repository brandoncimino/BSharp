using System.Collections;
using System.Collections.Immutable;

namespace Implementors;

/// <summary>
/// Delegates the implementation of <see cref="IImmutableSet{T}"/> to the <see cref="AsImmutableSet"/> property.
/// </summary>
/// <typeparam name="T">the type of the entries in <see cref="AsImmutableSet"/></typeparam>
public interface IHasImmutableSet<T> : IImmutableSet<T> {
    public IImmutableSet<T>           AsImmutableSet                                                   { get; }
    IEnumerator<T> IEnumerable<T>.    GetEnumerator()                                                  => AsImmutableSet.GetEnumerator();
    IEnumerator IEnumerable.          GetEnumerator()                                                  => ((IEnumerable)AsImmutableSet).GetEnumerator();
    int IReadOnlyCollection<T>.       Count                                                            => AsImmutableSet.Count;
    IImmutableSet<T> IImmutableSet<T>.Add(T value)                                                     => AsImmutableSet.Add(value);
    IImmutableSet<T> IImmutableSet<T>.Clear()                                                          => AsImmutableSet.Clear();
    bool IImmutableSet<T>.            Contains(T                        value)                         => AsImmutableSet.Contains(value);
    IImmutableSet<T> IImmutableSet<T>.Except(IEnumerable<T>             other)                         => AsImmutableSet.Except(other);
    IImmutableSet<T> IImmutableSet<T>.Intersect(IEnumerable<T>          other)                         => AsImmutableSet.Intersect(other);
    bool IImmutableSet<T>.            IsProperSubsetOf(IEnumerable<T>   other)                         => AsImmutableSet.IsProperSubsetOf(other);
    bool IImmutableSet<T>.            IsProperSupersetOf(IEnumerable<T> other)                         => AsImmutableSet.IsProperSupersetOf(other);
    bool IImmutableSet<T>.            IsSubsetOf(IEnumerable<T>         other)                         => AsImmutableSet.IsSubsetOf(other);
    bool IImmutableSet<T>.            IsSupersetOf(IEnumerable<T>       other)                         => AsImmutableSet.IsSupersetOf(other);
    bool IImmutableSet<T>.            Overlaps(IEnumerable<T>           other)                         => AsImmutableSet.Overlaps(other);
    IImmutableSet<T> IImmutableSet<T>.Remove(T                          value)                         => AsImmutableSet.Remove(value);
    bool IImmutableSet<T>.            SetEquals(IEnumerable<T>          other)                         => AsImmutableSet.SetEquals(other);
    IImmutableSet<T> IImmutableSet<T>.SymmetricExcept(IEnumerable<T>    other)                         => AsImmutableSet.SymmetricExcept(other);
    bool IImmutableSet<T>.            TryGetValue(T                     equalValue, out T actualValue) => AsImmutableSet.TryGetValue(equalValue, out actualValue);
    IImmutableSet<T> IImmutableSet<T>.Union(IEnumerable<T>              other) => AsImmutableSet.Union(other);
}