using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    private static InvalidOperationException _EmptyException(string typeName, [CallerMemberName] string? _caller = default) {
        return new InvalidOperationException($"💥 {_caller}: this {typeName} is empty!");
    }

    private static InvalidOperationException _Unreachable([CallerMemberName] string? _caller = default, [CallerFilePath] string? _file = default, [CallerLineNumber] int _lineNo = default) {
        return new InvalidOperationException($"🕵️‍♀️ {_caller} in {_file} @ line #{_lineNo} should have been unreachable!");
    }
}