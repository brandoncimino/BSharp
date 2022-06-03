using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings.Json;

/// <summary>
/// A simple implementation of <see cref="Newtonsoft"/>'s <see cref="ITraceWriter"/> that logs messages to <see cref="Console.WriteLine()"/>
/// </summary>
[PublicAPI]
public class ConsoleTraceWriter : ITraceWriter {
    public  string?        Nickname     { get; }
    private Stack<string?> InnerNames   { get; }               = new Stack<string?>();
    private Indenter       Indenter     { get; }               = new Indenter();
    public  TraceLevel     DefaultLevel { get;         set; }  = TraceLevel.Info;
    public  string         StartIcon    { private get; init; } = "▶";
    public  string         StopIcon     { private get; init; } = "⏹";
    public  string         SkipIcon     { private get; init; } = "⏭";
    public  string         InnerIcon    { private get; init; } = "↪";
    public  string         ErrorIcon    { private get; init; } = "🌋";
    public  string         InfoIcon     { private get; init; } = "📎";
    public  string         OffIcon      { private get; init; } = "🔇";
    public  string         VerboseIcon  { private get; init; } = "📜";
    public  string         WarningIcon  { private get; init; } = "⚠";

    private IEnumerable<string> GetNameParts(string nickname) => new[] {
        nickname,
        $"#{GetHashCode()}",
        Thread.CurrentThread.Name,
    }.NonBlank();

    private string GetFullName() {
        return InnerNames switch {
            { Count: 0 } => Nickname.IsBlank() ? "|" : $"{GetNameParts(Nickname).JoinString("-")}",
            _            => $"{InnerIcon} {InnerNames.Peek()}",
        };
    }

    public TraceLevel LevelFilter { get; set; }

    public ConsoleTraceWriter([CallerMemberName] string? nickname = default) {
        Nickname = nickname;
    }

    public ConsoleTraceWriter(Type? owner) : this(owner.Prettify()) { }

    public ConsoleTraceWriter(object? self) : this(self?.GetType()) { }

    public static ConsoleTraceWriter Start([CallerMemberName] string? starting = default, string? nickname = default) {
        var tracer = new ConsoleTraceWriter(nickname);
        tracer.Start(starting: starting);
        return tracer;
    }

    public virtual void Trace(TraceLevel level, string? message, Exception? ex) {
        Console.WriteLine(FormatMessage(level, message, ex));
    }

    protected virtual string FormatMessage(TraceLevel level, string? message, Exception? ex) {
        return new[] {
                GetTraceLevelIcon(level),
                GetFullName(),
                message,
                ex != null ? $"-> {ex}" : default,
            }.NonBlank()
             .JoinString(" ");
    }

    private string GetTraceLevelIcon(TraceLevel traceLevel) {
        return traceLevel switch {
            TraceLevel.Error   => ErrorIcon,
            TraceLevel.Info    => InfoIcon,
            TraceLevel.Off     => OffIcon,
            TraceLevel.Verbose => VerboseIcon,
            TraceLevel.Warning => WarningIcon,
            _                  => throw BEnum.UnhandledSwitch(traceLevel),
        };
    }

    public void Start(TraceLevel? traceLevel = default, [CallerMemberName] string? starting = default) {
        InnerNames.Push(starting ?? "<something 🤷‍>");
        Trace(
            traceLevel ?? DefaultLevel,
            $"{StartIcon} {starting}",
            null
        );
        Indenter.Indent();
    }

    public void End(TraceLevel? traceLevel = default) {
        Trace(traceLevel ?? DefaultLevel, $"{StopIcon} {InnerNames.Pop()}", null);
        Indenter.Outdent();
    }
}

/// <summary>
/// Tracks and formats an indentation for <see cref="ConsoleTraceWriter"/>.
/// </summary>
public class Indenter {
    public string IndentString { get; init; } = " ";

    private int _currentIndent;

    public int CurrentIndent {
        get => _currentIndent;
        set => _currentIndent = value.Clamp(0);
    }

    public int IndentSize      { get; init; } = 2;
    public int TotalIndentSize => CurrentIndent * IndentSize;

    public string Render()           => IndentString.RepeatToLength(TotalIndentSize);
    public string Render(object obj) => $"{Render()}{obj}";

    public Indenter Indent(int relativeIndent = 1) {
        CurrentIndent += relativeIndent;
        return this;
    }

    public Indenter Outdent(int relativeOutdent = 1) => Indent(relativeOutdent * -1);
}