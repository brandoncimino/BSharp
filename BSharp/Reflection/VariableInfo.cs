using System;
using System.Globalization;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents <b>either</b> a <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/>.
/// </summary>
public class VariableInfo : PropertyInfo {
    private readonly MemberInfo    _member;
    private          PropertyInfo? AsProp  => _member as PropertyInfo;
    private          FieldInfo?    AsField => _member as FieldInfo;

    public bool IsProperty => _member is PropertyInfo;
    public bool IsField    => _member is FieldInfo;

    public VariableInfo(MemberInfo member) {
        _member = member switch {
            PropertyInfo => member,
            FieldInfo    => member,
            _            => throw ReflectionException.NotAVariableException(member),
        };
    }

    public VariableInfo(PropertyInfo property) {
        _member = property;
    }

    public VariableInfo(FieldInfo field) {
        _member = field;
    }

    public override object[] GetCustomAttributes(bool inherit) => _member.GetCustomAttributes(inherit);

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _member.GetCustomAttributes(attributeType, inherit);

    public override bool IsDefined(Type attributeType, bool inherit) => _member.IsDefined(attributeType, inherit);

    public override Type?       DeclaringType => _member.DeclaringType;
    public override MemberTypes MemberType    => _member.MemberType;
    public override string      Name          => _member.Name;
    public override Type?       ReflectedType => _member.ReflectedType;

    public override MethodInfo[] GetAccessors(bool nonPublic) => (_member as PropertyInfo)?.GetAccessors(nonPublic) ?? Array.Empty<MethodInfo>();

    public override MethodInfo? GetGetMethod(bool nonPublic) => AsProp?.GetGetMethod(nonPublic);

    public override ParameterInfo[] GetIndexParameters() => AsProp?.GetIndexParameters() ?? Array.Empty<ParameterInfo>();

    public override MethodInfo? GetSetMethod(bool nonPublic) => AsProp?.GetSetMethod(nonPublic);

    public override object? GetValue(
        object?      obj,
        BindingFlags invokeAttr,
        Binder?      binder,
        object?[]?   index,
        CultureInfo? culture
    ) {
        return _member switch {
            PropertyInfo prop => prop.GetValue(obj, invokeAttr, binder, index, culture),
            FieldInfo field   => field.GetValue(obj),
            _                 => throw ReflectionException.NotAVariableException(_member),
        };
    }

    public override void SetValue(
        object?      obj,
        object?      value,
        BindingFlags invokeAttr,
        Binder?      binder,
        object?[]?   index,
        CultureInfo? culture
    ) {
        switch (_member) {
            case PropertyInfo prop:
                prop.SetValue(obj, value, invokeAttr, binder, index, culture!);
                break;
            case FieldInfo field:
                field.SetValue(obj!, value!, invokeAttr, binder, culture!);
                break;
            default:
                throw ReflectionException.NotAVariableException(_member);
        }
    }

    public override PropertyAttributes Attributes => AsProp?.Attributes ?? default;
    public override bool CanRead => _member switch {
        PropertyInfo prop => prop.CanRead,
        FieldInfo field   => !field.IsAutoPropertyBackingField(),
        _                 => throw ReflectionException.NotAVariableException(_member),
    };

    public override bool CanWrite => _member switch {
        PropertyInfo prop => prop.CanWrite,
        FieldInfo field   => field.IsInitOnly,
        _                 => throw ReflectionException.NotAVariableException(_member),
    };

    public Type VariableType => _member switch {
        PropertyInfo prop => prop.PropertyType,
        FieldInfo field   => field.FieldType,
        _                 => throw ReflectionException.NotAVariableException(_member),
    };

    public override Type? PropertyType => AsProp?.PropertyType;

    public FieldInfo BackingField => _member switch {
        PropertyInfo prop => prop.BackingField(),
        FieldInfo field   => field,
        _                 => throw ReflectionException.NotAVariableException(_member),
    };

    public static implicit operator VariableInfo(FieldInfo propertyInfo) {
        return new VariableInfo(propertyInfo);
    }
}

public static class VariableInfoExtensions {
    /// <summary>
    /// "Safely casts" a <see cref="MemberInfo"/> to a <see cref="VariableInfo"/>, returning <c>null</c> if it isn't a <see cref="PropertyInfo"/> or <see cref="FieldInfo"/>.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static VariableInfo? AsVariable(this MemberInfo member) => member switch {
        PropertyInfo prop => prop.AsVariable(),
        FieldInfo field   => field.AsVariable(),
        _                 => null,
    };

    public static VariableInfo AsVariable(this PropertyInfo property) => new VariableInfo(property);
    public static VariableInfo AsVariable(this FieldInfo    field)    => new VariableInfo(field);
}