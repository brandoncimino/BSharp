using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings.Indenter;

using JetBrains.Annotations;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings.Json;

/// <summary>
/// A simple implementation of <see cref="Newtonsoft"/>'s <see cref="ITraceWriter"/> that logs messages to <see cref="Console.WriteLine()"/>
/// </summary>
[PublicAPI]
public class ConsoleTraceWriter : ISuperTraceWriter {
    public        Stack<string?>                  Stack        { get; } = new();
    public        string?                         Nickname     => Stack.ElementAtOrDefault(0);
    private       string?                         PreviousName => Stack.ElementAtOrDefault(1);
    public        JaggedIndenter                  Indenter     { get;         init; }
    public        TraceLevel                      DefaultLevel { get;         init; } = TraceLevel.Info;
    public        string                          StartIcon    { private get; init; } = "▶";
    public        string                          StopIcon     { private get; init; } = "⏹";
    public        string                          SkipIcon     { private get; init; } = "⏭";
    public        string                          NextIcon     { private get; init; } = "⏩";
    public        string                          InnerIcon    { private get; init; } = "↪";
    public        string                          ErrorIcon    { private get; init; } = "🌋";
    public        string                          InfoIcon     { private get; init; } = "📎";
    public        string                          OffIcon      { private get; init; } = "🔇";
    public        string                          VerboseIcon  { private get; init; } = "📜";
    public        string                          WarningIcon  { private get; init; } = "⚠";
    private const int                             IconWidth = 2;
    public        Func<ISuperTraceWriter, string> LabelFormatter { get; init; } = HashedName;

    private static string HashedName(ISuperTraceWriter tracer) => new[] {
            tracer.Stack.Peek(),
            $"#{tracer.GetHashCode()}",
            Thread.CurrentThread.Name,
        }.NonBlank()
         .JoinString("-");

    private string GetFullName() {
        return LabelFormatter(this);
    }

    public TraceLevel LevelFilter { get; set; }

    public ConsoleTraceWriter([CallerMemberName] string? nickname = default) {
        Stack.Push(nickname);
        Indenter = new JaggedIndenter();
    }

    public ConsoleTraceWriter(Type? owner) : this(owner.Prettify()) { }

    public ConsoleTraceWriter(object? self) : this(self?.GetType()) { }

    public static ConsoleTraceWriter Start([CallerMemberName] string? starting = default, string? nickname = default, [CallerFilePath] string? callerFile = default) {
        nickname = nickname.IfBlank(() => BPath.GetFileNameWithoutExtensions(callerFile ?? ""));
        var tracer = new ConsoleTraceWriter(nickname) {
            LabelFormatter = tracer => $"{tracer.Stack.Peek()}",
        };
        tracer.Start(starting: starting);
        return tracer;
    }

    public virtual void Trace(TraceLevel level, string? message, Exception? ex) {
        Console.WriteLine(FormatMessage(level, message, ex));
    }

    protected virtual string FormatMessage(TraceLevel level, string? message, Exception? ex) {
        var msg = new[] {
                GetTraceLevelIcon(level),
                (Nickname ?? "").Align(width: Indenter.Render().Length, StringAlignment.Right),
                message,
                ex != null ? $"-> {ex}" : default,
            }.NonBlank()
             .JoinString(" ");
        return msg;
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
        var label = $"{StartIcon} {starting}";
        Trace(
            traceLevel ?? DefaultLevel,
            label,
            null
        );
        Stack.Push(starting ?? "");
        Indenter.Indent(label);
    }

    public void End(TraceLevel? traceLevel = default) {
        Trace(traceLevel ?? DefaultLevel, $"{StopIcon} {Stack.Pop()} ↩ {PreviousName}", null);
        Indenter.Outdent();
    }
}