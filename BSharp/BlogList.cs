using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp;

public sealed record BlogList : IDisposable, IRenderable {
    private readonly List<(IRenderable, IRenderable)> _rows = new();
    private readonly string?                          _title;
    public           Palette                          Palette { get; init; }

    public BlogList([CallerMemberName] string title = "") {
        _title = title;
    }

    public IRenderable? GetTitle() {
        return _title?.EscapeSpectre(Palette.Titles.Decorate(Decoration.Underline));
    }

    private IRenderable ToRenderable() {
        var grid = new Grid() {
            Expand = false
        };
        grid.AddColumns(2);

        foreach (var r in _rows) {
            grid.AddRow(r.Item1, r.Item2);
        }

        var title = GetTitle();
        if (title == null) {
            return grid;
        }

        return new Rows(title, grid) { Expand = false };
    }

    public BlogList Post<T, TLabel>(T? value, TLabel? label, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(value, label, palette, _expression));
        return this;
    }

    public BlogList Post<T>(T? value, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(value, palette, _expression));
        return this;
    }

    public BlogList Post<T, TLabel>(ReadOnlySpan<T> span, TLabel? label = default, Palette? palette = default, [CallerArgumentExpression("span")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(span, label, palette, _expression));
        return this;
    }

    public BlogList Post<T>(ReadOnlySpan<T> span, Palette? palette = default, [CallerArgumentExpression("span")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(span, palette, _expression));
        return this;
    }

    public void Dispose() {
        Brandon.Render(this);
    }

    public Measurement          Measure(RenderContext context, int maxWidth) => ToRenderable().GetMeasurement(context, maxWidth);
    public IEnumerable<Segment> Render(RenderContext  context, int maxWidth) => ToRenderable().GetSegments(context, maxWidth);
}