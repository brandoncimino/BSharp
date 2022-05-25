using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a>.
/// </summary>
public class AutoProperty : BackedProperty {
    private static readonly ConcurrentDictionary<MemberInfo, Lazy<AutoProperty?>> AutoPropertyCache = new(ReflectionUtils.MetadataTokenComparer.Instance);

    // /// <summary>
    // /// The actual <see cref="PropertyInfo"/>.
    // /// </summary>
    // public readonly PropertyInfo Property;

    /// <summary>
    /// The compiler-generated backing <see cref="FieldInfo"/> that stores the value of <see cref="BackedProperty.Property"/>.
    /// </summary>
    public FieldInfo BackingField => Backer.MustBeField();

    private AutoProperty(PropertyInfo property, FieldInfo backingField) : base(property, backingField) {
        if (property.DeclaringType == null) {
            throw ReflectionException.NoDeclaringTypeException(property);
        }

        if (backingField.DeclaringType == null) {
            throw ReflectionException.NoDeclaringTypeException(backingField);
        }

        if (property.DeclaringType != backingField.DeclaringType) {
            throw new ArgumentException($"Couldn't construct [{GetType()}]: {nameof(property)}.{nameof(MemberInfo.DeclaringType)} != {nameof(backingField)}.{nameof(MemberInfo.DeclaringType)}!");
        }
    }

    private static AutoProperty? _FromBackingField(FieldInfo backingField) {
        var declaringType = backingField.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(backingField);
        var prop          = declaringType.GetRuntimeProperty(GetBackedPropertyName(backingField.Name));
        return prop == null ? null : new AutoProperty(prop, backingField);
    }

    public static AutoProperty? FromBackingField(FieldInfo backingField) => AutoPropertyCache.GetOrAddLazily(backingField, _FromBackingField);

    private static AutoProperty? _FromProperty(PropertyInfo property) {
        var declaringType = property.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(property);
        var backingField  = declaringType.GetRuntimeField(FormatBackingFieldName(property.Name));
        return backingField == null ? null : new AutoProperty(property, backingField);
    }

    public static AutoProperty? FromProperty(PropertyInfo property) => AutoPropertyCache.GetOrAddLazily(property, _FromProperty);

    public static AutoProperty? FromMember(MemberInfo member) => member switch {
        FieldInfo field   => FromBackingField(field),
        PropertyInfo prop => FromProperty(prop),
        _                 => null,
    };

    public static string FormatBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";

    public static string GetBackedPropertyName(string backingFieldName) {
        var match = ReflectionUtils.AutoPropertyBackingFieldNamePattern.Match(backingFieldName);
        return match.Success ? match.Groups[ReflectionUtils.PropertyCaptureGroupName].Value : throw new ArgumentException($"[{backingFieldName}] doesn't match the pattern for auto-property backing fields!");
    }

    public const BindingFlags AutoPropertyBackingFieldBindingFlags =
        BindingFlags.Instance |
        BindingFlags.Static   |
        BindingFlags.NonPublic;
}

public static partial class ReflectionUtils {
    #region AutoProperty extensions

    public static IEnumerable<AutoProperty> GetAutoProperties(this Type self) {
        return self.GetFields(AutoProperty.AutoPropertyBackingFieldBindingFlags)
                   .Select(AutoProperty.FromBackingField)
                   .NonNull();
    }

    /// <param name="property">this <see cref="PropertyInfo"/></param>
    /// <returns><c>true</c> if this <see cref="PropertyInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a></returns>
    public static bool IsAutoProperty(this PropertyInfo property) => AutoProperty.FromProperty(property) != null;

    /// <param name="field">this <see cref="FieldInfo"/></param>
    /// <returns><c>true</c> if this <see cref="FieldInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a> Backing Field</returns>
    public static bool IsAutoPropertyBackingField(this FieldInfo field) => AutoProperty.FromBackingField(field) != null;

    #endregion
}