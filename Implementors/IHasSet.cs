using System.Collections;

namespace Implementors;

/// <summary>
/// Delegates the implementation of <see cref="ISet{T}"/> to the <see cref="AsSet"/> property.
/// </summary>
/// <typeparam name="T">the type of the entries in <see cref="ISet{T}"/></typeparam>
public interface IHasSet<T> : ISet<T> {
    protected ISet<T> AsSet { get; }

    #region Implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()                           => AsSet.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator()                           => ((IEnumerable)AsSet).GetEnumerator();
    void ICollection<T>.          Add(T                              item)  => AsSet.Add(item);
    void ISet<T>.                 ExceptWith(IEnumerable<T>          other) => AsSet.ExceptWith(other);
    void ISet<T>.                 IntersectWith(IEnumerable<T>       other) => AsSet.IntersectWith(other);
    bool ISet<T>.                 IsProperSubsetOf(IEnumerable<T>    other) => AsSet.IsProperSubsetOf(other);
    bool ISet<T>.                 IsProperSupersetOf(IEnumerable<T>  other) => AsSet.IsProperSupersetOf(other);
    bool ISet<T>.                 IsSubsetOf(IEnumerable<T>          other) => AsSet.IsSubsetOf(other);
    bool ISet<T>.                 IsSupersetOf(IEnumerable<T>        other) => AsSet.IsSupersetOf(other);
    bool ISet<T>.                 Overlaps(IEnumerable<T>            other) => AsSet.Overlaps(other);
    bool ISet<T>.                 SetEquals(IEnumerable<T>           other) => AsSet.SetEquals(other);
    void ISet<T>.                 SymmetricExceptWith(IEnumerable<T> other) => AsSet.SymmetricExceptWith(other);
    void ISet<T>.                 UnionWith(IEnumerable<T>           other) => AsSet.UnionWith(other);
    bool ISet<T>.                 Add(T                              item)  => AsSet.Add(item);
    void ICollection<T>.          Clear()                                   => AsSet.Clear();
    bool ICollection<T>.          Contains(T item)                          => AsSet.Contains(item);
    void ICollection<T>.          CopyTo(T[] array, int arrayIndex)         => AsSet.CopyTo(array, arrayIndex);
    bool ICollection<T>.          Remove(T   item) => AsSet.Remove(item);
    int ICollection<T>.           Count            => AsSet.Count;
    bool ICollection<T>.          IsReadOnly       => AsSet.IsReadOnly;

    #endregion
}