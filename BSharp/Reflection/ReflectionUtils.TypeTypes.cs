using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

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
    /// <returns>true if this <see cref="MethodInfo"/> returns <see cref="bool"/> or <see cref="Nullable{T}">bool?</see></returns>
    public static bool IsPredicate(this MethodInfo method) {
        return method.ReturnType.IsBooly();
    }

    public static bool IsVoid(this MethodInfo method) {
        return method.ReturnType == typeof(void);
    }

    #endregion
}