using System.Collections.Immutable;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;

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
public class PathPart : ValidatedString {
    public static readonly ImmutableHashSet<char> InvalidChars = Path.GetInvalidPathChars()
                                                                     .Union(Path.GetInvalidFileNameChars())
                                                                     .Union(BPath.SeparatorChars)
                                                                     .ToImmutableHashSet();

    public PathPart(string value) : base(value) {
        Must.NotContain(value, InvalidChars, methodName: $"new {GetType().Name}");
    }
}