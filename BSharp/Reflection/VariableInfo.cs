using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents <b>either</b> a <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/>.
/// </summary>
public class VariableInfo : MemberInfo {
    /// <summary>
    /// The actual <see cref="PropertyInfo"/> or <see cref="FieldInfo"/> that this <see cref="VariableInfo"/> represents.
    /// </summary>
    public readonly MemberInfo Member;
    public PropertyInfo? AsProp  => Member as PropertyInfo;
    public FieldInfo?    AsField => Member as FieldInfo;

    public bool IsProperty => Member is PropertyInfo;
    public bool IsField    => Member is FieldInfo;

    public VariableInfo(MemberInfo member) {
        Member = member switch {
            PropertyInfo => member,
            FieldInfo    => member,
            _            => throw NotAVariableException(member),
        };
    }

    public VariableInfo(PropertyInfo property) => Member = property;
    public VariableInfo(FieldInfo    field) => Member = field;
    public VariableInfo(VariableInfo variable) => Member = variable.Member;

    public VariableInfo(Type type, string variableName, BindingFlags bindingFlags) {
        var foundMembers = type.GetMember(variableName, MemberTypes.Field | MemberTypes.Property, bindingFlags);
        if (foundMembers.Length == 1) {
            Member = foundMembers.Single();
        }
        else {
            throw ReflectionException.VariableNotFoundException(type, variableName);
        }
    }

    public override object[] GetCustomAttributes(bool inherit) => Member.GetCustomAttributes(inherit);

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => Member.GetCustomAttributes(attributeType, inherit);

    public override bool IsDefined(Type attributeType, bool inherit) => Member.IsDefined(attributeType, inherit);

    public override Type?       DeclaringType => Member.DeclaringType;
    public override MemberTypes MemberType    => Member.MemberType;
    public override string      Name          => Member.Name;
    public override Type?       ReflectedType => Member.ReflectedType;

    #region Stuff from PropertyInfo

    ///<inheritdoc cref="PropertyInfo.GetAccessors()"/>
    public MethodInfo[] GetAccessors(bool nonPublic) => AsProp?.GetAccessors(nonPublic) ?? Array.Empty<MethodInfo>();

    ///<inheritdoc cref="PropertyInfo.GetGetMethod()"/>
    public MethodInfo? GetGetMethod(bool nonPublic) => AsProp?.GetGetMethod(nonPublic);

    ///<inheritdoc cref="PropertyInfo.GetSetMethod()"/>
    public MethodInfo? GetSetMethod(bool nonPublic) => AsProp?.GetSetMethod(nonPublic);

    /// <inheritdoc cref="PropertyInfo.GetIndexParameters"/>
    public ParameterInfo[] GetIndexParameters() => AsProp?.GetIndexParameters() ?? Array.Empty<ParameterInfo>();

    #endregion

    #region GetValue

    /// <summary>
    /// Gets the value of this <see cref="VariableInfo"/> owned by the specified object.
    /// </summary>
    /// <remarks>
    /// I have decided not to expose a delegate to <see cref="PropertyInfo.GetValue(object, BindingFlags, Binder, Object[], CultureInfo)"/>
    /// because that method is scary.
    /// </remarks>
    /// <param name="obj">the <see cref="object"/> that owns the variable, or <c>null</c> if the variable is <see cref="BindingFlags.Static"/></param>
    /// <seealso cref="PropertyInfo.GetValue(object)">GetValue(object) // Properties</seealso>
    /// <seealso cref="FieldInfo.GetValue">GetValue(object) // Fields</seealso>
    public object? GetValue(object? obj) {
        return Member switch {
            PropertyInfo prop => prop.GetValue(obj),
            FieldInfo field   => field.GetValue(obj),
            _                 => throw NotAVariableException(Member),
        };
    }

    /// <summary>
    /// Gets the value of this <see cref="VariableInfo"/> using the specified index.
    /// </summary>
    /// <inheritdoc cref="GetValue(object?)"/>
    /// <param name="obj">the <see cref="object"/> that owns the variable, or <c>null</c> if the variable is <see cref="BindingFlags.Static"/></param>
    /// <param name="index">the indexer variables used to access the variable</param>
    /// <returns>the value of this variable</returns>
    /// <exception cref="ArgumentException">if this <see cref="IsField"/></exception>
    public object? GetValue(object? obj, object[] index) {
        return Member switch {
            PropertyInfo prop => prop.GetValue(obj, index),
            FieldInfo field   => throw IndexedFieldException(field),
            _                 => throw NotAVariableException(Member),
        };
    }

    /// <summary>
    /// Gets the value of this <see cref="VariableInfo"/> as type <typeparamref name="T"/>.
    /// </summary>
    /// <inheritdoc cref="GetValue(object?)"/>
    /// <typeparam name="T">the return <see cref="Type"/> of the value</typeparam>
    public T? GetValue<T>(object? obj) {
        try {
            var raw = GetValue(obj);
            return (T?)raw;
        }
        catch (Exception e) {
            throw new TargetInvocationException($"Unable to get value of the {nameof(VariableInfo)} {Name}!", e);
        }
    }

    #endregion

    #region SetValue

    /// <summary>
    /// Sets the value of this <see cref="VariableInfo"/>.
    /// </summary>
    /// <param name="obj">the owning <see cref="object"/>, or <c>null</c> if this variable is <see cref="BindingFlags.Static"/></param>
    /// <param name="value">the new value</param>
    /// <seealso cref="PropertyInfo.SetValue(object,object)"/>
    /// <seealso cref="FieldInfo.SetValue(object,object)"/>
    public void SetValue(object? obj, object? value) {
        switch (Member) {
            case PropertyInfo prop:
                prop.SetValue(obj, value);
                return;
            case FieldInfo field:
                field.SetValue(obj, value);
                return;
            default:
                throw NotAVariableException(Member);
        }
    }

    /// <summary>
    /// Sets the value of this <see cref="VariableInfo"/> using the specified index.
    /// </summary>
    /// <param name="obj">the owning <see cref="object"/>, or <c>null</c> if this variable is <see cref="BindingFlags.Static"/></param>
    /// <param name="value">the new value</param>
    /// <exception cref="ArgumentException">if this <see cref="IsField"/></exception>
    /// <param name="index">the indexer variables used to access the variable</param>
    public void SetValue(object? obj, object? value, object[] index) {
        switch (Member) {
            case PropertyInfo prop:
                prop.SetValue(obj, value, index);
                return;
            case FieldInfo field:
                throw IndexedFieldException(field);
            default:
                throw NotAVariableException(Member);
        }
    }

    #endregion

    public bool CanRead => Member switch {
        PropertyInfo prop => prop.CanRead,
        FieldInfo field   => !field.IsAutoPropertyBackingField(),
        _                 => throw NotAVariableException(Member),
    };

    public bool CanWrite => Member switch {
        PropertyInfo prop => prop.CanWrite,
        FieldInfo field   => field.IsInitOnly == false,
        _                 => throw NotAVariableException(Member),
    };

    public Type VariableType => Member switch {
        PropertyInfo prop => prop.PropertyType,
        FieldInfo field   => field.FieldType,
        _                 => throw NotAVariableException(Member),
    };

    public static implicit operator VariableInfo(FieldInfo    field)    => new VariableInfo(field);
    public static implicit operator VariableInfo(PropertyInfo property) => new VariableInfo(property);

    private static ArgumentException NotAVariableException(MemberInfo notVariable, [CallerArgumentExpression("notVariable")] string? parameterName = default, [CallerMemberName] string? rejectedBy = default) {
        return Must.Reject(notVariable, parameterName, rejectedBy, $"must be a 'Variable' (either a {MemberTypes.Property} or a non-backing {MemberTypes.Field})!");
    }

    private static ArgumentException IndexedFieldException(FieldInfo noIndexer, [CallerArgumentExpression("noIndexer")] string? parameterName = default, [CallerMemberName] string? rejectedBy = default) {
        return Must.Reject(noIndexer, parameterName, rejectedBy, $"{MemberTypes.Field}s cannot have indexers!");
    }
}

public static class VariableInfoExtensions {
    /// <summary>
    /// "Safely casts" a <see cref="MemberInfo"/> to a <see cref="VariableInfo"/>, returning <c>null</c> if it isn't a <see cref="PropertyInfo"/> or <see cref="FieldInfo"/>.
    /// </summary>
    /// <param name="member">a <see cref="MemberInfo"/> that might be a <see cref="VariableInfo"/></param>
    /// <returns>a new <see cref="VariableInfo"/> instance</returns>
    public static VariableInfo? AsVariable(this MemberInfo member) => member switch {
        PropertyInfo prop => prop.AsVariable(),
        FieldInfo field   => field.AsVariable(),
        _                 => null,
    };

    public static VariableInfo AsVariable(this     PropertyInfo property)                                                                                                                          => new VariableInfo(property);
    public static VariableInfo AsVariable(this     FieldInfo    field)                                                                                                                             => new VariableInfo(field);
    public static PropertyInfo MustBeProperty(this VariableInfo variable, [CallerArgumentExpression("variable")] string? parameterName = default, [CallerMemberName] string? rejectedBy = default) => variable.AsProp  ?? throw Must.RejectWrongType(variable, typeof(PropertyInfo), parameterName, rejectedBy);
    public static FieldInfo    MustBeField(this    VariableInfo variable, [CallerArgumentExpression("variable")] string? parameterName = default, [CallerMemberName] string? rejectedBy = default) => variable.AsField ?? throw Must.RejectWrongType(variable, typeof(FieldInfo),    parameterName, rejectedBy);
}