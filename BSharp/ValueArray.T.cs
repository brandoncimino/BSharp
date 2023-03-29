using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.Implementors;

namespace FowlFever.BSharp;

/// <summary>
/// An <see cref="ImmutableArray{T}"/> that allows for <see cref="ValueType"/>-style semantics, like <see cref="op_Addition(ValueArray{T},ValueArray{T})"/>.
/// </summary>
/// <param name="AsImmutableArray">the actual <see cref="ImmutableArray{T}"/> that stores this <see cref="ValueArray{T}"/>'s contents</param>
/// <typeparam name="T">the type of the elements in this array</typeparam>
public readonly partial record struct ValueArray<T>(ImmutableArray<T> AsImmutableArray) :
    IHasImmutableArray<T>,
    IAsReadOnlySpan<T> where T : struct, IEquatable<T> {
    private readonly ImmutableArray<T> _parts = AsImmutableArray;
    public           ImmutableArray<T> AsImmutableArray => _parts.IsDefault ? ImmutableArray<T>.Empty : _parts;
    public           ReadOnlySpan<T>   AsSpan()         => AsImmutableArray.AsSpan();

    public bool IsEmpty    => AsImmutableArray.IsDefaultOrEmpty;
    public bool IsNotEmpty => !IsEmpty;
    public int  Length     => AsImmutableArray.Length;

    public ValueArray<T> Slice(int offset, int length) {
        Must.ContainRange(Length, offset, length);
        return ImmutableArray.Create(AsImmutableArray, offset, length);
    }

    public T this[int index] => AsImmutableArray[index];

    public static readonly ValueArray<T> Empty = default;

    /// <summary>
    /// Enumerates through the elements of this <see cref="ValueArray{T}"/>.
    /// </summary>
    /// <returns>a new <see cref="ImmutableArray{T}.Enumerator"/></returns>
    /// <remarks>
    /// This should avoid the allocations that would normally be incurred when a <see cref="ValueArray{T}"/> is boxed as an <see cref="IEnumerable{T}"/>.
    /// <p/>
    /// Note that using <see cref="System.Linq"/> (<i><see cref="Enumerable"/></i>) extension methods with <see cref="ValueArray"/> <i>will</i> incur a boxing cost.
    /// </remarks>
    public ImmutableArray<T>.Enumerator GetEnumerator() => AsImmutableArray.GetEnumerator();

    public ReadOnlySpan<T> AsReadOnlySpan() {
        return this.AsSpan();
    }
}