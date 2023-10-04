using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Clerical.Tests;

public static class TestHelpers {
    private static (A? a, B? b) RequireMutuallyExclusive<A, B>(A? a, B? b) where A : notnull where B : notnull {
        if (a is null ^ b is null) {
            return (a, b);
        }

        var aStr = a?.ToString() ?? "⛔";
        var bStr = b?.ToString() ?? "⛔";
        throw new ArgumentException($"Exactly ONE of the two values must be non-null! (actual: 🅰 {aStr}, 🅱 {bStr})");
    }

    public readonly record struct Result<T>(StrongBox<T>? Value, Exception? Exception, string? Description) {
        public StrongBox<T>? Value     { get; } = RequireMutuallyExclusive(Value, Exception).a;
        public Exception?    Exception { get; } = RequireMutuallyExclusive(Value, Exception).b;

        public T RequireResult() {
            Assert.That(Exception, Is.Null, $"Exception thrown by {Description}");
            Debug.Assert(Value != null);
            return Value.Value!;
        }

        public void AssertFailure() {
            Assert.That(Exception, Is.Not.Null, $"Exception thrown by {Description}");
        }

        [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple")]
        public void AssertSuccess(T expected) {
            Assert.That(Exception,    Is.Null, $"Exception thrown by {Description}");
            Assert.That(Value!.Value, Is.EqualTo(expected));
        }
    }

    public static Result<T> ResultOf<T>(Func<T> function, [CallerArgumentExpression("function")] string? _function = default) {
        try {
            return new Result<T>(new StrongBox<T>(function()), null, _function);
        }
        catch (Exception e) {
            return new Result<T>(default, e, _function);
        }
    }
}