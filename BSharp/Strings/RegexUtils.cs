using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.Implementors;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Mostly contains extension method version of basic <see cref="Regex"/> stuff.
/// </summary>
[PublicAPI]
public static class RegexUtils {
    #region Match

    /// <returns><see cref="Regex.Match(string, string, RegexOptions)"/></returns>
    [Pure]
    public static Match Match(this string str, string pattern, RegexOptions options = RegexOptions.None) => Regex.Match(str, pattern, options);

    /// <returns><see cref="Regex.Match(string)"/></returns>
    [Pure]
    public static Match Match(this string str, Regex pattern) => pattern.Match(str);

    /// <returns><see cref="Regex.Match(string)"/></returns>
    [Pure]
    public static Match Match(this string str, IHas<Regex> pattern) => pattern.Value.Match(str);

    #endregion

    #region AllMatches

    /// <summary>
    /// This is an extension method for <see cref="Regex.Matches(string)"/>.</summary>
    /// <remarks>
    /// <inheritdoc cref="Matches(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    /// </remarks>
    /// <returns><see cref="Regex.Matches(string, string)"/></returns>
    public static MatchCollection AllMatches(this string str, string pattern, RegexOptions options = RegexOptions.None) => Regex.Matches(str, pattern, options);

    /// <inheritdoc cref="AllMatches(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    /// <returns><see cref="Regex.Matches(string)"/></returns>
    public static MatchCollection AllMatches(this string str, Regex pattern) => pattern.Matches(str);

    /// <inheritdoc cref="AllMatches(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    public static MatchCollection AllMatches(this string str, IHas<Regex> pattern) => pattern.Value.Matches(str);

    #endregion

    #region Matches

    /// <summary>
    /// This is an extension method for <see cref="Regex.IsMatch(string)"/>.
    /// </summary>
    /// <remarks>
    /// I am aware that "Matches" would correspond to <see cref="Regex.Matches(string)"/> for consistency.
    /// However, that method name causes me intense physical discomfort.
    /// If you want to call that stupid dumbly named dumb bad method, use <see cref="AllMatches(string,string,System.Text.RegularExpressions.RegexOptions)"/> instead.
    /// <p/>
    /// Strangely, despite the hideous 80-thousand extra steps of using <a href="https://docs.oracle.com/javase/8/docs/api/java/util/regex/Pattern.html">Java's Regex library</a>, Java actually uses the correct verb, <a href="https://docs.oracle.com/javase/8/docs/api/java/util/regex/Pattern.html#matches-java.lang.String-java.lang.CharSequence-">Matches()</a>.
    /// </remarks>
    /// <returns><see cref="Regex.IsMatch(string, string)"/></returns>
    [Pure]
    public static bool Matches(this string str, string pattern, RegexOptions options = RegexOptions.None) => Regex.IsMatch(str, pattern, options);

    /// <inheritdoc cref="Matches(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    /// <returns><see cref="Regex.IsMatch(string)"/></returns>
    [Pure]
    public static bool Matches(this string str, Regex pattern) => pattern.IsMatch(str);

    /// <inheritdoc cref="Matches(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    [Pure]
    public static bool Matches(this string str, IHas<Regex> pattern) => pattern.Value.IsMatch(str);

    /// <summary>
    /// Inverse of <see cref="Matches(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    /// </summary>
    /// <param name="str"></param>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [Pure]
    public static bool DoesNotMatch(this string str, string pattern, RegexOptions options = RegexOptions.None) => !str.Matches(pattern);

    /// <summary>
    /// Inverse of <see cref="Matches(string,Regex)"/>
    /// </summary>
    /// <param name="str"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [Pure]
    public static bool DoesNotMatch(this string str, Regex pattern) => !str.Matches(pattern);

    /// <inheritdoc cref="DoesNotMatch(string,string,System.Text.RegularExpressions.RegexOptions)"/>
    [Pure]
    public static bool DoesNotMatch(this string str, IHas<Regex> pattern) => !str.Matches(pattern);

    #endregion

    #region MatchesAll

    [Pure] public static bool MatchesAll(this string str, IEnumerable<string> patterns, RegexOptions options = RegexOptions.None) => patterns.All(pat => Regex.IsMatch(str, pat, options));
    [Pure] public static bool MatchesAll(this string str, IEnumerable<Regex>  patterns, RegexOptions options = RegexOptions.None) => patterns.All(it => it.IsMatch(str));
    [Pure] public static bool MatchesAll(this string str, params string[]     patterns) => MatchesAll(str, patterns.AsEnumerable());
    [Pure] public static bool MatchesAll(this string str, params Regex[]      patterns) => MatchesAll(str, patterns.AsEnumerable());

    #endregion

    #region MatchesAny

    [Pure] public static bool MatchesAny(this string str, IEnumerable<string> patterns, RegexOptions options = RegexOptions.None) => patterns.Any(pat => Regex.IsMatch(str, pat, options));
    [Pure] public static bool MatchesAny(this string str, IEnumerable<Regex>  patterns) => patterns.Any(it => it.IsMatch(str));
    [Pure] public static bool MatchesAny(this string str, params string[]     patterns) => MatchesAny(str, patterns.AsEnumerable());
    [Pure] public static bool MatchesAny(this string str, params Regex[]      patterns) => MatchesAny(str, patterns.AsEnumerable());

    #endregion

    #region MatchesNone

    [Pure] public static bool MatchesNone(this string str, IEnumerable<string> patterns, RegexOptions options = RegexOptions.None) => patterns.None(pat => Regex.IsMatch(str, pat, options));
    [Pure] public static bool MatchesNone(this string str, IEnumerable<Regex>  patterns) => patterns.None(it => it.IsMatch(str));
    [Pure] public static bool MatchesNone(this string str, params string[]     patterns) => MatchesNone(str, patterns.AsEnumerable());
    [Pure] public static bool MatchesNone(this string str, params Regex[]      patterns) => MatchesNone(str, patterns.AsEnumerable());

    #endregion

    #region String Class Methods (like EndsWith())

    /// <summary>
    /// Similar to <see cref="string.EndsWith(string)"/>, but looks for a <see cref="Regex"/> pattern.
    /// </summary>
    /// <param name="input">the input <see cref="string"/></param>
    /// <param name="pattern">the <see cref="Regex"/> to look for</param>
    /// <returns>true if <paramref name="input"/> ends with <paramref name="pattern"/></returns>
    [Pure]
    public static bool EndsWith(this string input, Regex pattern) => new Regex($"{pattern}$").IsMatch(input);

    /// <summary>
    /// Similar to <see cref="string.StartsWith(string)"/>, but looks for a <see cref="Regex"/> pattern.
    /// </summary>
    /// <param name="input">the input <see cref="string"/></param>
    /// <param name="pattern">the <see cref="Regex"/> to look for</param>
    /// <returns>true if <paramref name="input"/> starts with <paramref name="pattern"/></returns>
    [Pure]
    public static bool StartsWith(this string input, Regex pattern) => new Regex($"^{pattern}").IsMatch(input);

    /// <summary>
    /// Similar to <see cref="string.Split(char[])"/>, but looks for a <see cref="Regex"/> pattern.
    /// </summary>
    /// <param name="input">the original <see cref="string"/></param>
    /// <param name="pattern">the <see cref="Regex"/> pattern used to split <paramref name="input"/></param>
    /// <returns></returns>
    [Pure]
    public static string[] Split(this string input, Regex pattern) => pattern.Split(input);

    #endregion

    /// <summary>
    /// Removes <b>at most <paramref name="count"/></b> instances of <paramref name="pattern"/> from the <b>end</b> of <paramref name="input"/>.
    /// </summary>
    /// <param name="input">the original <see cref="string"/></param>
    /// <param name="pattern">the <see cref="Regex"/> pattern being removed</param>
    /// <param name="count">the maximum number of <paramref name="pattern"/> matches to be removed</param>
    /// <returns><paramref name="input"/> with <b>at most <paramref name="count"/></b> instance of <paramref name="pattern"/> removed</returns>
    [Pure]
    public static string RemoveLast(this string input, Regex pattern, int count = 1) {
        pattern = new Regex(pattern.ToString(), RegexOptions.RightToLeft);
        return pattern.Replace(input, "", count);
    }

    /// <summary>
    /// Removes <b>at most <paramref name="count"/></b> instances of <paramref name="pattern"/> from the <b>beginning</b> of <paramref name="input"/>.
    /// </summary>
    /// <param name="input">the original <see cref="string"/></param>
    /// <param name="pattern">the <see cref="Regex"/> pattern being removed</param>
    /// <param name="count">the maximum number of <paramref name="pattern"/> matches to be removed</param>
    /// <returns><paramref name="input"/> with <b>at most <paramref name="count"/></b> instance of <paramref name="pattern"/> removed</returns>
    [Pure]
    public static string RemoveFirst(this string input, Regex pattern, int count = 1) {
        var opts = pattern.Options & ~RegexOptions.RightToLeft;
        pattern = new Regex(pattern.ToString(), opts);
        return pattern.Replace(input, "", count);
    }
}