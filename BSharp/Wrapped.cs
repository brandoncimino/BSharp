using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.BSharp;

/// <summary>
/// A base <c>record</c> for wrappers around a single <typeparamref name="T"/> value.
/// </summary>
/// <param name="Value">the actual <see cref="IHas{T}.Value"/></param>
/// <inheritdoc cref="IWrap{T}"/>
/// <remarks>
/// This is the basic implementation of the <see cref="IWrap{T}"/> interface.
/// <p/>
/// <p/>
/// The goal of this class is that working with a <see cref="Wrapped{T}"/> should be as close to working with a normal <typeparamref name="T"/>
/// as possible.
/// <br/>
/// It should be fully interoperable with:
/// <ul>
/// <li>Other <see cref="Wrapped{T}"/>s <i>(already covered by being a <c>record</c> type)</i></li>
/// <li>Basic <typeparamref name="T"/> instances</li>
/// <li>Anything that <see cref="IHas{T}"/></li>
/// </ul>
/// <p/>
/// To this end, <see cref="Wrapped{T}"/> provides a simple <see cref="IEquatable{T}"/> implementation
/// and an <c>implicit operator</c> cast to <typeparamref name="T"/>.
/// <p/>
/// 📎 We don't need to be exhaustive with our operator overloads like <c>==</c> and <c>!=</c> because of the implicit conversion to <typeparamref name="T"/>.
/// However, we do have to specify <see cref="Equals(T?)"/>, <see cref="CompareTo(T?)"/>, etc. methods in order to satisfy the <see cref="IEquatable{T}"/> interfaces, etc.
/// <p/>
/// 📎 While the base <see cref="Wrapped{T}"/> class contains an implicit cast to <typeparamref name="T"/>, individual implementers must provide casts <b>FROM</b> <typeparamref name="T"/>.
/// </remarks>>
public abstract record Wrapped<T>(T Value) : IWrap<T> {
    public T Value { get; } = Value;

    #region Equality

    /// <summary>
    /// Defines how the underlying <see cref="Value"/> should be compared for equality.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="EqualityComparer{T}.Default"/>.</remarks>
    protected virtual IEqualityComparer<T?> ValueEqualityComparer { get; } = EqualityComparer<T?>.Default;

    public bool Equals(T?        other) => ValueEqualityComparer.Equals(Value, other);
    public bool Equals(IHas<T?>? other) => other != null && ValueEqualityComparer.Equals(Value, other.Value);

    public static bool operator ==(Wrapped<T?>? a, T? b) => Equals(a, b);
    public static bool operator !=(Wrapped<T?>? a, T? b) => !(a == b);

    #endregion

    #region Comparison

    /// <summary>
    /// Defines how the underlying <see cref="Value"/> should be sorted.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Comparer{T}.Default"/>.
    /// </remarks>
    public virtual IComparer<T?> ValueComparer { get; } = Comparer<T?>.Default;

    public int CompareTo(T?        other) => ValueComparer.Compare(Value, other);
    public int CompareTo(IHas<T?>? other) => ValueComparer.Compare(Value, other.GetValueOrDefault());

    #endregion

    #region Casts

    [return: NotNullIfNotNull("wrapper")]
    public static implicit operator T?(Wrapped<T>? wrapper) => wrapper == null ? default : wrapper.Value;

    #endregion
}

public static class HasExtensions {
    public static T? GetValueOrDefault<T>(this IHas<T>? self) {
        return self == null ? default : self.Value;
    }
}