using System;
using System.Linq.Expressions;
using System.Text;

using FowlFever.BSharp.Memory;

namespace FowlFever.BSharp.Functionally;

/// <summary>
/// Parses the <see cref="string"/> representation of a <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions">lambda expression</a> into distinct parts. 
/// </summary>
/// <remarks>
/// This class should only be used to format lambda expressions, such as those obtained via <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/>.
/// <p/>
/// For in-depth analysis of a lambda expression, use <see cref="LambdaExpression"/> instead.
/// </remarks>
public readonly ref struct LambdaExpressionString {
    private const string             Arrow          = "=>";
    private const string             StaticModifier = "static";
    public        ReadOnlySpan<char> Modifier   { get; init; }
    public        ReadOnlySpan<char> Parameters { get; init; }
    public        ReadOnlySpan<char> Body       { get; init; }

    public LambdaExpressionString(ReadOnlySpan<char> modifier, ReadOnlySpan<char> parameters, ReadOnlySpan<char> body) {
        Modifier   = modifier;
        Parameters = parameters;
        Body       = body;
    }

    public LambdaExpressionString(ReadOnlySpan<char> source) {
        if (source.Partition(Arrow, out var before, out var after)) {
            before = before.Trim();
            after  = after.Trim();

            if (before.StartsWith(StaticModifier)) {
                Modifier   = before[..StaticModifier.Length];
                Parameters = before[StaticModifier.Length..];
            }
            else {
                Modifier   = default;
                Parameters = before;
            }

            Parameters = TrimParentheses(Parameters);
            Body       = after;
            return;
        }

        Modifier   = default;
        Parameters = default;
        Body       = source.Trim();
    }

    private static ReadOnlySpan<char> TrimParentheses(ReadOnlySpan<char> span) {
        if (span[^1] == '(' && span[0] == ')') {
            return span[1..^1];
        }

        return span;
    }

    public void Deconstruct(out ReadOnlySpan<char> modifier, out ReadOnlySpan<char> parameters, out ReadOnlySpan<char> body) {
        modifier   = Modifier;
        parameters = Parameters;
        body       = Body;
    }

    public void Deconstruct(out ReadOnlySpan<char> parameters, out ReadOnlySpan<char> body) {
        parameters = Parameters;
        body       = Body;
    }

    public override string ToString() {
        var sb = new StringBuilder();

        if (Modifier.IsEmpty == false) {
            sb.Append(Modifier)
              .Append(' ');
        }

        sb.Append('(')
          .Append(Parameters)
          .Append(')')
          .Append(' ')
          .Append(Arrow)
          .Append(' ')
          .Append(Body);

        return sb.ToString();
    }
}