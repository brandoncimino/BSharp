using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// A <see cref="Wrapped{T}"/> value that will always call <see cref="Validator.Validate{T}"/> when it is constructed.
/// </summary>
/// <typeparam name="T">the type of the underlying <see cref="Wrapped{T}.Value"/></typeparam>
public abstract record Validated<T> : Wrapped<T> {
    public override T Value { get; }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected Validated(T value) {
        Value = BeforeValidation(value);
        Validator.Validate(this);
        Value = AfterValidation(Value);
    }

    internal virtual T BeforeValidation(T value) => value;
    internal virtual T AfterValidation(T  value) => value;
}