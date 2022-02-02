using System.IO;
using System.Text.RegularExpressions;

using FluentValidation;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Guarantees that a string:
/// <ul>
/// <li>Doesn't contain <see cref="DirectorySeparator"/>s</li>
/// <li>Doesn't contain whitespace</li>
/// <li>Doesn't contain periods</li>
/// <li>TODO: Doesn't contain any <see cref="Path.GetInvalidPathChars"/></li>
/// <li>TODO: Doesn't contain any <see cref="Path.GetInvalidFileNameChars"/></li>
/// </ul>
/// </summary>
public class FileNamePart {
    public readonly string Value;

    public FileNamePart(string value) {
        var whitespacePattern = new Regex(@"\s");
        var arg = new ArgInfo<string?>(value, nameof(Value), $"new {GetType().Name}");
        Must.NotMatch(arg, whitespacePattern);
        Must.NotMatch(arg, BPath.DirectorySeparatorPattern);

        Value = value;
    }
}