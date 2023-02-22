using FowlFever.BSharp.Strings.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Settings;

/// <summary>
/// Settings that inform <a href="https://en.wikipedia.org/wiki/Typographic_alignment">typographic alignment</a>.
/// </summary>
[PublicAPI]
public record FillerSettings : Settings {
    /// <summary>
    /// Stores the <b>global default</b> <see cref="FillerSettings"/>.
    /// </summary>
    /// <seealso cref="SettingsExtensions.Resolve(FowlFever.BSharp.Strings.Settings.FillerSettings?)"/>
    public static FillerSettings Default { get; set; } = new();

    /// <summary>
    /// The maximum length of a string before it triggers the <see cref="OverflowStyle"/>
    /// </summary>
    [NonNegativeValue]
    public int LineLengthLimit { get; init; } = 70;
    /// <summary>
    /// See <a href="https://en.wikipedia.org/wiki/Typographic_alignment">typographic alignment</a>
    /// </summary>
    public StringAlignment Alignment { get; init; } = StringAlignment.Left;
    /// <summary>
    /// How to handle strings whose length exceeds the <see cref="LineLengthLimit"/>
    /// </summary>
    public OverflowStyle OverflowStyle { get; init; } = OverflowStyle.Overflow;
    /// <summary>
    /// The additive string used to achieve the desired <see cref="Alignment"/>
    /// </summary>
    public OneLine PadString { get; init; } = OneLine.Empty;
    /// <summary>
    /// Controls whether filling applied to the left (start) of the string will be <see cref="StringMirroringExtensions.Mirror(string)"/>:
    /// <code><![CDATA[
    /// StringMirroring.None        ---* str *---
    /// StringMirroring.Mirrored    *--- str *---
    /// ]]></code>
    /// </summary>
    public StringMirroring MirrorPadding { get; init; } = StringMirroring.Mirrored;
    /// <summary>
    /// Used when splitting filling between both the left and right sides of the string
    /// </summary>
    public RoundingMode LeftSideRounding { get; init; } = default;
    /// <summary>
    /// Replaces the last characters of a <see cref="Enums.OverflowStyle.Truncate"/>d string
    /// </summary>
    public OneLine TruncateTrail { get; init; } = OneLine.Ellipsis;

    public static implicit operator FillerSettings(StringAlignment          alignment)              => Default with { Alignment = alignment };
    public static implicit operator FillerSettings(OverflowStyle            overflowStyle)          => Default with { OverflowStyle = overflowStyle };
    public static implicit operator FillerSettings?(PrettificationSettings? prettificationSettings) => prettificationSettings?.FillerSettings;
}