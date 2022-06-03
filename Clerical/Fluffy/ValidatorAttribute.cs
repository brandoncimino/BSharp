using System.Reflection;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Fluffy;

[AttributeUsage(AttributeTargets.Method)]
public class ValidatorAttribute : BrandonAttribute {
    protected override void ValidateTarget_Hook(MemberInfo target) {
        var method = target.MustBe<MethodInfo>();
        var style  = Validator.InferValidatorStyle(method);
        if (style.AppliesTo(method) == false) {
            throw this.RejectInvalidTarget(target, $"{target} didn't match the {nameof(ValidatorStyle)}.{style}");
        }
    }
}