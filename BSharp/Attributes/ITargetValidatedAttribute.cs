using System.Reflection;

namespace FowlFever.BSharp.Attributes;

public interface ITargetValidatedAttribute {
    public void ValidateTarget(MemberInfo target);
}