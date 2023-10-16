using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

using NUnit.Framework.Constraints;

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

    public static void AssertCommutative<ACTUAL, EXPECTED>(
        ACTUAL                         a,
        ACTUAL                         b,
        Func<ACTUAL, ACTUAL, EXPECTED> commutativeFunction,
        EXPECTED                       expected,
        [CallerArgumentExpression("commutativeFunction")]
        string? _commutativeFunction = default
    ) {
        var ab = commutativeFunction(a, b);
        var ba = commutativeFunction(b, a);

        Assert.That(
            new { ab, ba },
            Is.EqualTo(new { ab = expected, ba = expected }),
            $"""
             {
                 _commutativeFunction
             } is commutative:
                a: {
                    a
                }
                b: {
                    b
                }
                
                Expected: {
                    expected
                }
                  (a, b): {
                      ab
                  }
                  (b, a): {
                      ba
                  }
             """
        );
    }

    public static void AssertThat<T>(T actual, IResolveConstraint constraint, [CallerArgumentExpression("actual")] string? _actual = default) {
        Assert.That(actual, constraint, _actual);
    }

    public static void Assert_Equality<T>(T? a, T? b, bool expectedEquality) where T : IEquatable<T>, IEqualityOperators<T, T, bool> {
        Assert.Multiple(
            () => {
                if (a == null || b == null) {
                    Assert.Fail($"Neither of the inputs should have been null! ({(a, b)})");
                    return;
                }

                AssertCommutative(a, b, static (x, y) => x.Equals(y),         expectedEquality);
                AssertCommutative(a, b, static (x, y) => x.Equals((object)y), expectedEquality);
                AssertCommutative(a, b, static (x, y) => Equals(x, y),        expectedEquality);
                AssertCommutative(a, b, static (x, y) => x == y,              expectedEquality);
                AssertCommutative(a, b, static (x, y) => x != y,              !expectedEquality);
                AssertCommutative(a, b, EqualityComparer<T>.Default.Equals,                                  expectedEquality);
                AssertCommutative(a, b, (Func<object, object, bool>)EqualityComparer<object>.Default.Equals, expectedEquality);
                AssertCommutative(a, b, static (x, y) => EqualityComparer<object>.Default.Equals(x, y),      expectedEquality);

                AssertThat(a.ToString()?.Equals(b.ToString(), StringComparison.Ordinal), Is.EqualTo(expectedEquality));
            }
        );
    }

    public static void Assert_Parses<T>(string input, T expected) where T : IParsable<T>, ISpanParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
        Assert.Multiple(
            () => {
                Stringy(input, expected);

                // spanny
                AssertThat(T.TryParse(input.AsSpan(), null, out var spanny), Is.True);
                Assert_Equality(spanny,                        expected, true);
                Assert_Equality(T.Parse(input.AsSpan(), null), expected, true);
            }
        );
        return;

        static void Stringy<X>(string input, X expected) where X : IParsable<X>, IEquatable<X>, IEqualityOperators<X, X, bool> {
            // stringy
            AssertThat(X.TryParse(input, null, out var stringy), Is.True);
            Assert_Equality(stringy,              expected, true);
            Assert_Equality(X.Parse(input, null), expected, true);
        }
    }

    public static void Assert_NoParse<T>(string input) where T : IParsable<T>, ISpanParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
        Assert.Multiple(
            () => {
                Stringy<T>(input);

                AssertThat(T.TryParse(input.AsSpan(), null, out var spanny), Is.False);
                Assert_Equality(spanny, default, true);
                AssertThat(() => T.Parse(input.AsSpan(), null), Throws.InstanceOf<FormatException>());
            }
        );
        return;

        static void Stringy<X>(string input) where X : IParsable<X>, IEquatable<X>, IEqualityOperators<X, X, bool> {
            AssertThat(X.TryParse(input, null, out var stringy), Is.False);
            Assert_Equality(stringy, default, true);
            AssertThat(() => X.Parse(input, null), Throws.InstanceOf<FormatException>());
        }
    }
}