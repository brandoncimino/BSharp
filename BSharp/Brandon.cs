using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Json;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp;

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
public static class Brandon {
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

    public static void Print<T>(
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
        Console.WriteLine(info);
    }

    public static ConsoleTraceWriter Start([CallerMemberName] string? caller = default, string? owner = default, [CallerFilePath] string? callerFile = default) {
        owner ??= BPath.GetFileNameWithoutExtensions(callerFile);
        return ConsoleTraceWriter.Start(caller, owner);
    }

    public static void Trace(
        Action<ITraceWriter>       code,
        [CallerMemberName] string? caller     = default,
        string?                    owner      = default,
        [CallerFilePath] string?   callerFile = default
    ) {
        owner ??= BPath.GetFileNameWithoutExtensions(callerFile);
        var tracer = Start(caller, owner);
        code(tracer);
        tracer.End();
    }

    public static void Trace(
        Action                     code,
        [CallerMemberName] string? caller     = default,
        string?                    owner      = default,
        [CallerFilePath] string?   callerFile = default
    ) {
        owner ??= BPath.GetFileNameWithoutExtensions(callerFile);
        Trace(_ => code(), caller, owner);
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
    public string? FileName => FilePath == null ? null : BPath.GetFileNameWithoutExtensions(FilePath);

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