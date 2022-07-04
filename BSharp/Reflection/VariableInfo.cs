using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

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

    /// <summary>
    /// This is kinda redundant with the specific constructors, but it gets stupid Rider to stop telling me to accept <see cref="MemberInfo"/> instead of specific types.
    /// </summary>
    /// <param name="member"></param>
    /// <exception cref="ArgumentException"></exception>
    private VariableInfo(MemberInfo member) {
        Member = member switch {
            PropertyInfo   => member,
            FieldInfo      => member,
            VariableInfo v => v.Member,
            _              => throw NotAVariableException(member),
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
    public object? GetValue(object? obj) => HandleFunc(
        ValueTuple.Create(obj),
        static (p, inputs) => p.GetValue(inputs.Item1),
        static (f, inputs) => f.GetValue(inputs.Item1)
    );

    /// <summary>
    /// Gets the value of this <see cref="VariableInfo"/> using the specified index.
    /// </summary>
    /// <inheritdoc cref="GetValue(object?)"/>
    /// <param name="obj">the <see cref="object"/> that owns the variable, or <c>null</c> if the variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
    /// <param name="index">the indexer variables used to access the variable</param>
    /// <returns>the value of this variable</returns>
    /// <exception cref="ArgumentException">if this <see cref="IsField"/></exception>
    public object? GetValue(
        object?  obj,
        object[] index
    ) => HandleFunc(
        (obj, index),
        static (p, inputs) => p.GetValue(inputs.obj, inputs.index),
        static (f, _) => throw IndexedFieldException(f)
    );

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
    /// <param name="owner">the owning <see cref="object"/>, or <c>null</c> if this variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
    /// <param name="value">the new value</param>
    /// <seealso cref="PropertyInfo.SetValue(object,object)"/>
    /// <seealso cref="FieldInfo.SetValue(object,object)"/>
    public void SetValue(object? owner, object? value) => HandleAction(
        (owner, value),
        static (p, inputs) => p.SetValue(inputs.owner, inputs.value),
        static (f, inputs) => f.SetValue(inputs.owner, inputs.value)
    );

    /// <summary>
    /// Sets the value of this <see cref="VariableInfo"/> using the specified index.
    /// </summary>
    /// <param name="owner">the owning <see cref="object"/>, or <c>null</c> if this variable is <see cref="System.Reflection.BindingFlags.Static"/></param>
    /// <param name="value">the new value</param>
    /// <exception cref="ArgumentException">if this <see cref="IsField"/></exception>
    /// <param name="index">the indexer variables used to access the variable</param>
    public void SetValue(object? owner, object? value, object[] index) => HandleAction(
        (owner, value, index),
        static (p, inputs) => p.SetValue(inputs.owner, inputs.value, inputs.index),
        static (f, _) => throw IndexedFieldException(f)
    );

    #endregion

    private static readonly ValueTuple Nothing = new ValueTuple();

    public bool CanRead      => HandleFunc(Nothing, static (p, _) => p.CanRead,      static (f, _) => !f.IsAutoPropertyBackingField());
    public bool CanWrite     => HandleFunc(Nothing, static (p, _) => p.CanWrite,     static (f, _) => !f.IsInitOnly);
    public Type VariableType => HandleFunc(Nothing, static (p, _) => p.PropertyType, static (f, _) => f.FieldType);

    public static implicit operator VariableInfo(FieldInfo    field)    => new VariableInfo(field);
    public static implicit operator VariableInfo(PropertyInfo property) => new VariableInfo(property);

    private static ArgumentException NotAVariableException(MemberInfo notVariable, [CallerArgumentExpression("notVariable")] string? parameterName = default, [CallerMemberName] string? rejectedBy = default) {
        return Must.Reject(notVariable, parameterName, rejectedBy, $"must be a 'Variable' (either a {MemberTypes.Property} or a non-backing {MemberTypes.Field})!");
    }

    private static ArgumentException IndexedFieldException(FieldInfo noIndexer, [CallerArgumentExpression("noIndexer")] string? parameterName = default, [CallerMemberName] string? rejectedBy = default) {
        return Must.Reject(noIndexer, parameterName, rejectedBy, $"{MemberTypes.Field}s cannot have indexers!");
    }

    public TOut HandleFunc<TInputs, TOut>(
        TInputs inputs,
        [RequireStaticDelegate]
        Func<PropertyInfo, TInputs, TOut> ifProperty,
        [RequireStaticDelegate]
        Func<FieldInfo, TInputs, TOut> ifField
    ) => Member switch {
        PropertyInfo p => ifProperty(p, inputs),
        FieldInfo f    => ifField(f, inputs),
        _              => throw NotAVariableException(Member),
    };

    public void HandleAction<TInputs>(
        TInputs inputs,
        [RequireStaticDelegate]
        Action<PropertyInfo, TInputs> ifProperty,
        [RequireStaticDelegate]
        Action<FieldInfo, TInputs> ifField
    ) {
        switch (Member) {
            case PropertyInfo p:
                ifProperty(p, inputs);
                break;
            case FieldInfo f:
                ifField(f, inputs);
                break;
            default: throw NotAVariableException(Member);
        }
    }
}