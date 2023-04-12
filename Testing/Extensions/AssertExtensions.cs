using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        Assert.That(actual, Is.EqualTo(expected).Using(comparer), $"{_actual} == {_expected}");
        return actual;
    }

    public static void AssertResult<IN, OUT>(
        this Func<IN, OUT>                            function,
        IN                                            input,
        OUT                                           expected,
        [CallerArgumentExpression("input")]    string _input    = "",
        [CallerArgumentExpression("function")] string _function = ""
    ) {
        Assert.That(() => function(input), Is.EqualTo(expected), () => FormatLambda(_function, _input));
    }

    private static StringBuilder AppendValue<T>(this StringBuilder stringBuilder, T? value) {
        if (value is null) {
            return stringBuilder.Append($"⛔({typeof(T).Name})");
        }

        if (value is Array arr) {
            stringBuilder.Append('[');

            for (int i = 0; i < arr.Length; i++) {
                if (i != 0) {
                    stringBuilder.Append(',');
                }

                stringBuilder.Append(arr.GetValue(i));
            }

            stringBuilder.Append(']');
            return stringBuilder;
        }

        return stringBuilder.Append(value);
    }

    private static RoSpan<T> Wrap<T>(this ReadOnlySpan<T> span) => new(span.ToImmutableArray());

    private readonly record struct RoSpan<T>(ImmutableArray<T> AsArray) {
        private const string Label = "↷";

        public override string ToString() {
            return typeof(T) == typeof(char) ? $"{Label}\"{AsArray.AsSpan().ToString()}\"" : $"{Label}[{string.Join(", ", AsArray)}]";
        }

        public static implicit operator ReadOnlySpan<T>(RoSpan<T> wrapper) {
            return wrapper.AsArray.AsSpan();
        }

        public bool Equals(RoSpan<T> other) {
            return AsArray.SequenceEqual(other.AsArray);
        }

        public override int GetHashCode() {
            return AsArray.GetHashCode();
        }
    }

    private static string FormatLambda(string expression, params object?[] args) {
        void AppendMethodReference(StringBuilder sb1) {
            sb1.Append(expression)
               .Append('(');

            for (int i = 0; i < args.Length; i++) {
                if (i > 0) {
                    sb1.Append(", ");
                }

                sb1.AppendValue(args[i]);
            }

            sb1.Append(')');
        }

        void AppendLambda(StringBuilder stringBuilder, LambdaExpressionSyntax lambdaExpressionSyntax) {
            var parms = lambdaExpressionSyntax switch {
                ParenthesizedLambdaExpressionSyntax p => p.ParameterList.Parameters,
                SimpleLambdaExpressionSyntax s        => SyntaxFactory.SingletonSeparatedList(s.Parameter),
                _                                     => throw Reject.Unreachable()
            };

            if (parms.Count != args.Length) {
                throw new ArgumentOutOfRangeException($"Mismatch in number of expression parameters ({parms.Count}) and argument strings ({args.Length})!");
            }

            stringBuilder.Append('(');

            for (int i = 0; i < parms.Count; i++) {
                if (i > 0) {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append(parms[i])
                             .AppendValue(args[i]);
            }

            stringBuilder.Append($") => {lambdaExpressionSyntax.Body}");
        }

        var sb  = new StringBuilder();
        var exp = SyntaxFactory.ParseExpression(expression).NormalizeWhitespace();

        if (exp is LambdaExpressionSyntax lambda) {
            AppendLambda(sb, lambda);
        }
        else {
            AppendMethodReference(sb);
        }

        return sb.ToString();
    }

    public static void AssertResult<IN, OUT>(
        this ReadOnlySpan<IN>                     input,
        ReadOnlySpanFunc<IN, OUT>                 code,
        OUT                                       expected,
        [CallerArgumentExpression("code")] string _code = ""
    ) {
        var inp = input.Wrap();
        Assert.That(() => code(inp), Is.EqualTo(expected), () => FormatLambda(_code, inp));
    }

    public static void AssertResult<IN, OUT>(
        this ReadOnlySpan<IN>                     input,
        RoSpanTransformer<IN, OUT>                code,
        ReadOnlySpan<OUT>                         expected,
        [CallerArgumentExpression("code")] string _code = ""
    ) {
        var arr = input.Wrap();
        var exp = expected.Wrap();
        Assert.That(() => code(arr).Wrap(), Is.EqualTo(exp), () => FormatLambda(_code, arr));
    }

    public static void AssertResult<IN, ARG, OUT>(
        this ReadOnlySpan<IN>     input,
        ARG                       arg,
        RoSpanMixer<IN, ARG, OUT> transformation,
        ReadOnlySpan<OUT>         expected,
        [CallerArgumentExpression("transformation")]
        string _transformation = ""
    ) {
        var arr = input.Wrap();
        var exp = expected.Wrap();
        Assert.That(() => transformation(arr, arg).Wrap(), Is.EqualTo(exp), FormatLambda(_transformation, arr, arg));
    }

    public static ReadOnlySpan<T> AssertEquals<T>(
        this ReadOnlySpan<T>                           actual,
        ReadOnlySpan<T>                                expected,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default
    ) {
        var act = actual.Wrap();
        var exp = expected.Wrap();
        Assert.That(act, Is.EqualTo(exp), () => $"{_actual} == {_expected}");
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