using System;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp;

/// <summary>
/// Contains either an <see cref="_explicit"/> <b>or</b> a <see cref="_lazy"/> value of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">the type of the actual value</typeparam>
public record Supplied<T> : Wrapped<T>
    where T : notnull {
    private enum SupplierStyle { Default, Explicit, Lazy }
    /// <summary>
    /// <c>default</c> with a <c>!</c>-suppressed nullability is used to prevent the need for <see cref="_explicit"/> to become a <see cref="Nullable{T}"/> when <typeparamref name="T"/> is a <see cref="ValueType"/>. 
    /// </summary>
    private readonly T _explicit = default!;
    private readonly Lazy<T>       _lazy = default!;
    private readonly SupplierStyle Style;
    public override T Value => Style switch {
        SupplierStyle.Default  => Must.NotBeNull(default(T)),
        SupplierStyle.Explicit => _explicit,
        SupplierStyle.Lazy     => _lazy.Value,
        _                      => throw BEnum.UnhandledSwitch(Style),
    };

    private Supplied() {
        Style = SupplierStyle.Default;
    }

    public Supplied(T value) {
        Style     = SupplierStyle.Explicit;
        _explicit = value;
    }

    public Supplied(Lazy<T>? value) {
        if (value == null) {
            Style = SupplierStyle.Default;
            return;
        }

        Style = SupplierStyle.Lazy;
        _lazy = value;
    }

    private Supplied(Func<T>? supplier) : this(supplier?.Lazily()) { }

    public static implicit operator Supplied<T>(T        value)    => new(value);
    public static implicit operator Supplied<T>(Lazy<T>? lazy)     => new(lazy);
    public static implicit operator Supplied<T>(Func<T>? supplier) => new(supplier);
    public static implicit operator T(Supplied<T>        supplied) => supplied.Value;
}