using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Strings;

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
        var calledBy = $"[{FileName}.{MemberName}]".ForceToLength(20, trail: $"{StringUtils.Ellipsis}]");
        var exp      = $"{ArgumentExpression}".ForceToLength(30);
        var value    = $"{Value.Prettify()}";
        return $"{calledBy} {exp} {value}";
    }
}