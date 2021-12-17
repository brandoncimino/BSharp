using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Tabler;

[PublicAPI]
public record Cell : IPrettifiable {
    private          PrettificationSettings? Settings { get; }
    public           object?                 Value    { get; }
    private readonly Lazy<string>            PrettyString;

    public Cell(object value, PrettificationSettings? settings = default) {
        // this prevents nesting of cells, i.e. Cell(Cell(5)) => Cell(5)
        Value = value is Cell cell ? cell.Value : value;
        //TODO: Support multiline cells...hoo boy
        Settings     = (settings ?? PrettificationSettings.Default) with { PreferredLineStyle = LineStyle.Single };
        PrettyString = new Lazy<string>(() => Value.Prettify(Settings));
    }

    public string Prettify(PrettificationSettings? settings = default) {
        return settings == null ? PrettyString.Value : Value.Prettify(settings with { PreferredLineStyle = LineStyle.Single });
    }
}