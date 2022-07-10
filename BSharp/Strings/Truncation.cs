using System;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings;

internal static class Truncation {
    /// <summary>
    /// The results of a <see cref="Truncation.PlanTruncation(int,FillerSettings)"/> operation.
    /// </summary>
    public readonly ref struct TruncationPlan {
        public Range FinalCut   { get; init; }
        public Range LeftTrail  { get; init; }
        public Range RightTrail { get; init; }

        public TruncationPlan(Range leftTrail, Range finalCut, Range rightTrail) {
            LeftTrail  = leftTrail;
            FinalCut   = finalCut;
            RightTrail = rightTrail;
        }

        public OneLine ApplyTo(OneLine leftTrail, OneLine source, OneLine rightTrail) {
            return OneLine.FlatJoin(leftTrail[LeftTrail], source[FinalCut], rightTrail[RightTrail]);
        }
    }

    public static TruncationPlan PlanTruncation(int original, FillerSettings settings) => PlanTruncation(original, settings.LineLengthLimit, settings.TruncateTrail.Length, settings.TruncateTrail.Length, settings.Alignment);

    /// <summary>
    /// Decides what of what goes where.
    /// </summary>
    /// <param name="original">what goes</param>
    /// <param name="limit">how much of what</param>
    /// <param name="leftTrail">replacement what</param>
    /// <param name="rightTrail">replacement what</param>
    /// <param name="alignment">where goes what</param>
    /// <returns>a <see cref="TruncationPlan"/></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static TruncationPlan PlanTruncation(
        int             original,
        int             limit,
        int             leftTrail,
        int             rightTrail,
        StringAlignment alignment = StringAlignment.Left
    ) {
        #region Parameter validations

        if (limit < 0) {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "must be >= 0");
        }

        #endregion

        if (original <= limit) {
            return new TruncationPlan {
                LeftTrail  = default,
                FinalCut   = Range.All,
                RightTrail = default
            };
        }

        var excess = original - limit;

        static TruncationPlan Lathe(int excess, int leftTrail, int rightTrail) {
            var (cut_l, cut_r) =  excess.Bisect();
            cut_l              += leftTrail;
            cut_r              += rightTrail;
            return new TruncationPlan(..leftTrail, cut_l..^cut_r, ..rightTrail);
        }

        return alignment switch {
            StringAlignment.Left   => new TruncationPlan(default,     ..(excess + rightTrail),  ..rightTrail),
            StringAlignment.Right  => new TruncationPlan(..leftTrail, (excess   + leftTrail).., default),
            StringAlignment.Center => Lathe(excess, leftTrail, rightTrail),
            _                      => throw BEnum.UnhandledSwitch(alignment),
        };
    }
}