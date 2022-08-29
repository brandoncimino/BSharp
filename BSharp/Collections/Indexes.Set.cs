using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

    IImmutableSet<int> IImmutableSet<int>.Clear() => default(Indexes);

    IImmutableSet<int> IImmutableSet<int>.Add(int value) {
        if (value == Count + 1) {
            return new Indexes(Count + 1);
        }

        return this.Append(value).ToImmutableHashSet();
    }

    IImmutableSet<int> IImmutableSet<int>.Remove(int value) {
        RequireIndex(value);
        var (before, after) = this.DropRange(value, 1);
        return before.Concat(after).ToImmutableHashSet();
    }

    bool IImmutableSet<int>.TryGetValue(int equalValue, out int actualValue) {
        actualValue = equalValue;
        return Contains(equalValue);
    }

    IImmutableSet<int> IImmutableSet<int>.Intersect(IEnumerable<int> other) {
        return other switch {
            ImmutableHashSet<int> hs   => hs.Intersect(this),
            ImmutableSortedSet<int> ss => ss.Intersect(this),
            Indexes ind                => Intersect(ind),
            _                          => this.Intersect(other).ToImmutableHashSet(),
        };
    }

    IImmutableSet<int> IImmutableSet<int>.Except(IEnumerable<int> other) {
        return other switch {
            ImmutableHashSet<int> hs   => hs.Except(this),
            ImmutableSortedSet<int> ss => ss.Except(this),
            Indexes ind                => Except(ind),
            _                          => this.Except(other).ToImmutableHashSet(),
        };
    }

    IImmutableSet<int> IImmutableSet<int>.SymmetricExcept(IEnumerable<int> other) {
        return other switch {
            ImmutableHashSet<int> hs   => hs.SymmetricExcept(this),
            ImmutableSortedSet<int> ss => ss.SymmetricExcept(this),
            Indexes ind                => SymmetricExcept(ind),
            _                          => this.Except(other).ToImmutableHashSet()
        };
    }

    IImmutableSet<int> IImmutableSet<int>.Union(IEnumerable<int> other) {
        return other switch {
            ImmutableHashSet<int> hs   => hs.Union(this),
            ImmutableSortedSet<int> ss => ss.Union(this),
            Indexes ind                => Count < ind.Count ? this : ind,
            _                          => this.Union(other).ToImmutableHashSet()
        };
    }

    public bool SetEquals(IEnumerable<int> other) {
        return other switch {
            Indexes ind  => Count == ind.Count,
            IntRange rng => rng.Start == 0 && rng.Length == Count,
            _            => this.ToImmutableHashSet().SetEquals(other)
        };
    }

    public bool IsProperSubsetOf(IEnumerable<int> other) {
        return other switch {
            Indexes ind => Count < ind.Count,
            _           => this.ToImmutableHashSet().IsProperSubsetOf(other)
        };
    }

    public bool IsProperSupersetOf(IEnumerable<int> other) {
        return other switch {
            Indexes ind => Count > ind.Count,
            _           => this.ToImmutableHashSet().IsProperSupersetOf(other)
        };
    }

    public bool IsSubsetOf(IEnumerable<int> other) {
        return other switch {
            Indexes ind  => Count <= ind.Count,
            IntRange rng => 0 >= rng.Start && Count <= rng.Length,
            _            => this.ToImmutableHashSet().IsSubsetOf(other)
        };
    }

    public bool IsSupersetOf(IEnumerable<int> other) {
        return other switch {
            Indexes ind  => Count >= ind.Count,
            IntRange rng => 0 <= rng.Start && 0 >= rng.Length,
            _            => this.ToImmutableHashSet().IsSubsetOf(other)
        };
    }

    public bool Overlaps(IEnumerable<int> other) {
        if (Count == 0) {
            return false;
        }

        return other switch {
            Indexes ind  => ind.Count != 0,
            IntRange rng => rng.Length != 0 && (rng.Start <= Count || rng.End > 0),
            _            => this.ToImmutableHashSet().Overlaps(other)
        };
    }

    #endregion

    #region Implementation of ICollection

    /// <inheritdoc cref="P:System.Collections.Immutable.ImmutableList`1.System#Collections#ICollection#IsSynchronized"/>
    bool ICollection.IsSynchronized => true;

    /// <inheritdoc cref="P:System.Collections.Immutable.ImmutableList`1.System#Collections#ICollection#SyncRoot"/>
    object ICollection.SyncRoot => this;

    #endregion
}