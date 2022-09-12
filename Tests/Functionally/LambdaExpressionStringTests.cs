using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp;
using FowlFever.BSharp.Functionally;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Functionally;

public class LambdaExpressionStringTests {
    public readonly struct AutoRegex {
        [MaybeNull] private readonly Regex _pattern;

        public Regex Pattern => _pattern ?? new Regex(".*");

        public AutoRegex(string exactMatch) {
            _pattern = new Regex(Regex.Escape(exactMatch));
        }

        public AutoRegex(Regex regex) {
            _pattern = regex;
        }

        public static implicit operator AutoRegex(Regex  regex)      => new(regex);
        public static implicit operator AutoRegex(string exactMatch) => new(exactMatch);
        public static implicit operator Regex(AutoRegex  regex)      => regex.Pattern;
    }

    public record LambdaExpressionExpectation<T>(
        T                                             Delgato,
        AutoRegex                                     Modifier,
        AutoRegex                                     Parameters,
        AutoRegex                                     Body,
        [CallerArgumentExpression("Delgato")] string? _Expression = default
    ) {
        public void Invoke() {
            var exp = new LambdaExpressionString(_Expression);
            Brandon.Print(exp.ToString());
            Brandon.Print(exp.GetRenderable());
            Brandon.Print(exp.Modifier);
            Brandon.Print(exp.Parameters);
            Brandon.Print(exp.Body);
            Asserter.WithHeading(_Expression)
                    .And(exp.Body.ToString(),       Is.Match(Body))
                    .And(exp.Modifier.ToString(),   Is.Match(Modifier))
                    .And(exp.Parameters.ToString(), Is.Match(Parameters))
                    .Invoke();
        }
    }

    public static object[] Expectations() {
        return new object[] {
            new LambdaExpressionExpectation<Func<string, int>>(
                static s => s.Length,
                "static",
                "s",
                "s.Length"
            ),
            new LambdaExpressionExpectation<Action>(
                () => Console.WriteLine("YOLO"),
                "",
                "",
                "Console.WriteLine(\"YOLO\")"
            ),
            new LambdaExpressionExpectation<Action<string>>(
                (string s) => Console.WriteLine(s),
                default,
                "string s",
                "Console.WriteLine(s)"
            ),
            new LambdaExpressionExpectation<Action<string>>(
                Console.WriteLine,
                "",
                "",
                "Console.WriteLine"
            ),
            new LambdaExpressionExpectation<SpanAction<char, string>>(
                (Span<char> span, string s) => {
                    if (string.IsNullOrEmpty(s)) {
                        s.AsSpan().CopyTo(span);
                    }
                },
                "",
                "Span<char> span, string s",
                @"
if (string.IsNullOrEmpty(s)) {
    s.AsSpan().CopyTo(span);
}
".Trim()
            ),
            new LambdaExpressionExpectation<Func<string, string, string>>(
                static string(a, b) => a + b,
                "static",
                new Regex(@"string\s*\(a, b\)"),
                "a + b"
            )
        };
    }

    [Test]
    public void TestLambdaExpressionExpectation<T>([ValueSource(nameof(Expectations))] LambdaExpressionExpectation<T> expectation) {
        expectation.Invoke();
    }

    [TestCase(
        @"   a
   b",
        @"a
b"
    )]
    public void UnIndentTest(string input, string expected) {
        var undented = input.AsSpan().UnIndent();
        Assert.That(undented.ToString(), Is.EqualTo(expected));
    }
}