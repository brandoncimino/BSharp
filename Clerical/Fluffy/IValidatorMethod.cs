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
    public bool Test(object? value);

    /// <summary>
    /// Invokes this <see cref="IValidatorMethod"/> "safely", returning the result as an <see cref="IFailable"/>.
    /// </summary>
    /// <param name="value">the object being validated</param>
    /// <returns>an <see cref="IFailable"/> containing the validation results</returns>
    public IFailable TryValidate(object? value);
}

public interface IValidatorMethod<in T> : IValidatorMethod {
    public void      Assert(T?                   actual);
    public void      Assert(IValidated<T?>       actual);
    public bool      Test(T?                     actual);
    public bool      Test(IValidated<T?>?        actual);
    public IFailable TryValidate(T?              actual);
    public IFailable TryValidate(IValidated<T?>? actual);
}