using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp;

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
public static class Brandon {
    static Brandon() {
        static void ForceAnsi() {
            AnsiConsole.WriteLine($"Forcing the default {nameof(AnsiConsole)} to use {AnsiSupport.Yes.Prettify()}, despite its better judgement.");
            AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings { Ansi = AnsiSupport.Yes });
        }

        ForceAnsi();
    }

    public static void Render<T>(T renderable)
        where T : IRenderable {
        AnsiConsole.Write(renderable);
    }

    public static IRenderwerks Renderwerks { get; set; } = new Panelwerks();

    /// <summary>
    /// Prints an <see cref="expression"/> and a <see cref="value"/> in a <see cref="Panel"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="label"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Print<T>(
        T                                           value,
        string?                                     label      = default,
        [CallerArgumentExpression("value")] string? expression = default
    ) {
        var renderable = value switch {
            IRenderable r => r,
            _             => Renderwerks.GetRenderable(value),
        };
        Render(renderable);
        return value;
    }

    /// <summary>
    /// Similar to <see cref="Print{T}(T,string?,string?)"/>, but accepts a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="expression">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of entries in the span</typeparam>
    /// <returns>this <paramref name="span"/></returns>
    public static ReadOnlySpan<T> Print<T>(ReadOnlySpan<T> span, [CallerArgumentExpression("span")] string? expression = default) {
        static string Prettify(ReadOnlySpan<T> span) {
            var sb = new StringBuilder();
            sb.Append($"ReadOnlySpan<{typeof(T).Name}>[{span.Length}]");
            sb.Append('{');
            foreach (var it in span) {
                sb.Append(it.OrNullPlaceholder());
                sb.Append(", ");
            }

            sb.Append('}');
            return sb.ToString();
        }

        var pretty = Prettify(span);

        var spectre = new Panel(pretty.EscapeMarkup()) {
            Header      = new PanelHeader(expression.EscapeMarkup()),
            Expand      = false,
            Border      = BoxBorder.Rounded,
            BorderStyle = new Style(Color.DarkOrange)
        };

        Render(spectre);
        return span;
    }

    /// <summary>
    /// Prints a horizontal <see cref="Rule"/> with the current <see cref="CallerMemberNameAttribute"/>.
    /// </summary>
    /// <param name="callerMemberName">see <see cref="CallerMemberNameAttribute"/></param>
    /// <returns>see <see cref="CallerMemberNameAttribute"/></returns>
    public static string PrintThisMember([CallerMemberName] string callerMemberName = "") {
        var spectre = new Rule(callerMemberName);
        AnsiConsole.Write(spectre);
        return callerMemberName;
    }
}