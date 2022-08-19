using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Extensions methods pertaining to <see cref="Spectre.Console"/>.
/// </summary>
public static class SpectreExtensions {
    /// <summary>
    /// Calls <see cref="StringExtensions.EscapeMarkup"/> and returns the result as <see cref="IRenderable"/> <see cref="Text"/>.
    /// </summary>
    /// <param name="str">the original <see cref="string"/></param>
    /// <param name="style">an optional <see cref="Style"/>. Defaults to <see cref="Style.Plain"/></param>
    /// <returns>a <see cref="Text"/> "widget"</returns>
    public static Text EscapeSpectre(this string? str, Style? style = default) => new(str.EscapeMarkup(), style);

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

    /// <summary>
    /// Converts an <see cref="IRenderable"/> to its <see cref="string"/> equivalent.
    /// </summary>
    /// <param name="renderable">some <see cref="IRenderable"/> content</param>
    /// <param name="ansiSupport">whether to <see cref="Style"/>ize the output</param>
    /// <returns>the <see cref="string"/> representation of the <paramref name="renderable"/></returns>
    public static string Epitaph(this IRenderable renderable, AnsiSupport ansiSupport = AnsiSupport.Yes) {
        using var recorder = AnsiConsole.Create(new AnsiConsoleSettings { Ansi = ansiSupport }).CreateRecorder();
        recorder.Write(renderable);
        return recorder.ExportText();
    }

    private static IRenderable AsRenderable<T>(this T? obj) {
        return obj as IRenderable ?? obj.OrNullPlaceholder().EscapeSpectre();
    }

    public static Table AddLabelled<T, TLabel>(this Table table, T? obj, TLabel? label) {
        var cols = new IRenderable[table.Columns.Count];
        cols[0] = label.AsRenderable();
        cols[1] = obj.AsRenderable();
        table.AddRow(cols);
        return table;
    }

    public static Table AddLabelled<T>(this Table table, T obj, [CallerArgumentExpression("obj")] string label = "") {
        return table.AddLabelled<T, string>(obj, label);
    }
}