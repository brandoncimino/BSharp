using System;
using System.Linq;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Strings.Enums;

/// <summary>
/// Indicates if and how a string should be <see cref="StringMirroringExtensions.Mirror(string)"/>ed.
/// </summary>
public enum StringMirroring { None, Mirrored, }

public static class StringMirroringExtensions {
    public static string ApplyTo(this StringMirroring mirroring, string forwardString) {
        return mirroring switch {
            StringMirroring.None     => forwardString,
            StringMirroring.Mirrored => forwardString.Mirror(),
            _                        => throw BEnum.UnhandledSwitch(mirroring),
        };
    }

    public static OneLine ApplyTo(this StringMirroring mirroring, OneLine forwardLine) => mirroring switch {
        StringMirroring.None     => forwardLine,
        StringMirroring.Mirrored => OneLine.CreateRisky(forwardLine.Reverse()),
        _                        => throw BEnum.UnhandledSwitch(mirroring),
    };

    /// <summary>
    /// <see cref="Array.Reverse(System.Array)"/>s the order of each character in this <see cref="string"/>.
    /// <p/>
    /// TODO: This can probably be made more efficient by using <see cref="MemoryExtensions.AsSpan(string?)"/>
    /// TODO: Handle certain sequences that shouldn't be reversed, like composite Emoji (📎 there's gotta be a library built to handle Emoji, right?) // Update: There IS, it's built-in as "StringInfo" which I've expanded with <see cref="TextElementString"/>
    /// </summary>
    /// <remarks>
    /// This was named "Backwards" to avoid ambiguity with <see cref="Enumerable.Reverse{TSource}"/>.
    /// </remarks>
    /// <param name="str">this <see cref="string"/></param>
    /// <returns>this <see cref="string"/>...but backwards</returns>
    public static string Mirror(this string str) {
        var chars = str.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static OneLine Mirror(this OneLine line) => OneLine.CreateRisky(line.Reverse());
}