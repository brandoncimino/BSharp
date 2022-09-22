using System;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Exceptions;

public partial class Must {
    /// <summary>
    /// Checks whether <paramref name="actual"/> == <paramref name="expected"/>, and if not, <b>returns</b> <i>(as opposed to throwing)</i> a <see cref="RejectionException"/>.
    /// </summary>
    /// <remarks>
    /// This is a "soft" equivalent to <see cref="Have{T}"/>.
    /// </remarks>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="details"></param>
    /// <param name="_actual"></param>
    /// <param name="_expected"></param>
    /// <param name="_caller"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <returns></returns>
    [MustUseReturnValue]
    public static Exception? Try<T, T2>(
        T?                                             actual,
        T2                                             expected,
        string?                                        details   = default,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default,
        [CallerMemberName]                     string? _caller   = default
    ) where T : IEquatable<T2> {
        if (ReferenceEquals(actual, expected) || actual?.Equals(expected) == true) {
            return default;
        }

        static string EqString<TX>(TX? expected, string? _expected) => expected?.ToString() == _expected ? _expected.OrNullPlaceholder() : $"({_expected}: [{expected}])";

        return new RejectionException(actual, details, _actual, _caller, $"({_actual}: [{actual}]) must equal {EqString(expected, _expected)}");
    }
}