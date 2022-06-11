using System;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;

namespace FowlFever.BSharp;

/// <summary>
/// Contains either an <see cref="_explicit"/> <b>or</b> a <see cref="_lazy"/> value of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">the type of the actual value</typeparam>
public record Supplied<T> : Wrapped<T?> {
    private enum SupplierStyle { Default, Explicit, Lazy }
    /// <summary>
    /// <c>default</c> with a <c>!</c>-suppressed nullability is used to prevent the need for <see cref="_explicit"/> to become a <see cref="Nullable{T}"/> when <typeparamref name="T"/> is a <see cref="ValueType"/>. 
    /// </summary>
    private readonly Optional<T> _explicit;
    private readonly Optional<Lazy<T>> _lazy;
    private readonly SupplierStyle     Style;

    public override T? Value => GetValue().OrElse(default);

    private Optional<T> GetValue() => Style switch {
        SupplierStyle.Default  => default,
        SupplierStyle.Explicit => _explicit,
        SupplierStyle.Lazy     => _lazy.Select(it => it.Value),
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

    public Supplied(Func<T>? supplier) : this(supplier?.Lazily()) { }

    public static implicit operator Supplied<T>(T        value)    => new(value);
    public static implicit operator Supplied<T>(Lazy<T>? lazy)     => new(lazy);
    public static implicit operator Supplied<T>(Func<T>? supplier) => new(supplier);
    public static implicit operator T(Supplied<T>        supplied) => supplied.Value;
}