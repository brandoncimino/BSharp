using System;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings.Settings;
using FowlFever.BSharp.Strings.Tabler;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

public static partial class StringUtils {
    #region Padding, filling, truncating, trimming, and trailing

    /// <summary>
    /// Reduces <paramref name="self"/> to <paramref name="maxLength"/> characters, replacing the extra bits with a <paramref name="trail"/> if specified.
    /// <p/>
    /// Which side of the string the extra bits are removed from can be controlled by <see cref="StringAlignment"/>.
    /// <p/>
    /// If the original <see cref="string.Length"/> is less than <paramref name="maxLength"/>, returns <paramref name="self"/>.
    /// </summary>
    /// <param name="self">the <see cref="string"/> being truncated</param>
    /// <param name="maxLength">the <b>maximum</b> size of the final string</param>
    /// <param name="trail">a <see cref="string"/> to replace the end bits of <paramref name="self"/> to show that it has been truncated. Defaults to an <see cref="Ellipsis"/></param>
    /// <param name="alignment">whether the beginning or end of the string should be preserved when we truncate</param>
    /// <returns>a <see cref="string"/> no longer than <paramref name="maxLength"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="maxLength"/> is negative</exception>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="trail"/> is longer than <paramref name="maxLength"/></exception>
    public static string Truncate(
        this string self,
        [NonNegativeValue]
        int maxLength,
        string?         trail     = Ellipsis,
        StringAlignment alignment = StringAlignment.Left
    ) {
        string Lathe() {
            var shortLimit = maxLength   + trail.Length * 2;
            var cutAmount  = self.Length - shortLimit;
            Must.BePositive(cutAmount);
            var (leftCut, rightCut) = cutAmount.Splerp(.5);
            return self[leftCut..^rightCut];
        }

        trail ??= Ellipsis;

        Must.BePositive(maxLength);
        Must.Compare(trail.Length, ComparisonOperator.LessThan, maxLength);
        if (self.Length <= maxLength) {
            return self;
        }

        var shortenedLength = maxLength - trail.Length;

        return alignment switch {
            StringAlignment.Left   => $"{self[..shortenedLength]}{trail}",
            StringAlignment.Right  => $"{trail}{self[^shortenedLength..]}",
            StringAlignment.Center => Lathe(),
            _                      => throw BEnum.UnhandledSwitch(alignment),
        };
    }

    #endregion

    #region Filling

    #endregion

    [Pure]
    public static string FormatHeading(string heading, PrettificationSettings? settings = default) {
        return Table.Of(Row.OfHeaders(heading)).Prettify(settings);
    }

    [Pure]
    public static string FormatHeading(
        string                  heading,
        OneLine?                border   = default,
        OneLine?                padding  = default,
        PrettificationSettings? settings = default
    ) {
        return FormatHeading(
            heading,
            settings.Resolve() with {
                TableSettings = settings.Resolve().TableSettings with {
                    HeaderSeparator = border ?? OneLine.Hyphen,
                },
                FillerSettings = settings.Resolve().FillerSettings with {
                    PadString = padding ?? OneLine.Space,
                },
            }
        );
    }

    #region Trimming

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

    private static string _GetTrimQuantifier(int? numberToTrim) {
        return numberToTrim.IfPresentOrElse(it => $"{{0,{it}}}", () => "*");
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
    /// <param name="numberToTrim">the maximum number of <paramref name="trimPattern"/>s to remove. If <c>null</c>, <b>all</b> instances will be removed</param>
    /// <returns><paramref name="input"/> with some number of <paramref name="trimPattern"/>s removed from the <b>beginning</b></returns>
    [Pure]
    public static string TrimStart(this string input, Regex trimPattern, int? numberToTrim = default) {
        var trimQuantifier = _GetTrimQuantifier(numberToTrim);
        var reg            = new Regex(@$"^({trimPattern}){trimQuantifier}(?<trimmed>.*?)$");
        var match          = reg.Match(input);

        return match.Success ? match.Groups["trimmed"].Value : input;
    }

    /// <summary>
    /// Removes up to <paramref name="numberToTrim"/> instances of <paramref name="trimString"/> from the <b>end</b> of <paramref name="input"/>.
    /// </summary>
    /// <remarks>
    /// If <paramref name="numberToTrim"/> is null, then <b>all</b> instances are removed.
    /// </remarks>
    /// <param name="input">the original <see cref="string"/></param>
    /// <param name="trimString">the <see cref="string"/> to be removed </param>
    /// <param name="numberToTrim">the maximum number of <paramref name="trimString"/>s to remove. If <c>null</c>, <b>all</b> instances will be removed</param>
    /// <returns><paramref name="input"/> with some number of <paramref name="trimString"/>s removed from the <b>end</b></returns>
    [Pure]
    public static string? TrimEnd(this string input, string trimString, int? numberToTrim = default) {
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
        var trimQuantifier = _GetTrimQuantifier(numberToTrim);
        var reg            = new Regex($@"^(?<trimmed>.*?)({trimPattern}){trimQuantifier}$");
        var match          = reg.Match(input);

        return match.Success ? match.Groups["trimmed"].Value : input;
    }

    #endregion

    #region Limiting

    private static string _Limit(
        string input,
        Regex  trimPattern,
        int    maxKept,
        bool   fromStart
    ) {
        var keep = $"({trimPattern}){{{maxKept}}}";
        var drop = $"({trimPattern})*";

        var pattern = fromStart
                          ? new Regex($"^{drop}(?<final>{keep}.*?)$", RegexOptions.RightToLeft)
                          : new Regex($"^(?<final>.*?{keep}){drop}$");

        var match = pattern.Match(input);
        return match.Success ? match.Groups["final"].Value : input;
    }

    [Pure]
    public static string Limit(
        this string input,
        Regex       trimPattern,
        int         maxKept
    ) {
        return _Limit(input, trimPattern, maxKept, false);
    }

    [Pure]
    public static string Limit(this string input, string trimString, int maxKept) {
        return Limit(input, new Regex(Regex.Escape(trimString)), maxKept);
    }

    [Pure]
    public static string LimitStart(
        this string input,
        Regex       trimPattern,
        int         maxKept
    ) {
        return _Limit(input, trimPattern, maxKept, true);
    }

    [Pure]
    public static string LimitStart(this string input, string trimString, int maxKept) {
        return LimitStart(input, new Regex(Regex.Escape(trimString)), maxKept);
    }

    #endregion

    #region "Forcing"

    internal enum TrimFrom { End, Start }

    private static string _ForcePattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int? minKept,
        [NonNegativeValue]
        int? maxKept,
        TrimFrom trimFrom
    ) {
        if (minKept < 0 || minKept > maxKept) {
            throw new ArgumentOutOfRangeException(nameof(minKept), minKept, $"Must be >= 0 and <= {nameof(maxKept)} ({maxKept})");
        }

        if (maxKept <= 0 || maxKept < minKept) {
            throw new ArgumentOutOfRangeException(nameof(maxKept), maxKept, $"Must be >= 0 and >= {nameof(minKept)} ({minKept})");
        }

        var pat = $"(?<chunks>{trimPattern})*";
        pat = trimFrom == TrimFrom.Start ? $"^{pat}" : $"{pat}$";
        var match = input.Match(pat);
        var grp   = match.Groups["chunks"];

        if (match.Success == false) {
            return input;
        }

        var capturesFound = grp.Captures.Count;

        if (minKept > capturesFound) {
            var additional = padString.Repeat((int)minKept - capturesFound);
            return trimFrom == TrimFrom.Start ? $"{additional}{input}" : $"{input}{additional}";
        }

        // ReSharper disable once InvertIf
        if (capturesFound > maxKept) {
            var numberToTrim = capturesFound - maxKept;
            return trimFrom == TrimFrom.Start ? TrimStart(input, trimPattern, numberToTrim) : TrimEnd(input, trimPattern, numberToTrim);
        }

        throw new BrandonException("Shouldn't have been able to reach this");
    }

    /// <summary>
    /// Ensures that there are between <paramref name="minKept"/> and <paramref name="maxKept"/> instances of <paramref name="trimPattern"/> at the <b>end</b>
    /// of this <see cref="string"/>.
    /// <br/>
    /// If there are too many, they are removed.
    /// <br/>
    /// If there are too few, then <paramref name="padString"/> is repeated once for each missing <paramref name="trimPattern"/>.
    /// </summary>
    /// <param name="input">the original <see cref="string"/></param>
    /// <param name="trimPattern">the <see cref="Regex"/> that should appear at the end of the <see cref="string"/></param>
    /// <param name="padString">the <see cref="string"/> appended to <paramref name="input"/> if it doesn't have enough instances of <paramref name="trimPattern"/></param>
    /// <param name="minKept">the <b>minimum</b> occurrences of <see cref="trimPattern"/>. Must be ≥ 0 and ≤ <paramref name="maxKept"/></param>
    /// <param name="maxKept">the <b>maximum</b> occurrences of <see cref="trimPattern"/>. Must be ≥ <see cref="minKept"/> ≥ 0</param>
    /// <returns>a new <see cref="string"/> with the desired ending</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <see cref="maxKept"/> ≥ <see cref="minKept"/> ≥ 0 isn't <c>true</c></exception>
    [Pure]
    public static string ForceEndingPattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int? minKept,
        [NonNegativeValue]
        int? maxKept
    ) {
        return _ForcePattern(input, trimPattern, padString, minKept, maxKept, TrimFrom.End);
    }

    [Pure]
    public static string EnsureEndingPattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int minimumRequired
    ) {
        return _ForcePattern(input, trimPattern, padString, minimumRequired, null, TrimFrom.End);
    }

    [Pure]
    public static string ForceStartingPattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int? minKept,
        [NonNegativeValue]
        int? maxKept
    ) {
        return _ForcePattern(input, trimPattern, padString, minKept, maxKept, TrimFrom.Start);
    }

    [Pure]
    public static string EnsureStartingPattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int minimumRequired
    ) {
        return _ForcePattern(input, trimPattern, padString, minimumRequired, null, TrimFrom.Start);
    }

    [Pure]
    public static string ForceStartingString(
        this string input,
        string      startingString,
        [NonNegativeValue]
        int startingStringRepetitions
    ) {
        return ForceStartingString(input, startingString, startingStringRepetitions, startingStringRepetitions);
    }

    [Pure]
    public static string ForceStartingString(
        this string input,
        string      startingString,
        [NonNegativeValue]
        int minKept,
        [NonNegativeValue]
        int maxKept
    ) {
        return input.ForceStartingPattern(RegexPatterns.Escaped(startingString), startingString, minKept, maxKept);
    }

    #endregion
}