using System.ComponentModel;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// TODO: add to <see cref="PrettificationSettings"/>
/// </summary>
public enum StringAlignment {
    Left, Right, Center,
}

public static class StringAlignmentExtensions {
    public static string ApplyTo(
        this StringAlignment alignment,
        string               str,
        string               padString = " ",
        FillerSettings?      settings  = default
    ) {
        return str.Align(
            alignment: alignment,
            padString: padString,
            settings: settings
        );
    }

    /// <summary>
    /// Applies a <see cref="StringAlignment"/> to this <see cref="string"/>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <param name="width">the length of the resulting <see cref="string"/></param>
    /// <param name="alignment">the type of <see cref="StringAlignment"/></param>
    /// <param name="padString">see <see cref="FillerSettings.PadString"/></param>
    /// <param name="settings">fine-grained <see cref="FillerSettings"/> <i>(📎 Individual parameters take precedence over these settings)</i></param>
    /// <returns>the <see cref="StringAlignment"/>-aligned <see cref="string"/></returns>
    public static string Align(
        this string str,
        [NonNegativeValue]
        int? width = default,
        StringAlignment? alignment = default,
        string?          padString = default,
        FillerSettings?  settings  = default
    ) {
        settings = settings.Resolve();
        settings = settings with {
            LineLengthLimit = width ?? settings.LineLengthLimit,
            Alignment = alignment   ?? settings.Alignment,
            PadString = padString   ?? settings.PadString,
        };
        return str.Align(settings);
    }

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
    /// <returns>a <see cref="string"/> with a <see cref="string.Length"/> >= <see cref="totalLength"/></returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown <see cref="StringAlignment"/> is passed</exception>
    public static string Fill(
        this string original,
        [NonNegativeValue]
        int totalLength,
        string?         filler    = default,
        StringAlignment alignment = StringAlignment.Left
    ) {
        return original.Align(
            settings: new FillerSettings {
                LineLengthLimit = totalLength,
                PadString       = filler.IfEmpty(original),
                Alignment       = alignment,
            }
        );
    }

    /// <summary>
    /// Aligns a string according to a set of <see cref="FillerSettings"/>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <param name="settings">a set of <see cref="FillerSettings"/></param>
    /// <returns>the aligned <see cref="string"/></returns>
    public static string Align(this string str, FillerSettings settings) {
        if (str.Length >= settings.LineLengthLimit) {
            return settings.OverflowStyle switch {
                OverflowStyle.Overflow => str,
                OverflowStyle.Truncate => str.Truncate(settings.LineLengthLimit, trail: settings.TruncateTrail, alignment: settings.Alignment),
                OverflowStyle.Wrap     => throw BEnum.Unsupported(settings.OverflowStyle, "I want to do this someday, though..."),
                _                      => throw BEnum.UnhandledSwitch(settings.OverflowStyle),
            };
        }

        var filler       = settings.PadString.IfEmpty(str);
        var fillerLength = settings.LineLengthLimit - str.Length;

        (int left, int right) fillAmounts = settings.Alignment switch {
            StringAlignment.Left   => (0, fillerLength),
            StringAlignment.Right  => (fillerLength, 0),
            StringAlignment.Center => fillerLength.Bisect(settings.LeftSideRounding),
            _                      => throw BEnum.UnhandledSwitch(settings.Alignment),
        };

        var rightFill = filler.RepeatToLength(fillAmounts.right);
        var leftFill  = fillAmounts.left == fillAmounts.right ? rightFill : filler.RepeatToLength(fillAmounts.left);
        leftFill = settings.MirrorPadding.ApplyTo(leftFill);

        return $"{leftFill}{str}{rightFill}";
    }
}