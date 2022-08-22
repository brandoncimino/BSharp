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
        Numbers    = new Style(Color.Purple),
        Strings    = new Style(Color.DodgerBlue1, decoration: Decoration.Italic),
        Hyperlinks = new Style(Color.Blue,        decoration: Decoration.Underline),
        Severity = new SeverityColors {
            Good = Color.Green,
            Bad  = Color.Red
        },
        Titles = new Style(decoration: Decoration.Bold)
    };

    /// <summary>
    /// A settable <see cref="Palette"/> to use as a fallback instead of <c>default</c> or <see cref="HardCoded"/>.
    /// </summary>
    /// <remarks>
    /// Specifically used as the fallback value for <c>null</c> <see cref="Palette"/>s in <see cref="PaletteExtensions"/> methods.
    /// <p/>
    /// Initially set to <see cref="HardCoded"/>, but you can modify it however you see fit.
    /// </remarks>
    public static Palette Fallback { get; set; } = HardCoded;
}

public static class PaletteExtensions {
    public static Palette OrFallback(this Palette? palette) => palette ?? Palette.Fallback;
}