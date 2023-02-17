using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings.Settings;

[PublicAPI]
public record PrettificationSettings : Settings {
    /// <summary>
    /// Stores the <b>global default</b> <see cref="PrettificationSettings"/>.
    /// </summary>
    /// <seealso cref="SettingsExtensions.Resolve(FowlFever.BSharp.Strings.Settings.PrettificationSettings?)"/>
    public static PrettificationSettings Default { get; set; } = new();

    /// <summary>
    /// TODO: probably just move this into <see cref="FillerSettings"/>
    /// </summary>
    [NonNegativeValue]
    public int LineLengthLimit {
        get => FillerSettings.LineLengthLimit;
        init => FillerSettings = FillerSettings with { LineLengthLimit = value }; // this is pretty sketchy...
    }

    public TableSettings  TableSettings      { get; init; } = new();
    public string         NullPlaceholder    { get; init; } = "⛔";
    public LineStyle      PreferredLineStyle { get; init; } = LineStyle.Dynamic;
    public TypeNameStyle  TypeLabelStyle     { get; init; } = TypeNameStyle.Full;
    public TypeNameStyle  EnumLabelStyle     { get; init; } = TypeNameStyle.None;
    public HeaderStyle    HeaderStyle        { get; init; } = HeaderStyle.None;
    public FillerSettings FillerSettings     { get; init; } = new();

    public ITraceWriter? TraceWriter { get; init; } = null;

    public static implicit operator PrettificationSettings(LineStyle        lineStyle)      => Default with { PreferredLineStyle = lineStyle };
    public static implicit operator PrettificationSettings(TypeNameStyle    typeLabelStyle) => Default with { TypeLabelStyle = typeLabelStyle };
    public static implicit operator PrettificationSettings(HeaderStyle      headerStyle)    => Default with { HeaderStyle = headerStyle };
    public static implicit operator PrettificationSettings?(FillerSettings? fillerSettings) => fillerSettings == null ? null : Default with { FillerSettings = fillerSettings };

    public IEnumerable<int> GetAutoColumnWidths() {
        return TableSettings.GetAutoColumnWidths(LineLengthLimit);
    }
}