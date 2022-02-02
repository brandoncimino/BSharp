using System.IO;
using System.Text.RegularExpressions;

using FluentValidation;
using FluentValidation.Validators;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Ensures that the <see cref="Value"/>:
/// <ul>
/// <li>Doesn't contain <see cref="DirectorySeparator"/>s</li>
/// <li>Doesn't contain whitespace</li>
/// <li>TODO: Doesn't contain any <see cref="Path.GetInvalidPathChars"/></li>
/// <li>TODO: Doesn't contain any <see cref="Path.GetInvalidFileNameChars"/></li>
/// </ul>
/// </summary>
/// <remarks>
/// This is a less-strict version of <see cref="FileNamePart"/>.
/// </remarks>
public class PathPart {
    public readonly string Value;

    public PathPart(string value) {
        var whitespacePattern = new Regex(@"\s");
        var arg               = new ArgInfo<string?>(value, nameof(Value), $"new {GetType().Name}");
        Must.NotMatch(arg, whitespacePattern);
        Must.NotMatch(arg, BPath.DirectorySeparatorPattern);

        Value = value;
    }

    public override string ToString() {
        return Value;
    }
}