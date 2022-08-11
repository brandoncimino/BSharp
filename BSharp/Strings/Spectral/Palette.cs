using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A set of <see cref="Style"/>s.
/// </summary>
public readonly record struct Palette {
    public Style? Numbers    { get; init; }
    public Style? Strings    { get; init; }
    public Style? Methods    { get; init; }
    public Style? Comments   { get; init; }
    public Style? Hyperlinks { get; init; }
    public Style? Titles     { get; init; }
    public Style? Borders    { get; init; }

    public record SeverityColors(Color? Good = default, Color? Bad = default);

    public SeverityColors Severity { get; init; }

    /// <summary>
    /// This <see cref="Palette"/> has been compiled and can <b>never</b> change.
    /// </summary>
    public static readonly Palette HardCoded = new() {
        Numbers = new Style(Color.Purple),
        Strings = new Style(Color.DodgerBlue1, decoration: Decoration.Italic),
        Severity = new SeverityColors {
            Good = Color.Green,
            Bad  = Color.Red
        },
    };

    public static Palette Default { get; set; } = HardCoded;
}

public static class PaletteExtensions {
    public static Palette OrDefault(this Palette? palette) => palette ?? Palette.Default;
}