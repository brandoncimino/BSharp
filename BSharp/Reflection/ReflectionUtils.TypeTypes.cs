using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Reflection;

public static partial class ReflectionUtils {
    #region Type Types

    /// <remarks>
    /// This is only necessary in .NET Standard 2.0, because in .NET Standard 2.1, an <a href="https://docs.microsoft.com/en-us/dotnet/api/System.Runtime.CompilerServices.ITuple?view=netframework-4.7.1">ITuple</a> interface is available.
    /// </remarks>
    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this <see cref="Type"/> is one of the <see cref="Tuple{T}"/> or <see cref="ValueTuple{T1}"/> types</returns>
    public static bool IsTupleType(this Type type) {
        return type.IsGenericType && TupleTypes.Any(it => type.GetGenericTypeDefinition().IsAssignableFrom(it));
    }

    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this <see cref="Type"/> inherits from <see cref="Exception"/></returns>
    public static bool IsExceptionType(this Type type) {
        return typeof(Exception).IsAssignableFrom(type);
    }

    public static bool IsNullable(this Type type) {
        return type.IsNullableValueType() || type.IsNullableReferenceType();
    }

    public static bool IsNullableValueType(this Type type) {
        return Nullable.GetUnderlyingType(type) != null;
    }

    private static readonly HashSet<string> NullableAttributes = new HashSet<string>() {
        "System.Runtime.CompilerServices.NullableAttribute", "JetBrains.Annotations.CanBeNullAttribute"
    };

    public static bool IsNullableReferenceType(this Type type) {
        return !type.IsValueType && (type._IsNullableAnnotated() || type._IsNullableAnnotated());
    }

    /// <summary>
    /// The comment <a href="https://stackoverflow.com/a/58454489">here</a> has some big check for constructor arguments and bytes instead of just the presence of the attribute...is that necessary at all...?
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    private static bool _IsNullableAnnotated(this MemberInfo memberInfo) {
        var attributes = memberInfo.GetCustomAttributesData().ToList();
        return attributes.FirstOrDefault(it => NullableAttributes.Contains(it.AttributeType.FullName))?._GetNullableByte() == 2;
    }

    private static bool _IsInsideNullableContext(this MemberInfo memberInfo) {
        var outerAttributes = memberInfo.DeclaringType?.GetCustomAttributesData().ToList();
        return outerAttributes?.FirstOrDefault(it => it.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute")?._GetNullableByte() == 2;
    }

    public static bool IsNullable(this ParameterInfo parameterInfo) {
        if (parameterInfo.ParameterType.IsValueType) {
            return parameterInfo.ParameterType.IsNullable();
        }

        return parameterInfo.CustomAttributes.Any(it => NullableAttributes.Contains(it.AttributeType.FullName)) ||
               parameterInfo.ParameterType.IsNullable();
    }

    public static bool IsNullable(this MethodInfo methodInfo) {
        if (methodInfo.ReturnParameter == null) {
            return false;
        }

        return methodInfo._IsNullableAnnotated()     ||
               methodInfo._IsInsideNullableContext() ||
               methodInfo.ReturnParameter.IsNullable();
    }

    public static bool IsNullable(this MemberInfo memberInfo) {
        if (memberInfo._IsNullableAnnotated() || memberInfo._IsInsideNullableContext()) {
            return true;
        }

        return memberInfo switch {
            FieldInfo f    => f.FieldType.IsNullable(),
            PropertyInfo p => p.PropertyType.IsNullable(),
            MethodInfo m   => m.ReturnType.IsNullable(),
            Type t         => t.IsNullable(),
            _              => false
        };
    }

    private static byte? _GetNullableByte(this CustomAttributeData attribute) {
        var arg = attribute.ConstructorArguments.FirstOrDefault();

        if (arg.ArgumentType == typeof(byte)) {
            return arg.Value as byte?;
        }

        if (arg.ArgumentType == typeof(byte[])) {
            var argArgs = (ReadOnlyCollection<CustomAttributeTypedArgument>)arg.Value!;
            return argArgs.FirstOrDefault().Value as byte?;
        }

        return null;
    }

    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this is <see cref="bool"/> or <see cref="Nullable{T}">bool?</see></returns>
    public static bool IsBooly(this Type type) => typeof(bool?).IsAssignableFrom(type);

    /// <param name="method">this <see cref="MethodInfo"/></param>
    /// <returns>the single <see cref="ParameterInfo"/> from <see cref="MethodBase.GetParameters"/>, if there is exactly 1; otherwise, returns <c>null</c>.</returns>
    public static ParameterInfo? GetSingleParameter(this MethodInfo method) => method.GetParameters().FindSingle();

    /// <summary>
    /// Inverse of <see cref="MethodBase.IsStatic"/>. Corresponds to <see cref="BindingFlags.Instance"/>.
    /// </summary>
    /// <param name="method">this method</param>
    /// <returns><c>true</c> if this is <see cref="BindingFlags.Instance"/>; otherwise, <c>false</c></returns>
    public static bool IsInstance(this MethodBase method) => !method.IsStatic;

    /// <summary>
    /// A "predicate" is either:
    /// <ul>
    /// <li>A <see cref="BindingFlags.Static"/> method with 1 parameter that returns <see cref="bool"/></li>
    /// <li>An <see cref="BindingFlags.Instance"/> method with 0 parameters that returns <see cref="bool"/></li>
    /// </ul>
    /// </summary>
    /// <param name="method">this <see cref="MethodInfo"/></param>
    /// <returns>true if this <see cref="MethodInfo"/> returns <see cref="bool"/> or <see cref="Nullable{T}">bool?</see></returns>
    public static bool IsPredicate(this MethodInfo method) {
        return method.ReturnType.IsBooly() &&
               method.GetParameters().Length == (method.IsStatic ? 1 : 0);
    }

    /// <summary>
    /// A "chainable" method is an <see cref="BindingFlags.Instance"/> method that returns its own type.
    /// </summary>
    /// <remarks>
    /// <see cref="IsChainable"/> does not imply that the returned value from the method is a <b>reference</b> to the same instance.
    /// It only indicates that the returned value is the <b>same type</b>.
    /// TODO: Improve the type checking to handle the <a href="https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern">curiously recurring template pattern</a>
    /// </remarks>
    /// <param name="method">this <see cref="MethodInfo"/></param>
    /// <returns>true if this <see cref="MethodInfo"/> returns an <b><i>instance</i></b> of itself</returns>
    public static bool IsChainable(this MethodInfo method) {
        return method.IsInstance() && (
                                          method.ReturnType == method.DeclaringType ||
                                          method.ReturnType == method.ReflectedType
                                      );
    }

    /// <summary>
    /// A "filter" method is a <see cref="BindingFlags.Static"/> method with a single <see cref="ParameterInfo.ParameterType"/> equal to its <see cref="MethodInfo.ReturnType"/>.
    /// </summary>
    /// <remarks>
    /// An <see cref="IsFilter"/> method is equivalent to Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/function/UnaryOperator.html"><![CDATA[UnaryOperator<T>]]></a> interface.
    /// <p/>
    /// However, technically a <a href="https://en.wikipedia.org/wiki/Unary_operation">unary operation</a> does not imply anything about the method's output, only its input.
    /// <p/>
    /// In other words, an <see cref="IsFilter"/> is a specialized <a href="https://en.wikipedia.org/wiki/Unary_operation">unary operation</a>.
    /// </remarks>
    /// <param name="method">this method</param>
    /// <returns><c>true</c> if this method has a single <see cref="ParameterInfo.ParameterType"/> equal to its <see cref="MethodInfo.ReturnType"/></returns>
    public static bool IsFilter(this MethodInfo method) {
        return method.IsStatic && method.GetSingleParameter()?.ParameterType == method.ReturnType;
    }

    /// <summary>
    /// A "checkpoint" method is either:
    /// <ul>
    /// <li>A <see cref="BindingFlags.Static"/> method that <see cref="IsFilter"/></li>
    /// <li>An <see cref="BindingFlags.Instance"/> method that <see cref="IsChainable"/></li>
    /// </ul>
    /// </summary>
    /// <param name="method">this method</param>
    /// <returns>true if this method <see cref="IsFilter"/> or <see cref="IsChainable"/></returns>
    public static bool IsCheckpoint(this MethodInfo method) {
        return method.IsChainable() || method.IsFilter();
    }

    public static bool IsVoid(this MethodInfo method) {
        return method.ReturnType == typeof(void);
    }

    #endregion
}