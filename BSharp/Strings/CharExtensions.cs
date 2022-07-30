using System.Globalization;
using System.Text.RegularExpressions;

namespace FowlFever.BSharp.Strings;

public static class CharExtensions {
    public static bool IsLineBreak(this char c) => c is '\n' or '\r';

    private static readonly Regex WordCharacter    = new(@"^\w$");
    private static readonly Regex NonWordCharacter = new(@"^\W$");
    public static           bool  IsWordCharacter(this    char character) => WordCharacter.IsMatch(character.ToString());
    public static           bool  IsNonWordCharacter(this char character) => NonWordCharacter.IsMatch(character.ToString());

    private const char ZeroWidthJoiner = '\u200D';
    public static bool IsZeroWidthJoiner(this char character) => character == ZeroWidthJoiner;

    /// <param name="c">this <see cref="char"/></param>
    /// <returns>true if <paramref name="c"/> is <c>/</c> or <c>\</c></returns>
    public static bool IsDirectorySeparator(this char c) => c is '\\' or '/';

    #region Built-In Method Extensions

    /// <inheritdoc cref="char.IsLetter(char)"/>
    public static bool IsLetter(this char c) => char.IsLetter(c);

    /// <inheritdoc cref="char.IsNumber(char)"/>
    public static bool IsNumber(this char c) => char.IsNumber(c);

    /// <inheritdoc cref="char.IsWhiteSpace(char)"/>
    public static bool IsWhitespace(this char c) => char.IsWhiteSpace(c);

    /// <inheritdoc cref="char.IsLower(char)"/>
    public static bool IsLower(this char c) => char.IsLower(c);

    /// <inheritdoc cref="char.IsUpper(char)"/>
    public static bool IsUpper(this char c) => char.IsUpper(c);

    /// <inheritdoc cref="char.GetUnicodeCategory(char)"/>
    public static UnicodeCategory GetUnicodeCategory(this char c) => char.GetUnicodeCategory(c);

    #endregion
}