using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Extensions methods pertaining to <see cref="Spectre.Console"/>.
/// </summary>
public static class Spectral {
    internal static void ForceAnsi() {
        if (AnsiConsole.Console.Profile.Capabilities.Ansi) {
            return;
        }

        AnsiConsole.WriteLine($"Forcing the default {nameof(AnsiConsole)} to use {AnsiSupport.Yes.Prettify()}, despite its better judgement.");
        AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings { Ansi = AnsiSupport.Yes, ColorSystem = ColorSystemSupport.TrueColor });
    }

    #region Escaping the dreaded Markup

    /// <summary>
    /// Calls <see cref="StringExtensions.EscapeMarkup"/> and returns the result as <see cref="IRenderable"/> <see cref="Text"/>.
    /// </summary>
    /// <param name="str">the original <see cref="string"/></param>
    /// <param name="style">an optional <see cref="Style"/>. Defaults to <see cref="Style.Plain"/></param>
    /// <returns>a <see cref="Text"/> "widget"</returns>
    public static Text EscapeSpectre(this string? str, Stylist style = default) => new(str.EscapeMarkup(), style);

    /// <summary>
    /// Creates a <see cref="Segment"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="str">the original <see cref="string"/></param>
    /// <param name="stylist">an optional <see cref="Stylist"/></param>
    /// <returns>a new <see cref="Segment"/></returns>
    public static Segment ToSegment(this string? str, Stylist stylist = default) => str.IsEmpty() ? Segment.Empty : new Segment(str, stylist);

    #endregion

    /// <summary>
    /// An <see cref="IAnsiConsole"/> that directs its output to a <see cref="StringBuilder"/>.
    /// </summary>
    public class StringOutputConsole : IAnsiConsole {
        private readonly IAnsiConsole _console;
        /// <summary>
        /// Contains the output of this <see cref="IAnsiConsole"/>.
        /// </summary>
        public StringBuilder StringBuilder { get; } = new();

        public StringOutputConsole(AnsiSupport ansi = default) {
            _console = AnsiConsole.Create(
                new AnsiConsoleSettings {
                    Ansi        = ansi,
                    Interactive = InteractionSupport.No,
                    Out         = new AnsiConsoleOutput(new StringWriter(StringBuilder))
                }
            );
        }

        public void Clear(bool home) {
            _console.Clear(home);
        }

        public void Write(IRenderable renderable) {
            _console.Write(renderable);
        }

        public Profile            Profile         => _console.Profile;
        public IAnsiConsoleCursor Cursor          => _console.Cursor;
        public IAnsiConsoleInput  Input           => _console.Input;
        public IExclusivityMode   ExclusivityMode => _console.ExclusivityMode;
        public RenderPipeline     Pipeline        => _console.Pipeline;
    }

    /// <inheritdoc cref="RenderString{T}"/>
    /// <remarks>
    /// This is a cutely named version of <see cref="RenderString{T}"/>.
    /// </remarks>
    public static string Epitaph(this IRenderable renderable, AnsiSupport ansiSupport = AnsiSupport.Yes) {
        return renderable.RenderString(ansiSupport);
    }

    /// <summary>
    /// Converts an <see cref="IRenderable"/> to its <see cref="string"/> equivalent.
    /// </summary>
    /// <param name="renderable">some <see cref="IRenderable"/> content</param>
    /// <param name="ansiSupport">whether to <see cref="Style"/>ize the output</param>
    /// <returns>the <see cref="string"/> representation of the <paramref name="renderable"/></returns>
    public static string RenderString<T>(this T renderable, AnsiSupport ansiSupport = AnsiSupport.Yes)
        where T : IRenderable {
        using var recorder = AnsiConsole.Create(new AnsiConsoleSettings { Ansi = ansiSupport }).CreateRecorder();
        recorder.Write(renderable);
        return recorder.ExportText();
    }

    /// <param name="renderable">this <see cref="IRenderable"/></param>
    /// <returns>this <see cref="IRenderable"/>, or <see cref="Text.Empty"/> if it was <c>null</c></returns>
    public static IRenderable OrEmpty(this IRenderable? renderable) => renderable ?? Text.Empty;

    #region Style

    /// <summary>
    /// Applies this <see cref="Style"/>'s <see cref="Style.ToMarkup"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="style">this <see cref="Style"/></param>
    /// <param name="text">the target <see cref="string"/></param>
    /// <returns>
    /// The marked-up <paramref name="text"/>, <b>IF</b> the <paramref name="text"/> wasn't <see cref="string.IsNullOrEmpty"/>;
    /// otherwise, the original <paramref name="text"/></returns>
    [return: NotNullIfNotNull("text")]
    public static string? ApplyMarkup(this Style? style, string? text) {
        string markup;

        if (style == null || string.IsNullOrEmpty(text) || (markup = style.ToMarkup()).IsEmpty()) {
            return text;
        }

        return $"[{markup}]{text}[/]";
    }

    /// <inheritdoc cref="ApplyMarkup(Spectre.Console.Style?,string?)"/>
    [return: NotNullIfNotNull("text")]
    public static string? ApplyMarkup(this Color color, string? text) {
        if (color == Color.Default || string.IsNullOrEmpty(text)) {
            return text;
        }

        return $"[{color.ToMarkup()}]{text}[/]";
    }

    [return: NotNullIfNotNull("text")]
    public static string? ApplyMarkup(this Decoration decoration, string? text) {
        if (decoration == Decoration.None || string.IsNullOrEmpty(text)) {
            return text;
        }

        return $"[{decoration.ToMarkup()}]{text}[/]";
    }

    /// <summary>
    /// Converts a <see cref="Decoration"/> to the corresponding <see cref="Markup"/> string.
    /// </summary>
    /// <param name="decoration">this <see cref="Decoration"/></param>
    /// <returns>a corresponding <see cref="Markup"/> string</returns>
    public static string ToMarkup(this Decoration decoration) => new Style(decoration: decoration).ToMarkup();

    #endregion

    #region IRenderable Implementations

    /// <inheritdoc cref="IRenderable.Measure"/>
    /// <remarks>
    /// This is an extension method equivalent to <see cref="IRenderable.Measure"/>.
    /// </remarks>
    public static Measurement GetMeasurement<T>(this T renderable, RenderContext renderContext, int maxWidth)
        where T : IRenderable {
        return renderable.Measure(renderContext, maxWidth);
    }

    /// <inheritdoc cref="IRenderable.Render"/>
    /// <remarks>
    /// This is an extension method equivalent to <see cref="IRenderable.Render"/>.
    /// </remarks>
    public static IEnumerable<Segment> GetSegments<T>(this T renderable, RenderContext renderContext, int maxWidth)
        where T : IRenderable {
        return renderable.Render(renderContext, maxWidth);
    }

    #endregion
}