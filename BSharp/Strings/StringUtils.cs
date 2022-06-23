using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

using NotNull = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace FowlFever.BSharp.Strings {
    [PublicAPI]
    public static partial class StringUtils {
        public const int    DefaultIndentSize   = 2;
        public const string DefaultIndentString = "  ";

        /// <summary>
        /// A <see cref="string"/> for a single-glyph <a href="https://en.wikipedia.org/wiki/Ellipsis">ellipsis</a>, i.e. <c>'…'</c>.
        ///
        /// <ul>
        /// <li><b>Unicode:</b> <c>U+2026 … HORIZONTAL ELLIPSIS</c></li>
        /// <li><b>HTML:</b> <c><![CDATA[&#8230;]]></c></li>
        /// </ul>
        /// </summary>
        internal const string Ellipsis = "…";

        /// <summary>
        /// A <see cref="string"/> for the glyph representing the <a href="https://en.wikipedia.org/wiki/Tab_key#Unicode">"tab" key</a>, i.e. one indent.
        ///
        /// <ul>
        /// <li><b>Unicode:</b> <c>U+21E5 ⇥ RIGHTWARDS ARROW TO BAR</c></li>
        /// </ul>
        /// </summary>
        internal const string TabArrow = "⇥";

        internal const char DefaultIndentChar = ' ';

        /// <summary>
        /// Valid strings for <a href="https://en.wikipedia.org/wiki/Newline">line breaks</a>, in <b>order of precedence</b> as required by <see cref="string.Split(string[], StringSplitOptions)"/>:
        /// <ul>
        /// <li><c>"\r\n"</c>, aka "Carriage Return + Line Feed", aka <c>"CRLF"</c></li>
        /// <li><c>"\r"</c>, aka <a href="https://en.wikipedia.org/wiki/Carriage_return">"Carriage Return"</a>, aka <c>"CR"</c></li>
        /// <li><c>"\n"</c>, aka <a href="https://en.wikipedia.org/wiki/Newline#In_programming_languages">"Newline"</a>, aka "Line Feed", aka <c>"LF"</c></li>
        /// </ul>
        /// </summary>
        /// <remarks>
        /// Intended to passed to <see cref="string.Split(string[], StringSplitOptions)"/>.
        /// </remarks>
        internal static readonly string[] LineBreakSplitters = {
            "\r\n", "\r", "\n"
        };

        #region Indentation

        /// <summary>
        /// Determines how <see cref="StringUtils.Indent(string,int,string,FowlFever.BSharp.Strings.StringUtils.IndentMode)">Indent() methods</see> should be applied.
        /// </summary>
        public enum IndentMode { Relative, Absolute }

        /// <summary>
        /// Prepends <paramref name="toIndent"/> with <paramref name="indentCount"/>
        /// </summary>
        /// <param name="toIndent">the <see cref="string" /> to be indented</param>
        /// <param name="indentCount">the number of indentations (i.e. number of times hitting "tab")</param>
        /// <param name="indentString">the <see cref="string"/> used for each indent. Defaults to <see cref="DefaultIndentString"/></param>
        /// <param name="indentMode">either <see cref="IndentMode.Relative"/> or <see cref="IndentMode.Absolute"/></param>
        /// <exception cref="ArgumentOutOfRangeException">if <see cref="IndentMode.Absolute"/> is used when <paramref name="indentCount"/> is negative</exception>
        /// <exception cref="ArgumentException">if <paramref name="indentString"/> <see cref="IsEmpty"/></exception>
        /// <returns>the indented <see cref="string"/></returns>
        /// <seealso cref="Indent(string,int,string,FowlFever.BSharp.Strings.StringUtils.IndentMode)"/>
        public static IEnumerable<string?> Indent(
            this string toIndent,
            int         indentCount  = 1,
            string      indentString = DefaultIndentString,
            IndentMode  indentMode   = IndentMode.Relative
        ) {
            Must.Have(indentString).NotBlank();
            return Enumerable.Repeat(toIndent, 1).Indent(indentCount, indentString, indentMode);
        }

        public static IEnumerable<string> Indent(
            this IEnumerable<string?> toIndent,
            int                       indentCount  = 1,
            string                    indentString = DefaultIndentString,
            IndentMode                indentMode   = IndentMode.Relative
        ) {
            Must.Have(indentString).NotEmpty();
            return indentMode switch {
                IndentMode.Absolute => IndentAbsolute(toIndent, indentCount, indentString),
                IndentMode.Relative => IndentRelative(toIndent, indentCount, indentString),
                _                   => throw BEnum.UnhandledSwitch(indentMode, nameof(indentMode), nameof(Indent)),
            };
        }

        public static IEnumerable<string> IndentRelative(IEnumerable<string?> toIndent, int indentCount = 1, string indentString = DefaultIndentString) {
            return indentCount switch {
                0   => toIndent.SplitLines(),
                > 0 => toIndent.SplitLines().Select(it => it.Prefix(indentString.Repeat(indentCount))),
                < 0 => toIndent.SplitLines().Select(it => TrimStart(it, indentString, indentCount * -1)),
            };
        }

        public static IEnumerable<string> IndentAbsolute(
            IEnumerable<string?> toIndent,
            [NonNegativeValue]
            int indentCount = 1,
            string indentString = DefaultIndentString
        ) {
            Must.Be(indentCount, Mathb.IsStrictlyPositive);
            Must.BePositive(indentCount, nameof(indentCount), nameof(IndentAbsolute));
            return toIndent.SplitLines().Select(it => it.ForceStartingString(indentString, indentCount));
        }

        #endregion

        /// <summary>
        /// Joins <paramref name="toRepeat" /> with itself <paramref name="repetitions" /> times, using the optional <paramref name="separator" />
        /// </summary>
        /// <param name="toRepeat">The <see cref="string" /> to be joined with itself.</param>
        /// <param name="repetitions">The number of times <paramref name="toRepeat" /> should be repeated.</param>
        /// <param name="separator">An optional character, analogous to </param>
        /// <returns><paramref name="toRepeat"/>, joined with itself, <paramref name="repetitions"/> times</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="repetitions"/> is negative</exception>
        public static string Repeat(this string toRepeat, [NonNegativeValue] int repetitions, string? separator = "") {
            Must.BePositive(repetitions, nameof(repetitions), nameof(Repeat));
            return string.Join(separator, Enumerable.Repeat(toRepeat, repetitions));
        }

        /// <inheritdoc cref="Repeat(string,int,string)" />
        public static string Repeat(this char toRepeat, [NonNegativeValue] int repetitions, string? separator = "") {
            Must.BePositive(repetitions, nameof(repetitions), nameof(Repeat));
            return separator is null ? new string(toRepeat, repetitions) : string.Join(separator, Enumerable.Repeat(toRepeat, repetitions));
        }

        /// <summary>
        /// Repeats a <see cref="string"/> until <see cref="desiredLength"/> is reached, with the last entry potentially being partial.
        /// </summary>
        /// <param name="toRepeat"></param>
        /// <param name="desiredLength"></param>
        /// <returns></returns>
        public static string RepeatToLength(this string toRepeat, [NonNegativeValue] int desiredLength) {
            var sb = new StringBuilder();
            for (var i = 0; i < desiredLength; i++) {
                sb.Append(toRepeat[i % toRepeat.Length]);
            }

            return sb.ToString();
            // The following is an example implementation using `string.Create()`
            // return string.Create(
            //     desiredLength,
            //     toRepeat,
            //     (span, source) => {
            //         for (int spanPos = 0; spanPos < desiredLength; spanPos++) {
            //             var sourcePos = spanPos % source.Length;
            //             span[spanPos] = source[sourcePos];
            //         }
            //     }
            // );
        }

        /// <summary>
        /// <see cref="Array.Reverse(System.Array)"/>s the order of each character in this <see cref="string"/>.
        /// <p/>
        /// TODO: This can probably be made more efficient by using <see cref="MemoryExtensions.AsSpan(string?)"/>
        /// TODO: Handle certain sequences that shouldn't be reversed, like composite Emoji (📎 there's gotta be a library built to handle Emoji, right?)
        /// </summary>
        /// <remarks>
        /// This was named "Backwards" to avoid ambiguity with <see cref="Enumerable.Reverse{TSource}"/>.
        /// </remarks>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns>this <see cref="string"/>...but backwards</returns>
        public static string Mirror(this string str) {
            var chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        ///     Joins together <paramref name="baseString" /> and <paramref name="stringToJoin" /> via <paramref name="separator" />,
        ///     <b>
        ///         <i>UNLESS</i>
        ///     </b>
        ///     <paramref name="baseString" /> is <c>null</c>, in which case (<paramref name="stringToJoin" /> <c>?? ""</c>) is returned.
        /// </summary>
        /// <remarks>
        ///     The idea of this is that it can be used to build out a single string and "list out" items, rather than building a <see cref="List{T}" /> and calling <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})" /> against it.
        /// </remarks>
        /// <example>
        ///     <code><![CDATA[
        /// "yolo".Join("swag")      ⇒ yoloswag
        /// "yolo".Join("swag","; ") ⇒ yolo; swag
        /// "".Join("swag", ":")     ⇒ :swag
        /// null.Join(":")           ⇒ swag
        /// ]]></code>
        /// </example>
        /// <param name="baseString">the first <see cref="string"/></param>
        /// <param name="stringToJoin">the second <see cref="string"/></param>
        /// <param name="separator">the <see cref="string"/> interposed betwixt <paramref name="baseString"/> and <paramref name="stringToJoin"/></param>
        /// <returns><paramref name="baseString"/>, <paramref name="separator"/>, and <paramref name="stringToJoin"/> combined together</returns>
        public static string Join(this string? baseString, string? stringToJoin, string? separator = "") {
            return baseString == null ? stringToJoin ?? "" : string.Join(separator, baseString, stringToJoin);
        }

        /// <summary>
        /// Joins two <see cref="string"/>s together <b>IF</b> they both <see cref="IsNotBlank(string?)"/>.
        /// </summary>
        /// <param name="baseString">this <see cref="string"/></param>
        /// <param name="stringToJoin">another <see cref="string"/></param>
        /// <param name="separator">an optional separator to interpose betwixt <paramref name="baseString"/> and <paramref name="stringToJoin"/></param>
        /// <returns>the combined <see cref="string"/></returns>
        public static string JoinNonBlank(this string? baseString, string? stringToJoin, string? separator = "") {
            return (baseString, stringToJoin).Select(IsBlank) switch {
                (true, true)   => "",
                (true, false)  => stringToJoin!,
                (false, true)  => baseString!,
                (false, false) => string.Join(separator, baseString, stringToJoin),
            };
        }

        /// <summary>
        /// Joins <paramref name="first"/> and <paramref name="second"/> together by a <b>single instance</b> of <paramref name="separator"/>.
        /// </summary>
        /// <remarks>
        /// Note that <paramref name="separator"/> won't removed unnecessarily.
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// "a/".JoinWith("/b","/")  →  "a/b"
        /// "a--".JoinWith("b","-")  →  "a-b"
        /// "_a".JoinWith("b_","_")  →  "_a_b_"
        /// null.JoinWith("b","!!")  →  "!!b"
        /// "a".JoinWith(null,"!!")  →  "a!!"
        /// null.JoinWith(null,"!")  →  "!!"
        /// "".JoinWith("a","!!")    →  "!!a"
        /// " ".JoinWith(" ","!!")   →  " !! "
        /// ]]></code>
        /// </example>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="separator"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string JoinWith(
            this string? first,
            string?      second,
            string       separator,
            int          min = 1,
            int          max = 1
        ) {
            //TODO: this will be very inefficient, but who cares!
            var subExp = $"(?<separators>{Regex.Escape(separator)})+";
            Console.WriteLine($"subExp: {subExp}");

            var firstPattern = new Regex(@$"^(?<base>.*?){subExp}$");
            var firstMatch   = first?.Match(firstPattern);
            Console.WriteLine("1 - " + firstMatch.Prettify());
            var firstBase  = firstMatch?.Groups["base"];
            var firstSep   = firstMatch?.Groups["separators"];
            var firstCount = firstSep?.Captures.Count;
            var firstValue = firstMatch?.Success == true ? firstBase?.Value : first;

            var secondPattern = new Regex($"^{subExp}(?<base>.*)$");
            var secondMatch   = second?.Match(secondPattern);
            Console.WriteLine("2 - " + secondMatch.Prettify());
            var secondBase  = secondMatch?.Groups["base"];
            var secondSep   = secondMatch?.Groups["separators"];
            var secondCount = secondSep?.Captures.Count;
            var secondValue = secondMatch?.Success == true ? secondBase?.Value : second;

            var capTotal = firstCount + secondCount;

            //TODO: this could probably be in its own method
            string trueCenter;

            if (capTotal > max) {
                trueCenter = separator.Repeat(max);
            }
            else if (capTotal < min) {
                trueCenter = separator.Repeat(min);
            }
            else {
                trueCenter = firstSep?.Value + secondSep?.Value;
            }

            // return string.Join(separator, first, second);
            return $"{firstValue}{trueCenter}{secondValue}";
        }

        #region Splitex (splitting via a string treated as a Regex pattern)

        /// <summary>
        /// An extension method to call <see cref="Regex.Split(string)"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] Splitex(this string input, string pattern) {
            return Regex.Split(input, pattern);
        }

        /**
         * <inheritdoc cref="Splitex(string,string)"/>
         */
        public static string[] Splitex(this string input, string pattern, RegexOptions options) {
            return Regex.Split(input, pattern, options);
        }

        /**
         * <inheritdoc cref="Splitex(string,string)"/>
         */
        public static string[] Splitex(
            this string  input,
            string       pattern,
            RegexOptions options,
            TimeSpan     matchTimeout
        ) {
            return Regex.Split(input, pattern, options, matchTimeout);
        }

        #endregion

        #region Containment

        #region ContainsAny

        /// <summary>
        /// Returns true if the given <see cref="string"/> <see cref="string.Contains(string)"/> <b>any</b> of the given <see cref="substrings"/>.
        /// </summary>
        /// <remarks>
        /// TODO: I wonder if it would be faster to search for the shortest substring first...? And maybe excluding substrings that wholly contain other substrings?
        /// </remarks>
        /// <param name="str">the <see cref="string"/> to check for <see cref="substrings"/></param>
        /// <param name="substrings">the possible <see cref="substrings"/></param>
        /// <returns>true if the given <see cref="string"/> <see cref="string.Contains(string)"/> <b>any</b> of the given <see cref="substrings"/></returns>
        public static bool ContainsAny(this string str, IEnumerable<string> substrings) {
            return substrings.Any(str.Contains);
        }

        /**
         * <inheritdoc cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static bool ContainsAny(this string str, params string[] substrings) {
            return ContainsAny(str, substrings.AsEnumerable());
        }

        #endregion

        #region DoesNotContain

        /// <param name="str">this <see cref="string"/></param>
        /// <param name="substring">the <see cref="string"/> that shouldn't be inside of <paramref name="str"/></param>
        /// <returns>the inverse of <see cref="string.Contains(string)"/></returns>
        public static bool DoesNotContain(this string str, string substring) {
            return !str.Contains(substring);
        }

        #endregion

        #region DoesNotContainAny

        /// <param name="str">this <see cref="string"/></param>
        /// <param name="substrings">the <see cref="string"/>s that shouldn't be inside of <paramref name="str"/></param>
        /// <returns>the inverse of <see cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/></returns>
        public static bool DoesNotContainAny(this string str, IEnumerable<string> substrings) {
            return !str.ContainsAny(substrings);
        }

        /// <param name="str">this <see cref="string"/></param>
        /// <param name="substrings">the <see cref="string"/>s that shouldn't be inside of <paramref name="str"/></param>
        /// <returns>the inverse of <see cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/></returns>
        public static bool DoesNotContainAny(this string str, params string[] substrings) {
            return !ContainsAny(str, substrings);
        }

        #endregion

        #region ContainsAll

        /// <summary>
        /// Returns true if the given <see cref="string"/> <see cref="string.Contains(string)"/> <b>all</b> of the given <see cref="substrings"/>.
        /// </summary>
        /// <param name="str">the <see cref="string"/> to search through</param>
        /// <param name="substrings">the possible substrings</param>
        /// <returns>true if the given <see cref="string"/> <see cref="string.Contains(string)"/> <b>all</b> of the given <see cref="substrings"/></returns>
        public static bool ContainsAll(this string str, IEnumerable<string> substrings) {
            return substrings.All(str.Contains);
        }

        /**
         * <inheritdoc cref="ContainsAll(string,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static bool ContainsAll(this string str, params string[] substrings) {
            return ContainsAll(str, substrings.AsEnumerable());
        }

        #endregion

        #endregion

        #region Line Management

        /// <summary>
        /// Splits <paramref name="multilineContent"/> via <c>"\r\n", "\r", or "\n"</c>.
        /// </summary>
        /// <remarks>
        /// This method returns an <see cref="Array"/> instead of an <see cref="IEnumerable{T}"/> because <see cref="string.Split(char[])"/>
        /// returns an <see cref="Array"/> already, so we don't lose anything.
        ///
        /// This method doesn't accept null <paramref name="multilineContent"/>s because passing it null would result in
        /// creating a new <see cref="Array.Empty{T}"/> array, while using null operators should be more efficient.
        /// </remarks>
        /// <param name="multilineContent">the <see cref="string"/> being <see cref="string.Split(char[])"/></param>
        /// <param name="options"><see cref="StringSplitOptions"/></param>
        /// <returns>an <see cref="Array"/> containing each individual line from <paramref name="multilineContent"/></returns>
        [Pure]
        public static string[] SplitLines(this string multilineContent, StringSplitOptions options = default) {
            return multilineContent.Split(LineBreakSplitters, options);
        }

        /// <summary>
        /// Runs <see cref="SplitLines(string,System.StringSplitOptions)"/> against each <see cref="string"/> in <paramref name="multilineContents"/>,
        /// flattening the results.
        /// </summary>
        /// <param name="multilineContents">a collection of <see cref="string"/>s that will each be passed to <see cref="SplitLines(string,System.StringSplitOptions)"/></param>
        /// <param name="options"><see cref="StringSplitOptions"/></param>
        /// <returns>all of the individual <see cref="string"/>s, split line-by-line, and flattened</returns>
        /// <seealso cref="SplitLines(string,System.StringSplitOptions)"/>
        /// <seealso cref="ToStringLines"/>
        [Pure]
        [LinqTunnel]
        public static IEnumerable<string> SplitLines(this IEnumerable<string?> multilineContents, StringSplitOptions options = default) {
            return multilineContents.SelectMany(content => content?.SplitLines(options) ?? Enumerable.Repeat<string>("", 1));
        }

        /// <summary>
        /// Shorthand to <see cref="string.Trim()"/> a collection of <see cref="string"/>s.
        /// </summary>
        /// <param name="strings">a collection of <see cref="string"/>s</param>
        /// <returns>a collection of <see cref="string.Trim()"/>med <see cref="string"/>s</returns>
        [Pure]
        [LinqTunnel]
        public static IEnumerable<string> TrimLines(this IEnumerable<string?> strings) {
            return strings.SplitLines().Select(it => it.Trim());
        }

        [Pure]
        public static string TrimLines(this string multilineString) {
            return multilineString.SplitLines().Select(it => it.Trim()).JoinLines();
        }

        #region LongestLine

        [Pure]
        [NonNegativeValue]
        public static int LongestLine([InstantHandle] this IEnumerable<string?> strings) {
            return strings
                   .SelectMany(it => (it?.SplitLines()).OrEmpty())
                   .Max(it => it.Length);
        }

        [Pure]
        [NonNegativeValue]
        public static int LongestLine(this string str) {
            return str.SplitLines().Max(it => it.Length);
        }

        #endregion

        #region LineCount

        [Pure]
        [NonNegativeValue]
        public static int LineCount(this string str) {
            return str.SplitLines().Length;
        }

        [Pure]
        [NonNegativeValue]
        public static int LineCount([InstantHandle] this IEnumerable<string?> strings) {
            return strings.SplitLines().Count();
        }

        #endregion

        [Pure]
        public static IEnumerable<string> IndentWithLabel(
            [InstantHandle]
            this IEnumerable<string?> lines,
            string? label,
            string? joiner = " "
        ) {
            if (label == null) {
                return lines.Select(it => $"{it}");
            }

            var firstLinePrefix = label.Suffix(joiner);
            var otherLinePrefix = $" ".Repeat(firstLinePrefix.Length);
            return lines.Select(
                (line, i) => i == 0 ? $"{firstLinePrefix}{line}" : $"{otherLinePrefix}{line}"
            );
        }

        #region Truncation & Collapsing

        /// <summary>
        /// Returns the first <see cref="lineCount"/> full lines of <see cref="lines"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineCount"></param>
        /// <param name="includeMessage"></param>
        /// <returns></returns>
        public static IEnumerable<string> TruncateLines(this IEnumerable<string> lines, int lineCount, bool includeMessage = true) {
            var (taken, leftover) = lines.TakeLeftovers(lineCount);

            // ReSharper disable once InvertIf
            if (includeMessage) {
                var leftoverCount = leftover.Count();
                if (leftoverCount > 0) {
                    taken = taken.SkipLast(1)
                                 .Append($"{Ellipsis}[{leftoverCount} lines omitted]");
                }
            }

            return taken;
        }

        public static IEnumerable<string> TruncateLines(this string contentWithLines, int lineCount, bool includeMessage = true) {
            return TruncateLines(contentWithLines.SplitLines(), lineCount, includeMessage);
        }

        public static string?[] CollapseLines(string?[] lines, Func<string?, bool> predicate) {
            var  filteredLines = new List<string?>();
            int? collapseFrom  = null;
            for (int i = 0; i < lines.Length; i++) {
                var matches = predicate.Invoke(lines[i]);

                // NOT currently collapsing
                if (!collapseFrom.HasValue) {
                    // Starting to collapse
                    if (matches) {
                        collapseFrom = i;
                        continue;
                    }

                    filteredLines.Add(lines[i]);
                    continue;
                }

                // Continue to collapse
                if (matches) {
                    continue;
                }

                // Finish collapsing
                int collapseSize = i - collapseFrom.Value;

                filteredLines.Add(collapseSize == 1 ? Ellipsis : $"{Ellipsis}({collapseSize}/{lines.Length} lines omitted)");
                collapseFrom = null;

                filteredLines.Add(lines[i]);
            }

            if (collapseFrom.HasValue) {
                int collapseSize = lines.Length - collapseFrom.Value;
                filteredLines.Add(collapseSize == 1 ? Ellipsis : $"{Ellipsis}({collapseSize}/{lines.Length} lines omitted)");
            }

            return filteredLines.ToArray();
        }

        public static string?[] CollapseLines(string?[] lines, StringFilter filter, params StringFilter[] additionalFilters) {
            return CollapseLines(lines, str => additionalFilters.Prepend(filter).Any(it => it.TestFilter(str)));
        }

        /// <summary>
        /// Converts <paramref name="obj"/> - and its entries, if it is an <see cref="IEnumerable{T}"/> - into their <see cref="object.ToString"/> representations,
        /// and splits the result line-by-line via <see cref="SplitLines(string,System.StringSplitOptions)"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nullPlaceholder"></param>
        /// <returns></returns>
        public static IEnumerable<string?> ToStringLines(this object? obj, string? nullPlaceholder = "") {
            return obj?
                   .ToString()
                   .Lines()
                   .Select(it => it.Value) ?? Enumerable.Empty<string?>();
        }

        #endregion

        #endregion Line Management

        #region "Default" Strings

        /// <summary>
        /// A variation on <see cref="object.ToString"/> that returns the specified <paramref name="nullPlaceholder"/> if the original <paramref name="obj"/> is <c>null</c>.
        /// </summary>
        /// <param name="obj">the original <see cref="object"/></param>
        /// <param name="nullPlaceholder">the <see cref="string"/> returned when <paramref name="obj"/> is <c>null</c></param>
        /// <returns>the <see cref="object.ToString"/> representation of <paramref name="obj"/>, or <c>null</c></returns>
        public static string ToString(this object? obj, string nullPlaceholder) {
            if (nullPlaceholder == null) {
                throw new ArgumentNullException(nameof(nullPlaceholder), $"Providing a null value as a {nameof(nullPlaceholder)} is redundant!");
            }

            return obj?.ToString() ?? nullPlaceholder;
        }

        /// <summary>
        /// Returns <paramref name="emptyPlaceholder"/> if this <see cref="string"/> <see cref="IsEmpty"/>; otherwise, returns this <see cref="string"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <param name="emptyPlaceholder">the fallback string if <paramref name="str"/> <see cref="IsEmpty"/>. Defaults to <c>""</c></param>
        /// <returns>this <see cref="string"/> or <paramref name="emptyPlaceholder"/></returns>
        [return: NotNullIfNotNull("emptyPlaceholder")]
        public static string? IfEmpty(this string? str, string? emptyPlaceholder) {
            return str.IsEmpty() ? emptyPlaceholder : str;
        }

        /// <param name="obj">this <see cref="object"/></param>
        /// <param name="nullPlaceholder">the output if this <see cref="Object"/> is <c>null</c>. Defaults to <see cref="Prettification.DefaultNullPlaceholder"/></param>
        /// <returns><see cref="object.ToString"/> if this <see cref="Object"/> isn't <c>null</c>; otherwise, <paramref name="nullPlaceholder"/></returns>
        public static string OrNullPlaceholder<T>(this T? obj, string? nullPlaceholder) {
            nullPlaceholder ??= Prettification.DefaultNullPlaceholder;
            return obj?.ToString() ?? nullPlaceholder;
        }

        /// <inheritdoc cref="OrNullPlaceholder{T}(T?,string?)"/>
        public static string OrNullPlaceholder<T>(this T? obj) => OrNullPlaceholder(obj, default(PrettificationSettings));

        /// <inheritdoc cref="OrNullPlaceholder{T}(T?,string?)"/>
        public static string OrNullPlaceholder<T>(this T? obj, PrettificationSettings? settings) {
            return OrNullPlaceholder(obj, settings.Resolve().NullPlaceholder);
        }

        /// <summary>
        /// Returns <paramref name="blankPlaceholder"/> if this <see cref="string"/> <see cref="IsBlank"/>; otherwise, returns this <see cref="string"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <param name="blankPlaceholder">the fallback string if <paramref name="str"/> <see cref="IsBlank"/>. Defaults to <c>""</c></param>
        /// <returns>this <see cref="string"/> or <paramref name="blankPlaceholder"/></returns>
        [return: NotNullIfNotNull("blankPlaceholder")]
        public static string? IfBlank(this string? str, string? blankPlaceholder) {
            return str.IsBlank() ? blankPlaceholder : str;
        }

        /// <inheritdoc cref="IfBlank(string?,string?)"/>
        public static string IfBlank(this string? str, Func<string> blankPlaceholder) {
            // 📎 This doesn't delegate to `IfBlank(blankPlaceholder.Invoke())` because that would require eagerly evaluating `blankPlaceholder`.
            //    Using a conditional expression instead means that `blankPlaceholder` will only be invoked if `str` actually `IsBlank()`.
            return str.IsBlank() ? blankPlaceholder.Invoke() : str;
        }

        /// <summary>
        /// Applies a transformation <see cref="Func{TResult}"/> to a <see cref="string"/> if and only if the <see cref="string"/> <see cref="IsNotBlank"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <param name="transformation">a transformation to apply to this <see cref="string"/></param>
        /// <returns>the modified <see cref="string"/></returns>
        [return: NotNullIfNotNull("str")]
        public static string? IfNotBlank(this string? str, Func<string, string> transformation) => str.IsBlank() ? str : transformation(str);

        #endregion

        #region Lapelle deux Vid

        /// <summary>
        /// An extension method for <see cref="string.IsNullOrEmpty"/>
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns><see cref="string.IsNullOrEmpty"/></returns>
        public static bool IsEmpty([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// The inverse of <see cref="IsEmpty(string?)"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns>!<see cref="IsEmpty(string?)"/></returns>
        public static bool IsNotEmpty([NotNullWhen(true)] this string? str) => !string.IsNullOrEmpty(str);

        /// <summary>
        /// An extension method for <see cref="string.IsNullOrWhiteSpace"/> matching Java's <a href="https://commons.apache.org/proper/commons-lang/apidocs/org/apache/commons/lang3/StringUtils.html#isBlank-java.lang.CharSequence-">StringUtils.isBlank()</a>
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns><see cref="string.IsNullOrWhiteSpace"/></returns>
        public static bool IsBlank([NotNullWhen(false)] this string? str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// The inverse of <see cref="IsBlank(string?)"/>
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns><b>NOT</b> <see cref="string.IsNullOrWhiteSpace"/></returns>
        public static bool IsNotBlank([NotNullWhen(true)] this string? str) => !str.IsBlank();

        #endregion

        #region {x}IfMissing

        public static string PrependIfMissing(this string str, string prefix) => PrefixIfMissing(str, prefix);
        public static string PrefixIfMissing(this  string str, string prefix) => str.StartsWith(prefix) ? str : str.Prefix(prefix);
        public static string AppendIfMissing(this  string str, string suffix) => SuffixIfMissing(str, suffix);
        public static string SuffixIfMissing(this  string str, string suffix) => str.EndsWith(suffix) ? str : str.Suffix(suffix);

        #endregion

        #region Substrings

        [Pure]
        public static string SubstringBefore(this string str, string splitter) {
            var first = str.IndexOf(splitter, StringComparison.Ordinal);
            return first > 0 ? str[..first] : "";
        }

        [Pure]
        public static string SubstringAfter(this string str, string splitter) {
            var last = str.LastIndexOf(splitter, StringComparison.Ordinal) + splitter.Length;
            return last.IsBetween(0, str.Length, Clusivity.Exclusive) ? str.Substring(last, str.Length - last) : "";
        }

        [Pure]
        public static string SubstringBefore(this string str, Regex pattern) {
            var match = pattern.Match(str);
            return match.Success ? str.Substring(0, match.Index) : "";
        }

        [Pure]
        public static string SubstringAfter(this string str, Regex pattern) {
            var rightToLeftPattern = new Regex(pattern.ToString(), pattern.Options | RegexOptions.RightToLeft);
            var match              = rightToLeftPattern.Match(str);
            if (match.Success) {
                // the substring starts from the END of the match
                var subStart  = match.Index + match.Length;
                var subEnd    = str.Length;
                var subLength = subEnd - subStart;
                return str.Substring(subStart, subLength);
            }
            else {
                return "";
            }
        }

        /// <summary>
        /// TODO: "Bisect" usually means "cut into two <b>equal</b> parts. I need a better name for <see cref="Bisect"/> and <see cref="BisectLast"/>.
        ///
        /// Splits <paramref name="str"/> by the <b>first</b> occurrence of <paramref name="splitter"/>.
        ///
        /// If <paramref name="splitter"/> isn't found, then <c>(<paramref name="str"/>, <see cref="string.Empty"/>)</c>
        /// </summary>
        /// <param name="str">the original <see cref="string"/></param>
        /// <param name="splitter">the <see cref="string"/> being used to split <paramref name="str"/> (📎 will <b>not</b> be included in the output)</param>
        /// <returns>the split <paramref name="str"/> if <paramref name="splitter"/> was found; otherwise, (<paramref name="str"/>, <see cref="string.Empty">""</see>)</returns>
        public static (string before, string after) Bisect(this string str, string splitter) {
            var matchStart = str.IndexOf(splitter, StringComparison.Ordinal);
            if (matchStart < 0) {
                return (str, "");
            }

            var matchEnd = matchStart + splitter.Length;
            var before   = str[..matchStart];
            var after    = str[matchEnd..];
            return (before, after);
        }

        /// <summary>
        /// Similar to <see cref="Bisect"/>, except this splits by the <b>last</b> occurrence of <paramref name="splitter"/>.
        /// </summary>
        /// <param name="str">the original <see cref="string"/></param>
        /// <param name="splitter">the <see cref="string"/> being used to split <paramref name="str"/> (📎 will <b>not</b> be included in the output)</param>
        /// <returns>the split <paramref name="str"/> if <paramref name="splitter"/> was found; otherwise, (<see cref="string.Empty">""</see>, <paramref name="str"/>)</returns>
        public static (string former, string latter) BisectLast(this string str, string splitter) {
            var matchStart = str.LastIndexOf(splitter, StringComparison.Ordinal);

            if (matchStart < 0) {
                return ("", str);
            }

            var matchEnd = matchStart + splitter.Length;
            var before   = str[..matchStart];
            var after    = str[matchEnd..];
            return (before, after);
        }

        /// <summary>
        /// Removes the <b>single latter-most</b> instance of <paramref name="toRemove"/>.
        /// </summary>
        /// <param name="str">the original <see cref="string"/></param>
        /// <param name="toRemove">the <see cref="string"/> to be removed</param>
        /// <returns>the original <see cref="string"/> with the <b>single latter-most</b> instance of <paramref name="toRemove"/> removed</returns>
        [Pure]
        public static string RemoveLast(this string str, string toRemove) {
            return str.EndsWith(toRemove) ? str.Remove(str.Length - toRemove.Length) : str;
        }

        /// <summary>
        /// Removes the <b>single first</b> instance of <paramref name="toRemove"/>.
        /// </summary>
        /// <param name="str">the original <see cref="string"/></param>
        /// <param name="toRemove">the <see cref="string"/>to be removed</param>
        /// <returns>the original <see cref="string"/> with the <b>single first</b> instance of <paramref name="toRemove"/> removed</returns>
        [Pure]
        public static string RemoveFirst(this string str, string toRemove) {
            return str.StartsWith(toRemove) ? str.Remove(0, toRemove.Length) : str;
        }

        #endregion
    }
}