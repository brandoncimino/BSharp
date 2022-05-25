using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// Methods to check for <c>null</c> and <c>!null</c>.
/// </summary>
public static partial class Must {
    #region Nullity

    /// <param name="actualValue">tautological</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="methodName">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="actualValue"/></typeparam>
    /// <returns><paramref name="actualValue"/> as a non-nullable type</returns>
    /// <exception cref="ArgumentNullException">if <paramref name="actualValue"/> is null</exception>
    public static T NotBeNull<T>(
        [NotNull] T? actualValue,
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="parameterName"></param>
    /// <param name="methodName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="RejectionException"></exception>
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