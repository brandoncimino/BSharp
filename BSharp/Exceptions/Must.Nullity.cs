using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// Methods to check for <c>null</c> and <c>!null</c>.
/// </summary>
public static partial class Must {
    #region Nullity
    
    public static T NotBeNull<T>(
        [NotNull]
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return actualValue switch {
            null => throw new ArgumentNullException(parameterName),
            _    => actualValue,
        };
    }

    public static T? BeNull<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return actualValue switch {
            null => actualValue,
            _    => throw Reject(actualValue, parameterName, methodName, "must be null"),
        };
    }

    #endregion
}