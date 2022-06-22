using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// A <i>(certainly misguided)</i> class that can be used to coalesce zero, one, or more <typeparamref name="T"/> entries
/// </summary>
/// <param name="Value">the underlying <see cref="IEnumerable{T}"/></param>
/// <typeparam name="T">the type of entries in <see cref="Value"/></typeparam>
public readonly record struct EnumerableShim<T>(IEnumerable<T>? Value) : IEnumerable<T>, IHas<IEnumerable<T>> {
    public IEnumerable<T>   Value           { get; } = Value.OrEmpty();
    public IEnumerator<T>   GetEnumerator() => Value.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public EnumerableShim() : this(Enumerable.Empty<T>()) { }
    public EnumerableShim(T?                    value) : this(value.WrapInEnumerable()) { }
    public EnumerableShim(IEnumerable<IHas<T>>? values) : this(values?.Select(it => it.Value)) { }

    public static implicit operator EnumerableShim<T>(T?                         value)  => new(value);
    public static implicit operator EnumerableShim<T>(T[]?                       values) => new(values);
    public static implicit operator EnumerableShim<T>(IHas<T>[]?                 values) => new(values);
    public static implicit operator EnumerableShim<T>(List<T>?                   values) => new(values);
    public static implicit operator EnumerableShim<T>(List<IHas<T>>?             values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableArray<T>          values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableArray<T>?         values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableArray<IHas<T>>?   values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableList<T>?          values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableList<IHas<T>>?    values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableHashSet<T>?       values) => new(values);
    public static implicit operator EnumerableShim<T>(ImmutableHashSet<IHas<T>>? values) => new(values);
    public static implicit operator EnumerableShim<T>(HashSet<T>?                values) => new(values);
    public static implicit operator EnumerableShim<T>(HashSet<IHas<T>>?          values) => new(values);
    public static implicit operator EnumerableShim<T>(Wrapped<T>?                value)  => new(value.GetValueOrDefault());
    public static implicit operator EnumerableShim<T>(Wrapped<IHas<T>>?          value)  => new(value.GetValueOrDefault().GetValueOrDefault());
}

public static class ShimExtensions {
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to an <see cref="EnumerableShim{T}"/>.
    /// </summary>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <typeparam name="T">the type of the entries in this <see cref="IEnumerable{T}"/></typeparam>
    /// <returns>an equivalent <see cref="EnumerableShim{T}"/> to this <see cref="IEnumerable{T}"/></returns>
    public static EnumerableShim<T> Shim<T>(this IEnumerable<T>? source) => source as EnumerableShim<T>? ?? new EnumerableShim<T>(source);
}