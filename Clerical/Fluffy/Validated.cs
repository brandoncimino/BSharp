using FowlFever.BSharp;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// A <see cref="Wrapped{T}"/> value that will always call <see cref="Validator.Validate{T}"/> when it is constructed.
/// </summary>
/// <typeparam name="T">the type of the underlying <see cref="Wrapped{T}.Value"/></typeparam>
public abstract record Validated<T> : Wrapped<T> {
    public override T Value { get; }

    protected Validated(T value) {
        // ReSharper disable once VirtualMemberCallInConstructor
        Value = Fluff(value);
        Validator.Validate(this);
    }

    internal virtual T Fluff(T value) => value;
}