using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp;

public sealed record BlogList : IDisposable, IRenderable {
    /// <summary>
    /// The <see cref="Strings.Spectral.Palette"/> used when the <see cref="BlogList"/>.<see cref="Palette"/> isn't specified.
    /// </summary>
    public static Func<Palette> DefaultPalette { get; set; } = () => Palette.Fallback;

    private readonly List<(IRenderable, IRenderable)> _rows = new();
    private readonly string?                          _title;
    private readonly Palette?                         _palette;

    public Palette Palette {
        get => _palette.OrFallback(DefaultPalette.Invoke(), Palette.Fallback);
        init => _palette = value;
    }

    public BlogList([CallerMemberName] string title = "") {
        _title = title;
    }

    private IRenderable? GetTitle() {
        return _title?.EscapeSpectre(Palette.Titles);
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

        var grid2 = new Grid().AddColumn()
                              .AddRow(title)
                              .AddRow(
                                  new HRule {
                                      Style = Palette.Borders
                                  }
                              )
                              .AddRow(grid);

        return new Panel(grid2) {
            Border      = BoxBorder.Rounded,
            BorderStyle = Palette.Borders
        };
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