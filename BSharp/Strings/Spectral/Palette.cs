using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A set of <see cref="Style"/>s.
/// </summary>
public readonly record struct Palette {
    public Stylist LastResort { get; init; }

    public Stylist Numbers    { get; init; }
    public Stylist Strings    { get; init; }
    public Stylist Methods    { get; init; }
    public Stylist Comments   { get; init; }
    public Stylist Hyperlinks { get; init; }
    public Stylist Titles     { get; init; }
    public Stylist Borders    { get; init; }
    public Stylist Delimiters { get; init; }

    public PathPalette      PathPalette      { get; init; }
    public ExceptionPalette ExceptionPalette { get; init; }

    public LogPalette LogPalette { get; init; }

    public readonly record struct SeverityPalette(
        Stylist Good        = default,
        Stylist Bad         = default,
        Stylist Warning     = default,
        Stylist Unimportant = default,
        Stylist Emergency   = default
    );

    public SeverityPalette Severity { get; init; }

    /// <summary>
    /// This <see cref="Palette"/> has been compiled and can <b>never</b> change.
    /// </summary>
    public static readonly Palette HardCoded = new() {
        Numbers    = Color.Purple,
        Strings    = new Stylist(Color.SkyBlue1, Decoration.Italic),
        Hyperlinks = new Stylist(Color.Blue,     Decoration.Underline),
        Severity = new SeverityPalette {
            Good        = Color.Green,
            Bad         = Color.Red,
            Warning     = new Stylist(Color.DarkGoldenrod),
            Emergency   = new Stylist(Color.DarkRed,        Decoration.Bold | Decoration.Underline | Decoration.Italic),
            Unimportant = new Stylist(Color.DarkSlateGray1, Decoration.Italic)
        },
        Titles           = new Style(decoration: Decoration.Bold),
        Delimiters       = Color.Default,
        Borders          = Color.Orange1,
        ExceptionPalette = new ExceptionPalette(),
        LogPalette       = LogPalette.Hardcoded,
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

    public static implicit operator PathPalette(Palette       palette) => palette.PathPalette;
    public static implicit operator ExceptionPalette(Palette  palette) => palette.ExceptionPalette;
    public static implicit operator ExceptionStyle(Palette    palette) => palette.ExceptionPalette;
    public static implicit operator ExceptionSettings(Palette palette) => palette.ExceptionPalette;
    public static implicit operator ExceptionFormats(Palette  palette) => palette.ExceptionPalette;

    #endregion
}

public static class PaletteExtensions {
    public static Palette OrFallback(this Palette? palette)                              => palette ?? Palette.Fallback;
    public static Palette OrFallback(this Palette? palette, params Palette?[] fallbacks) => palette ?? (fallbacks.FirstOrDefault(it => it.HasValue) ?? Palette.Fallback);
}