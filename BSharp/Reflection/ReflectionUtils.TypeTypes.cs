using System;
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
    [Pure]
    public static bool IsTupleType(this Type type) {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        return type.IsAssignableTo(typeof(System.Runtime.CompilerServices.ITuple));
#else
        return type.IsGenericType && TupleTypes.Contains(type.GetGenericTypeDefinition());
#endif
    }

    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this <see cref="Type"/> inherits from <see cref="Exception"/></returns>
    [Pure]
    public static bool IsExceptionType(this Type type) => typeof(Exception).IsAssignableFrom(type);

    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this <see cref="Type"/> is a <see cref="Nullable{T}"/></returns>
    [Pure]
    public static bool IsNullableValueType(this Type type) => Nullable.GetUnderlyingType(type) != null;

    #region Nullable Reference Types

    [Pure] public static NullabilityInfo  GetNullability(this FieldInfo     field)     => new NullabilityInfoContext().Create(field);
    [Pure] public static NullabilityInfo  GetNullability(this PropertyInfo  property)  => new NullabilityInfoContext().Create(property);
    [Pure] public static NullabilityInfo  GetNullability(this EventInfo     eventInfo) => new NullabilityInfoContext().Create(eventInfo);
    [Pure] public static NullabilityInfo  GetNullability(this ParameterInfo parameter) => new NullabilityInfoContext().Create(parameter);
    [Pure] public static NullabilityInfo  GetNullability(this VariableInfo  variable)  => variable.HandleFunc(ValueTuple.Create(), static (p, _) => p.GetNullability(), static (f, _) => f.GetNullability());
    [Pure] public static NullabilityInfo? GetNullability(this MethodInfo    method)    => method.ReturnParameter?.GetNullability();

    [Pure]
    public static NullabilityInfo? GetNullability(this MemberInfo memberInfo) => memberInfo switch {
        FieldInfo f    => f.GetNullability(),
        PropertyInfo p => p.GetNullability(),
        EventInfo e    => e.GetNullability(),
        MethodInfo m   => m.GetNullability(),
        VariableInfo v => v.GetNullability(),
        _              => default
    };

    #endregion

    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this is <see cref="bool"/> or <see cref="Nullable{T}">bool?</see></returns>
    public static bool IsBooly(this Type type) => typeof(bool?).IsAssignableFrom(type);

    /// <param name="method">this <see cref="MethodInfo"/></param>
    /// <returns>the single <see cref="ParameterInfo"/> from <see cref="MethodBase.GetParameters"/>, if there is exactly 1; otherwise, returns <c>null</c>.</returns>
    public static ParameterInfo? FindSingleParameter(this MethodInfo method) => method.GetParameters().FindSingle().OrDefault();

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
        return method.IsStatic && method.FindSingleParameter()?.ParameterType == method.ReturnType;
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