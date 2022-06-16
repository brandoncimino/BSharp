using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// Represents the indexes of an <see cref="ICollection"/>.
/// </summary>
/// <remarks>
/// The purpose of <see cref="Indexes"/> is to support <see cref="ICollection"/>- and <see cref="IEnumerable{T}"/>-like operations on an <see cref="int"/>,
/// and to allow interoperability with <see cref="IEnumerable{T}"/>-based APIs when we don't want to bother with an actual collection.
/// </remarks>
public readonly partial record struct Indexes(int Count) : IHas<int> {
    #region Implementations

    public IEnumerator<int> GetEnumerator()    => Enumerable.Repeat(0, Count).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()    => GetEnumerator();
    public bool             Contains(int item) => item >= 0 && item < Count;
    public bool             IsReadOnly         => true;

    public int Count { get; } = Must.BePositive(Count);

    private static Exception UnsupportedMethodException([CallerMemberName] string? methodName = default) => new NotSupportedException($"{nameof(Indexes)} does not support {methodName}");

    #region Implementation of IHas<out int>

    int IHas<int>.Value => Count;

    #endregion

    #endregion
}