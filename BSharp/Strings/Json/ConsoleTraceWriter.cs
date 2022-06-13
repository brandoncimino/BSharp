using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings.Indenter;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings.Json;

/// <summary>
/// A simple implementation of <see cref="Newtonsoft"/>'s <see cref="ITraceWriter"/> that logs messages to <see cref="Console.WriteLine()"/>
/// </summary>
internal class ConsoleTraceWriter : ISuperTraceWriter {
    private const TraceLevel                      Default_Level = TraceLevel.Info;
    public        Stack<string?>                  Stack        { get; } = new();
    public        string?                         Nickname     => Stack.ElementAtOrDefault(0);
    private       string?                         PreviousName => Stack.ElementAtOrDefault(1);
    public        JaggedIndenter                  Indenter     { get;         init; }
    public        TraceLevel                      DefaultLevel { get;         init; } = Default_Level;
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

    public TraceLevel LevelFilter { get; set; } = Default_Level;

    public ConsoleTraceWriter([CallerMemberName] string? nickname = default) {
        Stack.Push(nickname);
        Indenter = new JaggedIndenter();
    }

    public ConsoleTraceWriter(Type? owner) : this(owner.Prettify()) { }

    public ConsoleTraceWriter(object? self) : this(self?.GetType()) { }

    public static ConsoleTraceWriter Start(
        TraceLevel level = Default_Level,
        [CallerMemberName]
        string? starting = default,
        string? nickname = default,
        [CallerFilePath]
        string? callerFile = default
    ) {
        nickname = nickname.IfBlank(() => Path.GetFileNameWithoutExtension(callerFile ?? ""));
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

    public MethodTracer StartMethod(TraceLevel? traceLevel = default, [CallerMemberName] string? methodName = default) {
        return MethodTracer.Start(this, traceLevel ?? DefaultLevel, methodName);
    }

    public void End(TraceLevel? traceLevel = default) {
        Trace(traceLevel ?? DefaultLevel, $"{StopIcon} {Stack.Pop()} ↩ {PreviousName}", null);
        Indenter.Outdent();
    }
}

internal class MethodTracer : IDisposable {
    private ISuperTraceWriter Tracer { get; }

    private TraceLevel Level { get; }

    private MethodTracer(
        ISuperTraceWriter? tracer     = default,
        TraceLevel         level      = TraceLevel.Info,
        string?            methodName = default,
        string?            callerFile = default
    ) {
        Level  = level;
        Tracer = tracer ?? new ConsoleTraceWriter { DefaultLevel = Level };
        Tracer.Start(Level, methodName);
    }

    public static MethodTracer Start(
        ISuperTraceWriter? tracer = default,
        TraceLevel         level  = TraceLevel.Info,
        [CallerMemberName]
        string? methodName = default,
        [CallerFilePath]
        string? callerFile = default
    ) => new(tracer, level, methodName, callerFile);

    public void Dispose() {
        Tracer.End(Level);
    }
}