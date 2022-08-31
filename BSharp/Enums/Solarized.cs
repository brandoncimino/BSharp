using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

namespace FowlFever.BSharp.Enums;

/// <summary>
/// Enumerates the colors of the <a href="https://ethanschoonover.com/solarized/">Solarized</a> color scheme.
/// </summary>
/// <remarks>
/// <code><![CDATA[
/// SOLARIZED  HEX      16/8  TERMCOL  XTERM/HEX    L*A*B       RGB          HSB
/// ---------  -------  ----  -------  -----------  ----------  -----------  -----------
/// base03     #002b36   8/4  brblack  234 #1c1c1c  15 -12 -12    0  43  54  193 100  21
/// base02     #073642   0/4  black    235 #262626  20 -12 -12    7  54  66  192  90  26
/// base01     #586e75  10/7  brgreen  240 #585858  45 -07 -07   88 110 117  194  25  46
/// base00     #657b83  11/7  bryellow 241 #626262  50 -07 -07  101 123 131  195  23  51
/// base0      #839496  12/6  brblue   244 #808080  60 -06 -03  131 148 150  186  13  59
/// base1      #93a1a1  14/4  brcyan   245 #8a8a8a  65 -05 -02  147 161 161  180   9  63
/// base2      #eee8d5   7/7  white    254 #e4e4e4  92 -00  10  238 232 213   44  11  93
/// base3      #fdf6e3  15/7  brwhite  230 #ffffd7  97  00  10  253 246 227   44  10  99
/// yellow     #b58900   3/3  yellow   136 #af8700  60  10  65  181 137   0   45 100  71
/// orange     #cb4b16   9/3  brred    166 #d75f00  50  50  55  203  75  22   18  89  80
/// red        #dc322f   1/1  red      160 #d70000  50  65  45  220  50  47    1  79  86
/// magenta    #d33682   5/5  magenta  125 #af005f  50  65 -05  211  54 130  331  74  83
/// violet     #6c71c4  13/5  brmagenta 61 #5f5faf  50  15 -45  108 113 196  237  45  77
/// blue       #268bd2   4/4  blue      33 #0087ff  55 -10 -45   38 139 210  205  82  82
/// cyan       #2aa198   6/6  cyan      37 #00afaf  60 -35 -05   42 161 152  175  74  63
/// green      #859900   2/2  green     64 #5f8700  60 -20  65  133 153   0   68 100  60
/// ]]></code>
/// </remarks>
public enum Solarized {
    Base0,
    Base1,
    Base2,
    Base3,
    Base00,
    Base01,
    Base02,
    Base03,
    Yellow,
    Orange,
    Red,
    Magenta,
    Violet,
    Blue,
    Cyan,
    Green
}

public static class SolarizedExtensions {
    /// <param name="solarized">this <see cref="Solarized"/> color</param>
    /// <returns>the corresponding <a href="https://en.wikipedia.org/wiki/Web_colors#Hex_triplet">hex triplet</a> representation</returns>
    /// <exception cref="ArgumentOutOfRangeException">if an unknown <see cref="Solarized"/> color is provided</exception>
    public static string Hex(this Solarized solarized) {
        return solarized switch {
            Solarized.Base0   => "#002b36",
            Solarized.Base1   => "#073642",
            Solarized.Base2   => "#586e75",
            Solarized.Base3   => "#657b83",
            Solarized.Base00  => "#839496",
            Solarized.Base01  => "#93a1a1",
            Solarized.Base02  => "#eee8d5",
            Solarized.Base03  => "#fdf6e3",
            Solarized.Yellow  => "#b58900",
            Solarized.Orange  => "#cb4b16",
            Solarized.Red     => "#dc322f",
            Solarized.Magenta => "#d33682",
            Solarized.Violet  => "#6c71c4",
            Solarized.Blue    => "#268bd2",
            Solarized.Cyan    => "#2aa198",
            Solarized.Green   => "#859900",
            _                 => throw new ArgumentOutOfRangeException(nameof(solarized), solarized, $"{solarized} is not a valid {nameof(Solarized)} color!")
        };
    }

    public static byte Alpha(this Solarized solarized) => solarized.Color().A;
    public static byte Red(this   Solarized solarized) => solarized.Color().R;
    public static byte Green(this Solarized solarized) => solarized.Color().G;
    public static byte Blue(this  Solarized solarized) => solarized.Color().B;

    /// <param name="solarized">this <see cref="Solarized"/> color</param>
    /// <returns>the equivalent <see cref="Spectre"/>.<see cref="Spectre.Console"/>.<see cref="Spectre.Console.Color"/></returns>
    public static Spectre.Console.Color SpectreColor(this Solarized solarized) {
        var (r, g, b) = solarized.Color();
        return new Spectre.Console.Color(r, g, b);
    }

    /// <summary>
    /// Maps <see cref="Solarized"/> colors to <see cref="System.Drawing"/>.<see cref="System.Drawing.Color"/>s.
    /// </summary>
    public static readonly ImmutableDictionary<Solarized, Color> ColorLookup = new Dictionary<Solarized, Color> {
        [Solarized.Base0]   = System.Drawing.Color.FromArgb(255, 0,   43,  54),
        [Solarized.Base1]   = System.Drawing.Color.FromArgb(255, 7,   54,  66),
        [Solarized.Base2]   = System.Drawing.Color.FromArgb(255, 88,  110, 117),
        [Solarized.Base3]   = System.Drawing.Color.FromArgb(255, 101, 123, 131),
        [Solarized.Base00]  = System.Drawing.Color.FromArgb(255, 131, 148, 150),
        [Solarized.Base01]  = System.Drawing.Color.FromArgb(255, 147, 161, 161),
        [Solarized.Base02]  = System.Drawing.Color.FromArgb(255, 238, 232, 213),
        [Solarized.Base03]  = System.Drawing.Color.FromArgb(255, 253, 246, 227),
        [Solarized.Yellow]  = System.Drawing.Color.FromArgb(255, 181, 137, 0),
        [Solarized.Orange]  = System.Drawing.Color.FromArgb(255, 203, 75,  22),
        [Solarized.Red]     = System.Drawing.Color.FromArgb(255, 220, 50,  47),
        [Solarized.Magenta] = System.Drawing.Color.FromArgb(255, 211, 54,  130),
        [Solarized.Violet]  = System.Drawing.Color.FromArgb(255, 108, 113, 196),
        [Solarized.Blue]    = System.Drawing.Color.FromArgb(255, 38,  139, 210),
        [Solarized.Cyan]    = System.Drawing.Color.FromArgb(255, 42,  161, 152),
        [Solarized.Green]   = System.Drawing.Color.FromArgb(255, 133, 153, 0),
    }.ToImmutableDictionary();

    /// <param name="solarized">this <see cref="Solarized"/> color</param>
    /// <returns>the corresponding <see cref="System"/>.<see cref="System.Drawing"/>.<see cref="System.Drawing.Color"/></returns>
    public static Color Color(this Solarized solarized) => ColorLookup[solarized];
}