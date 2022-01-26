using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// Mostly contains extension method version of basic <see cref="Regex"/> stuff.
    /// </summary>
    [PublicAPI]
    public static class RegexUtils {
        #region Match

        /// <returns><see cref="Regex.Match(string,string)"/></returns>
        public static Match Match(this string str, string pattern) {
            return Regex.Match(str, pattern);
        }

        /// <returns><see cref="Regex.Match(string, string, RegexOptions)"/></returns>
        public static Match Match(this string str, string pattern, RegexOptions options) {
            return Regex.Match(str, pattern, options);
        }

        /// <returns><see cref="Regex.Match(string)"/></returns>
        public static Match Match(this string str, Regex pattern) {
            return pattern.Match(str);
        }

        #endregion

        #region AllMatches

        /// <summary>
        /// This is an extension method for <see cref="Regex.Matches(string)"/>.</summary>
        /// <remarks>
        /// <inheritdoc cref="Matches(string,string)"/>
        /// </remarks>
        /// <returns><see cref="Regex.Matches(string, string)"/></returns>
        public static MatchCollection AllMatches(this string str, string pattern) {
            return Regex.Matches(str, pattern);
        }

        /// <inheritdoc cref="AllMatches(string,string)"/>
        /// <returns><see cref="Regex.Matches(string, string, RegexOptions)"/></returns>
        public static MatchCollection AllMatches(this string str, string pattern, RegexOptions options) {
            return Regex.Matches(str, pattern, options);
        }

        /// <inheritdoc cref="AllMatches(string,string)"/>
        /// <returns><see cref="Regex.Matches(string)"/></returns>
        public static MatchCollection AllMatches(this string str, Regex pattern) {
            return pattern.Matches(str);
        }

        #endregion

        #region Matches

        /// <summary>
        /// This is an extension method for <see cref="Regex.IsMatch(string)"/>.
        /// </summary>
        /// <remarks>
        /// I am aware that "Matches" would correspond to <see cref="Regex.Matches(string)"/> for consistency.
        /// However, that method name causes me intense physical discomfort.
        /// If you want to call that stupid dumbly named dumb bad method, use <see cref="AllMatches(string,string)"/> instead.
        /// <p/>
        /// Strangely, despite the hideous 80-thousand extra steps of using <a href="https://docs.oracle.com/javase/8/docs/api/java/util/regex/Pattern.html">Java's Regex library</a>, Java actually uses the correct verb, <a href="https://docs.oracle.com/javase/8/docs/api/java/util/regex/Pattern.html#matches-java.lang.String-java.lang.CharSequence-">Matches()</a>.
        /// </remarks>
        /// <returns><see cref="Regex.IsMatch(string, string)"/></returns>
        [Pure]
        public static bool Matches(this string str, string pattern) {
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// Inverse of <see cref="Matches(string,string)"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [Pure]
        public static bool DoesNotMatch(this string str, string pattern) {
            return !str.Matches(pattern);
        }

        /// <summary>
        /// <inheritdoc cref="Matches(string,string)"/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="Matches(string,string)"/>
        /// </remarks>
        /// <param name="str"><inheritdoc cref="Matches(string,string)"/></param>
        /// <param name="pattern"><inheritdoc cref="Matches(string,string)"/></param>
        /// <param name="options">any applicable <see cref="RegexOptions"/>, like <see cref="RegexOptions.Multiline"/></param>
        /// <returns><see cref="Regex.IsMatch(string, string, RegexOptions)"/></returns>
        [Pure]
        public static bool Matches(this string str, string pattern, RegexOptions options) {
            return Regex.IsMatch(str, pattern, options);
        }

        /// <summary>
        /// Inverse of <see cref="Matches(string,string,RegexOptions)"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [Pure]
        public static bool DoesNotMatch(this string str, string pattern, RegexOptions options) {
            return !str.Matches(pattern, options);
        }

        /// <inheritdoc cref="Matches(string,string)"/>
        /// <returns><see cref="Regex.IsMatch(string)"/></returns>
        [Pure]
        public static bool Matches(this string str, Regex pattern) {
            return pattern.IsMatch(str);
        }

        /// <summary>
        /// Inverse of <see cref="Matches(string,Regex)"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [Pure]
        public static bool DoesNotMatch(this string str, Regex pattern) {
            return !str.Matches(pattern);
        }

        #endregion

        #region MatchesAll

        [Pure]
        public static bool MatchesAll(this string str, IEnumerable<string> patterns, RegexOptions? options = default) {
            return options.IfPresentOrElse(
                rOpts => patterns.All(pat => Regex.IsMatch(str, pat, rOpts)),
                () => patterns.All(pat => Regex.IsMatch(str,    pat))
            );
        }

        [Pure]
        public static bool MatchesAll(this string str, params string[] patterns) => MatchesAll(str, patterns.AsEnumerable());

        [Pure]
        public static bool MatchesAll(this string str, params Regex[] patterns) => MatchesAll(str, patterns.AsEnumerable());

        [Pure]
        public static bool MatchesAll(this string str, IEnumerable<Regex> patterns, RegexOptions? options = default) => patterns.All(it => it.IsMatch(str));

        #endregion

        #region MatchesAny

        [Pure]
        public static bool MatchesAny(this string str, IEnumerable<string> patterns, RegexOptions? options = default) {
            return options.IfPresentOrElse(
                rOpts => patterns.Any(pat => Regex.IsMatch(str, pat, rOpts)),
                () => patterns.Any(pat => Regex.IsMatch(str,    pat))
            );
        }

        [Pure]
        public static bool MatchesAny(this string str, params string[] patterns) => MatchesAny(str, patterns.AsEnumerable());

        [Pure]
        public static bool MatchesAny(this string str, IEnumerable<Regex> patterns) => patterns.Any(it => it.IsMatch(str));

        [Pure]
        public static bool MatchesAny(this string str, params Regex[] patterns) => MatchesAny(str, patterns.AsEnumerable());

        #endregion

        #region MatchesNone

        public static bool MatchesNone(this string str, IEnumerable<string> patterns, RegexOptions? options = default) {
            return options.IfPresentOrElse(
                rOpts => patterns.None(pat => Regex.IsMatch(str, pat, rOpts)),
                () => patterns.None(pat => Regex.IsMatch(str,    pat))
            );
        }

        [Pure]
        public static bool MatchesNone(this string str, params string[] patterns) => MatchesNone(str, patterns.AsEnumerable());

        [Pure]
        public static bool MatchesNone(this string str, IEnumerable<Regex> patterns) => patterns.None(it => it.IsMatch(str));

        [Pure]
        public static bool MatchesNone(this string str, params Regex[] patterns) => MatchesNone(str, patterns.AsEnumerable());

        #endregion

        #region String Class Methods (like EndsWith())

        /// <summary>
        /// Similar to <see cref="string.EndsWith(string)"/>, but looks for a <see cref="Regex"/> pattern.
        /// </summary>
        /// <param name="input">the input <see cref="string"/></param>
        /// <param name="pattern">the <see cref="Regex"/> to look for</param>
        /// <returns>true if <paramref name="input"/> ends with <paramref name="pattern"/></returns>
        [ContractAnnotation("input:null => false")]
        [Pure]
        public static bool EndsWith(this string? input, Regex pattern) {
            return input != null && new Regex($"{pattern}$").IsMatch(input);
        }

        /// <summary>
        /// Similar to <see cref="string.StartsWith(string)"/>, but looks for a <see cref="Regex"/> pattern.
        /// </summary>
        /// <param name="input">the input <see cref="string"/></param>
        /// <param name="pattern">the <see cref="Regex"/> to look for</param>
        /// <returns>true if <paramref name="input"/> starts with <paramref name="pattern"/></returns>
        [ContractAnnotation("input:null => false")]
        [Pure]
        public static bool StartsWith(this string? input, Regex pattern) {
            return input != null && new Regex($"^{pattern}").IsMatch(input);
        }

        /// <summary>
        /// Similar to <see cref="string.Split(char[])"/>, but looks for a <see cref="Regex"/> pattern.
        /// </summary>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="pattern">the <see cref="Regex"/> pattern used to split <paramref name="input"/></param>
        /// <returns></returns>
        [Pure]
        public static string[] Split(this string input, Regex pattern) {
            return pattern.Split(input);
        }

        #endregion

        /// <summary>
        /// Removes <b>at most 1</b> instance of <paramref name="pattern"/> from the <b>end</b> of <paramref name="input"/>
        /// </summary>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="pattern">the <see cref="Regex"/> pattern being removed</param>
        /// <returns><paramref name="input"/> with <b>at most 1</b> instance of <paramref name="pattern"/> removed</returns>
        [Pure]
        public static string RemoveLast(this string input, Regex pattern) {
            pattern = new Regex(pattern.ToString(), RegexOptions.RightToLeft);
            return pattern.Replace(input, "", 1);
        }

        /// <summary>
        /// Removes <b>at most 1</b> instance of <paramref name="pattern"/> from the <b>end</b> of <paramref name="input"/>
        /// </summary>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="pattern">the <see cref="Regex"/> pattern being removed</param>
        /// <returns><paramref name="input"/> with <b>at most 1</b> instance of <paramref name="pattern"/> removed</returns>
        [Pure]
        public static string RemoveFirst(this string input, Regex pattern) {
            var opts = pattern.Options & ~RegexOptions.RightToLeft;
            pattern = new Regex(pattern.ToString(), opts);
            return pattern.Replace(input, "", 1);
        }
    }
}