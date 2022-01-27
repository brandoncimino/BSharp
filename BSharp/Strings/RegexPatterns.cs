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
}