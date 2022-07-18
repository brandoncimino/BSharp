using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp;
using FowlFever.BSharp.Attributes;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// A <see cref="Wrapped{T}"/> value that will always call <see cref="Validator.Validate{T}"/> when it is constructed.
/// </summary>
/// <typeparam name="T">the type of the underlying <see cref="Wrapped{T}.Value"/></typeparam>
[Experimental(Validator.ExperimentalMessage)]
public abstract record Validated<T> : Wrapped<T>, IValidated<T> {
    public sealed override T Value { get; }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected Validated(T value) {
        Value = Fluff(value);
        Validate();

        // var (changed, fluffed) = TryFluff(value);

        // if (!changed) {
        // return;
        // }

        // Value = fluffed;
        // Validate();
    }

    private void Validate() {
        Validator.Validate(this);
    }

    internal virtual T Fluff(T value) {
        return value;
    }

    private (bool, T) TryFluff(T value) {
        var fluffed = Fluff(value);
        var changed = value?.GetHashCode() != fluffed?.GetHashCode() && !Equals(value, fluffed);
        return (changed, fluffed);
    }

    T IValidated<T>.ValidationTarget => Value;
}