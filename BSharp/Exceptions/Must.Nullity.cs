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
    /// <param name="details">additional user-provided info</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="actualValue"/></typeparam>
    /// <returns><paramref name="actualValue"/> as a non-nullable type</returns>
    /// <exception cref="ArgumentNullException">if <paramref name="actualValue"/> is null</exception>
    public static T NotBeNull<T>(
        [NotNull] T? actualValue,
        string?      details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return actualValue switch {
            null => throw Reject(actualValue, details, parameterName, rejectedBy),
            _    => actualValue,
        };
    }

    /// <inheritdoc cref="HaveValue{T}"/>
    public static T NotBeNull<T>(
        [NotNull] T? actualValue,
        string?      details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : struct {
        return actualValue switch {
            null => throw Reject(actualValue, details, parameterName, rejectedBy),
            _    => actualValue.Value,
        };
    }

    /// <inheritdoc cref="NotBeNull{T}(T?,string?,string?,string?)"/>
    /// <remarks>This is the <see cref="ValueType"/> equivalent of <see cref="NotBeNull{T}(T?,string?,string?,string?)"/>, which converts away from <see cref="Nullable{T}"/>.</remarks>
    public static T HaveValue<T>(
        [NotNull] T? actualValue,
        string?      details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    )
        where T : struct => NotBeNull(actualValue, details, parameterName, rejectedBy);

    /// <inheritdoc cref="NotBeNull{T}(T?,string?,string?,string?)"/>
    public static T MustNotBeNull<T>(
        [NotNull] this T? actualValue,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) => NotBeNull(actualValue, details, parameterName, rejectedBy);

    /// <inheritdoc cref="HaveValue{T}"/>
    public static T MustNotBeNull<T>(
        [NotNull] this T? actualValue,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    )
        where T : struct => NotBeNull(actualValue, parameterName, methodName);

    /// <param name="actualValue">on the tin</param>
    /// <param name="details">additional user-provided info</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of the validated value</typeparam>
    /// <returns><c>null</c> - like, literally, <b>always</b> <c>null</c></returns>
    /// <exception cref="RejectionException">if <paramref name="actualValue"/> is <c>null</c></exception>
    public static T? BeNull<T>(
        T?      actualValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return actualValue switch {
            null => actualValue,
            _    => throw Reject(actualValue, details, parameterName, rejectedBy, "must be null"),
        };
    }

    /// <inheritdoc cref="BeNull{T}"/>
    public static T? MustBeNull<T>(
        this T? actualValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return BeNull(actualValue, details, parameterName, rejectedBy);
    }

    #endregion
}