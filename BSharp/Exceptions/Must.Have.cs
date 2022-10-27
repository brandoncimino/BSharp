using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Exceptions;

public partial class Must {
    /// <summary>
    /// Returns <see cref="RejectionException"/> unless <paramref name="condition"/> equals <see cref="expected"/> according to <paramref name="equality"/>.
    /// </summary>
    /// <param name="paramName">the <see cref="ArgumentException.ParamName"/></param>
    /// <param name="condition">the value being tested</param>
    /// <param name="expected">the expected value of <paramref name="condition"/></param>
    /// <param name="details">additional user-provided details</param>
    /// <param name="_condition">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_expected">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="equality">the <see cref="IEqualityComparer{T}"/> used to compare <paramref name="condition"/> to <paramref name="expected"/>. Defaults to <see cref="EqualityComparer{T}.Default"/></param>
    /// <typeparam name="T">the type of <paramref name="condition"/> and <paramref name="expected"/></typeparam>
    /// <returns>a <see cref="RejectionException"/> if <paramref name="condition"/> != <paramref name="expected"/>; otherwise, <c>null</c></returns>
    [Pure]
    private static RejectionException? _CheckCondition<T>(
        string?                paramName,
        T?                     condition,
        T?                     expected,
        string?                details,
        string?                _condition,
        string?                _expected,
        string?                _caller,
        IEqualityComparer<T?>? equality = default
    ) {
        if (ReferenceEquals(condition, expected)) {
            return default;
        }

        equality ??= EqualityComparer<T?>.Default;
        if (equality.Equals(condition, expected)) {
            return default;
        }

        static string EqString(T? expected, string? _expected) => expected?.ToString() == _expected ? _expected.OrNullPlaceholder() : $"({_expected}: [{expected}])";

        var msg = $"The condition ({_condition} => {condition}) must evaluate to {EqString(expected, _expected)}!";
        return new RejectionException(condition, details, paramName, _caller, msg);
    }

    /// <summary>
    /// Throws a <see cref="RejectionException"/> if <paramref name="actual"/> and <paramref name="expected"/> are not equal according to the to the given <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="actual">the value that we <i>have</i></param>
    /// <param name="expected">the value that we <i>want</i></param>
    /// <param name="equality">the <see cref="IEqualityComparer{T}"/> used to compare <paramref name="actual"/> to <paramref name="expected"/>. Defaults to <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <param name="details">optional additional details</param>
    /// <param name="_actual">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_expected">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="actual"/></typeparam>
    /// <returns><paramref name="actual"/>, <b><i>if</i></b> it was equal to <paramref name="expected"/></returns>
    /// <exception cref="RejectionException">if <paramref name="actual"/> is <b><i>not</i></b> equal to <paramref name="expected"/></exception>
    [return: NotNullIfNotNull("actual")]
    public static T? Have<T>(
        T?                                             actual,
        T?                                             expected,
        IEqualityComparer<T?>?                         equality  = default,
        string?                                        details   = default,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default,
        [CallerMemberName]                     string? _caller   = default
    ) {
        _CheckCondition(_actual, actual, expected, details, _actual, _expected, _caller)?.Assert();
        return actual;
    }

    /// <summary>
    /// Throws a <see cref="RejectionException"/> if <paramref name="condition"/> is <c>false</c>.
    /// </summary>
    /// <param name="condition">the <b><i>result</i></b> of some validation</param>
    /// <param name="details">optional additional details describing the <paramref name="condition"/></param>
    /// <param name="_condition">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <exception cref="RejectionException">if <paramref name="condition"/> is <c>false</c></exception>
    public static void Have(
        [DoesNotReturnIf(false)] bool condition,
        string?                       details = default,
        [CallerArgumentExpression("condition")]
        string? _condition = default,
        [CallerMemberName] string? _caller = default
    ) {
        if (condition) {
            return;
        }

        throw new RejectionException(
            details: details,
            _caller: _caller,
            reason: $"{_condition} must be true"
        );
    }

    /// <summary>
    /// Throws a <see cref="RejectionException"/> unless <paramref name="condition"/> equals <paramref name="expected"/> according to the given <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="argumentBeingValidated">the actual argument that we are validating (as opposed to <paramref name="condition"/>, which is likely derived from <paramref name="argumentBeingValidated"/>)</param>
    /// <param name="condition">the value we are using to determine the validity of <paramref name="argumentBeingValidated"/></param>
    /// <param name="expected">the expected value of <paramref name="condition"/></param>
    /// <param name="details">optional additional details</param>
    /// <param name="_argumentBeingValidated">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_condition">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_expected">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="argumentBeingValidated"/></typeparam>
    /// <typeparam name="TCheck">the type of <paramref name="condition"/></typeparam>
    /// <returns><paramref name="argumentBeingValidated"/>, for method chaining</returns>
    /// <exception cref="RejectionException">if <paramref name="condition"/> != <paramref name="expected"/></exception>
    public static T MustHave<T, TCheck>(
        this T  argumentBeingValidated,
        TCheck  condition,
        TCheck  expected,
        string? details = default,
        [CallerArgumentExpression("argumentBeingValidated")]
        string? _argumentBeingValidated = default,
        [CallerArgumentExpression("condition")]
        string? _condition = default,
        [CallerArgumentExpression("expected")] string? _expected = default,
        [CallerMemberName]                     string? _caller   = default
    ) {
        _CheckCondition(_argumentBeingValidated, condition, expected, details, _condition, _expected, _caller)?.Assert();
        return argumentBeingValidated;
    }

    /// <summary>
    /// Throws a <see cref="RejectionException"/> if <paramref name="condition"/> is <c>false</c>.
    /// </summary>
    /// <param name="argumentBeingValidated">the actual argument that we are validating (as opposed to <paramref name="condition"/>, which should be <i>derived</i> from <paramref name="argumentBeingValidated"/>)</param>
    /// <param name="condition">the evaluated expression that we want to be <c>true</c></param>
    /// <param name="details">optional additional details</param>
    /// <param name="_argumentBeingValidated"><see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_condition">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="argumentBeingValidated"/></typeparam>
    /// <returns><paramref name="argumentBeingValidated"/>, for method chaining</returns>
    /// <exception cref="RejectionException">if <paramref name="condition"/> is <c>false</c></exception>
    public static T MustHave<T>(
        this                     T    argumentBeingValidated,
        [DoesNotReturnIf(false)] bool condition,
        string?                       details = default,
        [CallerArgumentExpression("argumentBeingValidated")]
        string? _argumentBeingValidated = default,
        [CallerArgumentExpression("condition")]
        string? _condition = default,
        [CallerMemberName] string? _caller = default
    ) {
        _CheckCondition(_argumentBeingValidated, condition, true, details, _argumentBeingValidated, _condition, _caller)?.Assert();
        return argumentBeingValidated;
    }
}