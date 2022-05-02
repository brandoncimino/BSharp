using System.Text.RegularExpressions;

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
public record FileNamePart : PathPart {
    public FileNamePart(string value) : base(value) {
        var whitespacePattern = new Regex(@"\s");
        var methodName        = $"new {GetType().Name}";
        Must.NotMatch(value, whitespacePattern,               methodName: methodName);
        Must.NotMatch(value, BPath.DirectorySeparatorPattern, methodName: methodName);
    }
}