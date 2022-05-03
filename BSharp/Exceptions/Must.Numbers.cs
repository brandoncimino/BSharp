using System;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Numbers

    [NonNegativeValue]
    public static T BePositive<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        T actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        NotBeNull(actualValue);
        return Be(actualValue, Mathb.IsPositive, parameterName, methodName, reason: "must be positive (x >= 0)");
    }

    [NonNegativeValue]
    public static T BeStrictlyPositive<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        T actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        NotBeNull(actualValue);
        return Be(actualValue, Mathb.IsStrictlyPositive, parameterName, methodName, reason: "must be strictly positive (x > 0)");
    }

    public static T BeNegative<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        T actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        NotBeNull(actualValue);
        return Be(actualValue, Mathb.IsNegative, parameterName, methodName, reason: "must be negative (x < 0)");
    }

    public static T BeGreaterThan<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]
        T firstValue,
        [System.Diagnostics.CodeAnalysis.NotNull]
        T secondValue,
        [CallerArgumentExpression("firstValue")]
        string? firstName = default,
        [CallerArgumentExpression("secondValue")]
        string? secondName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : IComparable<T> {
        return Compare(
                firstValue,
                ComparisonOperator.GreaterThan,
                secondValue,
                firstName,
                secondName,
                methodName
            )
            .first;
    }

    #endregion
}