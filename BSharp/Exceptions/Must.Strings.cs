using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Strings

    #region Blankness

    public static string NotBeBlank(
        [NotNull] string? actualValue,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        if (actualValue.IsNotBlank()) {
            return actualValue!;
        }

        throw Reject(actualValue, details: details, parameterName: parameterName, rejectedBy: methodName, reason: nameof(NotBeBlank));
    }

    public static string NotBeEmpty(
        [NotNull] string? actualValue,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        if (actualValue.IsNotEmpty()) {
            return actualValue;
        }

        throw Reject(actualValue, details: details, parameterName: parameterName, rejectedBy: methodName, reason: nameof(NotBeEmpty));
    }

    #endregion

    #region Containment

    [return: NotNullIfNotNull("actualValue")]
    public static string? NotContain(
        string? actualValue,
        string  unwantedString,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        if (actualValue?.Contains(unwantedString) == true) {
            throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: methodName, reason: $"must NOT contain the substring \"{unwantedString}\"");
        }

        return actualValue;
    }

    [return: NotNullIfNotNull("actualValue")]
    public static string? NotContain(
        string?           actualValue,
        IEnumerable<char> unwantedCharacters,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        return Be(
            actualValue,
            it => it?.ContainsAny(unwantedCharacters) == true,
            parameterName,
            methodName
        );
    }

    public static string Contain(
        [NotNull] string? actualValue,
        string            substring,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        if (actualValue?.Contains(substring) == true) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: methodName, reason: $"must contain the substring \"{substring}\"");
    }

    #endregion

    #region Matching

    public static string Match(
        [NotNull] string? actualValue,
        Regex             pattern,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        if (actualValue?.Matches(pattern) == true) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: methodName, reason: $"must match the {nameof(Regex)} pattern /{pattern}/");
    }

    [return: NotNullIfNotNull("actualValue")]
    public static string? NotMatch(
        string? actualValue,
        Regex   pattern,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default
    ) {
        if (actualValue?.Matches(pattern) != true) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: methodName, reason: $"must NOT match the {nameof(Regex)} pattern /{pattern}/");
    }

    #endregion

    #endregion
}