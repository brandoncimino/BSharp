using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

public static class RangeExtensions {
    /// <param name="range">a <see cref="Range"/> of values. <i>⚠ Must not contain any <see cref="Index.IsFromEnd"/> values!</i></param>
    /// <param name="value">the <see cref="int"/> that might be inside the <see cref="Range"/></param>
    /// <param name="clusivity">determines whether the <see cref="Range.Start"/> and <see cref="Range.End"/> values should be considered</param>
    /// <returns><c>true</c> if this <see cref="Range"/> contains the given <paramref name="value"/></returns>
    /// <exception cref="NotSupportedException">if the <see cref="Range.Start"/> or <see cref="Range.End"/> <see cref="Index.IsFromEnd"/></exception>
    public static bool Contains(this Range range, int value, Clusivity clusivity = Clusivity.Inclusive) {
        if (range.Start.IsFromEnd || range.End.IsFromEnd) {
            throw new NotSupportedException($"How would I even handle {nameof(Index.IsFromEnd)}?!");
        }

        return value.IsInRange(range.Start.Value, range.End.Value, clusivity);
    }
}