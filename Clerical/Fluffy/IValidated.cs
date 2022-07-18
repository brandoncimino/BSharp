using FowlFever.BSharp.Attributes;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// Defines a <see cref="ValidationTarget"/> that <see cref="IValidatorMethod{T}"/>s can be applied to.
/// </summary>
/// <typeparam name="T"></typeparam>
[Experimental(Validator.ExperimentalMessage)]
public interface IValidated<out T> {
    /// <summary>
    /// The actual value that <see cref="ValidatorMethod{T}"/>s are applied to.
    /// </summary>
    T ValidationTarget { get; }
}