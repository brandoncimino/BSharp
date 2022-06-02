using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Tabler;

[PublicAPI]
public readonly record struct Cell : IPrettifiable {
    public Optional<object?> Value { get; }

    public Cell(object? value) {
        // this prevents nesting of cells, i.e. Cell(Cell(5)) => Cell(5)
        Value = value is Cell cell ? cell.Value : value;
    }

    public string Prettify(PrettificationSettings? settings = default) {
        return Value.Prettify(settings.Resolve() with { PreferredLineStyle = LineStyle.Single });
    }
}