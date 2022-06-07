using System.Reflection;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// Indicates that the annotated <see cref="AttributeTargets.Method"/> is an <see cref="IValidatorMethod"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
public class ValidatorAttribute : BrandonAttribute {
    /// <summary>
    /// A detailed <see cref="IValidatorMethod.Description"/> of this validation.
    /// </summary>
    public string? Description { get; }

    /// <param name="description">a detailed <see cref="IValidatorMethod.Description"/> of this validation</param>
    public ValidatorAttribute(string? description = default) {
        Description = description;
    }

    protected override void ValidateTarget_Hook(MemberInfo target) {
        var method = target.MustBe<MethodInfo>();
        var style  = Validator.InferValidatorStyle(method);
        if (style.AppliesTo(method) == false) {
            throw this.RejectInvalidTarget(target, $"{target} didn't match the {nameof(ValidatorStyle)}.{style}");
        }
    }
}