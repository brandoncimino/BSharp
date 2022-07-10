using System.Runtime.CompilerServices;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings.Settings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Tabler;

[PublicAPI]
public readonly record struct Cell : IPrettifiable {
    public Optional<object?> Value { get; }
    public Row.RowStyle      Style { get; init; }
    public string?           Label { get; init; }

    public Cell(object? value, Row.RowStyle style = Row.RowStyle.Normal, [CallerArgumentExpression("value")] string? label = default) {
        // this prevents nesting of cells, i.e. Cell(Cell(5)) => Cell(5)
        Value = value is Cell cell ? cell.Value : value;
        Style = style;
        Label = label;
    }

    public string Prettify(PrettificationSettings? settings = default) {
        settings = settings.Resolve();
        settings = settings with { PreferredLineStyle = LineStyle.Single };
        return Value.Select(it => it.Prettify(settings))
                    .OrElse(settings.TableSettings.EmptyCellPlaceholder)
                    .Align(
                        Style switch {
                            Row.RowStyle.Header => settings.TableSettings.HeaderAlignment,
                            Row.RowStyle.Normal => settings.FillerSettings.Alignment,
                            _                   => throw BEnum.UnhandledSwitch(Style),
                        }
                    );
    }
}