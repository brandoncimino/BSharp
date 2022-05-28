using System;
using System.Reflection;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class BackedByAttribute : BrandonAttribute {
    public string BackerName { get; }

    public BackedByAttribute(string backerName) {
        BackerName = backerName;
    }

    public VariableInfo GetBacker(PropertyInfo annotatedProperty) {
        return annotatedProperty.MustGetReflectedType().MustGetVariable(BackerName);
    }

    protected override void ValidateTarget_Hook(MemberInfo target) {
        target.MustBe<PropertyInfo>()
              .MustNotBe(it => it.IsAutoProperty());
    }
}