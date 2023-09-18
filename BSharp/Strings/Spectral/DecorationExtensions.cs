using System.Collections.Generic;

using FowlFever.BSharp.Enums;

using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

public static class DecorationExtensions {
    internal static Ansi.AnsiByte OnByte(this Decoration decoration) {
        return decoration switch {
            Decoration.None          => Ansi.AnsiByte.Reset,
            Decoration.Bold          => Ansi.AnsiByte.Bold,
            Decoration.Dim           => Ansi.AnsiByte.Faint,
            Decoration.Italic        => Ansi.AnsiByte.Italic,
            Decoration.Underline     => Ansi.AnsiByte.Underline,
            Decoration.Invert        => Ansi.AnsiByte.ReverseGrounds,
            Decoration.Conceal       => Ansi.AnsiByte.Conceal,
            Decoration.SlowBlink     => Ansi.AnsiByte.SlowBlink,
            Decoration.RapidBlink    => Ansi.AnsiByte.RapidBlink,
            Decoration.Strikethrough => Ansi.AnsiByte.Strikethrough,
            _                        => throw new ArgumentOutOfRangeException(nameof(decoration), decoration, null)
        };
    }

    internal static Ansi.AnsiByte OffByte(this Decoration decoration) {
        return decoration switch {
            Decoration.None          => Ansi.AnsiByte.Reset,
            Decoration.Bold          => Ansi.AnsiByte.NormalIntensity,
            Decoration.Dim           => Ansi.AnsiByte.NormalIntensity,
            Decoration.Italic        => Ansi.AnsiByte.Italic_Off,
            Decoration.Underline     => Ansi.AnsiByte.Underline_Off,
            Decoration.Invert        => Ansi.AnsiByte.ReverseGrounds_Off,
            Decoration.Conceal       => Ansi.AnsiByte.Conceal_Off,
            Decoration.SlowBlink     => Ansi.AnsiByte.Blinking_Off,
            Decoration.RapidBlink    => Ansi.AnsiByte.Blinking_Off,
            Decoration.Strikethrough => Ansi.AnsiByte.Strikethrough_Off,
            _                        => throw new ArgumentOutOfRangeException(nameof(decoration), decoration, null)
        };
    }

    internal static IEnumerable<byte> EnumerateBytes_On(this Decoration decoration) {
        return decoration.EachFlag()
                         .Select(static it => (byte)it.OnByte());
    }

    internal static IEnumerable<byte> EnumerateBytes_Off(this Decoration decoration) {
        return decoration.EachFlag()
                         .Select(static it => (byte)it.OffByte());
    }
}