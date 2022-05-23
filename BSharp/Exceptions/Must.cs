using System;
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
    /// <param name="parameterName">the name of the parameter with which <paramref name="actualValue"/> corresponds. 📎 Automatically populated with <paramref name="actualValue"/>'s expression via <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="methodName">the entity that is doing the validation - usually a method. 📎 Automatically populated via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="reason">a description of what we <b>wanted</b> to happen. 📎 Automatically populated with <paramref name="predicate"/> via <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of the <paramref name="actualValue"/></typeparam>
    /// <returns><paramref name="actualValue"/>, <b>if</b> it satisfies the <paramref name="predicate"/></returns>
    /// <exception cref="ArgumentException">if the <paramref name="predicate"/> returns <c>false</c></exception>
    /// <exception cref="ArgumentNullException">if <paramref name="actualValue"/> is <c>null</c></exception>
    [return: NotNullIfNotNull("predicate")]
    public static T Be<T>(
        [NotNullIfNotNull("predicate")]
        T actualValue,
        Func<T, bool>? predicate,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default,
        [CallerArgumentExpression("predicate")]
        string? reason = default
    ) {
        predicate ??= it => it == null;
        if ((predicate, actualValue) is (null, null)) {
            return actualValue;
        }

        switch (predicate, actualValue) {
            case (null, null):     return actualValue;
            case (null, not null): throw Reject(actualValue, parameterName, methodName, "must be null");
        }

        NotBeNull(actualValue, parameterName, methodName);

        Exception? exc = null;

        try {
            if (predicate(actualValue)) {
                return actualValue;
            }
        }
        catch (Exception e) {
            exc = e;
        }

        throw Reject(actualValue, parameterName, methodName, reason, exc);
    }

    public static T NotBe<T>(
        T actualValue,
        [InstantHandle]
        Func<T, bool> predicate,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default,
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

        throw Reject(actualValue, parameterName, methodName, reason, exc);
    }

    public static T MustBe<T>(
        this T        actualValue,
        Func<T, bool> predicate,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default,
        [CallerArgumentExpression("predicate")]
        string? reason = null
    ) {
        return Be(actualValue, predicate, parameterName, methodName, reason);
    }

    public static T MustNotBe<T>(
        this T actualValue,
        [InstantHandle]
        Func<T, bool> predicate,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? methodName = default,
        [CallerArgumentExpression("predicate")]
        string? reason = null
    ) {
        return NotBe(actualValue, predicate, parameterName, methodName, reason);
    }

    #endregion

    #region Types

    public static T Be<T>(
        object actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        if (actualValue is T t) {
            return t;
        }

        throw Reject(actualValue, parameterName, rejectedBy, reason: $"was not an instance of {typeof(T).PrettifyType(default)}");
    }

    public static T MustBe<T>(
        this object actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) => Be<T>(actualValue, parameterName, rejectedBy);

    #endregion

    #region 2-arg Comparisons

    public static (T first, T2 second) Compare<T, T2>(
        T                  first,
        ComparisonOperator comparison,
        T2                 second,
        [CallerArgumentExpression("first")]
        string? firstName = default,
        [CallerArgumentExpression("second")]
        string? secondName = default,
        [CallerMemberName]
        string? methodName = default,
        [CallerArgumentExpression("comparison")]
        string? reason = default
    ) {
        return Be(
            (first, second),
            (tuple) => comparison.Predicate().Invoke(tuple.Item1, tuple.Item2),
            (firstName, secondName).ToString(),
            methodName,
            $"must have {firstName} {comparison.Symbol()} {secondName}"
        );
    }

    #endregion
}