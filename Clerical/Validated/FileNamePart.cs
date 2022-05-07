using System.Text.RegularExpressions;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Ensures that a string:
/// <ul>
/// <li>Is a valid <see cref="PathPart"/></li>
/// <li>Doesn't contain whitespace</li>
/// <li>Doesn't contain periods</li>
/// </ul>
/// </summary>
public class FileNamePart : PathPart {
    public FileNamePart(string value) : base(value) {
        var whitespacePattern = new Regex(@"\s");
        var methodName        = $"new {GetType().Name}";
        Must.NotMatch(value, whitespacePattern,               methodName: methodName);
        Must.NotMatch(value, BPath.DirectorySeparatorPattern, methodName: methodName);
    }
}