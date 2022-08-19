using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// Represents the indexes of an <see cref="ICollection"/>.
/// </summary>
/// <remarks>
/// The purpose of <see cref="Indexes"/> is to support <see cref="ICollection"/>- and <see cref="IEnumerable{T}"/>-like operations on an <see cref="int"/>,
/// and to allow interoperability with <see cref="IEnumerable{T}"/>-based APIs when we don't want to bother with an actual collection.
/// </remarks>
public readonly partial record struct Indexes : IRange<int> {
    /// <remarks>
    /// I tend to prefer "Length", which is what newer collections like <see cref="Span{T}"/> use, but most of the interfaces implemented by this class use <see cref="ICollection.Count"/>
    /// as derived from <see cref="ICollection"/>. 
    /// </remarks>
    [NonNegativeValue]
    public int Count { get; }

    public Indexes([NonNegativeValue] int count) {
        Count = Must.BePositive(count);
    }

    public static Indexes Of(int count) => new(count);

    public IEnumerator<int> GetEnumerator() => Enumerable.Repeat(0, Count).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region Contains

    public bool Contains(int                  item)  => item >= 0 && item < Count;
    public bool Contains(Index                index) => Contains(index.GetOffset(Count));
    public bool Contains(Range                range) => Contains(range.Start) && Contains(range.End);
    public bool Contains(IntRange             range) => Contains(range.Start) && Contains(range.End);
    public bool Contains((int start, int end) range) => Contains(range.start) && Contains(range.end);

    #endregion

    #region Casts

    [NonNegativeValue] public static implicit operator int(Indexes                    indexes) => indexes.Count;
    public static implicit operator                    Indexes([NonNegativeValue] int count)   => new(count);
    public static implicit operator                    IntRange(Indexes               indexes) => new(0, indexes);

    #endregion

    #region Set-like operations with other Indexes and with IntRange

    public Indexes Intersect(Indexes other) => Union(other);

    public IntRange Except(Indexes other) {
        var diff = Count - other.Count;
        return diff switch {
            <= 0 => default,
            > 0  => new IntRange((Count, other.Count))
        };
    }

    public IntRange SymmetricExcept(Indexes other) {
        var diff = Count - other.Count;
        return diff switch {
            < 0 => new IntRange((Count, other.Count)),
            0   => this,
            > 0 => new IntRange((other.Count, Count)),
        };
    }

    public Indexes Union(Indexes other) => Count < other.Count ? this : other;

    #endregion

    #region Exceptions

    private static Exception UnsupportedMethodException([CallerMemberName] string? methodName = default) => new NotSupportedException($"🙅 {nameof(Indexes)} does not support {methodName}!");

    private int RequireIndex(int index) => Contains(index) ? index : throw new IndexOutOfRangeException($"🙅 {nameof(Indexes)}({Count}) does not contain [{index}]!");

    #endregion

    public MinBound<int>? Min => new(0, Clusivity.Inclusive);
    public MaxBound<int>? Max => new(Count, Clusivity.Exclusive);
}