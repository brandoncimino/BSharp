using FowlFever.BSharp.Enums;

namespace FowlFever.Clerical;

/// <summary>
/// Enumerates the <i>reasonable</i> <a href="https://en.wikipedia.org/wiki/Path_(computing)#Representations_of_paths_by_operating_system_and_shell">path directory separators</a>.
/// </summary>
public enum DirectorySeparator {
    /// <summary>
    /// The "slash" - <c>/</c> - aka <a href="https://en.wikipedia.org/wiki/Path_(computing)#Unix_style">"Unix style"</a>.
    /// </summary>
    /// <remarks>
    /// Equivalent to the character returned by <see cref="Path.AltDirectorySeparatorChar"/>, which, despite the docs claiming to be platform-dependent, is always <c>/</c>.
    /// </remarks>
    /// <seealso cref="Path.AltDirectorySeparatorChar"/>
    Universal,
    /// <summary>
    /// The obnoxious "backslash" - \.
    /// </summary>
    Windows,
    /// <summary>
    /// Either <see cref="Universal"/> or <see cref="Windows"/>, depending on the current <see cref="OperatingSystem"/>.
    /// </summary>
    /// <remarks>
    /// Corresponds to the character returned by <see cref="Path.DirectorySeparatorChar"/>.
    /// </remarks>
    PlatformDependent
}

public static class DirectorySeparatorExtensions {
    /// <param name="separator">this <see cref="DirectorySeparator"/></param>
    /// <returns>the <see cref="char"/> that corresponds to this <see cref="DirectorySeparator"/> - either <c>/</c>, <c>\</c>, or <see cref="Path.DirectorySeparatorChar"/></returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unknown <see cref="DirectorySeparator"/> is provided</exception>
    public static char ToChar(this DirectorySeparator separator) {
        return separator switch {
            DirectorySeparator.Universal         => '/',
            DirectorySeparator.Windows           => '\\',
            DirectorySeparator.PlatformDependent => Path.DirectorySeparatorChar,
            _                                    => throw BEnum.InvalidEnumArgumentException(nameof(separator), separator),
        };
    }

    /// <param name="separator">this <see cref="DirectorySeparator"/></param>
    /// <returns>the <see cref="string"/> representation of <see cref="ToChar"/></returns>
    public static string ToCharString(this DirectorySeparator separator) => separator.ToChar().ToString();
}