using System.Text.RegularExpressions;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Ensures that <see cref="ValidatedString.Value"/>:
/// <ul>
/// <li>Doesn't contain whitespace</li>
/// <li>Contains at most 1 leading period</li>
/// </ul>
/// </summary>
public class FileExtension : PathPart {
    private static readonly Regex Pattern = new Regex(@"^\.[^\s\.]$");

    public FileExtension(string value) : base(Fluff(value)) {
        Must.Match(value, Pattern);
    }

    private static string Fluff(string value) => value.PrependIfMissing(".");
}