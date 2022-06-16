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
public readonly partial record struct Indexes(int Count) : IHas<int>, ICollection {
    #region Implementations

    public IEnumerator<int> GetEnumerator()    => Enumerable.Repeat(0, Count).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()    => GetEnumerator();
    public bool             Contains(int item) => item >= 0 && item < Count;
    public bool             IsReadOnly         => true;

    public int Count { get; } = Must.BePositive(Count);

    #region Unuspported methods from ICollection<int>

    private static Exception UnsupportedMethodException([CallerMemberName] string? methodName = default) => new NotSupportedException($"{nameof(Indexes)} does not support {methodName}");

    void ICollection<int>.Add(int item)                       => throw UnsupportedMethodException();
    void ICollection<int>.Clear()                             => throw UnsupportedMethodException();
    void ICollection<int>.CopyTo(int[] array, int arrayIndex) => throw UnsupportedMethodException();
    bool ICollection<int>.Remove(int   item) => throw UnsupportedMethodException();

    #endregion

    #region Implementation of IHas<out int>

    int IHas<int>.Value => Count;

    #endregion

    #endregion

    #region Implementation of IReadOnlyList<out int>

    public int this[int index] => Must.BeIndexOf(index, this);

    #endregion
}