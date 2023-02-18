using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Enums;

using NUnit.Framework;

namespace FowlFever.Testing.Extensions;

public static class AssertExtensions {
    public enum EqualityStyle { Default, OpEquality, ReferenceEquals }

    private static IEqualityComparer<T> GetEqualityComparer<T>(this EqualityStyle style) {
        return style switch {
            EqualityStyle.Default         => EqualityComparer<T>.Default,
            EqualityStyle.OpEquality      => new DelegateEqualityComparer<T>(static (x, y) => CSharpOperator.Equality.InvokeBoolean(x, y)),
            EqualityStyle.ReferenceEquals => new DelegateEqualityComparer<T>(static (x, y) => ReferenceEquals(x, y)),
            _                             => throw BEnum.UnhandledSwitch(style),
        };
    }

    private static string Format<T>(
        this EqualityStyle                             style,
        T?                                             actual,
        T?                                             expected,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default
    ) {
        return style switch {
            EqualityStyle.Default         => $"Equals({_actual}: {actual}, {_expected}: {expected})",
            EqualityStyle.OpEquality      => $"({_actual}: {actual} == {_expected}: {expected}",
            EqualityStyle.ReferenceEquals => $"ReferenceEquals({_actual}: {actual}, {_expected}: {expected}",
            _                             => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
    }

    private static bool InvokeEquality<T>(T? a, T? b, EqualityStyle style) {
        return style.GetEqualityComparer<T>().Equals(a, b);
    }

    private record DelegateEqualityComparer<T>(Func<T?, T?, bool> Equality, Func<T, int>? Hasher = default) : IEqualityComparer<T> {
        public bool Equals(T? x, T? y) {
            return Equality(x, y);
        }

        public int GetHashCode(T obj) {
            return Hasher?.Invoke(obj) ?? obj?.GetHashCode() ?? default;
        }
    }

    /// <summary>
    /// The simplest <see cref="AssertExtensions"/> method.
    /// </summary>
    /// <param name="actual">the value that you have</param>
    /// <param name="expected">the value that you want</param>
    /// <param name="comparer">how the <typeparamref name="T"/> values should be compared. Defaults to <see cref="EqualityComparer{T}.Default"/></param>
    /// <param name="_actual">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_expected">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of the checked values</typeparam>
    /// <returns>the <paramref name="actual"/> value</returns>
    public static T AssertEquals<T>(
        this T                                         actual,
        T                                              expected,
        IEqualityComparer<T>?                          comparer  = default,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default
    ) {
        comparer ??= EqualityComparer<T>.Default;
        Assert.That(actual, Is.EqualTo(expected).Using(comparer));
        return actual;
    }

    public static bool AssertTrue(this bool actual, [CallerArgumentExpression("actual")] string? _actual = default) {
        Assert.That(actual, Is.True, _actual);
        return actual;
    }

    public static T AssertEquals<T>(
        this T                                         actual,
        T                                              expected,
        EqualityStyle                                  equalityStyle,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default
    ) {
        Assert.That(() => InvokeEquality(expected, actual, equalityStyle), Is.True, equalityStyle.Format(actual, expected, _actual, _expected));
        return actual;
    }
}