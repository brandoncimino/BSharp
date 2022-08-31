using System.Drawing;

namespace FowlFever.BSharp;

/// <summary>
/// Adds <a href="https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct">deconstructor</a> support for various types.
/// </summary>
public static partial class Deconstructor {
    #region System.Drawing.Color

    [Pure]
    public static void Deconstruct(this Color color, out byte red, out byte green, out byte blue) {
        red   = color.R;
        green = color.G;
        blue  = color.B;
    }

    [Pure]
    public static void Deconstruct(
        this Color color,
        out  byte  alpha,
        out  byte  red,
        out  byte  green,
        out  byte  blue
    ) {
        alpha = color.A;
        red   = color.R;
        green = color.G;
        blue  = color.B;
    }

    #endregion
}