using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Json;
using FowlFever.BSharp.Strings.Tabler;

namespace FowlFever.BSharp;

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
internal static class Brandon {
    public static CallerInfo<T> Info<T>(
        T value,
        [CallerArgumentExpression("value")]
        string? argumentExpression = default,
        [CallerMemberName]
        string? memberName = default,
        [CallerFilePath]
        string? filePath = default,
        [CallerLineNumber]
        int lineNumber = default
    ) => new(value, argumentExpression, memberName, filePath, lineNumber);

    public static T Print<T>(
        T value,
        [CallerArgumentExpression("value")]
        string? expression = default
    ) {
        var str = Row.Of(expression, value).Prettify();
        Console.WriteLine(str);
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

    internal static ConsoleTraceWriter Start(
        TraceLevel level = TraceLevel.Info,
        [CallerMemberName]
        string? caller = default,
        string? owner = default,
        [CallerFilePath]
        string? callerFile = default
    ) {
        owner ??= Path.GetFileNameWithoutExtension(callerFile);
        return ConsoleTraceWriter.Start(level, caller, owner);
    }
}

public record CallerInfo<T>(
    T Value,
    [CallerArgumentExpression("Value")]
    string? ArgumentExpression = default,
    [CallerMemberName]
    string? MemberName = default,
    [CallerFilePath]
    string? FilePath = default,
    [CallerLineNumber]
    int LineNumber = default
) {
    public Type    Type     => typeof(T);
    public string? FileName => Path.GetFileNameWithoutExtension(FilePath);

    public static implicit operator CallerInfo<T>(T obj) {
        return new CallerInfo<T>(obj);
    }

    public override string ToString() {
        const int calledByLength = 30;
        var       memberNameStr  = $"::{MemberName}";
        var       remaining      = calledByLength - memberNameStr.Length;
        var       fileNameStr    = FileName?.Truncate(remaining);
        var       callerStr      = $"{fileNameStr}{memberNameStr}".Align(width: calledByLength, alignment: StringAlignment.Right);
        Must.Equal(callerStr.Length, calledByLength);
        var exp   = $"{ArgumentExpression}".Align(40);
        var value = $"{Value.Prettify()}";
        return $"{callerStr}  {exp}  {value}";
    }
}