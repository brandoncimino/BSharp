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
}