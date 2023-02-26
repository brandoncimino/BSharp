using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Contains <c>const</c> <see cref="string"/>s for basic <a href="https://en.wikipedia.org/wiki/ANSI_escape_code#SGR_(Select_Graphic_Rendition)_parameters">ANSI Select Graphic Rendition</a> codes.
/// </summary>
/// <remarks>
/// <ul>
/// <li>"<see cref="Reset.Fg"/>" is short for "foreground".</li>
/// <li>"<see cref="Reset.Bg"/>" is short for "background".</li>
/// </ul> 
/// </remarks>
/// <todo>
/// TODO: This class can become quite spicy with the addition of <a href="https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members">static virtual interface members</a> 😈
/// </todo>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
public static partial class Ansi {
    private const  string _fg             = "3";
    private const  string _bg             = "4";
    private const  string _bright_fg      = "9";
    private const  string _bright_bg      = "10";
    private const  string _decoration_off = "2";
    internal const string csi             = "\x1b[";

    public static class Reset {
        public const string All = $"{csi}m";
        public const string Fg  = $"{csi}39m";
        public const string Bg  = $"{csi}49m";
    }

    #region Colors

    public static class Black {
        public const  byte         Byte         = 0;
        private const string       Base         = "0m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.Black;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.DarkGray;
        }
    }

    /// <summary>
    /// TODO: what should map to "gray"?
    /// the ANSI standard has, from dark -> light:
    ///     - Black
    ///     - Bright Black
    ///     - White
    ///     - Bright White
    ///
    /// Meanwhile, <see cref="System.ConsoleColor"/> has, from dark -> light:
    ///     - <see cref="System.ConsoleColor.Black"/>
    ///     - <see cref="System.ConsoleColor.DarkGray"/>
    ///     - <see cref="System.ConsoleColor.Gray"/>
    ///     - <see cref="System.ConsoleColor.White"/>
    ///
    /// Verdict:
    /// <ul>
    /// <li><see cref="System.ConsoleColor.Black"/> -> <see cref="Black"/></li>
    /// <li><see cref="System.ConsoleColor.DarkGray"/> -> <see cref="Black.Bright"/></li>
    /// <li><see cref="System.ConsoleColor.Gray"/> -> <see cref="White"/></li>
    /// <li><see cref="System.ConsoleColor.White"/> -> <see cref="White.Bright"/></li>
    /// </ul>
    /// 
    /// </summary>
    public static class Gray {
        public const string       Fg           = White.Fg;
        public const string       Bg           = White.Bg;
        public const ConsoleColor ConsoleColor = System.ConsoleColor.Gray;

        public static class Dark {
            public const string       Fg           = Black.Bright.Fg;
            public const string       Bg           = Black.Bright.Bg;
            public const ConsoleColor ConsoleColor = System.ConsoleColor.DarkGray;
        }
    }

    public static class Red {
        private const string       Base         = "1m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.DarkRed;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.Red;
        }
    }

    public static class Green {
        private const string       Base         = "2m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.DarkGreen;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.Green;
        }
    }

    public static class Yellow {
        private const string       Base         = "3m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.DarkYellow;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.Yellow;
        }
    }

    public static class Blue {
        private const string       Base         = "4m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.DarkBlue;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.Blue;
        }
    }

    public static class Magenta {
        private const string       Base         = "5m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.DarkMagenta;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.Magenta;
        }
    }

    public static class Cyan {
        private const string       Base         = "6m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.DarkCyan;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.Cyan;
        }
    }

    public static class White {
        private const string       Base         = "7m";
        public const  string       Fg           = $"{csi}{_fg}{Base}";
        public const  string       Bg           = $"{csi}{_bg}{Base}";
        public const  ConsoleColor ConsoleColor = System.ConsoleColor.Gray;

        public static class Bright {
            public const string       Fg           = $"{csi}{_bright_fg}{Base}";
            public const string       Bg           = $"{csi}{_bright_bg}{Base}";
            public const ConsoleColor ConsoleColor = System.ConsoleColor.White;
        }
    }

    #endregion

    #region Effects

    public static class Bold {
        public const string On  = $"{csi}1m";
        public const string Off = $"{csi}22m";
    }

    public static class Italic {
        private const string Base = "3";
        public const  string On   = $"{csi}{Base}m";
        public const  string Off  = $"{csi}{_decoration_off}{Base}m";
    }

    public static class Underline {
        private const string Base = "4";
        public const  string On   = $"{csi}{Base}m";
        public const  string Off  = $"{csi}{_decoration_off}{Base}m";
    }

    #endregion

    internal static AnsiByte ToAnsiByte_Foreground(this ConsoleColor consoleColor) {
        return consoleColor switch {
            ConsoleColor.Black       => AnsiByte.Black,
            ConsoleColor.DarkBlue    => AnsiByte.Blue,
            ConsoleColor.DarkGreen   => AnsiByte.Green,
            ConsoleColor.DarkCyan    => AnsiByte.Cyan,
            ConsoleColor.DarkRed     => AnsiByte.Red,
            ConsoleColor.DarkMagenta => AnsiByte.Magenta,
            ConsoleColor.DarkYellow  => AnsiByte.Yellow,
            ConsoleColor.Gray        => AnsiByte.White,
            ConsoleColor.DarkGray    => AnsiByte.Black_Bright,
            ConsoleColor.Blue        => AnsiByte.Blue_Bright,
            ConsoleColor.Green       => AnsiByte.Green_Bright,
            ConsoleColor.Cyan        => AnsiByte.Cyan_Bright,
            ConsoleColor.Red         => AnsiByte.Red_Bright,
            ConsoleColor.Magenta     => AnsiByte.Magenta_Bright,
            ConsoleColor.Yellow      => AnsiByte.Yellow_Bright,
            ConsoleColor.White       => AnsiByte.White_Bright,
            _                        => throw new ArgumentOutOfRangeException(nameof(consoleColor), consoleColor, null)
        };
    }

    internal static AnsiByte ToAnsiByte_Background(this ConsoleColor consoleColor) {
        return consoleColor switch {
            ConsoleColor.Black       => AnsiByte.Black_Background,
            ConsoleColor.DarkBlue    => AnsiByte.Blue_Background,
            ConsoleColor.DarkGreen   => AnsiByte.Green_Background,
            ConsoleColor.DarkCyan    => AnsiByte.Cyan_Background,
            ConsoleColor.DarkRed     => AnsiByte.Red_Background,
            ConsoleColor.DarkMagenta => AnsiByte.Magenta_Background,
            ConsoleColor.DarkYellow  => AnsiByte.Yellow_Background,
            ConsoleColor.Gray        => AnsiByte.White_Background,
            ConsoleColor.DarkGray    => AnsiByte.Black_Bright_Background,
            ConsoleColor.Blue        => AnsiByte.Blue_Bright_Background,
            ConsoleColor.Green       => AnsiByte.Green_Bright_Background,
            ConsoleColor.Cyan        => AnsiByte.Cyan_Bright_Background,
            ConsoleColor.Red         => AnsiByte.Red_Bright_Background,
            ConsoleColor.Magenta     => AnsiByte.Magenta_Bright_Background,
            ConsoleColor.Yellow      => AnsiByte.Yellow_Bright_Background,
            ConsoleColor.White       => AnsiByte.White_Bright_Background,
            _                        => throw new ArgumentOutOfRangeException(nameof(consoleColor), consoleColor, null)
        };
    }

    public static string Foreground(ConsoleColor consoleColor) {
        return consoleColor switch {
            ConsoleColor.Black       => Black.Fg,
            ConsoleColor.DarkBlue    => Blue.Fg,
            ConsoleColor.DarkGreen   => Green.Fg,
            ConsoleColor.DarkCyan    => Cyan.Fg,
            ConsoleColor.DarkRed     => Red.Fg,
            ConsoleColor.DarkMagenta => Magenta.Fg,
            ConsoleColor.DarkYellow  => Yellow.Fg,
            ConsoleColor.Gray        => White.Fg,
            ConsoleColor.DarkGray    => Black.Bright.Fg,
            ConsoleColor.Blue        => Blue.Bright.Fg,
            ConsoleColor.Green       => Green.Bright.Fg,
            ConsoleColor.Cyan        => Cyan.Bright.Fg,
            ConsoleColor.Red         => Red.Bright.Fg,
            ConsoleColor.Magenta     => Magenta.Bright.Fg,
            ConsoleColor.Yellow      => Yellow.Bright.Fg,
            ConsoleColor.White       => White.Bright.Fg,
            _                        => throw new ArgumentOutOfRangeException(nameof(consoleColor), consoleColor, null)
        };
    }

    public static string Background(ConsoleColor consoleColor) {
        return consoleColor switch {
            ConsoleColor.Black       => Black.Bg,
            ConsoleColor.DarkBlue    => Blue.Bg,
            ConsoleColor.DarkGreen   => Green.Bg,
            ConsoleColor.DarkCyan    => Cyan.Bg,
            ConsoleColor.DarkRed     => Red.Bg,
            ConsoleColor.DarkMagenta => Magenta.Bg,
            ConsoleColor.DarkYellow  => Yellow.Bg,
            ConsoleColor.Gray        => White.Bg,
            ConsoleColor.DarkGray    => Black.Bright.Bg,
            ConsoleColor.Blue        => Blue.Bright.Bg,
            ConsoleColor.Green       => Green.Bright.Bg,
            ConsoleColor.Cyan        => Cyan.Bright.Bg,
            ConsoleColor.Red         => Red.Bright.Bg,
            ConsoleColor.Magenta     => Magenta.Bright.Bg,
            ConsoleColor.Yellow      => Yellow.Bright.Bg,
            ConsoleColor.White       => White.Bright.Bg,
            _                        => throw new ArgumentOutOfRangeException(nameof(consoleColor), consoleColor, null)
        };
    }

    internal static string RenderSequence(byte a)                 => $"{csi}{a}m";
    internal static string RenderSequence(byte a, byte b)         => $"{csi}{a};{b}m";
    internal static string RenderSequence(byte a, byte b, byte c) => $"{csi}{a};{b};{c}m";

    internal static string RenderSequence(IEnumerable<byte> bytes) {
        bool anyBytes = false;
        var  sb       = new StringBuilder();
        sb.Append(csi);

        using var erator = bytes.GetEnumerator();
        while (erator.MoveNext()) {
            if (anyBytes) {
                sb.Append(';');
            }
            else {
                anyBytes = true;
            }

            sb.Append(erator.Current);
        }

        return anyBytes ? sb.Append('m').ToString() : "";
    }

    internal static string RenderSequence(ReadOnlySpan<byte> bytes) {
        static int MaxCharCount(int byteCount) {
            var fromSeparators = byteCount - 1;
            var fromBytes      = byteCount * 3;
            return csi.Length + 1 + fromSeparators + fromBytes;
        }

        if (bytes is not { Length: > 0 }) {
            return Ansi.Reset.All;
        }

        var cap = MaxCharCount(bytes.Length);
        var sb  = new StringBuilder(cap, cap);
        sb.Append(Ansi.csi);
        for (int i = 0; i < bytes.Length; i++) {
            if (i > 0) {
                sb.Append(';');
            }

            sb.Append(bytes[i]);
        }

        sb.Append('m');
        return sb.ToString();
    }

    internal static IEnumerable<byte> Enumerate_Foreground_On(this  Spectre.Console.Color color) => color.EnumerateAnsiBytes(false, true);
    internal static IEnumerable<byte> Enumerate_Foreground_Off(this Spectre.Console.Color color) => color.EnumerateAnsiBytes(false, false);
    internal static IEnumerable<byte> Enumerate_Background_On(this  Spectre.Console.Color color) => color.EnumerateAnsiBytes(true,  true);
    internal static IEnumerable<byte> Enumerate_Background_Off(this Spectre.Console.Color color) => color.EnumerateAnsiBytes(true,  false);

    internal static IEnumerable<byte> EnumerateAnsiBytes(this Spectre.Console.Color color, bool background = false, bool on = true) {
        if (on == false) {
            yield return background switch {
                true  => (byte)AnsiByte.Default_Background,
                false => (byte)AnsiByte.Default_Foreground,
            };
            yield break;
        }

        yield return background switch {
            true  => (byte)AnsiByte.RGB_Background,
            false => (byte)AnsiByte.RGB_Foreground,
        };

        yield return 2;
        yield return color.R;
        yield return color.G;
        yield return color.B;
    }
}