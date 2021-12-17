using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Strings {
    [PublicAPI]
    public static class StringUtils {
        public const int DefaultIndentSize = 2;

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
        internal static readonly string[] LineBreakSplitters = { "\r\n", "\r", "\n" };

        /// <summary>
        /// Prepends <paramref name="toIndent"/>.
        /// </summary>
        /// <param name="toIndent">The <see cref="string" /> to be indented.</param>
        /// <param name="indentCount">The number of indentations (i.e. number of times hitting "tab").</param>
        /// <param name="indentSize">The <see cref="string.Length"/> of each individual indentation. Defaults to <see cref="DefaultIndentSize"/>.</param>
        /// <param name="indentChar">The <see cref="char"/> that is <see cref="Repeat(char,int,string)"/>ed to build a single indentation. Defaults to <see cref="DefaultIndentChar"/>.</param>
        /// <returns>The indented <see cref="string"/>.</returns>
        /// <seealso cref="Indent(IEnumerable{string},int,int,char)"/>
        [ContractAnnotation("toIndent:null => null")]
        [ContractAnnotation("toIndent:notnull => notnull")]
        public static string? Indent(
            this string? toIndent,
            [NonNegativeValue]
            int indentCount = 1,
            [NonNegativeValue]
            int indentSize = DefaultIndentSize,
            char indentChar = ' '
        ) {
            if (indentCount.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentCount));
            }

            if (indentSize.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentSize));
            }

            return indentChar
                   .Repeat(indentSize)
                   .Repeat(indentCount)
                   .Suffix(toIndent);
        }

        /// <summary>
        /// Joins <paramref name="toRepeat" /> with itself <paramref name="repetitions" /> times, using the optional <paramref name="separator" />
        /// </summary>
        /// <param name="toRepeat">The <see cref="string" /> to be joined with itself.</param>
        /// <param name="repetitions">The number of times <paramref name="toRepeat" /> should be repeated.</param>
        /// <param name="separator">An optional character, analogous to </param>
        /// <returns></returns>
        public static string Repeat(this string toRepeat, [NonNegativeValue] int repetitions, string? separator = "") {
            if (repetitions.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(repetitions));
            }

            var list = new List<string>();
            for (var i = 0; i < repetitions; i++) {
                list.Add(toRepeat);
            }

            return string.Join(separator, list);
        }

        /// <inheritdoc cref="Repeat(string,int,string)" />
        public static string Repeat(this char toRepeat, [NonNegativeValue] int repetitions, string separator = "") {
            if (repetitions.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(repetitions));
            }

            return Repeat(toRepeat.ToString(), repetitions, separator);
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
        /// Joins <paramref name="first"/> and <paramref name="second"/> together by a <b>single instance</b> of <paramref name="separator"/>.
        /// </summary>
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
        /// <returns></returns>
        public static string JoinWith(this string? first, string? second, string? separator) {
            separator ??= "";
            first     =   first?.TrimEnd(separator)    ?? "";
            second    =   second?.TrimStart(separator) ?? "";
            return string.Join(separator, first, second);
        }

        #region Prettification

        [Obsolete]
        public static string Prettify(object thing, bool recursive = true, int recursionCount = 0) {
            const int recursionMax = 10;
            var       type         = thing.GetType().ToString();
            string    method       = null;
            string    prettyString = null;

            switch (thing) {
                //don't do anything special with strings
                //check for value types (int, char, etc.), which we shouldn't do anything fancy with
                //TODO 7/4/2021: The line written above is a BOLD FACED LIE
                //  Actually, this whole method is just hot garbage and needs to be REBORN
                case string s:
                    method       = "string";
                    prettyString = s;
                    break;
                case ValueType _:
                    method       = nameof(ValueType);
                    prettyString = thing.ToString();
                    break;
                case IEnumerable enumerableThing:
                    method = $"recursion, {recursionCount}";

                    recursionCount++;

                    if (!recursive || recursionCount >= recursionMax) {
                        goto default;
                    }

                    foreach (var entry in enumerableThing) {
                        prettyString += "\n" + Prettify(entry, true, recursionCount);
                    }

                    break;
                default:
                    try {
                        method       = "JSON";
                        prettyString = JsonConvert.SerializeObject(thing, Formatting.Indented);
                    }
                    catch (Exception) {
                        method = "JSON - FAILED!";
                    }

                    break;
            }

            // account for null prettyString and method
            // (we're doing this here, rather than initializing them to default values, so we can trigger things if there's a failure)
            // NOTE from Brandon on 8/22/2021: this fancy-schmancy ??= operator is...funky
            prettyString ??= thing.ToString();
            method       ??= "NO METHOD FOUND";


            return $"[{method}]{prettyString}".Indent(indentCount: recursionCount);
        }

        //TODO: Add an extension method version of "Prettify" and re-do the fuck out of "Prettify"
        //TODO: Move prettification methods into a dedicated class
        //TODO: the fuck is the difference between "Prettify" and "Pretty"?! "Prettify" is definitely a better name!
        public static string Pretty(this object obj) {
            throw new NotImplementedException("DEAR GOD I immediately started making a 'PrettyOptions' enum to go along with this ARGH");
        }

        public static string ListVariables(object obj) {
            return ListMembers(obj, MemberTypes.Property | MemberTypes.Field);
        }

        public static string ListProperties(object obj) {
            return ListMembers(obj, MemberTypes.Property);
        }

        public static string ListFields(object obj) {
            return ListMembers(obj, MemberTypes.Field);
        }

        public static string ListMembers(object obj, MemberTypes memberTypes = MemberTypes.All) {
            //if obj is a already a type, cast it and use it; otherwise, grab its type
            Type objType = obj is Type type ? type : obj.GetType();
            return objType.GetMembers().Where(member => memberTypes.HasFlag(member.MemberType)).Aggregate($"[{objType}] {memberTypes}:", (current, member) => current + $"\n\t{FormatMember(member, obj)}");
        }

        /// <summary>
        /// TODO: rename this to "PrettifyMember"
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FormatMember(MemberInfo memberInfo, object obj = null) {
            var result = $"[{memberInfo.MemberType}] {memberInfo}";

            try {
                if (obj != null) {
                    switch (memberInfo) {
                        case PropertyInfo propertyInfo:
                            result += $": {propertyInfo.GetValue(obj)}";
                            break;
                        case FieldInfo fieldInfo:
                            result += $": {fieldInfo.GetValue(obj)}";
                            break;
                    }
                }
            }
            catch (Exception e) {
                result += $"ERROR: {e.Message}";
            }

            return result;
        }

        #endregion

        #region Padding, filling, truncating, trimming, and trailing

        /// <summary>
        /// Reduces <paramref name="self"/> to <paramref name="maxLength"/> characters, replacing the last bits with a <paramref name="trail"/> if specified.
        ///
        /// If the original <see cref="string.Length"/> is less than <paramref name="maxLength"/>, returns <paramref name="self"/>.
        /// </summary>
        /// <param name="self">the <see cref="string"/> being truncated</param>
        /// <param name="maxLength">the <b>maximum</b> size of the final string</param>
        /// <param name="trail">a <see cref="string"/> to replace the end bits of <paramref name="self"/> to show that it has been truncated. Defaults to an <see cref="Ellipsis"/></param>
        /// <returns>a <see cref="string"/> no longer than <paramref name="maxLength"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="maxLength"/> is negative</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="trail"/> is longer than <paramref name="maxLength"/></exception>
        public static string Truncate(
            this string? self,
            [NonNegativeValue]
            int maxLength,
            string? trail = Ellipsis
        ) {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (maxLength < 0) {
                throw new ArgumentOutOfRangeException(nameof(maxLength), "Must be positive");
            }

            if (trail?.Length >= maxLength) {
                throw new ArgumentOutOfRangeException(nameof(trail), $"{nameof(trail)}.{nameof(trail.Length)} [{trail?.Length}] must be less than {nameof(maxLength)} [{maxLength}]");
            }

            if (self == null || maxLength == 0) {
                return "";
            }

            if (self.Length > maxLength) {
                var shortened = self.Substring(0, maxLength - (trail?.Length ?? 0));
                return $"{shortened}{trail}";
            }

            return self;
        }

        /// <summary>
        /// Uses either <see cref="Truncate"/> or <see cref="FillRight"/> to get <paramref name="self"/> to be <paramref name="desiredLength"/> long.
        /// </summary>
        /// <param name="self">the original <see cref="string"/></param>
        /// <param name="desiredLength">the <see cref="string.Length"/> that <paramref name="self"/> will have</param>
        /// <param name="filler">the <see cref="string"/> used to <see cref="FillRight"/> if <paramref name="self"/> is shorter than <paramref name="desiredLength"/></param>
        /// <param name="trail">the <see cref="string"/> used to indicated that <paramref name="self"/> has been <see cref="Truncate"/>d</param>
        /// <returns>a <see cref="string"/> with a <see cref="string.Length"/> of <paramref name="desiredLength"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ForceToLength(this string? self, [ValueRange(0, long.MaxValue)] int desiredLength, string? filler = " ", string? trail = Ellipsis) {
            self ??= "";

            return self.Length.CompareTo(desiredLength).Sign() switch {
                -1 => self.FillRight(desiredLength, filler),
                0  => self,
                1  => self.Truncate(desiredLength, trail),
                _  => throw new ArgumentException($"This should be unreachable, because we used a {nameof(int.CompareTo)} function AND got the {nameof(Mathb.Sign)} of it, so we definitely should've only had the possibilities of -1, 0, or 1.")
            };
        }


        [ContractAnnotation("filler:null => stop")]
        public static string FillRight(this string? self, [NonNegativeValue] int totalLength, string filler) {
            ValidateFillParameters(filler, totalLength);

            self ??= "";

            if (self.Length >= totalLength) {
                return self;
            }

            var additionalLengthNeeded = totalLength - self.Length;
            return self + filler.Fill(additionalLengthNeeded);
        }


        [ContractAnnotation("filler:null => stop")]
        public static string FillLeft(this string? self, [NonNegativeValue] int totalLength, string filler) {
            ValidateFillParameters(filler, totalLength);

            self ??= "";

            if (self.Length >= totalLength) {
                return self;
            }

            var additionalLengthNeeded = totalLength - self.Length;
            return self + filler.Fill(additionalLengthNeeded).Reverse().JoinString();
        }


        [ContractAnnotation("filler:null => stop")]
        public static string Fill(this string filler, [NonNegativeValue] int totalLength) {
            ValidateFillParameters(filler, totalLength);

            var fullLength  = totalLength / filler.Length;
            var extraLength = totalLength % filler.Length;
            var filled      = filler.Repeat(fullLength) + filler.Substring(0, extraLength);
            return filled;
        }

        [ContractAnnotation("filler:null => stop")]
        private static void ValidateFillParameters(string filler, [NonNegativeValue] int totalLength) {
            if (filler == null) {
                throw new ArgumentNullException(nameof(filler));
            }

            if (string.IsNullOrEmpty(filler)) {
                throw new ArgumentException($"Cannot fill with an empty string!", nameof(filler));
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (totalLength < 0) {
                throw new ArgumentOutOfRangeException(nameof(totalLength), "Must be positive");
            }
        }

        public static string FormatHeading(string heading, string border = "=", string padding = " ") {
            var middle = $"{border}{padding}{heading}{padding}{border}";
            var hRule  = border.FillRight(middle.Length, border);
            return $"{hRule}\n{middle}\n{hRule}";
        }

        [ContractAnnotation("input:null => null")]
        [Pure]
        public static string? Trim(this string? input, string trimString) {
            if (input == null) {
                return null;
            }

            var pattern = new Regex(Regex.Escape(trimString));
            return Trim(input, pattern);
        }

        [Pure]
        public static string Trim(this string input, Regex trimPattern) {
            var reg   = new Regex($"^({trimPattern})*(?<trimmed>.*?)({trimPattern})*$");
            var match = reg.Match(input);

            return match.Success ? match.Groups["trimmed"].Value : input;
        }

        private static string GetTrimQuantifier(int? numberToTrim) {
            return numberToTrim.IfPresentOrElse(it => $"{{0,{it}}}", () => "*");
        }

        /// <summary>
        /// Removes up to <paramref name="numberToTrim"/> instances of <paramref name="trimString"/> from the <b>end</b> of <paramref name="input"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="numberToTrim"/> is null, then <b>all</b> instances are removed.
        /// </remarks>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="trimString">the <see cref="string"/> to be removed </param>
        /// <param name="numberToTrim">the maximum number of <paramref name="trimString"/>s to remove</param>
        /// <returns><paramref name="input"/> with some number of <paramref name="trimString"/>s removed from the <b>end</b></returns>
        [Pure]
        public static string TrimEnd(this string input, string trimString, int? numberToTrim = default) {
            var trimPattern = new Regex(Regex.Escape(trimString));
            return TrimEnd(input, trimPattern, numberToTrim);
        }

        /// <summary>
        /// Removes up to <paramref name="numberToTrim"/> instances of <paramref name="trimPattern"/> from the <b>end</b> of <paramref name="input"/>.
        /// </summary>
        /// <remarks><inheritdoc cref="TrimEnd(string,string,System.Nullable{int})"/></remarks>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="trimPattern">the <see cref="Regex"/> pattern to be removed</param>
        /// <param name="numberToTrim">the maximum number of <paramref name="trimPattern"/>s to remove</param>
        /// <returns><paramref name="input"/> with some number of <paramref name="trimPattern"/>s removed from the <b>end</b></returns>
        [Pure]
        public static string TrimEnd(this string input, Regex trimPattern, int? numberToTrim = default) {
            var trimQuantifier = GetTrimQuantifier(numberToTrim);
            var reg            = new Regex($@"^(?<trimmed>.*?)({trimPattern}){trimQuantifier}$");
            var match          = reg.Match(input);

            return match.Success ? match.Groups["trimmed"].Value : input;
        }

        /// <summary>
        /// Removes up to <paramref name="numberToTrim"/> instances of <paramref name="trimString"/> from the <b>beginning</b> of <paramref name="input"/>.
        /// </summary>
        /// <remarks><inheritdoc cref="TrimEnd(string,string,System.Nullable{int})"/></remarks>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="trimString">the <see cref="Regex"/> pattern to be removed</param>
        /// <param name="numberToTrim">the maximum number of <paramref name="trimString"/>s to remove</param>
        /// <returns><paramref name="input"/> with some number of <paramref name="trimString"/>s removed from the <b>beginning</b></returns>
        [Pure]
        public static string TrimStart(this string input, string trimString, int? numberToTrim = default) {
            var pattern = new Regex(Regex.Escape(trimString));
            return TrimStart(input, pattern, numberToTrim);
        }

        /// <summary>
        /// Removes up to <paramref name="numberToTrim"/> instances of <paramref name="trimPattern"/> from the <b>beginning</b> of <paramref name="input"/>.
        /// </summary>
        /// <remarks><inheritdoc cref="TrimEnd(string,string,System.Nullable{int})"/></remarks>
        /// <param name="input">the original <see cref="string"/></param>
        /// <param name="trimPattern">the <see cref="Regex"/> pattern to be removed</param>
        /// <param name="numberToTrim">the maximum number of <paramref name="trimPattern"/>s to remove</param>
        /// <returns><paramref name="input"/> with some number of <paramref name="trimPattern"/>s removed from the <b>beginning</b></returns>
        [Pure]
        public static string TrimStart(this string input, Regex trimPattern, int? numberToTrim = default) {
            var trimQuantifier = GetTrimQuantifier(numberToTrim);
            var reg            = new Regex(@$"^({trimPattern}){trimQuantifier}(?<trimmed>.*?)$");
            var match          = reg.Match(input);

            return match.Success ? match.Groups["trimmed"].Value : input;
        }

        #endregion

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
        public static string[] Splitex(this string input, string pattern, RegexOptions options, TimeSpan matchTimeout) {
            return Regex.Split(input, pattern, options, matchTimeout);
        }

        /// <summary>
        /// Returns true if the given <see cref="string"/> <see cref="string.Contains"/> <b>any</b> of the given <see cref="substrings"/>.
        /// </summary>
        /// <remarks>
        /// TODO: I wonder if it would be faster to search for the shortest substring first...? And maybe excluding substrings that wholly contain other substrings?
        /// </remarks>
        /// <param name="str">the <see cref="string"/> to check for <see cref="substrings"/></param>
        /// <param name="substrings">the possible <see cref="substrings"/></param>
        /// <returns>true if the given <see cref="string"/> <see cref="string.Contains"/> <b>any</b> of the given <see cref="substrings"/></returns>
        public static bool ContainsAny(this string str, IEnumerable<string> substrings) {
            return substrings.Any(str.Contains);
        }

        /**
         * <inheritdoc cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static bool ContainsAny(this string str, params string[] substrings) {
            return ContainsAny(str, substrings.AsEnumerable());
        }

        /// <summary>
        /// Returns true if the given <see cref="string"/> <see cref="string.Contains"/> <b>all</b> of the given <see cref="substrings"/>.
        /// </summary>
        /// <param name="str">the <see cref="string"/> to search through</param>
        /// <param name="substrings">the possible substrings</param>
        /// <returns>true if the given <see cref="string"/> <see cref="string.Contains"/> <b>all</b> of the given <see cref="substrings"/></returns>
        public static bool ContainsAll(this string str, IEnumerable<string> substrings) {
            return substrings.All(str.Contains);
        }

        /**
         * <inheritdoc cref="ContainsAll(string,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static bool ContainsAll(this string str, params string[] substrings) {
            return ContainsAll(str, substrings.AsEnumerable());
        }

        #region Line Management

        /// <summary>
        /// Splits <paramref name="multilineContent"/> via <c>"\r\n", "\r", or "\n"</c>.
        /// </summary>
        /// <example>
        /// This will match any
        /// </example>
        /// <param name="multilineContent">the <see cref="string"/> being <see cref="string.Split(char[])"/></param>
        /// <param name="options"><see cref="StringSplitOptions"/></param>
        /// <returns>an <see cref="Array"/> containing each individual line from <paramref name="multilineContent"/></returns>
        [Pure]
        public static string[] SplitLines(this string? multilineContent, StringSplitOptions options = default) {
            return multilineContent?.Split(LineBreakSplitters, options) ?? Array.Empty<string>();
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
        public static string[] SplitLines([InstantHandle] this IEnumerable<string?>? multilineContents, StringSplitOptions options = default) {
            return multilineContents?.SelectMany(content => content.SplitLines(options)).ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Shorthand to <see cref="string.Trim()"/> a collection of <see cref="string"/>s.
        /// </summary>
        /// <param name="strings">a collection of <see cref="string"/>s</param>
        /// <returns>a collection of <see cref="string.Trim()"/>med <see cref="string"/>s</returns>
        [Pure]
        [LinqTunnel]
        public static IEnumerable<string?> TrimLines(this IEnumerable<string?> strings) {
            return strings.Select(it => it?.Trim());
        }

        #region LongestLine

        [Pure]
        [NonNegativeValue]
        public static int LongestLine([InstantHandle] this IEnumerable<string?>? strings) {
            return strings?.SelectMany(it => it.SplitLines()).Max(it => it.Length) ?? 0;
        }

        [Pure]
        [NonNegativeValue]
        public static int LongestLine(this string? str) {
            return str.SplitLines().Max(it => it.Length);
        }

        #endregion

        #region LineCount

        [Pure]
        [NonNegativeValue]
        public static int LineCount(this string? str) {
            return str.SplitLines().Length;
        }

        [Pure]
        [NonNegativeValue]
        public static int LineCount([InstantHandle] this IEnumerable<string?>? strings) {
            return strings.SplitLines().Length;
        }

        #endregion

        /// <summary>
        /// <see cref="Indent(string,int,int,char)"/>s each <see cref="string"/> in <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">a collection of <see cref="string"/>s which are treated as separate lines</param>
        /// <param name="indentCount">how many "indents" to add, i.e. how many times the "tab" key should be hit</param>
        /// <param name="indentSize">the <see cref="string.Length"/> of a single indent. Defaults to <see cref="DefaultIndentSize"/></param>
        /// <param name="indentChar">the <see cref="char"/> that is <see cref="Repeat(char,int,string)"/>ed to form a single indent. Defaults to <see cref="DefaultIndentChar"/></param>
        /// <returns>the indented <see cref="string"/>s</returns>
        /// <seealso cref="Indent(string,int,int,char)"/>
        [ContractAnnotation("lines:null => null")]
        [ContractAnnotation("lines:notnull => notnull")]
        [Pure]
        [LinqTunnel]
        public static IEnumerable<string>? Indent(
            this IEnumerable<string?>? lines,
            [NonNegativeValue]
            int indentCount = 1,
            [NonNegativeValue]
            int indentSize = DefaultIndentSize,
            char indentChar = DefaultIndentChar
        ) {
            if (indentCount.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentCount));
            }

            if (indentSize.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentSize));
            }

            return lines?.Select(it => it.Indent(indentCount, indentSize, indentChar)).ToList();
        }

        [ContractAnnotation("lines:null => null")]
        public static IEnumerable<string>? IndentWithLabel(this IEnumerable<string>? lines, string? label, string? joiner = " ") {
            if (lines == null) {
                return null;
            }

            var firstLinePrefix = $"{label}{joiner}";
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
        public static string[] TruncateLines(this IEnumerable<string> lines, int lineCount, bool includeMessage = true) {
            var lns = lines.ToArray();
            if (lns.Length <= lineCount) {
                return lns.ToArray();
            }

            if (includeMessage) {
                return lns.Take(lineCount - 1)
                          .Append($"{Ellipsis}[{lns.Length - lineCount} lines omitted]")
                          .ToArray();
            }
            else {
                return lns.Take(lineCount)
                          .ToArray();
            }
        }

        public static string[] TruncateLines(this string contentWithLines, int lineCount, bool includeMessage = true) {
            return TruncateLines(contentWithLines.SplitLines(), lineCount, includeMessage);
        }

        public static string[] CollapseLines(string[] lines, Func<string, bool> predicate) {
            var  filteredLines = new List<string>();
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

        public static string[] CollapseLines(string[] lines, StringFilter filter, params StringFilter[] additionalFilters) {
            return CollapseLines(lines, str => additionalFilters.Prepend(filter).Any(it => it.TestFilter(str)));
        }

        /// <summary>
        /// Converts <paramref name="obj"/> - and its entries, if it is an <see cref="IEnumerable{T}"/> - into their <see cref="object.ToString"/> representations,
        /// and splits the result line-by-line via <see cref="SplitLines(string,System.StringSplitOptions)"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nullPlaceholder"></param>
        /// <returns></returns>
        public static string[] ToStringLines(this object? obj, string? nullPlaceholder = "") {
            if (obj is IEnumerable<object> e) {
                return e.SelectMany(it => it.ToStringLines(nullPlaceholder)).SplitLines();
            }

            return obj?.ToString().SplitLines() ?? new[] {
                nullPlaceholder
            };
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
        [ContractAnnotation("nullPlaceholder:null => stop")]
        public static string ToString(this object? obj, string nullPlaceholder) {
            if (nullPlaceholder == null) {
                throw new ArgumentNullException(nameof(nullPlaceholder), $"Providing a null value as a {nameof(nullPlaceholder)} is redundant!");
            }

            return obj == null ? nullPlaceholder : obj.ToString();
        }

        /// <summary>
        /// Applies <paramref name="formatter"/> to <paramref name="obj"/>, returning the result or, if the result is null, <paramref name="nullPlaceholder"/>.
        /// </summary>
        /// <param name="obj">the original <see cref="object"/></param>
        /// <param name="formatter">the <see cref="Func{T,T2}"/> that will produce the formatted <paramref name="obj"/></param>
        /// <param name="nullPlaceholder">the <see cref="string"/> returned when the result of <paramref name="formatter"/> is null</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [ContractAnnotation("formatter:null => stop")]
        [ContractAnnotation("nullPlaceholder:null => stop")]
        public static string ToString<T>(
            this T?         obj,
            Func<T, string> formatter,
            string?         nullPlaceholder = Prettification.DefaultNullPlaceholder
        ) {
            if (nullPlaceholder == null) {
                throw new ArgumentNullException(nameof(nullPlaceholder), $"Providing a null value as a {nameof(nullPlaceholder)} is redundant!");
            }

            if (formatter == null) {
                throw new ArgumentNullException(nameof(formatter));
            }

            return formatter.MustNotBeNull().Try(obj).OrElse(default) ?? nullPlaceholder;
        }

        /// <summary>
        /// Returns <paramref name="emptyPlaceholder"/> if this <see cref="string"/> <see cref="IsNullOrEmpty"/>; otherwise, returns this <see cref="string"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <param name="emptyPlaceholder">the fallback string if <paramref name="str"/> <see cref="IsNullOrEmpty"/>. Defaults to <c>""</c></param>
        /// <returns>this <see cref="string"/> or <paramref name="emptyPlaceholder"/></returns>
        [ContractAnnotation("emptyPlaceholder:notnull => notnull")]
        [ContractAnnotation("emptyPlaceholder:null => canbenull")]
        public static string? IfEmpty(this string? str, string emptyPlaceholder) {
            if (emptyPlaceholder == null) {
                throw new ArgumentNullException(nameof(emptyPlaceholder));
            }

            return str.IsNullOrEmpty() ? emptyPlaceholder : str;
        }


        public static string OrNullPlaceholder(this object? obj, string? nullPlaceholder = Prettification.DefaultNullPlaceholder) {
            nullPlaceholder ??= Prettification.DefaultNullPlaceholder;
            return obj?.ToString() ?? nullPlaceholder;
        }


        public static string OrNullPlaceholder(this object? obj, PrettificationSettings? settings) {
            settings ??= PrettificationSettings.Default;
            return OrNullPlaceholder(obj, settings.NullPlaceholder);
        }

        /// <summary>
        /// Returns <paramref name="blankPlaceholder"/> if this <see cref="string"/> <see cref="IsBlank"/>; otherwise, returns this <see cref="string"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <param name="blankPlaceholder">the fallback string if <paramref name="str"/> <see cref="IsBlank"/>. Defaults to <c>""</c></param>
        /// <returns>this <see cref="string"/> or <paramref name="blankPlaceholder"/></returns>
        [ContractAnnotation("blankPlaceholder:notnull => notnull")]
        [ContractAnnotation("blankPlaceholder:null => canbenull")]
        public static string? IfBlank(this string? str, string? blankPlaceholder) {
            blankPlaceholder ??= "";
            return str.IsBlank() ? blankPlaceholder : str;
        }


        [ContractAnnotation("null => stop")]
        public static string MustNotBeBlank(this string? str) {
            return str.IsNotBlank() ? str : throw new ArgumentException($"The string must not be blank! (Actual: [{str ?? Prettification.DefaultNullPlaceholder}]");
        }

        #endregion

        #region Lapelle deux Vid

        /// <summary>
        /// An extension method for <see cref="string.IsNullOrEmpty"/>
        /// </summary>
        /// <param name="str"></param>
        /// <returns><see cref="string.IsNullOrEmpty"/></returns>
        [ContractAnnotation("null => true", true)]
        public static bool IsNullOrEmpty(this string? str) {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// An extension method for <see cref="string.IsNullOrWhiteSpace"/>
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns><see cref="string.IsNullOrWhiteSpace"/></returns>
        [ContractAnnotation("null => true", true)]
        public static bool IsNullOrWhiteSpace(this string? str) {
            return string.IsNullOrWhiteSpace(str);
        }

        /**
         * <inheritdoc cref="IsNullOrEmpty"/>
         */
        [ContractAnnotation("null => true")]
        public static bool IsEmpty(this string? str) {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// The inverse of <see cref="IsEmpty"/>.
        /// </summary>
        /// <param name="str">this <see cref="string"/></param>
        /// <returns>!<see cref="IsEmpty"/></returns>
        [ContractAnnotation("null => false")]
        public static bool IsNotEmpty(this string? str) {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// An alias for <see cref="IsNullOrWhiteSpace"/> matching Java's <a href="https://commons.apache.org/proper/commons-lang/apidocs/org/apache/commons/lang3/StringUtils.html#isBlank-java.lang.CharSequence-">StringUtils.isBlank()</a>
        /// </summary>
        /// <param name="str">a <see cref="string"/></param>
        /// <returns><see cref="IsNullOrWhiteSpace"/></returns>
        [ContractAnnotation("null => true", true)]
        public static bool IsBlank(this string? str) {
            return str.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// An alias for <see cref="IsNullOrWhiteSpace"/> matching Java's <a href="https://commons.apache.org/proper/commons-lang/apidocs/org/apache/commons/lang3/StringUtils.html#isBlank-java.lang.CharSequence-">StringUtils.isBlank()</a>
        /// </summary>
        /// <param name="str">a <see cref="string"/></param>
        /// <returns><see cref="IsNullOrWhiteSpace"/></returns>
        [ContractAnnotation("null => false", true)]
        public static bool IsNotBlank(this string? str) {
            return !str.IsBlank();
        }

        #endregion

        #region {x}IfMissing

        public static string PrependIfMissing(this string? str, string? prefix) {
            return PrefixIfMissing(str, prefix);
        }


        public static string PrefixIfMissing(this string? str, string? prefix) {
            str    ??= "";
            prefix ??= "";
            return str.StartsWith(prefix) == true ? str : str.Prefix(prefix);
        }


        public static string AppendIfMissing(this string? str, string? suffix) {
            return SuffixIfMissing(str, suffix);
        }


        public static string SuffixIfMissing(this string? str, string? suffix) {
            str    ??= "";
            suffix ??= "";
            return str.EndsWith(suffix) ? str : str.Suffix(suffix);
        }

        #endregion

        #region Substrings

        [Pure]
        public static string SubstringBefore(this string? str, string? splitter) {
            if (splitter.IsNullOrEmpty()) {
                return "";
            }

            var first = str?.IndexOf(splitter, StringComparison.Ordinal);
            return first > 0 ? str.Substring(0, first.Value) : "";
        }

        [Pure]
        public static string SubstringAfter(this string? str, string? splitter) {
            if (str.IsNullOrEmpty() || splitter.IsNullOrEmpty()) {
                return "";
            }

            var last = str.LastIndexOf(splitter, StringComparison.Ordinal) + splitter.Length;
            return last.IsBetween(0, str.Length, Clusivity.Exclusive) ? str.Substring(last, str.Length - last) : "";
        }

        [Pure]
        public static string SubstringBefore(this string? str, Regex pattern) {
            if (str.IsNullOrEmpty()) {
                return "";
            }

            var match = pattern.Match(str);
            return match.Success ? str.Substring(0, match.Index) : "";
        }

        [Pure]
        public static string SubstringAfter(this string? str, Regex pattern) {
            if (str.IsNullOrEmpty()) {
                return "";
            }

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
        /// <c>null</c> is returned if <b>any</b> of the following is true:
        /// <ul>
        /// <li><b>either</b> <paramref name="str"/> or <paramref name="splitter"/> <see cref="IsNullOrEmpty"/></li>
        /// <li><paramref name="str"/> didn't contain <paramref name="splitter"/></li>
        /// </ul>
        ///
        /// </summary>
        /// <param name="str">the original <see cref="string"/></param>
        /// <param name="splitter">the <see cref="string"/> being used to split <paramref name="str"/>, which will <b>not</b> be included in the output</param>
        /// <returns></returns>
        [ContractAnnotation("str:null => null")]
        [ContractAnnotation("splitter:null => null")]
        public static (string, string)? Bisect(this string? str, string? splitter) {
            if (str.IsNullOrEmpty() || splitter.IsNullOrEmpty()) {
                return null;
            }

            var matchStart = str.IndexOf(splitter, StringComparison.Ordinal);
            if (matchStart < 0) {
                return null;
            }

            var matchEnd = matchStart + splitter.Length;
            var before   = str.Substring(0, matchStart);
            var after    = str.Substring(matchEnd);
            return (before, after);
        }

        /// <summary>
        /// Similar to <see cref="Bisect"/>, except this splits by the <b>last</b> occurrence of <paramref name="splitter"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        [ContractAnnotation("str:null => null")]
        [ContractAnnotation("splitter:null => null")]
        public static (string, string)? BisectLast(this string? str, string? splitter) {
            if (str.IsNullOrEmpty() || splitter.IsNullOrEmpty()) {
                return null;
            }

            var matchStart = str.LastIndexOf(splitter, StringComparison.Ordinal);
            if (matchStart < 0) {
                return null;
            }

            var matchEnd = matchStart + splitter.Length;
            var before   = str.Substring(0, matchStart);
            var after    = str.Substring(matchEnd);
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