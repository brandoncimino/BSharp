using System;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a>.
/// </summary>
internal class AutoProperty {
    /// <summary>
    /// The <see cref="MemberInfo.DeclaringType"/> of <b>both</b> the <see cref="Property"/> and <see cref="BackingField"/>.
    /// </summary>
    public readonly Type DeclaringType;

    /// <summary>
    /// The actual <see cref="PropertyInfo"/>.
    /// </summary>
    public readonly PropertyInfo Property;

    /// <summary>
    /// The compiler-generated backing <see cref="FieldInfo"/> that stores the value of <see cref="Property"/>.
    /// </summary>
    public readonly FieldInfo BackingField;

    private AutoProperty(PropertyInfo property, FieldInfo backingField) {
        if (property.DeclaringType == null) {
            throw ReflectionException.NoDeclaringTypeException(property);
        }

        if (backingField.DeclaringType == null) {
            throw ReflectionException.NoDeclaringTypeException(backingField);
        }

        if (property.DeclaringType != backingField.DeclaringType) {
            throw new ArgumentException($"Couldn't construct [{GetType()}]: {nameof(property)}.{nameof(MemberInfo.DeclaringType)} != {nameof(backingField)}.{nameof(MemberInfo.DeclaringType)}!");
        }

        DeclaringType = property.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(property);
        BackingField  = backingField;
        Property      = property;
    }

    public static AutoProperty? FromBackingField(FieldInfo backingField) {
        var declaringType = backingField.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(backingField);
        var prop          = declaringType.GetRuntimeProperty(GetBackedPropertyName(backingField.Name));
        return prop == null ? null : new AutoProperty(prop, backingField);
    }

    public static AutoProperty? FromProperty(PropertyInfo property) {
        var declaringType = property.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(property);
        var backingField  = declaringType.GetRuntimeField(GetBackingFieldName(property.Name));
        return backingField == null ? null : new AutoProperty(property, backingField);
    }

    public static string GetBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";

    public static string GetBackedPropertyName(string backingFieldName) {
        var match = ReflectionUtils.AutoPropertyBackingFieldNamePattern.Match(backingFieldName);
        return match.Success ? match.Groups[ReflectionUtils.PropertyCaptureGroupName].Value : throw new ArgumentException($"[{backingFieldName}] doesn't match the pattern for auto-property backing fields!");
    }
}