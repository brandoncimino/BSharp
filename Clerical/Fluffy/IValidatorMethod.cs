using System.Reflection;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// Acts as a bridge between different method signatures of <see cref="ValidatorStyle"/>.
/// </summary>
public interface IValidatorMethod {
    /// <summary>
    /// Describes the method signature of the underlying <see cref="Method"/>.
    /// </summary>
    public ValidatorStyle Style { get; }

    /// <summary>
    /// The actual underlying <see cref="Method"/>.
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    /// Throws an <see cref="Exception"/> if the <see cref="object"/> is invalid.
    /// </summary>
    /// <param name="value">the <see cref="object"/> being validated</param>
    public void Assert(object? value);

    /// <summary>
    /// Returns <c>false</c> if the <see cref="object"/> is invalid.
    /// </summary>
    /// <param name="value">the <see cref="object"/> being validated</param>
    /// <returns><c>true</c> if the <see cref="object"/> is valid; otherwise, <c>false</c></returns>
    public bool Try(object? value);

    /// <summary>
    /// Validates the <see cref="object"/>, returning an equivalent <see cref="object"/> if it passes,
    /// or throwing an <see cref="Exception"/> if it fails. 
    /// </summary>
    /// <param name="value">the <see cref="object"/> being validated</param>
    /// <returns>an equivalent <see cref="object"/></returns>
    /// <remarks>
    /// The returned <see cref="object"/> may or may not be <see cref="object.ReferenceEquals"/> to the input <paramref name="value"/>.
    ///
    /// This facilitates small modifications, such as <see cref="string.Trim()"/>ming white space, before passing the object on to the next <see cref="IValidatorMethod"/>.
    /// </remarks>
    public object? Check(object? value);
}