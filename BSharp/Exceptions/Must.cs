using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings.Prettifiers;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// Methods for validating arguments and generating <see cref="Exception"/>s.
/// </summary>
/// <remarks>
/// Each of these methods should return the original value if their validation succeeds.
/// <br/>
/// <see cref="Exception"/> objects should be created by <see cref="Reject{T}"/> methods.
/// </remarks>
[PublicAPI]
[StackTraceHidden]
public static partial class Must {
    #region Arbitration

    /// <summary>
    /// Throws an <see cref="ArgumentException"/>if <paramref name="actualValue"/> doesn't satisfy the given <paramref name="predicate"/>.
    /// </summary>
    /// <remarks>
    /// 📎 An <paramref name="actualValue"/> of <c>null</c> will <b>always</b> fail, matching the behavior of <c>is</c> expressions.
    /// 
    /// Conversely, <see cref="BeNull{T}"/> will always <b>pass</b> for <c>null</c>. 
    /// </remarks>
    /// <param name="actualValue">the <typeparamref name="T"/> value being validated</param>
    /// <param name="predicate">a condition that <paramref name="actualValue"/> must satisfy</param>
    /// <param name="details">user-provided additional details</param>
    /// <param name="parameterName">the name of the parameter with which <paramref name="actualValue"/> corresponds. 📎 Automatically populated with <paramref name="actualValue"/>'s expression via <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">the entity that is doing the validation - usually a method. 📎 Automatically populated via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="reason">a description of what we <b>wanted</b> to happen. 📎 Automatically populated with <paramref name="predicate"/> via <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of the <paramref name="actualValue"/></typeparam>
    /// <returns><paramref name="actualValue"/>, <b>if</b> it satisfies the <paramref name="predicate"/></returns>
    /// <exception cref="RejectionException">if the <paramref name="predicate"/> returns <c>false</c></exception>
    /// <exception cref="RejectionException">if <paramref name="actualValue"/> is <c>null</c></exception>
    [return: NotNullIfNotNull("predicate")]
    public static T Be<T>(
        [NotNullIfNotNull("predicate")]
        T actualValue,
        Func<T, bool>? predicate,
        string?        details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("predicate")]
        string? reason = default
    ) {
        switch (predicate, actualValue) {
            case (null, null):     return actualValue;
            case (null, not null): throw Reject(actualValue, parameterName, rejectedBy, "must be null");
        }

        NotBeNull(actualValue, parameterName, rejectedBy);

        Exception? exc = null;

        try {
            if (predicate(actualValue)) {
                return actualValue;
            }
        }
        catch (Exception e) {
            exc = e;
        }

        throw Reject(actualValue, details, parameterName, rejectedBy, reason, exc);
    }

    /// <inheritdoc cref="Be{T}(T,System.Func{T,bool}?,string?,string?,string?,string?)"/>
    public static T Be<T>(
        T       actualValue,
        T       expectedValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("expectedValue")]
        string? reason = default
    ) {
        return Be(
            actualValue: actualValue,
            predicate: it => Equals(it, expectedValue),
            details: details,
            parameterName: parameterName,
            rejectedBy: rejectedBy,
            reason: $"must equal {reason} ({expectedValue})"
        );
    }

    /// <inheritdoc cref="Be{T}(T,System.Func{T,bool}?,string?,string?,string?,string?)"/>
    public static T Equal<T>(
        T       actualValue,
        T       expectedValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("expectedValue")]
        string? reason = default
    ) {
        return Be(actualValue, expectedValue, details, parameterName, rejectedBy, reason);
    }

    /// <inheritdoc cref="Be{T}(object,string?,string?,string?)"/>
    public static void Be(
        bool?   predicateResult,
        string? details = default,
        [CallerArgumentExpression("predicateResult")]
        string? description = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        Be(predicateResult, true, details, description, rejectedBy);
    }

    public static T NotBe<T>(
        T actualValue,
        [InstantHandle]
        Func<T, bool> predicate,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("predicate")]
        string? reason = default
    ) {
        Exception? exc = default;

        try {
            if (!predicate(actualValue)) {
                return actualValue!;
            }
        }
        catch (Exception e) {
            exc = e;
        }

        throw Reject(actualValue, details, parameterName, rejectedBy, reason, exc);
    }

    /// <inheritdoc cref="NotEqual{T}"/>
    public static T NotBe<T>(
        T       actualValue,
        T       expectedValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("expectedValue")]
        string? reason = default
    ) {
        return NotEqual(
            actualValue,
            expectedValue,
            details,
            parameterName,
            rejectedBy,
            $"must NOT equal {reason} ({expectedValue})"
        );
    }

    public static T NotEqual<T>(
        T       actualValue,
        T       expectedValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("expectedValue")]
        string? reason = default
    ) {
        return Be(
            actualValue: actualValue,
            predicate: it => !Equals(it, expectedValue),
            details: details,
            parameterName: parameterName,
            rejectedBy: rejectedBy,
            reason: $"must NOT equal {reason}"
        );
    }

    public static T MustBe<T>(
        this T        actualValue,
        Func<T, bool> predicate,
        string?       details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("predicate")]
        string? reason = null
    ) {
        return Be(actualValue, predicate, details, parameterName, rejectedBy, reason);
    }

    public static T MustNotBe<T>(
        this T actualValue,
        [InstantHandle]
        Func<T, bool> predicate,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("predicate")]
        string? reason = null
    ) {
        return NotBe(actualValue, predicate, details, parameterName, rejectedBy, reason);
    }

    #endregion

    #region Types

    /// <summary>
    /// Requires that <paramref name="actualValue"/> is an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="actualValue">the value being tested</param>
    /// <param name="details">optional additional details</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the desired output <see cref="Type"/></typeparam>
    /// <returns><paramref name="actualValue"/> cast to type <typeparamref name="T"/></returns>
    /// <exception cref="RejectionException">if <paramref name="actualValue"/> isn't an instance of <typeparamref name="T"/></exception>
    public static T Be<T>(
        object  actualValue,
        string? details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        if (actualValue is T t) {
            return t;
        }

        throw Reject(actualValue, details, parameterName, rejectedBy, reason: $"was not an instance of {typeof(T).PrettifyType(default)}");
    }

    /// <inheritdoc cref="Be{T}(object,string?,string?,string?)"/>
    public static T MustBe<T>(
        this object actualValue,
        string?     details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) => Be<T>(actualValue, details, parameterName, rejectedBy);

    #endregion

    #region 2-arg Comparisons

    public static (T first, T2 second) Compare<T, T2>(
        T                  first,
        ComparisonOperator comparison,
        T2                 second,
        string?            details = default,
        [CallerArgumentExpression("first")]
        string? firstName = default,
        [CallerArgumentExpression("second")]
        string? secondName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("comparison")]
        string? reason = default
    ) {
        return Be(
            (first, second),
            (tuple) => comparison.Predicate().Invoke(tuple.Item1, tuple.Item2),
            details,
            (firstName, secondName).ToString(),
            rejectedBy,
            $"must have {firstName} {comparison.Symbol()} {secondName}"
        );
    }

    #endregion

    #region Actions

    [return: NotNullIfNotNull("actualValue")]
    public static T NotThrow<T>(
        T         actualValue,
        Action<T> assertion,
        string?   details = default,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default,
        [CallerArgumentExpression("assertion")]
        string? reason = default
    ) {
        try {
            assertion(actualValue);
            return actualValue;
        }
        catch (Exception e) {
            throw new RejectionException(actualValue, details, parameterName, rejectedBy, reason, e);
        }
    }

    #endregion
}