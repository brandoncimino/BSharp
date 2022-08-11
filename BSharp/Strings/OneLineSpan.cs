using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// A "by-ref" version of <see cref="OneLine"/>.
/// </summary>
public readonly ref struct OneLineSpan {
    public ReadOnlySpan<char> Line { get; }

    public OneLineSpan(ReadOnlySpan<char> line) : this(line, OneLine.ShouldValidate.Yes) { }

    internal OneLineSpan(ReadOnlySpan<char> line, OneLine.ShouldValidate shouldValidate) {
        Line = shouldValidate switch {
            OneLine.ShouldValidate.Yes => OneLine.Validate(line),
            OneLine.ShouldValidate.No  => line,
            _                          => throw BEnum.UnhandledSwitch(shouldValidate)
        };
    }

    public OneLine ToOneLine() => new(Line, OneLine.ShouldValidate.No);
}