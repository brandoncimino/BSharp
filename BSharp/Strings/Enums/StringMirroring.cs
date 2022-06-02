using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Strings.Enums;

/// <summary>
/// Indicates if and how a string should be <see cref="StringUtils.Mirror"/>ed.
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
}