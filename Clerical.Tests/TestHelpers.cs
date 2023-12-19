using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Spectral;
using FowlFever.Clerical;

using JetBrains.Annotations;

using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

using Spectre.Console;

namespace Clerical.Tests;

[StackTraceHidden]
internal static partial class TestHelpers {
    private static (A? a, B? b) RequireMutuallyExclusive<A, B>(A? a, B? b) where B : notnull {
        if (a is null ^ b is null) {
            return (a, b);
        }

        var aStr = a?.ToString() ?? "⛔";
        var bStr = b?.ToString() ?? "⛔";
        throw new ArgumentException($"Exactly ONE of the two values must be non-null! (actual: 🅰 {aStr}, 🅱 {bStr})");
    }

    [MustUseReturnValue]
    [StackTraceHidden]
    public static Result<T> ResultOf<T>(Func<T> function, [CallerArgumentExpression("function")] string? _function = default) {
        // using var _ = new TestExecutionContext.IsolatedContext();

        try {
            return new Result<T>(new ValueBox<T>(function()), null, _function);
        }
        catch (Exception e) {
            return new Result<T>(default, e, _function);
        }
    }

    [MustUseReturnValue]
    public static Result<ValueTuple> ResultOf(Action action, [CallerArgumentExpression("action")] string? _action = default) {
        return ResultOf(action.AsFunc(), _action);
    }

    [MustUseReturnValue]
    public static Result<T> ResultOfAssert<T>(T actual, IResolveConstraint constraint, string? description = null, [CallerArgumentExpression("actual")] string? _actual = default) {
        return ResultOf([StackTraceHidden]() => IsolatedAssert(actual, constraint, description, _actual), description);
    }

    public static T IsolatedAssert<T>(T actual, IResolveConstraint constraint, string? description = default, [CallerArgumentExpression("actual")] string? _actual = default) {
        var conRes = constraint.Resolve().ApplyTo(actual);
        if (conRes.IsSuccess) {
            return actual;
        }

        var mr = new TextMessageWriter(description);
        conRes.WriteMessageTo(mr);
        throw new AssertionException(mr.ToString());
    }

    private static string GetOptionalDescriptionLine(string? description) {
        return string.IsNullOrWhiteSpace(description) ? "" : $"{description}\n";
    }

    private static string JoinNonBlankLines(IEnumerable<string> lines) {
        return string.Join("\n", lines.Where(it => string.IsNullOrWhiteSpace(it) == false));
    }

    public static Result<OUT> ResultOf_Commutative<IN, OUT>(
        IN                a,
        IN                b,
        Func<IN, IN, OUT> commutativeFunction,
        [CallerArgumentExpression("commutativeFunction")]
        string? _commutativeFunction = default
    ) {
        var ab = ResultOf(() => commutativeFunction(a, b), _function: _commutativeFunction);
        var ba = ResultOf(() => commutativeFunction(b, a), _function: _commutativeFunction);

        var descr = $"""
                     Commutative results for the inputs (🅰 {
                         a
                     }, 🅱 {
                         b
                     }):
                       🅰🅱: {
                           ab.Description
                       }
                       🅱🅰: {
                           ba.Description
                       }
                     """;

        if (ab == ba) {
            return new Result<OUT>(ab.ValueBox, null, descr);
        }

        return new Result<OUT>(null, new AssertionException("Results were not commutative!"), descr);
    }

    public static void AssertCommutative<ACTUAL, EXPECTED>(
        ACTUAL                         a,
        ACTUAL                         b,
        Func<ACTUAL, ACTUAL, EXPECTED> commutativeFunction,
        EXPECTED                       expected,
        string?                        description = default,
        [CallerArgumentExpression("commutativeFunction")]
        string? _commutativeFunction = default
    ) {
        var ab = commutativeFunction(a, b);
        var ba = commutativeFunction(b, a);

        var funcSpan   = _commutativeFunction.AsSpan();
        var arrowIndex = funcSpan.IndexOf("=>", StringComparison.Ordinal);
        if (arrowIndex > 0) {
            funcSpan = funcSpan[(arrowIndex + 2)..].Trim();
        }

        Assert.That(
            new { ab, ba },
            Is.EqualTo(new { ab = expected, ba = expected }),
            $"""
             {
                 GetOptionalDescriptionLine(description)
             }`{
                 funcSpan
             }` is commutative:
                a: `{
                    a
                }`
                b: `{
                    b
                }`
                
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

    public static void AssertThat<T>(T actual, IResolveConstraint constraint, string? description = default, [CallerArgumentExpression("actual")] string? _actual = default) {
        Assert.That(actual, constraint, GetOptionalDescriptionLine(description) + _actual);
    }

    public static void Assert_AllEqual<T>(IEnumerable<Result<T>> results, T expected) {
        results = results.ToImmutableList();
        if (results.All(it => it.Equals(expected))) {
            return;
        }

        var sb = new StringBuilder($"Expected all of the following results to equal {expected.OrNullPlaceholder("null")}:");
        sb.AppendLine(RenderResultGrid(results, expected));
        throw new AssertionException(sb.ToString());
    }

    public static void Assert_AllSucceeded(string? header, IEnumerable<IResult> results) {
        results = results.ToImmutableList();
        if (results.All(it => it.HasValue)) {
            return;
        }

        var sb = new StringBuilder()
                 .AppendNonBlankLine(header)
                 .AppendLine("Expected all of the following to succeed without exception:")
                 .AppendLine(RenderResultGrid(results));

        throw new AssertionException(sb.ToString());
    }

    private static Result<bool>[] Gather_Equality_Results<T>(T? x, T? y) where T : IEquatable<T>, IEqualityOperators<T, T, bool> {
        if (x == null || y == null) {
            throw new ArgumentNullException($"Neither of the inputs should have been null! ({x}, {y})");
        }

        var results = new[] {
            ResultOf_Commutative(x, y, static (a, b) => a == b),
            ResultOf_Commutative(x, y, static (a, b) => !(a != b)),
            ResultOf_Commutative(x, y, static (a, b) => a.Equals(b)),
            ResultOf_Commutative(x, y, static (a, b) => a.Equals((object)b)),
            ResultOf_Commutative(x, y, static (a, b) => Equals(a, b)),
            ResultOf_Commutative(x, y, static (a, b) => EqualityComparer<T>.Default.Equals(a, b)),
            ResultOf_Commutative(x, y, static (a, b) => EqualityComparer<object>.Default.Equals(a, b)),
            ResultOf_Commutative(x, y, static (a, b) => a.ToString() == b.ToString())
        };

        return results;
    }

    public static void Assert_Equality<T>(T? a, T? b, bool expectedEquality) where T : IEquatable<T>, IEqualityOperators<T, T, bool> {
        var results = Gather_Equality_Results(a, b);
        Assert_AllEqual(results, expectedEquality);
    }

    private static string RenderResultGrid<T>(IEnumerable<Result<T>> results, T expected) {
        var grid = new Grid().AddColumns(4);
        var i    = 0;
        foreach (var it in results) {
            i += 1;
            grid.AddRow(
                i.ToString(),
                it.Equals(expected) ? "✅" : "❌",
                it.ToString().EscapeMarkup(),
                it.Description.Value.EscapeMarkup()
            );
        }

        return grid.RenderString();
    }

    [MustUseReturnValue]
    private static string RenderResultGrid(IEnumerable<IResult> results) {
        var grid = new Grid().AddColumns(4);
        var i    = 0;
        foreach (var it in results) {
            i += 1;
            grid.AddRow(
                i.ToString().EscapeMarkup(),
                it.HasValue ? "✅" : "❌",
                it.Describe().JoinLines().EscapeMarkup()
            );
        }

        return grid.RenderString();
    }

    public static void Assert_RequiresStyles<T, P>(
        string         input,
        T              expected,
        ClericalStyles requiredStyles,
        string?        description = default
    )
        where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool>
        where P : IClericalParser<T, ClericalStyles, string?> {
        Assert.Multiple(
            () => {
                // Without any styles, the input should NOT parse
                Assert_Stylish_NoParse<T, P>(input, ClericalStyles.None, description);

                // With all styles EXCEPT the required ones, the input should NOT parse
                Assert_Stylish_NoParse<T, P>(input, ClericalStyles.Any & ~requiredStyles, description);

                // With the styles, the input SHOULD parse
                Assert_Parser_Parses<T, P>(input, expected, requiredStyles, description);
            }
        );
    }

    public static void Assert_Parser_Parses<T, P>(string input, T expected, ClericalStyles styles, string? description = default) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> where P : IClericalParser<T, ClericalStyles, string?> {
        description = string.Join('\n', new[] { description, $"input: {input}, styles: {styles}" }.Where(it => !string.IsNullOrWhiteSpace(it)));
        Assert.Multiple(
            () => {
                AssertThat(P.TryParse_Internal(input, styles, out var result), Is.Null, description);
                Assert_Equality(result,                          expected, true);
                Assert_Equality(P.Parse_Internal(input, styles), expected, true);
            }
        );
    }

    public static void Assert_Stylish_NoParse<T, P>(string input, ClericalStyles styles, string? description = default) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> where P : IClericalParser<T, ClericalStyles, string?> {
        description = string.Join('\n', new[] { description, $"input: {input}, styles: {styles}" }.Where(it => !string.IsNullOrWhiteSpace(it)));
        Assert.Multiple(
            () => {
                AssertThat(P.TryParse_Internal(input, styles, out var result), Is.Not.Null, description);
                Assert_Equality(result, default, true);
                AssertThat(() => P.Parse_Internal(input, styles), Throws.InstanceOf<FormatException>(), description);
            }
        );
    }

    public static char SwapIfSeparator(char c) {
        return c switch {
            '/'  => '\\',
            '\\' => '/',
            _    => c
        };
    }

    public static string AlternateSeparators(string input, char first = '\\') {
        var prev = SwapIfSeparator(first);
        return string.Join(
            "",
            input.Select(
                it => it switch {
                    '/' or '\\' => prev = SwapIfSeparator(prev),
                    _           => it
                }
            )
        );
    }

    public static string SwapSeparators(string input) {
        return string.Join("", input.Select(SwapIfSeparator));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Unicode has several <a href="https://en.wikipedia.org/wiki/Control_Pictures#">Control Pictures</a>:
    /// <ul>
    /// <li><a href="https://en.wikipedia.org/wiki/C0_and_C1_control_codes#C0_controls">C0 control codes</a> (which max out at 31)</li>
    /// <li>The two "pseudo-C0 control codes" <i>(see <a href="https://en.wikipedia.org/wiki/ISO/IEC_2022#Fixed_coded_characters">fixed coded characters</a>)</i>:
    /// <ul>
    /// <li><c>'␠'</c> for <c>' '</c> (32)</li>
    /// <li><c>'␡'</c> for <a href="https://en.wikipedia.org/wiki/Delete_character">DEL</a> (127)</li>
    /// </ul>
    /// </li>
    /// <li><c>'␣'</c> for generic <see cref="char.IsWhiteSpace(char)"/></li>
    /// <li><c>'␤'</c> for generic <see cref="Environment.NewLine"/>s</li>
    /// <li><c>'␢'</c> for <a href="https://en.wikipedia.org/wiki/Whitespace_character#U+2422">"blank"</a></li>
    /// <li><c>'␥'</c> for 🤷‍♀️</li>
    /// <li><c>'␦'</c> for 🤷‍♀️</li>
    /// <li><a href="https://en.wikipedia.org/wiki/Whitespace_character#U+237D">'⍽'</a> for <a href="https://en.wikipedia.org/wiki/Non-breakable_space">non-breaking space</a> <i>(📎 Note: this symbol is in the <a href="https://en.wikipedia.org/wiki/Miscellaneous_Technical">Miscellaneous Technical</a> block)</i></li>
    /// </ul>
    /// </remarks>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char ReplaceControlCharsWithSymbols(char c) {
        const char max_c0 = (char)31;
        return c switch {
            <= max_c0 => (char)(c + 9216),
            ' '       => '␣', // 📎 Technically, ␠ is _more_ correct, but this is still correct and much easier to read
            (char)127 => '␡', // DEL
            '\u00A0'  => '⍽', // NBSP
            _         => c
        };
    }

    public static Func<ValueTuple> AsFunc(this Action action) => [StackTraceHidden]() => {
        action();
        return default;
    };

    private static string RenderGrid<T>(IEnumerable<T> rows, params Func<T, int, object?>[] columns) {
        var grid   = new Grid().AddColumns(columns.Length);
        int rowNum = 0;
        foreach (var r in rows) {
            var thisRow = rowNum;

            var cells = columns.Select(col => col(r, thisRow))
                               .Select(it => Renderwerks.GetRenderable(it))
                               .ToArray();
            grid.AddRow(cells);

            rowNum++;
        }

        return grid.RenderString();
    }

    public static void AssertAll(params Action[] assertions) {
        AssertAll(null, assertions);
    }

    public static void AssertAll(string? header, params Action[] more) {
        // 📎 While it's nice to have a signature that enforces 1+ `Action`s, that makes the code hints really ugly, so I'll just stick with this
        var results = more
                      .Select([StackTraceHidden](it) => ResultOf(it))
                      .ToImmutableArray();

        Assert.That(
            results.Count(it => it.HasValue),
            Is.EqualTo(results.Length),
            () => {
                var sb = new StringBuilder();
                if (string.IsNullOrWhiteSpace(header) == false) {
                    sb.AppendLine(header);
                }

                var failCount = results.Count(it => it.HasValue == false);
                sb.AppendLine($"{failCount} of {results.Length} assertions failed:");
                sb.Append(
                    RenderGrid(
                        results,
                        (r, i) => i + 1,
                        (r, i) => r.HasValue ? "✅" : "❌",
                        (r, i) => r.Exception?.Message
                    )
                );
                return sb.ToString();
            }
        );
    }

    public static StringBuilder AppendNonBlankLine(this StringBuilder stringBuilder, string? line) {
        if (string.IsNullOrWhiteSpace(line) == false) {
            stringBuilder.AppendLine(line);
        }

        return stringBuilder;
    }

    public static StringBuilder AppendType(this StringBuilder stringBuilder, Type t, int depthRemaining, bool includeOuterTypeNames = true) {
        if (depthRemaining < 0) {
            stringBuilder.Append('…');
            return stringBuilder;
        }

        var typeBaseName = GetTypeBaseName(t);

        var genTypes = t.GetGenericArguments();

        if (genTypes.Length == 0) {
            stringBuilder.Append(typeBaseName);
            return stringBuilder;
        }

        var lastGrave = typeBaseName.LastIndexOf('`');
        stringBuilder.Append(typeBaseName.AsSpan()[..lastGrave]);
        stringBuilder.Append('<');
        for (var i = 0; i < genTypes.Length; i++) {
            if (i > 0) {
                stringBuilder.Append(',');
            }

            AppendType(stringBuilder, genTypes[i], depthRemaining - 1);
        }

        stringBuilder.Append('>');

        return stringBuilder;

        static string GetTypeBaseName(Type t) {
            return t switch {
                {
                    IsNested: true,
                    FullName.Length: > 0,
                    Namespace.Length: > 0
                } => t.FullName[(t.Namespace.Length + 1)..],
                _ => t.Name
            };
        }
    }

    public static string TypeName(this Type t, int depth = 1) {
        return new StringBuilder().AppendType(t, depth).ToString();
    }

    public static string MethodName(this Type t, string methodName) {
        return $"{TypeName(t)}.{methodName}";
    }
}