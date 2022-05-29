using System;

using FowlFever.BSharp;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.Testing;

/// <summary>
/// hacky class used to support <see cref="MultipleAsserter{TSelf,TActual}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
internal class OneTimeOnly<T> {
    private Optional<T?>       _explicitValue;
    private Optional<Lazy<T?>> _lazy;

    private enum SetStyle { Explicit, Lazy }

    private SetStyle? SetAs;

    public void Set(T? value) {
        if (SetAs.HasValue) {
            throw new InvalidOperationException($"{GetType().Prettify()} was already set!");
        }

        _explicitValue = value;
        SetAs          = SetStyle.Explicit;
    }

    public void Set(Func<T?> supplier) {
        if (supplier == null) {
            throw new ArgumentNullException(nameof(supplier));
        }

        if (SetAs.HasValue) {
            throw new InvalidOperationException($"{GetType().Prettify()} was already set!");
        }

        _lazy = supplier.Lazily();
        SetAs = SetStyle.Lazy;
    }

    public T? Get(string? message = default) {
        if (SetAs == null) {
            throw new InvalidOperationException(message?.Suffix(": ") + $"{GetType().Prettify()} was never set!");
        }

        return SetAs switch {
            SetStyle.Explicit => _explicitValue.Value,
            SetStyle.Lazy     => _lazy.Value.Value,
            _                 => throw BEnum.InvalidEnumArgumentException(SetAs),
        };
    }

    public T? Value => Get();

    public Optional<T?> ToOptional() => SetAs != null ? Optional.Of(Get()) : default;
}