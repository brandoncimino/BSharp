using System.Text.RegularExpressions;

using FowlFever.BSharp;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Ensures that <see cref="Wrapped{}.Value"/>:
/// <ul>
/// <li>Doesn't contain whitespace</li>
/// <li>Contains at most 1 leading period</li>
/// </ul>
/// </summary>
public record FileExtension(string Value) : FileNamePart(Value) {
    private static readonly Regex Pattern = new Regex(@"^\.[^\s\.]$");

    internal override string AfterValidation(string value) => value.PrependIfMissing(".");
}