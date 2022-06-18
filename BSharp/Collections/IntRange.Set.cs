using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct IntRange : IImmutableSet<int>, ISet<int> {
    bool ISet<int>.Add(int                        item)  => throw Reject.ReadOnly();
    void ISet<int>.ExceptWith(IEnumerable<int>    other) => throw Reject.ReadOnly();
    void ISet<int>.IntersectWith(IEnumerable<int> other) => throw Reject.ReadOnly();

    public bool IsProperSubsetOf(IEnumerable<int>   other) => AsSet.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<int> other) => AsSet.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<int>         other) => AsSet.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<int>       other) => AsSet.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<int>           other) => AsSet.Overlaps(other);
    public bool SetEquals(IEnumerable<int>          other) => AsSet.SetEquals(other);

    void ISet<int>.SymmetricExceptWith(IEnumerable<int> other) => throw Reject.ReadOnly();
    void ISet<int>.UnionWith(IEnumerable<int>           other) => throw Reject.ReadOnly();

    IImmutableSet<int> IImmutableSet<int>.Clear()           => AsSet.Clear();
    IImmutableSet<int> IImmutableSet<int>.Add(int    value) => AsSet.Add(value);
    IImmutableSet<int> IImmutableSet<int>.Remove(int value) => AsSet.Remove(value);

    public bool               TryGetValue(int                  equalValue, out int actualValue) => AsSet.TryGetValue(equalValue, out actualValue);
    public IImmutableSet<int> Intersect(IEnumerable<int>       other) => AsSet.Intersect(other);
    public IImmutableSet<int> Except(IEnumerable<int>          other) => AsSet.Except(other);
    public IImmutableSet<int> SymmetricExcept(IEnumerable<int> other) => AsSet.SymmetricExcept(other);
    public IImmutableSet<int> Union(IEnumerable<int>           other) => AsSet.Union(other);
};