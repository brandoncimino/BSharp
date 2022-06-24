using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    [Pure]
    private static string PrettifyType(Type? type) {
        if (type == null) {
            return "⛔";
        }

        return PrettifyNullableType(type)
               ?? PrettifyTupleType(type)
               ?? PrettifyGenericType(type)
               ?? type.Name;

        #region Local methods

        static string? PrettifyGenericType(Type genericType) {
            if (genericType.IsGenericType == false) {
                return null;
            }

            // Make sure to use `.GetGenericArguments()` and not `.GenericTypeArguments`, because the latter will return an empty array for
            // a generic type definition like `List<>`
            var genArgs       = genericType.GetGenericArguments();
            var genArgStrings = string.Join(", ", genArgs.Select(PrettifyType));
            return genericType.Name.Replace($"`{genArgs.Length}", genArgStrings);
        }

        static string? PrettifyTupleType(Type tupleType) {
            static bool IsValueTuple(Type type) => type.IsValueType && Regex.IsMatch(type.Name, @"ValueTuple`\d");

            if (IsValueTuple(tupleType) == false) {
                return null;
            }

            var genArgs = tupleType.GetGenericArguments().Select(PrettifyType);
            return $"({string.Join(",", genArgs)})";
        }

        static string? PrettifyNullableType(Type type) {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null ? $"{PrettifyType(underlyingType)}?" : null;
        }

        #endregion
    }

    public static Type Extend(
        Type?   actualValue,
        Type    parentType,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        if (parentType.IsAssignableFrom(actualValue) == false) {
            throw Reject(actualValue, details, parameterName, rejectedBy, $"{PrettifyType(actualValue)} must extend {PrettifyType(parentType)}");
        }

        return actualValue;
    }
}