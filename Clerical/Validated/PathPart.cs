using System.Collections.Immutable;
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
/// <li>Doesn't contain any <see cref="Path.GetInvalidPathChars"/></li>
/// <li>Doesn't contain any <see cref="Path.GetInvalidFileNameChars"/></li>
/// </ul>
/// </summary>
/// <remarks>
/// This is a less-strict version of <see cref="FileNamePart"/>.
/// </remarks>
public class PathPart {
    public static readonly ImmutableHashSet<char> InvalidChars = Path.GetInvalidPathChars()
                                                                     .Union(Path.GetInvalidFileNameChars())
                                                                     .Union(BPath.Separators)
                                                                     .ToImmutableHashSet();

    public readonly string Value;

    public PathPart(string value) {
        var arg = new ArgInfo<string?>(value, nameof(value), $"new {GetType().Name}");
        Must.NotContain(arg, InvalidChars);

        Value = value;
    }

    public override string ToString() {
        return Value;
    }
}