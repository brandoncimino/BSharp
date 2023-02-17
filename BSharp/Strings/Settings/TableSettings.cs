using System.Collections.Generic;

using FowlFever.BSharp.Collections.Apportion;

namespace FowlFever.BSharp.Strings.Settings;

public record TableSettings : Settings {
    /// <summary>
    /// Used as the "default" number of columns so unrelated <see cref="Prettification.Prettify{T}(T?)"/> calls can line up.
    /// </summary>
    public int AutoColumnCount { get;                  init; } = 4;
    public OneLine         HeaderSeparator      { get; init; } = OneLine.Hyphen;
    public OneLine         ColumnSeparator      { get; init; } = OneLine.Space;
    public StringAlignment HeaderAlignment      { get; init; } = StringAlignment.Center;
    public string          EmptyCellPlaceholder { get; init; } = " - ";

    public IEnumerable<int> GetAutoColumnWidths(int totalWidth) {
        return Apportion.Evenly(totalWidth, AutoColumnCount);
    }
}