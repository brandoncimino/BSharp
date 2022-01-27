using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
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
    public static T NotBeNull<T>(T? actualValue, string parameterName, string methodName) where T : class? {
        return actualValue ?? throw RejectArgument.WasNull(actualValue, parameterName, methodName);
    }

    #region Numbers

    public static T BeANumericValue<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue?.IsNumber() == true) {
            return actualValue;
        }

        throw RejectArgument.WasNotANumericValue(actualValue, parameterName, methodName);
    }

    [NonNegativeValue]
    public static T BePositive<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue != null && Mathb.IsPositive(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotPositive(actualValue, parameterName, methodName);
    }

    [NonNegativeValue]
    public static T BeStrictlyPositive<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue != null && Mathb.IsStrictlyPositive(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotStrictlyPositive(actualValue, parameterName, methodName);
    }

    public static T BeNegative<T>(T? actualValue, string parameterName, string methodName) {
        if (actualValue != null && Mathb.IsNegative(actualValue)) {
            return actualValue;
        }

        throw RejectArgument.WasNotNegative(actualValue, parameterName, methodName);
    }

    #endregion

    #region Strings

    /// <summary>
    ///
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="parameterName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string NotBeBlank(string? actualValue, string parameterName, string methodName) {
        if (actualValue?.IsNotBlank() == true) {
            return actualValue;
        }

        throw RejectArgument.WasBlank(actualValue, parameterName, methodName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="actualValue"></param>
    /// <param name="parameterName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string NotBeEmpty(string? actualValue, string parameterName, string methodName) {
        if (actualValue?.IsNotEmpty() == true) {
            return actualValue;
        }

        throw RejectArgument.WasEmpty(actualValue, parameterName, methodName);
    }

    #endregion
}

public static class RejectArgument {
    private const string Icon = "🚮";

    private static string GetPreamble<T>(T? actualValue, string parameterName, string methodName) {
        return $"{Icon} {methodName} rejected parameter {parameterName}";
    }

    public static ArgumentNullException WasNull<T>(T? actualValue, string parameterName, string methodName) where T : class? {
        return new ArgumentNullException(parameterName, GetPreamble(actualValue, parameterName, methodName));
    }

    #region Numbers

    [Pure]
    public static ArgumentOutOfRangeException WasNotPositive<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(actualValue, parameterName, methodName)}: Must be positive (x ≥ 0)!"
        );
    }

    [Pure]
    public static ArgumentOutOfRangeException WasNotStrictlyPositive<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(actualValue, parameterName, methodName)}: Must be strictly positive (x > 0)!"
        );
    }

    [Pure]
    public static ArgumentOutOfRangeException WasNotNegative<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentOutOfRangeException(
            parameterName,
            actualValue,
            $"{GetPreamble(actualValue, parameterName, methodName)}: Must be negative (x < 0)!"
        );
    }

    [Pure]
    public static ArgumentException WasNotANumericValue<T>(T? actualValue, string parameterName, string methodName) {
        var preamble = GetPreamble(actualValue, parameterName, methodName);
        var message  = $"{preamble}: [{typeof(T).Prettify()}]{actualValue} was not a numeric value!";
        return new ArgumentException(message, parameterName);
    }

    #endregion

    #region Switches

    [Pure]
    public static ArgumentException UnhandledSwitchBranch<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentException($"{GetPreamble(actualValue, parameterName, methodName)}: Value was unhandled by any switch branch!");
    }

    [Pure]
    public static ArgumentException UnhandledSwitchType<T>(T? actualValue, string parameterName, string methodName) {
        return new ArgumentException($"{GetPreamble(actualValue, parameterName, methodName)}: Value of type {actualValue?.GetType() ?? typeof(T)} was unhandled by any switch branch!");
    }

    [Pure]
    public static InvalidEnumArgumentException UnhandledSwitchEnum<T>(
        T? actualValue,
        string parameterName,
        string methodName
    ) where T : Enum {
        return new InvalidEnumArgumentException(
            $"{GetPreamble(actualValue, parameterName, methodName)}: The {typeof(T).Name} value [{actualValue.OrNullPlaceholder()}] was not handled by any branches of the switch statement!"
        );
    }

    #endregion

    #region Strings

    [Pure]
    public static ArgumentException WasBlank(string? actualValue, string parameterName, string methodName) {
        return new ArgumentException($"{GetPreamble(actualValue, parameterName, methodName)}: Must not be blank (null, empty, or whitespace)!");
    }

    [Pure]
    public static ArgumentException WasEmpty(string? actualValue, string parameterName, string methodName) {
        return new ArgumentException($"{GetPreamble(actualValue, parameterName, methodName)}: Must not be null or an empty string!");
    }

    #endregion
}