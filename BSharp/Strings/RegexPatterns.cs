using System.Collections.Generic;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Methods to help build & transform <see cref="Regex"/> patterns.
/// </summary>
/// <remarks>
/// If possible, these methods should be avoided in favor of <see cref="T:System.Text.RegularExpressions.GeneratedRegexAttribute"/>.
/// </remarks>
[PublicAPI]
public static class RegexPatterns {
    public static readonly Regex NotStart = new Regex("(?<!^)");
    public static readonly Regex NotEnd   = new Regex("(?!$)");

    [Pure]
    public static string InnerMatch(string originalPattern) {
        return $"{NotStart}{originalPattern}{NotEnd}";
    }

    [Pure]
    public static Regex InnerMatch(Regex originalPattern) {
        return new Regex(InnerMatch(originalPattern.ToString()), originalPattern.Options);
    }

    [Pure]
    private static string OuterMatch(string originalPattern) {
        return $"(^{originalPattern}|{originalPattern}$)";
    }

    [Pure]
    public static Regex OuterMatch(Regex originalPattern) {
        return new Regex(OuterMatch(originalPattern.ToString()));
    }

    [Pure]
    public static Regex Escaped(string exactString) {
        return new Regex(Regex.Escape(exactString));
    }

    /// <summary>
    /// Creates a character group regex, which may be <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#positive-character-group--">positive</a>,
    /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#negative-character-group-">negative</a>,
    /// or <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#character-class-subtraction-base_group---excluded_group">subtractive</a>.
    /// </summary>
    /// <param name="inclusions">the <see cref="char"/>s that the group <b><i>should</i></b> match</param>
    /// <param name="exclusions">the <see cref="char"/>s that the group <b><i>should not</i></b> match</param>
    /// <returns>a new <see cref="Regex"/></returns>
    [Pure]
    public static Regex CharacterSet(IEnumerable<char>? inclusions, IEnumerable<char>? exclusions) => new RegexCharacterSet(inclusions, exclusions);

    /// <summary>
    /// Creates a <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#positive-character-group--">positive character group</a>.
    /// </summary>
    /// <param name="inclusions">the <see cref="char"/>s that you WANT to match</param>
    /// <returns>a new <see cref="Regex"/></returns>
    [Pure]
    public static Regex InclusiveCharacterGroup(IEnumerable<char> inclusions) => new RegexCharacterSet(Inclusions: inclusions);

    /// <summary>
    /// Creates a <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#negative-character-group-">negative character group</a>.
    /// </summary>
    /// <param name="exclusions"></param>
    /// <returns></returns>
    [Pure]
    public static Regex ExclusiveCharacterGroup(IEnumerable<char> exclusions) => new RegexCharacterSet(Exclusions: exclusions);
}