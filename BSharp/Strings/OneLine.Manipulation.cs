using System.ComponentModel;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings.Enums;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings;

public readonly partial record struct OneLine {
    #region Manipulation

    /// <summary>
    /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges#implicit-range-support">Implicit Range support</a>
    /// </summary>
    public OneLine Slice(int start, int length) => CreateRisky((_stringInfo ?? TextElementString.Empty).Slice(start, length));

    [Pure]
    public OneLine Truncate(int maxLength, FillerSettings? settings = default) {
        settings = settings.Resolve();
        var truncation = Truncation.PlanTruncation(maxLength, settings);
        return FlatJoin(
            settings.TruncateTrail[truncation.LeftTrail],
            this[truncation.FinalCut],
            settings.TruncateTrail[truncation.RightTrail]
        );
    }

    /// <summary>
    /// Cuts this <see cref="OneLine"/> down to the given <see cref="FillerSettings.LineLengthLimit"/>.
    /// </summary>
    /// <param name="settings">settings that inform the <see cref="Truncation"/></param>
    /// <returns>a new <see cref="OneLine"/> with a <see cref="Length"/> <![CDATA[<=]]> <see cref="FillerSettings.LineLengthLimit"/></returns>
    [Pure]
    public OneLine Truncate(FillerSettings settings) {
        return Truncate(settings.LineLengthLimit, settings);
    }

    /// <inheritdoc cref="GraphemeClusterExtensions.RepeatToLength(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.GraphemeCluster},int)"/>
    [Pure]
    public OneLine RepeatToLength(int desiredLength) => CreateRisky(Must.NotBeEmpty(_stringInfo).RepeatToLength(desiredLength));

    /// <summary>
    /// <inheritdoc cref="Fill(FillerSettings?)"/>
    /// </summary>
    /// <param name="desiredLength">see <see cref="FillerSettings.LineLengthLimit"/></param>
    /// <param name="padString">see <see cref="FillerSettings.PadString"/></param>
    /// <param name="settings">additional <see cref="FillerSettings"/></param>
    /// <returns>a <see cref="OneLine"/> with a <see cref="Length"/> of at least <paramref name="desiredLength"/></returns>
    [Pure]
    public OneLine Fill(int desiredLength, OneLine padString, FillerSettings? settings = default) {
        return Fill(settings.Resolve() with { LineLengthLimit = desiredLength, PadString = padString });
    }

    /// <summary>
    /// Adds <see cref="FillerSettings.PadString"/> to this <see cref="Value"/> in order to reach <see cref="FillerSettings.LineLengthLimit"/>.
    /// </summary>
    /// <param name="settings">controls how the fill should be performed</param>
    /// <returns><see cref="OneLine"/> with a <see cref="Length"/> of <b><i>at least</i></b> <see cref="FillerSettings.LineLengthLimit"/></returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an  unknown <see cref="StringAlignment"/> is provided</exception>
    /// <remarks>
    /// This method is purely additive, meaning that the output will always contain <b><i>at least</i></b> the original <see cref="Value"/>.
    /// If the <see cref="Length"/> is already greater than <see cref="FillerSettings.LineLengthLimit"/>, then no changes will be applied.
    /// <p/>
    /// The "destructive" aka "subtractive" equivalent is <see cref="Truncate(int,FillerSettings?)"/>.
    /// </remarks>
    [Pure]
    public OneLine Fill(FillerSettings settings) {
        var fillerLength = settings.LineLengthLimit - Length;

        if (fillerLength <= 0) {
            return this;
        }

        var filler = settings.PadString.IsEmpty ? this : settings.PadString;

        (int left, int right) fillAmounts = settings.Alignment switch {
            StringAlignment.Left   => (0, fillerLength),
            StringAlignment.Right  => (fillerLength, 0),
            StringAlignment.Center => fillerLength.Bisect(settings.LeftSideRounding),
            _                      => throw BEnum.UnhandledSwitch(settings.Alignment),
        };

        var rightFill = filler.RepeatToLength(fillAmounts.right);
        var leftFill  = fillAmounts.left == fillAmounts.right ? rightFill : filler.RepeatToLength(fillAmounts.left);
        leftFill = settings.MirrorPadding.ApplyTo(leftFill);
        return OneLine.FlatJoin(leftFill, this, rightFill);
    }

    /// <summary>
    /// Either <see cref="Fill(int,FowlFever.BSharp.Strings.OneLine,FillerSettings?)"/> or <see cref="Truncate(int,FillerSettings?)"/>s
    /// this <see cref="OneLine"/> so that it reaches <see cref="FillerSettings.LineLengthLimit"/>.
    /// </summary>
    /// <param name="settings">determines how the alignment should be performed</param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown <see cref="FillerSettings.OverflowStyle"/> is provided</exception>
    [Pure]
    public OneLine Fit(FillerSettings? settings) {
        settings = settings.Resolve();

        var comparisonResult = Length.ComparedWith(settings.LineLengthLimit);
        return comparisonResult switch {
            ComparisonResult.LessThan    => Fill(settings),
            ComparisonResult.EqualTo     => this,
            ComparisonResult.GreaterThan => HandleOverflow(this, settings),
            _                            => throw BEnum.UnhandledSwitch(comparisonResult),
        };

        static OneLine HandleOverflow(OneLine og, FillerSettings settings) {
            return settings.OverflowStyle switch {
                OverflowStyle.Overflow => og,
                OverflowStyle.Truncate => og.Truncate(settings),
                OverflowStyle.Wrap     => throw BEnum.NotSupported(settings.OverflowStyle, $"Because this returns a {nameof(OneLine)}, {nameof(OverflowStyle)}.{OverflowStyle.Wrap} the result would be equivalent to {OverflowStyle.Truncate}."),
                _                      => throw BEnum.UnhandledSwitch(settings.OverflowStyle),
            };
        }
    }

    #endregion
}