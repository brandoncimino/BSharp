using System.Reflection;

using FowlFever.BSharp;
using FowlFever.BSharp.Optional;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// Acts as a bridge between different method signatures of <see cref="ValidatorStyle"/>.
/// </summary>
public interface IValidatorMethod : IHas<MethodInfo> {
    /// <summary>
    /// Describes the method signature of the underlying <see cref="IHas{T}.Value"/>.
    /// </summary>
    public ValidatorStyle Style { get; }

    /// <summary>
    /// A detailed description of this <see cref="IValidatorMethod"/>.
    /// </summary>
    public string? Description { get; }

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

    /// <summary>
    /// Invokes this <see cref="IValidatorMethod"/> "safely", returning the result as an <see cref="IFailable"/>.
    /// </summary>
    /// <param name="value">the object being validated</param>
    /// <returns>an <see cref="IFailable"/> containing the validation results</returns>
    public IFailable TryValidate(object? value);
}

public interface IValidatorMethod<in T, out T2> : IValidatorMethod
    where T2 : T {
    public void Assert(T? actual);
    public bool Try(T?    actual);
    public T2?  Check(T?  actual);

    public IFailableFunc<T2?> TryValidate(T? actual);
}