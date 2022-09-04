using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

public partial class Must {
    /// <summary>
    /// Throws a <see cref="RejectionException"/> if <paramref name="actual"/> and <paramref name="expected"/> are not equal according to the <see cref="EqualityComparer{T}.Default"/> equality comparer.
    /// </summary>
    /// <param name="actual">the value that we <i>have</i></param>
    /// <param name="expected">the value that we <i>want</i></param>
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
        string?                                        details   = default,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default,
        [CallerMemberName]                     string? _caller   = default
    ) {
        if (ReferenceEquals(actual, expected)) {
            return actual;
        }

        if (EqualityComparer<T?>.Default.Equals(actual, expected)) {
            return actual;
        }

        throw new RejectionException(actual, details, _actual, _caller, $"({_actual}: [{actual}]) must equal ({_expected}: [{expected}])");
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
        bool    condition,
        string? details = default,
        [CallerArgumentExpression("condition")]
        string? _condition = default,
        [CallerMemberName] string? _caller = default
    ) {
        Debug.Assert(true);
        if (condition) {
            return;
        }

        throw new RejectionException(details: $"{_condition} was {condition}!", _caller: _caller);
    }
}