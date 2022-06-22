using System;
using System.Linq;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Represents a <see cref="string"/> that doesn't contain any <see cref="LineBreakChars"/>.
/// </summary>
public sealed record OneLine : Wrapped<string> {
    private const    string LineBreakChars = "\n\r";
    private readonly string _value         = default!;
    private string InitValue {
        get => _value;
        init => _value = Must.NotContainAny(value, LineBreakChars.AsEnumerable());
    }
    public override string Value => InitValue;

    public OneLine(string value) => InitValue = value;

    /// <summary>
    /// <inheritdoc cref="string.Length"/>
    /// </summary>
    public int Length => Value.Length;

    /// <inheritdoc cref="string.this[int]"/>
    public char this[int index] => Value[index];

    /// <inheritdoc cref="string.Substring(int,int)"/>
    public OneLine this[Range range] => new(Value[range]);
}