using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents <b>either</b> a <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/>.
/// </summary>
public class VariableInfo : MemberInfo {
    private const MemberTypes VariableMemberTypes = System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property;

    public const BindingFlags VariableBindingFlags =
        System.Reflection.BindingFlags.Default   |
        System.Reflection.BindingFlags.Instance  |
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Public    |
        System.Reflection.BindingFlags.Static;

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

    public VariableInfo(Type type, string variableName, BindingFlags bindingFlags = VariableBindingFlags) {
        var foundMembers = type.GetMember(variableName, MemberTypes.Field | MemberTypes.Property, bindingFlags);
        if (foundMembers.Length == 1) {
            Member = foundMembers.Single();
        }
        else {
            throw ReflectionException.VariableNotFoundException(type, variableName);
        }
    }

    /// <summary>
    /// Constructs a <see cref="VariableInfo"/> to represent the given <see cref="MemberInfo"/>, if possible.
    /// </summary>
    /// <param name="member">the wrapped <see cref="MemberInfo"/></param>
    /// <returns>a new <see cref="VariableInfo"/>, if possible; otherwise, <c>null</c></returns>
    public static VariableInfo? From(MemberInfo member) => member switch {
        VariableInfo v => v,
        PropertyInfo p => new VariableInfo(p),
        FieldInfo f    => f.IsAutoPropertyBackingField() ? null : new VariableInfo(f),
        _              => null,
    };

    /// <param name="type">a <see cref="Type"/></param>
    /// <returns>all of the <see cref="VariableInfo"/>s found in the <see cref="Type"/></returns>
    public static IEnumerable<VariableInfo> From(Type type) => type.GetMembers(VariableBindingFlags).Select(From).NonNull();

    /// <summary>
    /// "Safely" finds a <see cref="VariableInfo"/> by name, returning <c>null</c> if it can't be found.
    /// </summary>
    /// <param name="type">the <see cref="Type"/> that has the variable</param>
    /// <param name="name">the <see cref="Name"/> of the variable</param>
    /// <param name="bindingFlags">optional <see cref="BindingFlags"/> <i>(📎 defaults to <see cref="VariableBindingFlags"/>)</i></param>
    /// <returns>the <see cref="VariableInfo"/>, if found; otherwise, <c>null</c></returns>
    /// <exception cref="AmbiguousMatchException">if more than one <see cref="MemberInfo"/> matches the <paramref name="name"/></exception>
    public static VariableInfo? From(Type type, string name, BindingFlags bindingFlags = VariableBindingFlags) {
        var member = type.GetMember(name, VariableMemberTypes, bindingFlags);
        return member switch {
            { Length: < 1 } => null,
            { Length: 1 }   => From(member.Single()),
            { Length: > 1 } => throw new AmbiguousMatchException($"Multiple {VariableMemberTypes} members found for [{type.Name}]::{name}!"),
        };
    }

    /// <inheritdoc cref="From(Type, String, BindingFlags)"/>
    public static VariableInfo? From<T>(string name, BindingFlags bindingFlags = VariableBindingFlags) => From(typeof(T), name, bindingFlags);

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
    /// I have decided not to expose a delegate to <see cref="PropertyInfo.GetValue(object, System.Reflection.BindingFlags, Binder, Object[], CultureInfo)"/>
    /// because that method is scary.
    /// </remarks>
    /// <param name="obj">the <see cref="object"/> that owns the variable, or <c>null</c> if the variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
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
    /// <param name="obj">the <see cref="object"/> that owns the variable, or <c>null</c> if the variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
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
    /// <param name="obj">the owning <see cref="object"/>, or <c>null</c> if this variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
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
    /// <param name="obj">the owning <see cref="object"/>, or <c>null</c> if this variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
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

    /// <summary>
    /// Checks if a <see cref="MemberInfo"/> can be converted to a <see cref="VariableInfo"/>.
    /// </summary>
    /// <remarks>
    /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
    /// TODO: Previously, this excluded <see cref="AutoPropertyExtensions.IsAutoPropertyBackingField"/>s. Should that still be the case? Alternatively, should we:
    ///     - Throw an exception when we construct a <see cref="VariableInfo"/> from an <see cref="AutoPropertyExtensions.IsAutoPropertyBackingField"/>
    ///     - Construct a <see cref="VariableInfo"/> from an auto-prop backing field's <see cref="BackedProperty.Front"/> instead
    /// </remarks>
    /// <param name="member">this <see cref="MemberInfo"/></param>
    /// <returns>true if the <see cref="MemberInfo"/> is a variable</returns>
    public static bool IsVariable(this MemberInfo member) => member.AsVariable() != null;

    /// <summary>
    /// Returns all of the <see cref="IsVariable"/> <see cref="MemberInfo"/>s from the given <paramref name="type"/>.
    /// </summary>
    /// <remarks>
    /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
    /// It does <b>not</b> include <see cref="AutoPropertyExtensions.IsAutoPropertyBackingField"/>s.
    /// </remarks>
    /// <param name="type">The <see cref="Type"/> to retrieve the fields and properties of.</param>
    /// <returns>
    /// </returns>
    [Pure]
    public static IEnumerable<VariableInfo> GetVariables(this Type type) => VariableInfo.From(type);

    /// <summary>
    /// Returns the <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with the the <see cref="MemberInfo.Name"/> <paramref name="variableName"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that should have a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></param>
    /// <param name="variableName">The expected <see cref="VariableInfo.Name"/></param>
    /// <returns>The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></returns>
    /// <exception cref="MissingMemberException">If <paramref name="variableName"/> couldn't be retrieved</exception>
    [Pure]
    public static VariableInfo MustGetVariable(this Type type, string variableName) => new VariableInfo(type, variableName);

    /// <inheritdoc cref="VariableInfo.From(Type, String, BindingFlags)"/>
    [Pure]
    public static VariableInfo? GetVariable(this Type type, string variableName) => VariableInfo.From(type, variableName);

    /// <summary>
    /// <inheritdoc cref="GetVariable"/>
    /// </summary>
    /// <param name="obj">An object to infer <typeparamref name="T"/> from</param>
    /// <param name="variableName"><inheritdoc cref="GetVariable"/></param>
    /// <typeparam name="T">The type to retrieve <paramref name="variableName"/> from</typeparam>
    /// <returns><inheritdoc cref="GetVariable"/></returns>
    [Pure]
    public static VariableInfo? GetVariableInfo<T>(this T obj, string variableName) => VariableInfo.From<T>(variableName);

    /// <summary>
    /// Returns the <b>value</b> (either <see cref="PropertyInfo"/>.<see cref="PropertyInfo.GetValue(object)"/> or <see cref="FieldInfo"/>.<see cref="FieldInfo.GetValue"/>)
    /// of the <see cref="GetVariable"/> named <paramref name="variableName"/>.
    /// </summary>
    /// <param name="obj">The object to retrieve the value from</param>
    /// <param name="variableName">The name of the variable</param>
    /// <typeparam name="T">The <b>return type</b> of <b><paramref name="variableName"/></b></typeparam>
    /// <returns>The value of the <see cref="GetVariable"/></returns>
    /// <exception cref="InvalidCastException">If the retrieved value cannot be cast to <typeparamref name="T"/></exception>
    /// <exception cref="MissingMemberException"><inheritdoc cref="GetVariable"/></exception>
    [Pure]
    public static T? GetVariableValue<T>(this object obj, string variableName) {
        var variableInfo = obj.GetType().MustGetVariable(variableName);
        var value        = variableInfo.GetValue(obj);

        try {
            //TODO: Handle this better?
            return (T)value;
        }
        catch (Exception e) when (e is NullReferenceException or InvalidCastException) {
            throw e.ModifyMessage($"A member for {variableInfo.Prettify(TypeNameStyle.Full)} was found on the [{obj.GetType().Name}]'{obj}', but it couldn't be cast to a {typeof(T).PrettifyType(default)}!");
        }
    }

    /// <summary>
    /// <inheritdoc cref="GetVariableValue{T}"/>
    /// </summary>
    /// <param name="obj"><inheritdoc cref="GetVariableValue{T}"/></param>
    /// <param name="variableName"><inheritdoc cref="GetVariableValue{T}"/></param>
    /// <returns><inheritdoc cref="GetVariableValue{T}"/></returns>
    [Pure]
    //TODO: Could we use `dynamic` here instead of `object`...?
    public static object? GetVariableValue(object obj, string variableName) => GetVariableValue<object>(obj, variableName);

    public static void SetVariableValue<T>(object obj, string variableName, T newValue) => obj.GetType().GetVariable(variableName)?.SetValue(obj, newValue);

    /// <param name="fieldOrProperty">either a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/></param>
    /// <returns>true if we <see cref="VariableInfo.CanWrite"/> to this <paramref name="fieldOrProperty"/></returns>
    [Pure]
    public static bool IsSettable(this MemberInfo fieldOrProperty) => VariableInfo.From(fieldOrProperty)?.CanWrite == true;
}