using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;

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
            var (leftCut, rightCut) = cutAmount.Sperp(.5);
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

    /// <summary>
    /// Uses either <see cref="Truncate"/> or <see cref="FillRight"/> to get <paramref name="self"/> to be <paramref name="desiredLength"/> long.
    /// </summary>
    /// <param name="self">the original <see cref="string"/></param>
    /// <param name="desiredLength">the <see cref="string.Length"/> that <paramref name="self"/> will have</param>
    /// <param name="filler">the <see cref="string"/> used to <see cref="FillRight"/> if <paramref name="self"/> is shorter than <paramref name="desiredLength"/></param>
    /// <param name="trail">the <see cref="string"/> used to indicated that <paramref name="self"/> has been <see cref="Truncate"/>d</param>
    /// <param name="alignment">controls which side of the string should have characters removed to match <see cref="desiredLength"/></param>
    /// <param name="mirrorPadding">if <c>true</c>, padding applied to the left (beginning) of the string will be <see cref="Backwards"/></param>
    /// <returns>a <see cref="string"/> with a <see cref="string.Length"/> of <paramref name="desiredLength"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ForceToLength(
        this string? self,
        [ValueRange(0, long.MaxValue)]
        int desiredLength,
        string?         filler        = " ",
        string?         trail         = Ellipsis,
        StringAlignment alignment     = StringAlignment.Left,
        bool            mirrorPadding = true
    ) {
        self ??= "";

        return self.Length.CompareTo(desiredLength) switch {
            <= -1 => self.Fill(desiredLength, filler, alignment, mirrorPadding: mirrorPadding),
            0     => self,
            >= 1  => self.Truncate(desiredLength, trail, alignment),
        };
    }

    #region Alignment

    /// <summary>
    /// <see cref="StringAlignment.Center"/>-aligns this <see cref="string"/> by evenly <see cref="FillLeft"/>- and <see cref="FillRight"/>-ing so that <paramref name="input"/> is as long as <paramref name="lineWidth"/>.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="lineWidth"></param>
    /// <param name="padString"></param>
    /// <param name="roundingDirection"></param>
    /// <param name="mirrorPadding">if true, the left padding will be <see cref="Backwards"/>:
    /// <code><![CDATA[
    /// true    ---* str *---
    /// false   *--- str *---
    /// ]]></code>
    /// </param>
    /// <returns>the <see cref="StringAlignment.Center"/>-aligned <see cref="string"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="input"/> is longer than <paramref name="lineWidth"/></exception>
    public static string Center(
        this string       input,
        int               lineWidth,
        string            padString         = " ",
        RoundingDirection roundingDirection = RoundingDirection.Floor,
        bool              mirrorPadding     = true
    ) {
        Must.NotBeNull(input, nameof(input), nameof(Center));

        if (input.Length > lineWidth) {
            throw new ArgumentOutOfRangeException($"Unable to center the string because its length ({input.Length}) was greater than the {nameof(lineWidth)} {lineWidth} !");
        }

        var padAmount = lineWidth - input.Length;
        var (leftPad, rightPad) = padAmount.Sperp(.5).Select(input.RepeatToLength);
        if (mirrorPadding) {
            leftPad = leftPad.Backwards();
        }

        return string.Join(leftPad, padAmount, rightPad);
    }

    #endregion

    #region Filling

    [System.Diagnostics.Contracts.Pure]
    public static string FillRight(this string self, [NonNegativeValue] int totalLength, string? filler = " ") => self.Fill(totalLength, filler, alignment: StringAlignment.Right);

    [System.Diagnostics.Contracts.Pure]
    public static string FillLeft(this string self, [NonNegativeValue] int totalLength, string filler) => self.Fill(totalLength, filler, alignment: StringAlignment.Left);

    /// <summary>
    /// Adds repetitions of <see cref="filler"/> until <see cref="original"/> is at least <see cref="totalLength"/>.
    /// <p/>
    /// If <see cref="original"/> is longer than <see cref="totalLength"/>, nothing happens.
    /// </summary>
    /// <param name="original">this <see cref="string"/></param>
    /// <param name="totalLength">the desired <see cref="string.Length"/> of the result</param>
    /// <param name="filler">the <see cref="string"/> used to pad <paramref name="original"/>.
    /// <br/>
    /// 📎 If <see cref="string.IsNullOrEmpty"/>, <see cref="original"/> is used instead.</param>
    /// <param name="alignment">where the <see cref="original"/> string should be aligned within the result</param>
    /// <param name="mirrorPadding">if <c>true</c>, filling applied to the left (start) of the string will be <see cref="Backwards"/></param>
    /// <param name="leftSideRounding">the <see cref="RoundingDirection"/> used when determining how much filling should be on the <b>left</b> side</param>
    /// <returns>a <see cref="string"/> with a <see cref="string.Length"/> >= <see cref="totalLength"/></returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown <see cref="StringAlignment"/> is passed</exception>
    public static string Fill(
        this string original,
        [NonNegativeValue]
        int totalLength,
        string?           filler           = default,
        StringAlignment   alignment        = StringAlignment.Left,
        bool              mirrorPadding    = true,
        RoundingDirection leftSideRounding = default
    ) {
        if (original.Length >= totalLength) {
            return original;
        }

        filler = string.IsNullOrEmpty(filler) ? original : filler;

        var neededLength = totalLength - original.Length;
        var repeated     = filler.RepeatToLength(neededLength);

        return alignment switch {
            StringAlignment.Left   => $"{original}{repeated}",
            StringAlignment.Right  => $"{repeated.Backwards()}{original}",
            StringAlignment.Center => original.Center(totalLength, filler, leftSideRounding, mirrorPadding),
            _                      => throw BEnum.UnhandledSwitch(alignment),
        };
    }

    #endregion

    public static string FormatHeading(string heading, string border = "=", string padding = " ") {
        var middle = $"{border}{padding}{heading}{padding}{border}";
        var hRule  = border.FillRight(middle.Length, border);
        return $"{hRule}\n{middle}\n{hRule}";
    }

    [System.Diagnostics.Contracts.Pure]
    public static string? Trim(this string? input, string trimString) {
        if (input == null) {
            return null;
        }

        var pattern = new Regex(Regex.Escape(trimString));
        return Trim(input, pattern);
    }

    #region Trimming

    [System.Diagnostics.Contracts.Pure]
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
    [System.Diagnostics.Contracts.Pure]
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
    [System.Diagnostics.Contracts.Pure]
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
    [System.Diagnostics.Contracts.Pure]
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
    [System.Diagnostics.Contracts.Pure]
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

    [System.Diagnostics.Contracts.Pure]
    public static string Limit(
        this string input,
        Regex       trimPattern,
        int         maxKept
    ) {
        return _Limit(input, trimPattern, maxKept, false);
    }

    [System.Diagnostics.Contracts.Pure]
    public static string Limit(this string input, string trimString, int maxKept) {
        return Limit(input, new Regex(Regex.Escape(trimString)), maxKept);
    }

    [System.Diagnostics.Contracts.Pure]
    public static string LimitStart(
        this string input,
        Regex       trimPattern,
        int         maxKept
    ) {
        return _Limit(input, trimPattern, maxKept, true);
    }

    [System.Diagnostics.Contracts.Pure]
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

        var capCount = grp.Captures.Count;

        if (capCount < minKept) {
            var additional = padString.Repeat((int)minKept - capCount);
            return trimFrom == TrimFrom.Start ? $"{additional}{input}" : $"{input}{additional}";
        }

        // ReSharper disable once InvertIf
        if (capCount > maxKept) {
            var numberToTrim = capCount - maxKept;
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
    [System.Diagnostics.Contracts.Pure]
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

    [System.Diagnostics.Contracts.Pure]
    public static string EnsureEndingPattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int minimumRequired
    ) {
        return _ForcePattern(input, trimPattern, padString, minimumRequired, null, TrimFrom.End);
    }

    [System.Diagnostics.Contracts.Pure]
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

    [System.Diagnostics.Contracts.Pure]
    public static string EnsureStartingPattern(
        this string input,
        Regex       trimPattern,
        string      padString,
        [NonNegativeValue]
        int minimumRequired
    ) {
        return _ForcePattern(input, trimPattern, padString, minimumRequired, null, TrimFrom.Start);
    }

    [System.Diagnostics.Contracts.Pure]
    public static string ForceStartingString(
        this string input,
        string      startingString,
        [NonNegativeValue]
        int startingStringRepetitions
    ) {
        return ForceStartingString(input, startingString, startingStringRepetitions, startingStringRepetitions);
    }

    [System.Diagnostics.Contracts.Pure]
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

    #endregion
}