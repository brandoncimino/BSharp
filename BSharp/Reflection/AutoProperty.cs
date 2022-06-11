using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a>.
/// </summary>
public class AutoProperty : BackedProperty {
    private static readonly RegexGroup                                            PropertyNameGroup                   = new RegexGroup("property", @"\w+");
    private static readonly Regex                                                 AutoPropertyBackingFieldNamePattern = new Regex(@$"^<{PropertyNameGroup}>k__BackingField$");
    private static readonly ConcurrentDictionary<MemberInfo, Lazy<AutoProperty?>> AutoPropertyCache                   = new(MetadataTokenComparer.Instance);

    private AutoProperty(PropertyInfo front, VariableInfo back) : base(front, back) {
        if (front.DeclaringType == null) {
            throw ReflectionException.NoDeclaringTypeException(front);
        }

        if (back.DeclaringType == null) {
            throw ReflectionException.NoDeclaringTypeException(back);
        }

        if (front.DeclaringType != back.DeclaringType) {
            throw new ArgumentException($"Couldn't construct [{GetType()}]: {nameof(front)}.{nameof(MemberInfo.DeclaringType)} [{front.DeclaringType} != {nameof(back)}.{nameof(MemberInfo.DeclaringType)} [{back.DeclaringType}]!");
        }
    }

    private static AutoProperty? _FromBackingField(FieldInfo backingField) {
        var declaringType = backingField.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(backingField);
        var propName      = GetBackedPropertyName(backingField.Name);
        if (propName == null) {
            return null;
        }

        var prop = declaringType.GetRuntimeProperty(propName);
        return prop == null ? null : new AutoProperty(prop, backingField);
    }

    private static AutoProperty? _FromProperty(PropertyInfo property) {
        var declaringType = property.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(property);
        var backingField  = declaringType.GetRuntimeField(FormatBackingFieldName(property.Name));
        return backingField == null ? null : new AutoProperty(property, backingField);
    }

    public static AutoProperty? AutoPropertyFrom(MemberInfo member) => member switch {
        FieldInfo field    => AutoPropertyCache.GetOrAddLazily(field, _FromBackingField),
        PropertyInfo prop  => AutoPropertyCache.GetOrAddLazily(prop,  _FromProperty),
        VariableInfo varia => AutoPropertyFrom(varia.Member),
        _                  => null,
    };

    private static string FormatBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";

    internal static string? GetBackedPropertyName(string backingFieldName) {
        var match = AutoPropertyBackingFieldNamePattern.Match(backingFieldName);
        return match.Success ? match.Groups[ReflectionUtils.PropertyCaptureGroupName].Value : null;
    }

    public const BindingFlags AutoPropertyBackingFieldBindingFlags =
        BindingFlags.Instance |
        BindingFlags.Static   |
        BindingFlags.NonPublic;
}

public static class AutoPropertyExtensions {
    /// <param name="self">this <see cref="Type"/></param>
    /// <returns>all of the <see cref="AutoProperty"/>s found in this <see cref="Type"/></returns>
    public static IEnumerable<AutoProperty> GetAutoProperties(this Type self) {
        return self.GetFields(AutoProperty.AutoPropertyBackingFieldBindingFlags)
                   .Select(AutoProperty.AutoPropertyFrom)
                   .NonNull();
    }

    /// <param name="property">this <see cref="PropertyInfo"/></param>
    /// <returns><c>true</c> if this <see cref="PropertyInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a></returns>
    public static bool IsAutoProperty(this PropertyInfo property) => AutoProperty.AutoPropertyFrom(property) != null;

    /// <param name="field">this <see cref="FieldInfo"/></param>
    /// <returns><c>true</c> if this <see cref="FieldInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property</a> Backing Field</returns>
    public static bool IsAutoPropertyBackingField(this FieldInfo field) => AutoProperty.GetBackedPropertyName(field.Name).IsNotBlank();
}