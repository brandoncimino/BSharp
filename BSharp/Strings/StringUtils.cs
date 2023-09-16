using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings.Settings;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

using NotNull = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace FowlFever.BSharp.Strings;

public static partial class StringUtils {
    public const int    DefaultIndentSize   = 2;
    public const string DefaultIndentString = "  ";
    /// <summary>
    /// 📝 <see cref="StringSplitOptions"/>.<a href="https://docs.microsoft.com/en-us/dotnet/api/system.stringsplitoptions?view=net-6.0#system-stringsplitoptions-trimentries">TrimEntries</a> doesn't exist until .NET 5,
    /// but we can pretend it does by hard-casting its <see cref="int"/> value, <c>2</c>, directly to <see cref="StringSplitOptions"/>.
    /// </summary>
    internal const StringSplitOptions TrimEntriesOption = (StringSplitOptions)2;

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
        Muster.Have(indentString).NotEmpty();
        return Enumerable.Repeat(toIndent, 1).Indent(indentCount, indentString, indentMode);
    }

    public static IEnumerable<string> Indent(
        this IEnumerable<string?> toIndent,
        int                       indentCount  = 1,
        string                    indentString = DefaultIndentString,
        IndentMode                indentMode   = IndentMode.Relative
    ) {
        Muster.Have(indentString).NotEmpty();
        return indentMode switch {
            IndentMode.Absolute => IndentAbsolute(toIndent, indentCount, indentString),
            IndentMode.Relative => IndentRelative(toIndent, indentCount, indentString),
            _                   => throw BEnum.UnhandledSwitch(indentMode, nameof(indentMode), nameof(Indent)),
        };
    }

    public static IEnumerable<string> IndentRelative(IEnumerable<string?> toIndent, int indentCount = 1, string indentString = DefaultIndentString) {
        return indentCount switch {
            0   => toIndent.SplitLines(),
            > 0 => toIndent.SplitLines().Select(it => it.Prefix(indentString.Repeat(indentCount)).Render()),
            < 0 => toIndent.SplitLines().Select(it => TrimStart(it, indentString, indentCount * -1)),
        };
    }

    [Experimental("Not sure what this method was supposed to do")]
    public static IEnumerable<string> IndentAbsolute(
        IEnumerable<string?>   toIndent,
        [NonNegativeValue] int indentCount  = 1,
        string                 indentString = DefaultIndentString
    ) {
        Must.Be(indentCount, static it => it.IsStrictlyPositive());
        Must.BePositive(indentCount);
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
        Must.BePositive(repetitions);
        return string.Join(separator, Enumerable.Repeat(toRepeat, repetitions));
    }

    /// <inheritdoc cref="Repeat(string,int,string)" />
    public static string Repeat(this char toRepeat, [NonNegativeValue] int repetitions, ReadOnlySpan<char> separator = default) {
        if (repetitions <= 1 || separator.IsEmpty) {
            return new string(toRepeat, repetitions);
        }

        Span<char> chars = stackalloc char[repetitions + (repetitions - 1) * separator.Length];
        var        pos   = 0;

        for (int i = 0; i < repetitions; i++) {
            chars.WriteJoin(toRepeat, separator, ref pos);
        }

        return chars.ToString();
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

    /// <inheritdoc cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/>
    public static bool ContainsAny(this string str, params string[] substrings) => ContainsAny(str, substrings.AsEnumerable());

    #endregion

    #endregion

    #region DoesNotContainAny

    /// <param name="str">this <see cref="string"/></param>
    /// <param name="substrings">the <see cref="string"/>s that shouldn't be inside of <paramref name="str"/></param>
    /// <returns>the inverse of <see cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/></returns>
    public static bool ContainsNone(this string str, IEnumerable<string> substrings) => !str.ContainsAny(substrings);

    /// <param name="str">this <see cref="string"/></param>
    /// <param name="substrings">the <see cref="string"/>s that shouldn't be inside of <paramref name="str"/></param>
    /// <returns>the inverse of <see cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/></returns>
    public static bool ContainsNone(this string str, params string[] substrings) => !ContainsAny(str, substrings);

    public static bool ContainsNone(this ReadOnlySpan<char> str, ReadOnlySpan<char> chars) => str.IndexOfAny(chars) == -1;

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
        return multilineContent.Split(
            new[] {
                "\r\n", "\r", "\n"
            },
            options
        );
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

    [Pure]
    public static IEnumerable<string> IndentWithLabel(
        [InstantHandle] this IEnumerable<string?> lines,
        string?                                   label,
        string?                                   joiner = " "
    ) {
        if (label == null) {
            return lines.Select(it => $"{it}");
        }

        var firstLinePrefix = label.Suffix(joiner).Render();
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
    [Obsolete("inefficient due to object boxing")]
    public static IEnumerable<string?> ToStringLines(this object? obj, string? nullPlaceholder = "") {
        return obj?
               .ToString()
               .Lines()
               .Select(it => it.Value) ?? Enumerable.Empty<string?>();
    }

    #endregion

    #endregion Line Management

    #region "Default" Strings

    /// <param name="obj">this <see cref="object"/></param>
    /// <param name="nullPlaceholder">the output if this <see cref="Object"/> is <c>null</c>. Defaults to <see cref="Prettification.DefaultNullPlaceholder"/></param>
    /// <returns><see cref="object.ToString"/> if this <see cref="Object"/> isn't <c>null</c>; otherwise, <paramref name="nullPlaceholder"/></returns>
    [Pure]
    public static string OrNullPlaceholder<T>(this T? obj, string? nullPlaceholder) {
        nullPlaceholder ??= Prettification.DefaultNullPlaceholder;
        return obj?.ToString() ?? nullPlaceholder;
    }

    /// <inheritdoc cref="OrNullPlaceholder{T}(T?,string?)"/>
    [Pure]
    public static string OrNullPlaceholder<T>(this T? obj) => OrNullPlaceholder(obj, default(PrettificationSettings));

    /// <inheritdoc cref="OrNullPlaceholder{T}(T?,string?)"/>
    [Pure]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static bool IsEmpty([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str);

    /// <summary>
    /// The inverse of <see cref="IsEmpty(string?)"/>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns>!<see cref="IsEmpty(string?)"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static bool IsNotEmpty([NotNullWhen(true)] this string? str) => !string.IsNullOrEmpty(str);

    /// <summary>
    /// An extension method for <see cref="string.IsNullOrWhiteSpace"/> matching Java's <a href="https://commons.apache.org/proper/commons-lang/apidocs/org/apache/commons/lang3/StringUtils.html#isBlank-java.lang.CharSequence-">StringUtils.isBlank()</a>
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns><see cref="string.IsNullOrWhiteSpace"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static bool IsBlank([NotNullWhen(false)] this string? str) => string.IsNullOrWhiteSpace(str);

    /// <summary>
    /// The inverse of <see cref="IsBlank(string?)"/>
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns><b>NOT</b> <see cref="string.IsNullOrWhiteSpace"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static bool IsNotBlank([NotNullWhen(true)] this string? str) => !string.IsNullOrWhiteSpace(str);

    #endregion

    #region {x}IfMissing

    [Pure] public static string PrependIfMissing(this string str, char               prefix)                                                             => str.StartsWith(prefix) ? str : prefix + str;
    [Pure] public static string PrependIfMissing(this string str, ReadOnlySpan<char> prefix, StringComparison comparisonType = StringComparison.Ordinal) => str.AsSpan().StartsWith(prefix, comparisonType) ? str : str.Prefix(prefix);
    [Pure] public static string AppendIfMissing(this  string str, char               suffix)                                                             => str.EndsWith(suffix) ? str : str + suffix;
    [Pure] public static string AppendIfMissing(this  string str, ReadOnlySpan<char> suffix, StringComparison comparisonType = StringComparison.Ordinal) => str.AsSpan().EndsWith(suffix, comparisonType) ? str : str.Suffix(suffix);

    #endregion

    #region Substrings

    /// <summary>
    /// <ul>
    /// <li>Trimming operations on <see cref="string"/>s should return a <see cref="ReadOnlySpan{T}"/> representing a sub-string of the input.</li>
    /// <li>When we want to go back to a <see cref="string"/>, we must call <see cref="ReadOnlySpan{T}.ToString"/> - unfortunately, this will allocate a new <see cref="string"/> even if the <see cref="ReadOnlySpan{T}"/> is identical to the <see cref="string"/>.</li>
    /// <li>To prevent this allocation in the event that we didn't actually trim the original <see cref="string"/>, we re-use that <see cref="string"/> <i>(<paramref name="og"/>)</i> if the <see cref="string.Length"/> matches <paramref name="neu"/>'s <see cref="ReadOnlySpan{T}.Length"/>.</li>
    /// <li>This is the same strategy used by <see cref="Path.GetFileNameWithoutExtension(string)"/>, etc.</li>
    /// </ul>
    /// </summary>
    /// <param name="neu">the new, possibly truncated <see cref="ReadOnlySpan{T}"/>, which we <b><i>assume</i></b> is derived from <paramref name="og"/></param>
    /// <param name="og">the original <see cref="string"/></param>
    /// <returns>the <see cref="string"/> representation of <paramref name="neu"/> - re-using <paramref name="og"/> if possible</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    private static string IfLengthChanged(this ReadOnlySpan<char> neu, string og) {
        // Make sure that `neu` actually contains characters of `og` (if it isn't empty)
        // 📎 We _assume_ this condition outside of `Debug` mode!
        Debug.Assert(neu.IsEmpty || neu.Overlaps(og));

        return og.Length == neu.Length ? og : neu.ToString();
    }

    [Pure] public static string SubstringBefore(this    string str, ReadOnlySpan<char> splitter, StringComparison comparisonType = StringComparison.Ordinal) => str.AsSpan().BeforeFirst(splitter, comparisonType).IfLengthChanged(str);
    [Pure] public static string SubstringBeforeAny(this string str, ReadOnlySpan<char> anyOf)    => str.AsSpan().BeforeFirstAny(anyOf).IfLengthChanged(str);
    [Pure] public static string SubstringAfter(this     string str, ReadOnlySpan<char> splitter) => str.AsSpan().AfterLast(splitter).IfLengthChanged(str);
    [Pure] public static string SubstringAfterAny(this  string str, ReadOnlySpan<char> anyOf)    => str.AsSpan().AfterLastAny(anyOf).IfLengthChanged(str);

    /// <summary>
    /// Splits <paramref name="str"/> by the <b>first</b> occurrence of <paramref name="splitter"/>.
    /// <p/>
    /// If <paramref name="splitter"/> isn't found, then <c>(<paramref name="str"/>, <see cref="string.Empty"/>)</c>
    /// </summary>
    /// <param name="str">the original <see cref="string"/></param>
    /// <param name="splitter">the <see cref="string"/> being used to split <paramref name="str"/> (📎 will <b>not</b> be included in the output)</param>
    /// <returns>the split <paramref name="str"/> if <paramref name="splitter"/> was found; otherwise, (<paramref name="str"/>, <see cref="string.Empty">""</see>)</returns>
    [Pure]
    public static (string before, string after) Partition(this string str, string splitter) {
        return str.AsSpan().TryPartition(splitter, out var before, out var after) ? (before.ToString(), after.ToString()) : (str, "");
    }

    /// <summary>
    /// Similar to <see cref="Partition"/>, except this splits by the <b>last</b> occurrence of <paramref name="splitter"/>.
    /// </summary>
    /// <param name="str">the original <see cref="string"/></param>
    /// <param name="splitter">the <see cref="string"/> being used to split <paramref name="str"/> (📎 will <b>not</b> be included in the output)</param>
    /// <returns>the split <paramref name="str"/> if <paramref name="splitter"/> was found; otherwise, (<see cref="string.Empty">""</see>, <paramref name="str"/>)</returns>
    [Pure]
    public static (string before, string after) PartitionLast(this string str, string splitter) {
        return str.AsSpan().TryPartitionLast(splitter, out var before, out var after) ? (before.ToString(), after.ToString()) : ("", str);
    }

    #endregion
}