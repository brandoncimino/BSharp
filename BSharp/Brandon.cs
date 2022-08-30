using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp;

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
public static class Brandon {
    static Brandon() {
        ForceAnsi();
    }

    private static void ForceAnsi() {
        AnsiConsole.WriteLine($"Forcing the default {nameof(AnsiConsole)} to use {AnsiSupport.Yes.Prettify()}, despite its better judgement.");
        AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings { Ansi = AnsiSupport.Yes });
    }

    /// <summary>
    /// Renders an <see cref="IRenderable"/> to the default <see cref="AnsiConsole.Console"/>.
    /// </summary>
    /// <remarks>
    /// On its own, this method doesn't really do much, since most of the extensibility for <see cref="Spectre"/> is accessed by constructing a custom <see cref="IAnsiConsole"/>
    /// and setting the default <see cref="AnsiConsole.Console"/> (see <see cref="ForceAnsi"/>, for example).
    /// </remarks>
    /// <param name="renderable"></param>
    /// <typeparam name="T"></typeparam>
    public static void Render<T>(T renderable)
        where T : IRenderable =>
        AnsiConsole.Write(renderable);

    /// <summary>
    /// <see cref="Render{T}"/>s an <see cref="IRenderable"/> via an extension method that won't conflict with the deprecated <see cref="AnsiConsole.Render"/>.
    /// </summary>
    /// <param name="renderable"></param>
    /// <typeparam name="T"></typeparam>
    public static void Rend<T>(this T renderable)
        where T : IRenderable => Render(renderable);

    /// <summary>
    /// Prints a <paramref name="label"/>led <paramref name="value"/> to the <see cref="AnsiConsole.Console"/>.
    /// </summary>
    /// <param name="value">the main thing being printed</param>
    /// <param name="label">a description of the <paramref name="value"/>. Defaults to <paramref name="_expression"/></param>
    /// <param name="_expression">see <see cref="CallerArgumentExpressionAttribute"/>. Used as the <paramref name="label"/> if the <paramref name="label"/> is <c>null</c></param>
    /// <typeparam name="T">the type of <paramref name="value"/></typeparam>
    /// <typeparam name="TLabel">the type of <paramref name="label"/></typeparam>
    /// <returns>the input <paramref name="value"/></returns>
    public static T Print<T, TLabel>(
        T                                           value,
        TLabel?                                     label       = default,
        [CallerArgumentExpression("value")] string? _expression = default
    ) {
        var rLabel = label == null ? Renderwerks.GetRenderable(_expression) : Renderwerks.GetRenderable(label);
        var rValue = Renderwerks.GetRenderable(value);
        AutoRow(rLabel, rValue);
        return value;
    }

    /// <inheritdoc cref="Print{T,TLabel}"/>
    public static T Print<T>(T value, [CallerArgumentExpression("value")] string? _expression = default) => Print(value, default(object), _expression);

    /// <inheritdoc cref="Print{T,TLabel}"/>
    public static ReadOnlySpan<T> Print<T>(ReadOnlySpan<T> span, [CallerArgumentExpression("span")] string? _expression = default) {
        Print(Renderwerks.GetRenderable(span), _expression);
        return span;
    }

    /// <inheritdoc cref="Print{T,TLabel}"/>
    public static Span<T> Print<T>(Span<T> span, [CallerArgumentExpression("span")] string? _expression = default) {
        Print(Renderwerks.GetRenderable(span), _expression);
        return span;
    }

    #region PrintRow

    public static void AutoRow(params IRenderable[]     cells)            => Render(new AutoColumns(cells));
    public static void AutoRow(IEnumerable<IRenderable> cells)            => Render(new AutoColumns(cells));
    public static void AutoRow<A>(A                     a)                => AutoRow(Renderwerks.GetRenderable(a));
    public static void AutoRow<A, B>(A                  a, B b)           => AutoRow(Renderwerks.GetRenderable(a), Renderwerks.GetRenderable(b));
    public static void AutoRow<A, B, C>(A               a, B b, C c)      => AutoRow(Renderwerks.GetRenderable(a), Renderwerks.GetRenderable(b), Renderwerks.GetRenderable(c));
    public static void AutoRow<A, B, C, D>(A            a, B b, C c, D d) => AutoRow(Renderwerks.GetRenderable(a), Renderwerks.GetRenderable(b), Renderwerks.GetRenderable(c), Renderwerks.GetRenderable(d));

    public static void AutoRow<A, B, C, D, TRest>(
        A              a,
        B              b,
        C              c,
        D              d,
        params TRest[] rest
    ) {
        var row = rest.Select(it => Renderwerks.GetRenderable(it))
                      .Prepend(Renderwerks.GetRenderable(d))
                      .Prepend(Renderwerks.GetRenderable(c))
                      .Prepend(Renderwerks.GetRenderable(b))
                      .Prepend(Renderwerks.GetRenderable(a));

        AutoRow(row);
    }

    #endregion

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