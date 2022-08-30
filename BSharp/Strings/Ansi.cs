using System;
using System.Diagnostics.CodeAnalysis;

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
public static class Ansi {
    private const string _fg             = "3";
    private const string _bg             = "4";
    private const string _bright_fg      = "9";
    private const string _bright_bg      = "10";
    private const string _decoration_off = "2";
    private const string csi             = "\x1b[";

    public static class Reset {
        public const string All = $"{csi}m";
        public const string Fg  = $"{csi}39m";
        public const string Bg  = $"{csi}49m";
    }

    #region Colors

    public static class Black {
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
    ///     - <see cref="System.ConsoleColor.Gray"/>
    ///     - <see cref="System.ConsoleColor.DarkGray"/>
    ///     - <see cref="System.ConsoleColor.White"/> 
    /// </summary>
    public static class Gray {
        public const string       Fg           = Black.Bright.Fg;
        public const string       Bg           = Black.Bright.Bg;
        public const ConsoleColor ConsoleColor = System.ConsoleColor.Gray;

        public static class Dark {
            public const string Fg = Black.Fg;
            public const string Bg = Black.Bg;
            //TODO: this is ambiguous between ConsoleColor.Black and ConsoleColor.DarkGray 🤔
            // public const ConsoleColor ConsoleColor = System.ConsoleColor.Black;
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
}