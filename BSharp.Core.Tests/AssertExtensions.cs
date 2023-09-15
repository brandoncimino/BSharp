using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Core.Tests;

public static class AssertExtensions {
    public static T AssertEquals<T>(
        this T                                         actual,
        T                                              expected,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default
    ) {
        Assert.That(actual, Is.EqualTo(expected), $"`{_actual}` equals `{_expected}`");
        return actual;
    }

    public static T AssertNotEquals<T>(this T actual, T notExpected, [CallerArgumentExpression("actual")] string? _actual = default, [CallerArgumentExpression("notExpected")] string? _notExpected = default) {
        Assert.That(actual, Is.Not.EqualTo(notExpected), $"`{_actual}` does not equal `{_notExpected}`");
        return actual;
    }

    public static T AssertSame<T>(this T actual, T expected, [CallerArgumentExpression("actual")] string? _actual = default, [CallerArgumentExpression("expected")] string? _expected = default) {
        Assert.That(actual, Is.SameAs(expected), $"`{_actual}` is the same object as `{_expected}`");
        return actual;
    }
}