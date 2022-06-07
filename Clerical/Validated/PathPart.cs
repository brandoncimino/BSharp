using System.Collections.Immutable;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;
using FowlFever.Clerical.Fluffy;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Ensures that the <see cref="PathPart.Value"/>:
/// <ul>
/// <li>Doesn't contain <see cref="DirectorySeparator"/>s</li>
/// <li>Doesn't contain any <see cref="Path.GetInvalidPathChars"/></li>
/// <li>Doesn't contain any <see cref="Path.GetInvalidFileNameChars"/></li>
/// </ul>
/// </summary>
/// <remarks>
/// This is a less-strict version of <see cref="FileNamePart"/>.
/// </remarks>
/// <seealso cref="FileNamePart"/>
public record PathPart(string Value) : Validated<string>(Value) {
    public static readonly ImmutableHashSet<char> InvalidChars = Path.GetInvalidPathChars()
                                                                     .Union(Path.GetInvalidFileNameChars())
                                                                     .Union(BPath.SeparatorChars)
                                                                     .ToImmutableHashSet();

    public static implicit operator PathPart(string str) => new(str);

    [Validator] internal void NoInvalidPathChars() => Must.NotContain(Value, InvalidChars);

    [Validator] internal void NotBlank() => Must.NotBeBlank(Value);
}