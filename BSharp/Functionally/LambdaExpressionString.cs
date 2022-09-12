using System;
using System.Linq.Expressions;
using System.Text;

using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;

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
    private const  string             Arrow          = "=>";
    internal const string             StaticModifier = "static";
    public         ReadOnlySpan<char> Modifier        { get; init; }
    public         ReadOnlySpan<char> Parameters      { get; init; }
    public         ReadOnlySpan<char> ReturnType      { get; init; }
    public         RoMultiSpan<char>  SplitParameters => Parameters.Spliterate(',').ToMultiSpan();
    public         ReadOnlySpan<char> Body            { get; init; }

    private LambdaExpressionString(RoSpanTuple<char, char, char, char> parts) {
        (Modifier, ReturnType, Parameters, Body) = parts;
        Modifier                                 = Modifier.Trim().UnIndent();
        Parameters                               = Parameters.Trim().TrimParentheses('(', ')').UnIndent();
        Body                                     = Body.Trim().TrimParentheses('{', '}').UnIndent();
    }

    public LambdaExpressionString(ReadOnlySpan<char> source) : this(_Parse_Parts(source)) { }

    //region Parse

    public static LambdaExpressionString Parse(ReadOnlySpan<char> source) {
        _Parse(source, out var mod, out var ret, out var par, out var bod);
        return new LambdaExpressionString(new RoSpanTuple<char, char, char, char>(mod, ret, par, bod));
    }

    private static RoSpanTuple<char, char, char> _Parse_BeforeArrow(ReadOnlySpan<char> beforeArrow) {
        throw new NotImplementedException();
    }

    private static void _Parse(
        ReadOnlySpan<char>     source,
        out ReadOnlySpan<char> modifier,
        out ReadOnlySpan<char> returnType,
        out ReadOnlySpan<char> parameters,
        out ReadOnlySpan<char> body
    ) {
        //todo
        returnType = default;

        if (source.IsEmpty) {
            modifier   = default;
            parameters = default;
            body       = default;
        }

        source = source.UnIndent();

        if (source.TryPartition(Arrow, out var before, out var after)) {
            before = before.TrimStart();

            if (before.StartsWith(StaticModifier)) {
                modifier   = before[..StaticModifier.Length];
                parameters = before[StaticModifier.Length..];
            }
            else {
                modifier   = default;
                parameters = before;
            }

            body = after;
            return;
        }

        modifier   = default;
        parameters = default;
        body       = source;
    }

    private static RoSpanTuple<char, char, char, char> _Parse_Parts(ReadOnlySpan<char> source) {
        _Parse(source, out var mod, out var ret, out var par, out var bod);
        return new RoSpanTuple<char, char, char, char>(mod, ret, par, bod);
    }

    //endregion

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

    public Paragraph GetRenderable(Palette? palette = default) {
        var pal = palette.OrFallback();
        var pg  = new Paragraph();

        if (Modifier.IsEmpty == false) {
            pg.Append(Modifier.ToString(), pal.Comments)
              .Append(" ");
        }

        pg.Append("(", pal.ExceptionPalette.Parenthesis)
          .Append(Parameters.ToString(), pal.ExceptionPalette.ParameterName)
          .Append(")",                   pal.ExceptionPalette.Parenthesis)
          .Append(" ")
          .Append(Arrow, pal.Borders)
          .Append(" ")
          .Append(Body.ToString(), pal.Methods);

        return pg;
    }
}

internal static class LambdaExpressingStringHelpers {
    public static ReadOnlySpan<char> TrimParentheses(this ReadOnlySpan<char> span, char first, char last) {
        span = span.Trim();

        if (span.Length < 2) {
            return span;
        }

        if (span[0] == first && span[^1] == last) {
            return span[1..^1];
        }

        return span;
    }

    internal static ReadOnlySpan<char> UnIndent(this ReadOnlySpan<char> text) {
        var lastLineBreak = text.LastIndexWhere(static c => c.IsLineBreak());
        if (lastLineBreak < 0) {
            return text;
        }

        var  textStartsAt = -1;
        bool firstLine    = true;

        foreach (var line in text.EnumerateLines()) {
            if (firstLine) {
                firstLine = false;
                continue;
            }

            var lineStartsAt = line.IndexWhere(static c => char.IsWhiteSpace(c), false);

            // line doesn't have any indent - can't get smaller than 0, so we don't need to keep looping
            if (lineStartsAt == 0) {
                return text;
            }

            // line was entirely whitespace - we can skip it
            if (lineStartsAt < 0) {
                continue;
            }

            // at this point, we've found whitespace; we need to update our `textStartsAt` position

            // if this is the first non-whitespace line, set `textStartsAt` and then continue
            if (textStartsAt < 0) {
                textStartsAt = lineStartsAt;
                continue;
            }

            textStartsAt = Math.Min(textStartsAt, lineStartsAt);
        }


        // loop through again, this time trimming up to `textStartsAt` chars from each line
        Span<char> trimmed = stackalloc char[text.Length];
        var        pos     = 0;
        foreach (var line in text.EnumerateLines()) {
            var trimLine = line.SkipWhile(static c => c.IsWhitespace(), textStartsAt);
            trimmed.WriteJoin(trimLine, "\n", ref pos);
        }

        return new string(trimmed[..pos]);
    }
}