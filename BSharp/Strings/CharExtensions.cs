using System.Linq;
using System.Text.RegularExpressions;

namespace FowlFever.BSharp.Strings;

public static class CharExtensions {
    private const           string LineBreakCharacters = "\n\r";
    public static           bool   IsLineBreak(this char character) => LineBreakCharacters.Contains(character);
    private static readonly Regex  WordCharacter = new Regex(@"^\w$");
    public static           bool   IsWordCharacter(this char character) => WordCharacter.IsMatch(character.ToString());
    private static readonly Regex  NonWordCharacter = new Regex(@"^\W$");
    public static           bool   IsNonWordCharacter(this char character) => NonWordCharacter.IsMatch(character.ToString());
    private const           char   ZeroWidthJoiner = '\u200D';
    public static           bool   IsZeroWidthJoiner(this char character) => character == ZeroWidthJoiner;
}