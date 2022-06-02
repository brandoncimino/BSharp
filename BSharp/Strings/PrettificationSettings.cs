using System;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings.Enums;

using JetBrains.Annotations;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings;

public abstract record Settings { }

[PublicAPI]
public record PrettificationSettings : Settings {
    /// <summary>
    /// Stores the <b>global default</b> <see cref="PrettificationSettings"/>.
    /// </summary>
    /// <seealso cref="SettingsExtensions.Resolve(FowlFever.BSharp.Strings.PrettificationSettings?)"/>
    public static PrettificationSettings Default { get; set; } = new PrettificationSettings();

    /// <summary>
    /// TODO: probably just move this into <see cref="FillerSettings"/>
    /// </summary>
    [NonNegativeValue]
    public int LineLengthLimit {
        get => FillerSettings.LineLengthLimit;
        init => FillerSettings = FillerSettings with { LineLengthLimit = value }; // this is pretty sketchy...
    }

    public string          TableHeaderSeparator { get; init; } = "-";
    public string          TableColumnSeparator { get; init; } = " ";
    public StringAlignment TableHeaderAlignment { get; init; } = StringAlignment.Center;
    public string          NullPlaceholder      { get; init; } = "⛔";
    public LineStyle       PreferredLineStyle   { get; init; } = LineStyle.Dynamic;
    public TypeNameStyle   TypeLabelStyle       { get; init; } = TypeNameStyle.Full;
    public TypeNameStyle   EnumLabelStyle       { get; init; } = TypeNameStyle.None;
    public HeaderStyle     HeaderStyle          { get; init; } = HeaderStyle.None;
    public FillerSettings  FillerSettings       { get; init; } = new();

    public ITraceWriter? TraceWriter { get; init; } = null;

    public static implicit operator PrettificationSettings(LineStyle      lineStyle)      => Default with { PreferredLineStyle = lineStyle };
    public static implicit operator PrettificationSettings(TypeNameStyle  typeLabelStyle) => Default with { TypeLabelStyle = typeLabelStyle };
    public static implicit operator PrettificationSettings(HeaderStyle    headerStyle)    => Default with { HeaderStyle = headerStyle };
    public static implicit operator PrettificationSettings(FillerSettings fillerSettings) => Default with { FillerSettings = fillerSettings };
}

/// <summary>
/// TODO: move stuff like <see cref="PrettificationSettings.TableColumnSeparator"/> into here
/// </summary>
public record TableSettings {
    public TableSettings() => throw new NotImplementedException();
}

/// <summary>
/// Settings that inform <a href="https://en.wikipedia.org/wiki/Typographic_alignment">typographic alignment</a>.
/// </summary>
[PublicAPI]
public record FillerSettings : Settings {
    /// <summary>
    /// Stores the <b>global default</b> <see cref="FillerSettings"/>.
    /// </summary>
    /// <seealso cref="SettingsExtensions.Resolve(FowlFever.BSharp.Strings.FillerSettings?)"/>
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
    public string PadString { get; init; } = " ";
    /// <summary>
    /// Controls whether filling applied to the left (start) of the string will be <see cref="StringUtils.Mirror"/>:
    /// <code><![CDATA[
    /// StringMirroring.None        ---* str *---
    /// StringMirroring.Mirrored    *--- str *---
    /// ]]></code>
    /// </summary>
    public StringMirroring MirrorPadding { get; init; } = StringMirroring.Mirrored;
    /// <summary>
    /// Used when splitting filling between both the left and right sides of the string
    /// </summary>
    public RoundingDirection LeftSideRounding { get; init; } = default;
    /// <summary>
    /// Replaces the last characters of a <see cref="Enums.OverflowStyle.Truncate"/>d string
    /// </summary>
    public string TruncateTrail { get; init; } = "…";

    public static implicit operator FillerSettings(StringAlignment        alignment)              => Default with { Alignment = alignment };
    public static implicit operator FillerSettings(OverflowStyle          overflowStyle)          => Default with { OverflowStyle = overflowStyle };
    public static implicit operator FillerSettings(PrettificationSettings prettificationSettings) => prettificationSettings.FillerSettings;
}

public static class SettingsExtensions {
    public static PrettificationSettings Resolve(this PrettificationSettings? settings) => settings ?? PrettificationSettings.Default;
    public static FillerSettings         Resolve(this FillerSettings?         settings) => settings ?? FillerSettings.Default;
}