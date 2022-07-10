using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
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

    public static (string?, CallerInfo) Info<T>(
        T value,
        [CallerArgumentExpression("value")]
        string? argumentExpression = default,
        [CallerMemberName]
        string? memberName = default,
        [CallerFilePath]
        string? filePath = default,
        [CallerLineNumber]
        int lineNumber = default
    ) => (argumentExpression, new CallerInfo(memberName, filePath, lineNumber));

    public static T Print<T>(
        T value,
        [CallerArgumentExpression("value")]
        string? expression = default
    ) {
        var spectre = new Panel(value.Prettify().EscapeMarkup()) {
            Header      = new PanelHeader(expression.EscapeMarkup()),
            Expand      = false,
            Border      = BoxBorder.Rounded,
            BorderStyle = new Style(Color.DarkOrange),
        };

        AnsiConsole.Write(spectre);

        return value;
    }

    public static string Verbose<T>(
        T value,
        [CallerArgumentExpression("value")]
        string? argumentExpression = default,
        [CallerMemberName]
        string? memberName = default,
        [CallerFilePath]
        string? filePath = default,
        [CallerLineNumber]
        int lineNumber = default
    ) {
        var info = Info(value, argumentExpression, memberName, filePath, lineNumber);
        var str  = info.Prettify();
        Console.WriteLine(str);
        return str;
    }
}

public record CallerInfo(
    [CallerMemberName]
    string? MemberName = default,
    [CallerFilePath]
    string? FilePath = default,
    [CallerLineNumber]
    int LineNumber = default
) : IHasRenderable {
    public string? FileName => Path.GetFileNameWithoutExtension(FilePath);

    public Hyperlink? GetLineLink() {
        return FilePath switch {
            { Length: > 0 } => new Hyperlink(FilePath) {
                DisplayText = $"{FileName}:{LineNumber}"
            },
            _ => null
        };
    }

    public IRenderable GetRenderable() {
        var epitaph = new Epitaph().AddNonNull(new Epitaph(MemberName));

        if (GetLineLink() != null) {
            epitaph.Add(" @ ")
                   .Add(FilePath);
        }

        return epitaph;
    }
}