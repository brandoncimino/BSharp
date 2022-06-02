using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Tabler;

[PublicAPI]
public readonly record struct Row : IReadOnlyList<Cell>, IPrettifiable {
    public enum RowStyle { Normal, Header, }

    private readonly IReadOnlyList<Cell> Cells;
    public           RowStyle            Style { get; init; }
    public           int                 Count => Cells.Count;
    public Cell this[int index] => Cells[index];

    public Row(IEnumerable<Cell> cells, RowStyle style = default) {
        Cells = cells.Select(it => it with { Style = style }).ToList();
        Style = style;
    }

    public Row(IEnumerable<object?> cells, RowStyle style = default) : this(cells.Select(it => new Cell(it)), style) { }

    public Row(params Cell[]    cells) : this(cells.AsEnumerable()) { }
    public Row(params object?[] cells) : this(cells.AsEnumerable()) { }

    #region Factories

    public static Row Of(IEnumerable<object?> cells) {
        return new Row(cells);
    }

    public static Row Of(params object?[] cells) {
        return new Row(cells);
    }

    public static Row OfHeaders(IEnumerable<object> cells) {
        return new Row(cells, RowStyle.Header);
    }

    public static Row OfHeaders(params object[] cells) {
        return new Row(cells, RowStyle.Header);
    }

    public static Row Of<TKey, TVal>(KeyValuePair<TKey, TVal> kvp) {
        return Of(kvp.Key, kvp.Value);
    }

    #endregion

    public IEnumerable<int> GetWidths(PrettificationSettings? settings = default) {
        return Cells.Select(it => it.Prettify(settings).LongestLine());
    }

    public string Prettify(PrettificationSettings? settings = default) {
        return Render(GetWidths(settings), settings);
    }

    public string Render(IEnumerable<int> widths, PrettificationSettings? settings = default) {
        var widthList = widths.AsList();
        if (widthList.Count != Cells.Count) {
            throw new ArgumentException($"Provided {widthList.Count} widths for {Cells.Count} cells!");
        }

        return Cells.Select(it => it.Prettify(settings))
                    .JoinString(settings.Resolve().TableColumnSeparator);
    }

    public IEnumerator<Cell> GetEnumerator() {
        return Cells.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)Cells).GetEnumerator();
    }
}