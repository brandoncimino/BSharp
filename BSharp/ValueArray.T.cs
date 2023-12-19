using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using FowlFever.BSharp.Memory;

namespace FowlFever.BSharp;

/// <summary>
/// An <see cref="ImmutableArray{T}"/> that allows for <see cref="ValueType"/>-style semantics, like <see cref="op_Addition(ValueArray{T},ValueArray{T})"/>.
/// </summary>
/// <param name="asImmutableArray">the actual <see cref="ImmutableArray{T}"/> that stores this <see cref="ValueArray{T}"/>'s contents</param>
/// <typeparam name="T">the type of the elements in this array</typeparam>
public readonly struct ValueArray<T>(ImmutableArray<T> asImmutableArray) :
    IImmutableList<T>,
    IEquatable<ValueArray<T>>
#if NET7_0_OR_GREATER
    ,
    IEqualityOperators<ValueArray<T>, ValueArray<T>, bool>,
    IAdditionOperators<ValueArray<T>, ValueArray<T>, ValueArray<T>>,
    IAdditionOperators<ValueArray<T>, T, ValueArray<T>>
#endif
    where T : struct, IEquatable<T> {
    public readonly ImmutableArray<T> AsImmutableArray = asImmutableArray;
    public          ReadOnlySpan<T>   AsSpan() => AsImmutableArray.AsSpan();

    /// <inheritdoc cref="ImmutableArray{T}.IsEmpty"/>
    public bool IsEmpty => AsImmutableArray.IsEmpty;
    /// <inheritdoc cref="ImmutableArray{T}.IsDefault"/>
    public bool IsDefault => AsImmutableArray.IsDefault;
    /// <inheritdoc cref="ImmutableArray{T}.IsDefaultOrEmpty"/>
    public bool IsDefaultOrEmpty => AsImmutableArray.IsDefaultOrEmpty;
    public bool IsNotEmpty => !IsEmpty;
    /// <inheritdoc cref="ImmutableArray{T}.Length"/>
    public int Length => AsImmutableArray.Length;

    /// <inheritdoc cref="ImmutableArray{T}.Slice"/>
    public ValueArray<T> Slice(int offset, int length) => AsImmutableArray.Slice(offset, length);

    public T this[int index] => AsImmutableArray[index];

    /// <inheritdoc cref="ImmutableArray{T}.Empty"/>
    public static readonly ValueArray<T> Empty = default;

    #region Operators

    public static ValueArray<T> operator +(ValueArray<T> a, T             b) => a.Add(b);
    public static ValueArray<T> operator +(ValueArray<T> a, ValueArray<T> b) => a.AddRange(b);

    public static implicit operator ValueArray<T>(ImmutableArray<T> parts)       => new(parts);
    public static implicit operator ReadOnlySpan<T>(ValueArray<T>   parts)       => parts.AsSpan();
    public static implicit operator ValueArray<T>(T                 singleValue) => new(ImmutableArray.Create(singleValue));

    #endregion

    #region Equality

    public          bool Equals(ValueArray<T>   other) => AsSpan().SequenceEqual(other.AsSpan());
    public          bool Equals(ReadOnlySpan<T> other) => AsSpan().SequenceEqual(other);
    public override bool Equals(object?         obj)   => obj is ValueArray<T> va && Equals(va);

    public static bool operator ==(ValueArray<T> left, ValueArray<T> right) => left.Equals(right);
    public static bool operator !=(ValueArray<T> left, ValueArray<T> right) => !left.Equals(right);

    public override int GetHashCode() {
        var hashCode = new HashCode();

        foreach (var it in this) {
            hashCode.Add(it);
        }

        return hashCode.GetHashCode();
    }

    #endregion

    public override string ToString() {
        const char   prefix = '[';
        const char   suffix = ']';
        const char   joiner = ',';
        const string empty  = "[]";

        if (IsDefaultOrEmpty) {
            return empty;
        }

        switch (AsImmutableArray) {
            // check if the result has a predictable length
            case ImmutableArray<char> charArray: {
                var strLength = Length + (Length - 1) + 2;
                return string.Create(
                    strLength,
                    charArray,
                    (span, array) => {
                        span[0] = prefix;
                        var spanAfterPrefix = span[1..];
                        spanAfterPrefix[0] = array[0];
                        for (int i = 1; i < array.Length; i++) {
                            spanAfterPrefix[i * 2]     = joiner;
                            spanAfterPrefix[i * 2 + 1] = array[i];
                        }

                        span[^1] = suffix;
                    }
                );
            }
            case ImmutableArray<string> stringArray: {
                // TODO: Benchmark a `foreach` loop against `Sum(it => it.Length)`
                var strLength = 0;
                foreach (var s in stringArray) {
                    strLength += s.Length;
                }

                return string.Create(
                    strLength,
                    stringArray,
                    (span, array) => {
                        var pos = 0;
                        span.Write('[', ref pos);

                        foreach (var it in array) {
                            span.WriteJoin(it, ',', ref pos);
                        }

                        span.Write(']', ref pos);
                        span.Finish(pos);
                    }
                );
            }
        }

        var sb = new StringBuilder();
        sb.Append('[');
        sb.AppendJoin(',', this);
        sb.Append(']');
        return sb.ToString();
    }

    #region Implementation of IImmutableList<T>

    /*
     * Many of these methods contain an explicit implementation from `IImmutableList<T>` that returns `IImmutableList<T>`,
     * with an equivalent method that returns `ValueArray<T>`.
     *
     * This has 2 benefits:
     *  1. It means you can chain together `ValueArray<T>` methods.
     *  2. It avoids boxing `ValueArray<T>`s into `IImmutableList<T>`s unnecessarily.
     *
     * You can see this pattern in the implementation of `System.Collections.Immutable.ImmutableArray<T>`.
     */

    int IReadOnlyCollection<T>.Count => AsImmutableArray.Length;

    /// <inheritdoc cref="ImmutableArray{T}.Add"/>
    public ValueArray<T> Add(T value) => AsImmutableArray.Add(value);

    IImmutableList<T> IImmutableList<T>.Add(T value) => AsImmutableArray.Add(value);

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(System.Collections.Generic.IEnumerable{T})"/>
    public ValueArray<T> AddRange(IEnumerable<T> items) => AsImmutableArray.AddRange(items);

    IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items) => AsImmutableArray.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Clear"/>
    public ValueArray<T> Clear() => AsImmutableArray.Clear();

    IImmutableList<T> IImmutableList<T>.Clear() => AsImmutableArray.Clear();

    public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer = null) {
        return AsImmutableArray.IndexOf(item, index, count, equalityComparer);
    }

    /// <inheritdoc cref="ImmutableList{T}.Insert"/>
    public ValueArray<T> Insert(int index, T item) => AsImmutableArray.Insert(index, item);

    IImmutableList<T> IImmutableList<T>.Insert(int index, T item) => AsImmutableArray.Insert(index, item);

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int,System.Collections.Generic.IEnumerable{T})"/>
    public ValueArray<T> InsertRange(int index, IEnumerable<T> items) => AsImmutableArray.InsertRange(index, items);

    IImmutableList<T> IImmutableList<T>.InsertRange(int index, IEnumerable<T> items) => AsImmutableArray.InsertRange(index, items);

    public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer = null) {
        return AsImmutableArray.LastIndexOf(item, index, count, equalityComparer);
    }

    /// <inheritdoc cref="M:System.Collections.Immutable.ImmutableArray`1.Remove(`0,System.Collections.Generic.IEqualityComparer{`0})"/>
    public ValueArray<T> Remove(T value, IEqualityComparer<T>? equalityComparer = null) => AsImmutableArray.Remove(value, equalityComparer);

    IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T>? equalityComparer) => AsImmutableArray.Remove(value, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.RemoveAll"/>
    public ValueArray<T> RemoveAll(Predicate<T> match) => AsImmutableArray.RemoveAll(match);

    IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match) => AsImmutableArray.RemoveAll(match);

    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt"/>
    public ValueArray<T> RemoveAt(int index) => AsImmutableArray.RemoveAt(index);

    IImmutableList<T> IImmutableList<T>.RemoveAt(int index) => AsImmutableArray.RemoveAt(index);

    /// <inheritdoc cref="ImmutableArray{T}.RemoveRange(System.Collections.Generic.IEnumerable{T})"/>
    public ValueArray<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer = null) => AsImmutableArray.RemoveRange(items, equalityComparer);

    IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer) => AsImmutableArray.RemoveRange(items, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.RemoveRange(int,int)"/>
    public ValueArray<T> RemoveRange(int index, int count) => AsImmutableArray.RemoveRange(index, count);

    IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count) => AsImmutableArray.RemoveRange(index, count);

    /// <inheritdoc cref="ImmutableArray{T}.Replace(T,T,IEqualityComparer{T})"/>
    public ValueArray<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer = null) => AsImmutableArray.Replace(oldValue, newValue, equalityComparer);

    IImmutableList<T> IImmutableList<T>.Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer) => AsImmutableArray.Replace(oldValue, newValue, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.SetItem"/>
    public ValueArray<T> SetItem(int index, T value) => AsImmutableArray.SetItem(index, value);

    IImmutableList<T> IImmutableList<T>.SetItem(int index, T value) => AsImmutableArray.SetItem(index, value);

    /// <inheritdoc cref="ImmutableArray{T}.GetEnumerator"/>
    /// <remarks>
    /// This should avoid the allocations that would normally be incurred when a <see cref="ValueArray{T}"/> is boxed as an <see cref="IEnumerable{T}"/>.
    /// <p/>
    /// Note that using <see cref="System.Linq"/> (<i><see cref="Enumerable"/></i>) extension methods with <see cref="ValueArray"/> <i>will</i> incur a boxing cost.
    /// </remarks>
    public ImmutableArray<T>.Enumerator GetEnumerator() => AsImmutableArray.GetEnumerator();

    IEnumerator IEnumerable.      GetEnumerator() => ((IEnumerable)AsImmutableArray).GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)AsImmutableArray).GetEnumerator();

    #endregion
}