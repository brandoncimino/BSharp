namespace FowlFever.BSharp.Strings;

public static partial class Ansi {
    /// <summary>
    /// Enumerates the <a href="https://en.wikipedia.org/wiki/ANSI_escape_code#SGR_(Select_Graphic_Rendition)_parameters">SGR (Select Graphic Rendition)</a> bytes.
    /// </summary>
    internal enum AnsiByte : byte {
        /// 0	Reset or normal	All attributes off
        Reset = 0,
        /// 1	Bold or increased intensity	As with faint, the color change is a PC (SCO / CGA) invention.[38][better source needed]
        /// <seealso cref="DoubleUnderlineMaybe"/>
        /// <seealso cref="NormalIntensity"/>
        Bold = 1,
        /// 2	Faint, decreased intensity, or dim	May be implemented as a light font weight like bold.[39]
        /// <seealso cref="NormalIntensity"/>
        Faint = 2,
        /// 3	Italic	Not widely supported. Sometimes treated as inverse or blink.[38]
        /// <seealso cref="Italic_Off"/>
        Italic = 3,
        /// 4	Underline	Style extensions exist for Kitty, VTE, mintty and iTerm2.[40][41]
        /// <seealso cref="Underline_Off"/>
        Underline = 4,
        /// 5	Slow blink	Sets blinking to less than 150 times per minute
        /// <seealso cref="Blinking_Off"/>
        SlowBlink = 5,
        /// 6	Rapid blink	MS-DOS ANSI.SYS, 150+ per minute; not widely supported
        /// <seealso cref="Blinking_Off"/>
        RapidBlink = 6,
        /// 7	Reverse video or invert	Swap foreground and background colors; inconsistent emulation[42]
        /// <seealso cref="ReverseGrounds_Off"/>
        ReverseGrounds = 7,
        /// 8	Conceal or hide	Not widely supported.
        /// <see cref="Conceal_Off"/>
        Conceal = 8,
        /// 9	Crossed-out, or strike	Characters legible but marked as if for deletion. Not supported in Terminal.app
        /// <see cref="Strikethrough_Off"/>
        Strikethrough = 9,
        /// 10	Primary (default) font	
        PrimaryFont = 10,
        /// 11–19	Alternative font	Select alternative font n − 10
        AltFont1 = 11,
        AltFont2 = 12,
        AltFont3 = 13,
        AltFont4 = 14,
        AltFont5 = 15,
        AltFont6 = 16,
        AltFont7 = 17,
        AltFont8 = 18,
        AltFont9 = 19,
        /// 20	Fraktur (Gothic)	Rarely supported
        FrakturGothic = 20,
        /// 21	Doubly underlined; or: not bold	Double-underline per ECMA-48,[5]: 8.3.117  but instead disables bold intensity on several terminals, including in the Linux kernel's console before version 4.17.[43]
        DoubleUnderlineMaybe = 21,
        /// 22	Normal intensity	Neither bold nor faint; color changes where intensity is implemented as such.
        /// <see cref="Bold"/>
        /// <seealso cref="Faint"/>
        NormalIntensity = 22,
        /// 23	Neither italic, nor blackletter 
        /// <seealso cref="Italic"/>
        /// <remarks>Who knows what "blackletter" is supposed to mean</remarks>
        Italic_Off = 23,
        /// 24	Not underlined	Neither singly nor doubly underlined
        /// <see cref="Underline"/>
        Underline_Off = 24,
        /// 25	Not blinking	Turn blinking off
        /// <see cref="SlowBlink"/>
        /// <see cref="RapidBlink"/>
        Blinking_Off = 25,
        /// 26	Proportional spacing	ITU T.61 and T.416, not known to be used on terminals
        ProportionalSpacing = 26,
        /// 27	Not reversed
        /// <see cref="ReverseGrounds"/>
        ReverseGrounds_Off = 27,
        /// 28	Reveal	Not concealed
        /// <see cref="Conceal"/>
        Conceal_Off = 28,
        /// 29	Not crossed out
        /// <see cref="Strikethrough"/>	
        Strikethrough_Off = 29,
        /// 30–37	Set foreground color	
        Black = 30,
        Red     = 31,
        Green   = 32,
        Yellow  = 33,
        Blue    = 34,
        Magenta = 35,
        Cyan    = 36,
        White   = 37,
        /// 38	Set foreground color	Next arguments are 5;n or 2;r;g;b
        RGB_Foreground = 38,
        /// 39	Default foreground color	Implementation defined (according to standard)
        Default_Foreground = 39,
        /// 40–47	Set background color	
        Black_Background = 40,
        Red_Background     = 41,
        Green_Background   = 42,
        Yellow_Background  = 43,
        Blue_Background    = 44,
        Magenta_Background = 45,
        Cyan_Background    = 46,
        White_Background   = 47,
        /// 48	Set background color	Next arguments are 5;n or 2;r;g;b
        RGB_Background = 48,
        /// 49	Default background color	Implementation defined (according to standard)
        Default_Background = 49,
        /// 50	Disable proportional spacing	T.61 and T.416
        DisableProportionalSpacing = 50,
        /// 51	Framed	Implemented as "emoji variation selector" in mintty.[44]
        Framed = 51,
        /// 52	Encircled
        Encircled = 52,
        /// 53	Overlined	Not supported in Terminal.app
        Overlined = 53,
        /// 54	Neither framed nor encircled	
        FramedEncircled_Off = 54,
        /// 55	Not overlined	
        Overlined_Off = 55,
        // _UNUSED_56 = 56,
        // _UNUSED_57 = 57,
        /// 58	Set underline color	Not in standard; implemented in Kitty, VTE, mintty, and iTerm2.[40][41] Next arguments are 5;n or 2;r;g;b.
        RGB_Underline = 58,
        /// 59	Default underline color	Not in standard; implemented in Kitty, VTE, mintty, and iTerm2.[40][41]
        Default_Underline = 59,
        /// 60	Ideogram underline or right side line	Rarely supported
        Ideogram_Underline = 60,
        /// 61	Ideogram double underline, or double line on the right side
        Ideogram_DoubleUnderline = 61,
        /// 62	Ideogram overline or left side line
        Ideogram_Overline = 62,
        /// 63	Ideogram double overline, or double line on the left side
        Ideogram_DoubleOverline = 63,
        /// 64	Ideogram stress marking
        Ideogram_StressMarking = 64,
        /// 65	No ideogram attributes	Reset the effects of all of 60–64
        Ideogram_Off = 65,
        /// 73	Superscript	Implemented only in mintty[44]
        Superscript = 73,
        /// 74	Subscript
        Subscript = 74,
        /// 75	Neither superscript nor subscript
        SubSuperScript_Off = 75,
        /// 90–97	Set bright foreground color	Not in standard; originally implemented by aixterm[29]
        Black_Bright = 90,
        Red_Bright     = 91,
        Green_Bright   = 92,
        Yellow_Bright  = 93,
        Blue_Bright    = 94,
        Magenta_Bright = 95,
        Cyan_Bright    = 96,
        White_Bright   = 97,
        /// 100–107	Set bright background color
        Black_Bright_Background = 100,
        Red_Bright_Background     = 101,
        Green_Bright_Background   = 102,
        Yellow_Bright_Background  = 103,
        Blue_Bright_Background    = 104,
        Magenta_Bright_Background = 105,
        Cyan_Bright_Background    = 106,
        White_Bright_Background   = 107,
    }
}