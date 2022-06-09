using System;

namespace FowlFever.BSharp;

/// <summary>
/// Contains either an <see cref="_explicit"/> <b>or</b> a <see cref="_lazy"/> value of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">the type of the actual value</typeparam>
public record Supplied<T> : IHas<T> {
    /// <summary>
    /// <c>default</c> with a <c>!</c>-suppressed nullability is used to prevent the need for <see cref="_explicit"/> to become a <see cref="Nullable{T}"/> when <typeparamref name="T"/> is a <see cref="ValueType"/>. 
    /// </summary>
    private readonly T _explicit = default!;
    private readonly Lazy<T>? _lazy;
    public           T        Value => _lazy == null ? _explicit : _lazy.Value;

    public Supplied(T       value) => _explicit = value;
    public Supplied(Lazy<T> value) => _lazy = value;
    public Supplied(Func<T> supplier) : this(new Lazy<T>(supplier)) { }
    public Supplied(IHas<T> wrapper) : this(new Lazy<T>(wrapper.Get)) { }

    public static implicit operator Supplied<T>(T          value)    => new(value);
    public static implicit operator Supplied<T>(Lazy<T>    lazy)     => new(lazy);
    public static implicit operator Supplied<T>(Func<T>    supplier) => new(supplier);
    public static implicit operator Supplied<T>(Wrapped<T> wrapped)  => new((IHas<T>)wrapped);
    public static implicit operator T(Supplied<T>          supplied) => supplied.Value;
}