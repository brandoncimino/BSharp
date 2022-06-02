using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Tabler;

[PublicAPI]
public readonly record struct Cell : IPrettifiable {
    public Optional<object?> Value    { get; }
    public bool              IsHeader { get; }

    public Cell(object? value, bool isHeader = false) {
        // this prevents nesting of cells, i.e. Cell(Cell(5)) => Cell(5)
        Value    = value is Cell cell ? cell.Value : value;
        IsHeader = isHeader;
    }

    public string Prettify(PrettificationSettings? settings = default) {
        settings = settings.Resolve();
        settings = settings with { PreferredLineStyle = LineStyle.Single };
        return Value.Prettify(settings)
                    .Align(IsHeader ? settings.TableHeaderAlignment : default);
    }
}