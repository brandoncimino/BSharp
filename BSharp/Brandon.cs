using System.Diagnostics.CodeAnalysis;
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
        var row    = new Columns(rLabel, rValue);
        Render(row);
        return value;
    }

    /// <inheritdoc cref="Print{T,TLabel}"/>
    public static T Print<T>(
        T                                           value,
        [CallerArgumentExpression("value")] string? _expression = default
    ) {
        return Print(value, default(object), _expression);
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