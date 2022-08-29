using System.Linq;

using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A set of <see cref="Style"/>s.
/// </summary>
public readonly record struct Palette {
    public Stylist Numbers    { get; init; }
    public Stylist Strings    { get; init; }
    public Stylist Methods    { get; init; }
    public Stylist Comments   { get; init; }
    public Stylist Hyperlinks { get; init; }
    public Stylist Titles     { get; init; }
    public Stylist Borders    { get; init; }
    public Stylist Delimiters { get; init; }

    public PathPalette PathPalette { get; init; }

    public record SeverityColors(Color? Good = default, Color? Bad = default);

    public SeverityColors Severity { get; init; }

    /// <summary>
    /// This <see cref="Palette"/> has been compiled and can <b>never</b> change.
    /// </summary>
    public static readonly Palette HardCoded = new() {
        Numbers    = Color.Purple,
        Strings    = new Stylist(Color.DodgerBlue1, Decoration.Italic),
        Hyperlinks = new Stylist(Color.Blue,        Decoration.Underline),
        Severity = new SeverityColors {
            Good = Color.Green,
            Bad  = Color.Red,
        },
        Titles     = new Style(decoration: Decoration.Bold),
        Delimiters = Color.DarkBlue,
        Borders    = Color.Orange1,
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

    #region Conversions

    public static implicit operator PathPalette(Palette palette) => palette.PathPalette;

    #endregion
}

public static class PaletteExtensions {
    public static Palette OrFallback(this Palette? palette)                              => palette ?? Palette.Fallback;
    public static Palette OrFallback(this Palette? palette, params Palette?[] fallbacks) => palette ?? (fallbacks.FirstOrDefault(it => it.HasValue) ?? Palette.Fallback);
}