using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    #region Strings

    #region Blankness

    #region Muster-extensions

    [return: NotNull]
    public static T NotBlank<T>(this Muster<T, string> muster, string? details = default) {
        if (muster.ValidationTarget.IsBlank()) {
            throw muster.Reject(details);
        }

        return muster.TrueSelf!;
    }

    [return: NotNull]
    public static T NotEmpty<T>(this Muster<T, string> muster, string? details = default) {
        if (muster.ValidationTarget.IsEmpty()) {
            throw muster.Reject(details);
        }

        return muster.TrueSelf;
    }

    #endregion

    public static string NotBeBlank(
        [NotNull] string? actualValue,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        if (!actualValue.IsNotBlank()) {
            throw Reject(actualValue, details: details, parameterName: parameterName, rejectedBy: rejectedBy, reason: nameof(NotBeBlank));
        }

        return actualValue!;
    }

    public static string NotBeEmpty(
        [NotNull] string? actualValue,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        if (!actualValue.IsNotEmpty()) {
            throw Reject(actualValue, details: details, parameterName: parameterName, rejectedBy: rejectedBy, reason: nameof(NotBeEmpty));
        }

        return actualValue;
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
        [CallerMemberName] string? rejectedBy = default
    ) {
        if (actualValue?.Contains(unwantedString) == true) {
            throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: rejectedBy, reason: $"must NOT contain the substring \"{unwantedString}\"");
        }

        return actualValue;
    }

    public static string Contain(
        [NotNull] string? actualValue,
        string            substring,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        if (actualValue?.Contains(substring) == true) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: rejectedBy, reason: $"must contain the substring \"{substring}\"");
    }

    #endregion

    #region Matching

    public static string Match(
        [NotNull] string? actualValue,
        Regex             pattern,
        string?           details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        if (actualValue?.Matches(pattern) == true) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: rejectedBy, reason: $"must match the {nameof(Regex)} pattern /{pattern}/");
    }

    [return: NotNullIfNotNull("actualValue")]
    public static string? NotMatch(
        string? actualValue,
        Regex   pattern,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName] string? rejectedBy = default
    ) {
        if (actualValue?.Matches(pattern) != true) {
            return actualValue;
        }

        throw Reject(actualValue, details, parameterName: parameterName, rejectedBy: rejectedBy, reason: $"must NOT match the {nameof(Regex)} pattern /{pattern}/");
    }

    #endregion

    #endregion
}