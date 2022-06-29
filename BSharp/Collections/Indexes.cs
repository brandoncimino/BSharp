using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

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
public readonly partial record struct Indexes : IHas<int> {
    public           int                         Count { get; }
    private readonly Lazy<ImmutableList<int>>    _list;
    private          ImmutableList<int>          AsList           => _list.Value;
    private          IList                       AsNonGenericList => AsList;
    private readonly Lazy<ImmutableHashSet<int>> _set;
    private          ImmutableHashSet<int>       AsSet  => _set.Value;
    private          ICollection                 AsColl => AsSet;

    #region Constructors & Factories

    public Indexes() {
        Count = 0;
        _list = new Lazy<ImmutableList<int>>(() => ImmutableList<int>.Empty);
        _set  = new Lazy<ImmutableHashSet<int>>(() => ImmutableHashSet<int>.Empty);
    }

    public Indexes(int count) {
        Count = Must.BePositive(count);
        _list = new Lazy<ImmutableList<int>>(Enumerable.Range(0,    Count).ToImmutableList);
        _set  = new Lazy<ImmutableHashSet<int>>(Enumerable.Range(0, Count).ToImmutableHashSet);
    }

    public static Indexes Of<T>(ICollection<T>         source) => new(source.Count);
    public static Indexes Of<T>(IReadOnlyCollection<T> source) => new(source.Count);

    #endregion

    public IEnumerator<int> GetEnumerator()    => Enumerable.Repeat(0, Count).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()    => GetEnumerator();
    public bool             Contains(int item) => item >= 0 && item < Count;
    public bool             IsReadOnly         => true;

    private static Exception UnsupportedMethodException([CallerMemberName] string? methodName = default) => new NotSupportedException($"{nameof(Indexes)} does not support {methodName}");

    int IHas<int>.Value => Count;

    [NonNegativeValue]
    public static implicit operator int(Indexes indexes) => indexes.Count;

    public static implicit operator Indexes([NonNegativeValue] int count)   => new(count);
    public static implicit operator Indexes(string                 str)     => str.Length;
    public static implicit operator Indexes(Array                  array)   => array.Length;
    public static implicit operator Range(Indexes                  indexes) => ..indexes.Count;
}