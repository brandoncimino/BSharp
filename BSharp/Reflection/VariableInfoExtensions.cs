using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

namespace FowlFever.BSharp.Reflection;

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