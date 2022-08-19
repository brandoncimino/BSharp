using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Attributes;

namespace FowlFever.BSharp.Exceptions;

public partial class Must {
    [Experimental($"Need to clean up all the different {nameof(Must)}.{nameof(Be)} overloads")]
    [return: NotNullIfNotNull("actual")]
    public static T? Have<T>(
        T?                                             actual,
        T?                                             expected,
        string?                                        details   = default,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default,
        [CallerMemberName]                     string? _caller   = default
    )
        where T : IEquatable<T?> {
        if (ReferenceEquals(actual, expected)) {
            return actual;
        }

        if (actual?.Equals(expected) == true) {
            return actual;
        }

        throw new RejectionException(actual, details, _actual, _caller, $"must == {expected}");
    }
}