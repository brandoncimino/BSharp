using System.Collections.Generic;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Methods to help build & transform <see cref="Regex"/> patterns.
/// </summary>
[PublicAPI]
public static class RegexPatterns {
    public static readonly Regex NotStart = new Regex("(?<!^)");
    public static readonly Regex NotEnd   = new Regex("(?!$)");

    [System.Diagnostics.Contracts.Pure]
    public static string InnerMatch(string originalPattern) {
        return $"{NotStart}{originalPattern}{NotEnd}";
    }

    [System.Diagnostics.Contracts.Pure]
    public static Regex InnerMatch(Regex originalPattern) {
        return new Regex(InnerMatch(originalPattern.ToString()), originalPattern.Options);
    }

    [System.Diagnostics.Contracts.Pure]
    private static string OuterMatch(string originalPattern) {
        return $"(^{originalPattern}|{originalPattern}$)";
    }

    [System.Diagnostics.Contracts.Pure]
    public static Regex OuterMatch(Regex originalPattern) {
        return new Regex(OuterMatch(originalPattern.ToString()));
    }

    [System.Diagnostics.Contracts.Pure]
    public static Regex Escaped(string exactString) {
        return new Regex(Regex.Escape(exactString));
    }

    [System.Diagnostics.Contracts.Pure]
    public static Regex CharacterSet(IEnumerable<char>? inclusions, IEnumerable<char>? exclusions) => new RegexCharacterSet(inclusions, exclusions);

    [System.Diagnostics.Contracts.Pure]
    public static Regex InclusiveSet(IEnumerable<char> inclusions) => new RegexCharacterSet(Inclusions: inclusions);

    [System.Diagnostics.Contracts.Pure]
    public static Regex ExclusiveSet(IEnumerable<char> exclusions) => new RegexCharacterSet(Exclusions: exclusions);
}