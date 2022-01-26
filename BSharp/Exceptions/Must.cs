using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;
namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// Methods for validating arguments and generating <see cref="Exception"/>s.
/// </summary>
/// <remarks>
/// Each of these methods should return the original value if their validation succeeds.
/// <br/>
/// <see cref="Exception"/> objects should be created by <see cref="RejectArgument"/> methods.
/// </remarks>
[PublicAPI]
public static class Must {
    public static T NotBeNull<T>(string parameterName, T? actualValue, string methodName) where T : class? {
        return actualValue ?? throw RejectArgument.WasNull(parameterName, actualValue, methodName);
    }

    #region Numbers

    public static T BeANumericValue<T>(string parameterName, T? actualValue, string methodName) {
        if (actualValue?.IsNumber() == true) {
            return actualValue;
        }

        throw RejectArgument.WasNotANumericValue(parameterName, actualValue, methodName);
    }

    public static T BePositive<T>(string parameterName, T? actualValue, string methodName) {
        if (actualValue != null && Mathb.IsPositive(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotPositive(parameterName, actualValue, methodName);
    }

    public static T BeStrictlyPositive<T>(string parameterName, T? actualValue, string methodName) {
        if (actualValue != null && Mathb.IsStrictlyPositive(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotStrictlyPositive(parameterName, actualValue, methodName);
    }

    public static T BeNegative<T>(string parameterName, T? actualValue, string methodName) {
        if (actualValue != null && Mathb.IsNegative(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotNegative(parameterName, actualValue, methodName);
    }

    #endregion

    #region Strings

    public static string NotBeBlank(string parameterName, string? actualValue, string methodName) {
        if (actualValue?.IsNotBlank() == true) {
            return actualValue;
        }

        throw RejectArgument.WasBlank(parameterName, actualValue, methodName);
    }

    public static string NotBeEmpty(string parameterName, string? actualValue, string methodName) {
        if (actualValue?.IsNotEmpty() == true) {
            return actualValue;
        }

        throw RejectArgument.WasEmpty(parameterName, actualValue, methodName);
    }

    #endregion
}

public static class RejectArgument {
    private const string Icon = "🚮";

    private static string GetPreamble<T>(string parameterName, T? actualValue, string methodName) {
        return $"{Icon} {methodName} rejected parameter {parameterName}";
    }

    public static ArgumentNullException WasNull<T>(string parameterName, T? actualValue, string methodName) where T : class? {
        return new ArgumentNullException(parameterName, GetPreamble<T>(parameterName, null, methodName));
    }

    #region Numbers

    [Pure]
    public static ArgumentOutOfRangeException WasNotPositive<T>(string parameterName, T? actualValue, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(parameterName, actualValue, methodName)}: Must be positive (x ≥ 0)!"
        );
    }

    [Pure]
    public static ArgumentOutOfRangeException WasNotStrictlyPositive<T>(string parameterName, T? actualValue, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(parameterName, actualValue, methodName)}: Must be strictly positive (x > 0)!"
        );
    }

    [Pure]
    public static ArgumentOutOfRangeException WasNotNegative<T>(string parameterName, T? actualValue, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(parameterName, actualValue, methodName)}: Must be negative (x < 0)!"
        );
    }

    [Pure]
    public static ArgumentException WasNotANumericValue<T>(string parameterName, T? actualValue, string methodName) {
        var preamble = GetPreamble(parameterName, actualValue, methodName);
        var message  = $"{preamble}: [{typeof(T).Prettify()}]{actualValue} was not a numeric value!";
        return new ArgumentException(message, parameterName);
    }

    #endregion

    #region Switches

    [Pure]
    public static ArgumentException UnhandledSwitchBranch<T>(string parameterName, T? actualValue, string methodName) {
        return new ArgumentException($"{GetPreamble(parameterName, actualValue, methodName)}: Value was unhandled by any switch branch!");
    }

    [Pure]
    public static ArgumentException UnhandledSwitchType<T>(string parameterName, T? actualValue, string methodName) {
        return new ArgumentException($"{GetPreamble(parameterName, actualValue, methodName)}: Value of type {actualValue?.GetType() ?? typeof(T)} was unhandled by any switch branch!");
    }

    #endregion

    #region Strings

    [Pure]
    public static ArgumentException WasBlank(string parameterName, string? actualValue, string methodName) {
        return new ArgumentException($"{GetPreamble(parameterName, actualValue, methodName)}: Must not be blank (null, empty, or whitespace)!");
    }

    [Pure]
    public static ArgumentException WasEmpty(string parameterName, string? actualValue, string methodName) {
        return new ArgumentException($"{GetPreamble(parameterName, actualValue, methodName)}: Must not be null or an empty string!");
    }

    #endregion
}