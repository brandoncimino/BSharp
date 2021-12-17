using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings;

public record PrettificationSettings {
    public static PrettificationSettings Default { get; set; } = new PrettificationSettings();

    public int           LineLengthLimit      { get; init; } = 70;
    public string        TableHeaderSeparator { get; init; } = "-";
    public string        TableColumnSeparator { get; init; } = " ";
    public string        NullPlaceholder      { get; init; } = "⛔";
    public LineStyle     PreferredLineStyle   { get; init; } = LineStyle.Dynamic;
    public TypeNameStyle TypeLabelStyle       { get; init; } = TypeNameStyle.Full;
    public TypeNameStyle EnumLabelStyle       { get; init; } = TypeNameStyle.None;
    public HeaderStyle   HeaderStyle          { get; init; } = HeaderStyle.None;

    public ITraceWriter? TraceWriter { get; init; } = null;

    public static implicit operator PrettificationSettings(LineStyle lineStyle) {
        return Default with { PreferredLineStyle = lineStyle };
    }


    public static implicit operator PrettificationSettings(TypeNameStyle typeLabelStyle) {
        return Default with { TypeLabelStyle = typeLabelStyle };
    }


    public static implicit operator PrettificationSettings(HeaderStyle headerStyle) {
        return Default with { HeaderStyle = headerStyle };
    }
}