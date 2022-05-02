using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Numbers

    [NonNegativeValue]
    public static T BePositive<T>(
        [NotNull]
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
        [NotNull]
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
        [NotNull]
        T actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        NotBeNull(actualValue);
        return Be(actualValue, Mathb.IsNegative, parameterName, methodName, reason: "must be negative (x < 0)");
    }

    #endregion
}