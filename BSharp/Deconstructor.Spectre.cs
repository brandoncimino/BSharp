using Spectre.Console;

namespace FowlFever.BSharp;

public static partial class Deconstructor {
    #region Spectre

    #region Spectre.Console.Color

    [Pure]
    public static void Deconstruct(this Spectre.Console.Color color, out byte red, out byte green, out byte blue) {
        red   = color.R;
        green = color.G;
        blue  = color.B;
    }

    #endregion

    #region Spectore.Console.Style

    public static void Deconstruct(this Style style, out Color foreground, out Color background, out Decoration decoration) {
        foreground = style.Foreground;
        background = style.Background;
        decoration = style.Decoration;
    }

    #endregion

    #endregion
}