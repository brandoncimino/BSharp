using System;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing.NUnitExtensionPoints;

public abstract class Assert : NUnit.Framework.Assert {
    public static void True(Func<bool> condition, [CallerArgumentExpression("condition")] string? _condition = default) => NUnit.Framework.Assert.That(condition, _condition);
    public static void That(Func<bool> condition, [CallerArgumentExpression("condition")] string? _condition = default) => True(condition, _condition);

    public static void True(bool condition, [CallerArgumentExpression("condition")] string? _condition = default) => NUnit.Framework.Assert.True(condition, _condition);
    public static void That(bool condition, [CallerArgumentExpression("condition")] string? _condition = default) => True(condition, _condition);

    public static void That<T>(T actual, IConstraint constraint, [CallerArgumentExpression("actual")] string? _actual = default) => NUnit.Framework.Assert.That(actual, constraint, _actual);

    public static void That<T>(Func<T> actual, IConstraint constraint, [CallerArgumentExpression("actual")] string? _actual = default) {
        NUnit.Framework.Assert.That(actual, constraint, _actual);
    }

    public static void True<T>(
        T                                            actual,
        Func<T, bool>                                condition,
        IConstraint?                                 constraint = null,
        [CallerArgumentExpression("actual")] string? _actual    = default,
        [CallerArgumentExpression("condition")]
        string? _condition = default
    ) {
        constraint ??= Is.True;
        if (_actual == null || _condition == null) {
            Assert.That(() => condition(actual), constraint);
            return;
        }


        NUnit.Framework.Assert.That(
            () => condition(actual),
            constraint,
            () => FormatLambda(_condition, _actual) ?? $"[{_actual}] satisfies {_condition}"
        );
    }

    private static string? FormatLambda(string expression, string arg) {
        var syntax = SyntaxFactory.ParseExpression(expression);

        return syntax switch {
            MemberAccessExpressionSyntax or IdentifierNameSyntax => $"{syntax}({arg})",
            LambdaExpressionSyntax lambda                        => $"({GetSingleParam(lambda).Identifier}: {arg}) => {lambda.Body.NormalizeWhitespace()}",
            _                                                    => null
        };
    }

    private static ParameterSyntax GetSingleParam(LambdaExpressionSyntax lambda) {
        return lambda switch {
            SimpleLambdaExpressionSyntax simple       => simple.Parameter,
            ParenthesizedLambdaExpressionSyntax paren => paren.ParameterList.Parameters.Single(),
            _                                         => throw new ArgumentOutOfRangeException(nameof(lambda), lambda, null)
        };
    }
}