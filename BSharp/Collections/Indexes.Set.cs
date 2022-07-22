using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct Indexes : IImmutableSet<int>, ISet<int> {
    #region Implementation of ISet<int>

    bool ISet<int>.Add(int                              item)  => throw UnsupportedMethodException();
    void ISet<int>.ExceptWith(IEnumerable<int>          other) => throw UnsupportedMethodException();
    void ISet<int>.IntersectWith(IEnumerable<int>       other) => throw UnsupportedMethodException();
    void ISet<int>.SymmetricExceptWith(IEnumerable<int> other) => throw UnsupportedMethodException();
    void ISet<int>.UnionWith(IEnumerable<int>           other) => throw UnsupportedMethodException();

    #endregion

    #region Implementation of IImmutableSet<int>

    IImmutableSet<int> IImmutableSet<int>.Clear()                                                              => AsSet.Clear();
    IImmutableSet<int> IImmutableSet<int>.Add(int                             value)                           => AsSet.Add(value);
    IImmutableSet<int> IImmutableSet<int>.Remove(int                          value)                           => AsSet.Remove(value);
    bool IImmutableSet<int>.              TryGetValue(int                     equalValue, out int actualValue) => AsSet.TryGetValue(equalValue, out actualValue);
    IImmutableSet<int> IImmutableSet<int>.Intersect(IEnumerable<int>          other) => AsSet.Intersect(other);
    IImmutableSet<int> IImmutableSet<int>.Except(IEnumerable<int>             other) => AsSet.Except(other);
    IImmutableSet<int> IImmutableSet<int>.SymmetricExcept(IEnumerable<int>    other) => AsSet.SymmetricExcept(other);
    IImmutableSet<int> IImmutableSet<int>.Union(IEnumerable<int>              other) => AsSet.Union(other);
    public bool                           SetEquals(IEnumerable<int>          other) => AsSet.SetEquals(other);
    public bool                           IsProperSubsetOf(IEnumerable<int>   other) => AsSet.IsProperSubsetOf(other);
    public bool                           IsProperSupersetOf(IEnumerable<int> other) => AsSet.IsProperSupersetOf(other);
    public bool                           IsSubsetOf(IEnumerable<int>         other) => AsSet.IsSubsetOf(other);
    public bool                           IsSupersetOf(IEnumerable<int>       other) => AsSet.IsSupersetOf(other);
    public bool                           Overlaps(IEnumerable<int>           other) => AsSet.Overlaps(other);

    #endregion

    #region Implementation of ICollection

    void ICollection.  CopyTo(Array array, int index) => AsColl.CopyTo(array, index);
    bool ICollection.  IsSynchronized => AsColl.IsSynchronized;
    object ICollection.SyncRoot       => AsColl.SyncRoot;

    #endregion
}