using System;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Attributes;

namespace FowlFever.BSharp.Sugar;

/// <summary>
/// Provides implicit casting to and from <see cref="Lazy{T}"/>. 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Intended for use as a parameter, allowing a single method to accept <typeparamref name="T"/>, <see cref="Lazy{T}"/>, <see cref="Func{TResult}"/>, etc.
/// </remarks>
[Experimental]
public readonly ref struct LazyShim<T> {
    [MaybeNull] private readonly Lazy<T> _lazy;
    public                       Lazy<T> Lazy => _lazy ?? throw new InvalidOperationException($"{nameof(LazyShim<T>)} hasn't been initialized!");

    public bool IsDefault => _lazy == null;

    public LazyShim(Lazy<T> source) {
        _lazy = source;
    }

    public LazyShim(T       value) : this(new Lazy<T>(value)) { }
    public LazyShim(Func<T> supplier) : this(new Lazy<T>(supplier)) { }

    public static implicit operator LazyShim<T>(Lazy<T> value)    => new(value);
    public static implicit operator LazyShim<T>(T       value)    => new(value);
    public static implicit operator LazyShim<T>(Func<T> supplier) => new(supplier);
    public static implicit operator Lazy<T>(LazyShim<T> shim)     => shim.Lazy;

    public LazyShim<T2> AndThen<T2>(Func<T, T2> transformation) {
        return Lazy.AndThen(transformation);
    }
}