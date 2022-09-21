using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp;

public sealed record Blog : IDisposable, IRenderable {
    /// <summary>
    /// The <see cref="Strings.Spectral.Palette"/> used when the <see cref="Blog"/>.<see cref="Palette"/> isn't specified.
    /// </summary>
    public static Func<Palette> DefaultPalette { get; set; } = () => Palette.Fallback;

    private readonly List<(IRenderable, IRenderable)> _rows = new();
    private readonly string?                          _title;
    private readonly Palette?                         _palette;

    public Palette Palette {
        get => _palette.OrFallback(DefaultPalette.Invoke(), Palette.Fallback);
        init => _palette = value;
    }

    public Blog(string? title = default, Palette? palette = default, [CallerMemberName] string? _caller = default) {
        _title   = title ?? _caller;
        _palette = palette;
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

    #region Post

    public Blog Post<T, TLabel>(T? value, TLabel? label, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(value, label, palette ?? Palette, _expression));
        return this;
    }

    public Blog Post<T>(T? value, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(value, palette ?? Palette, _expression));
        return this;
    }

    public Blog Post<T, TLabel>(ReadOnlySpan<T> span, TLabel? label = default, Palette? palette = default, [CallerArgumentExpression("span")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(span, label, palette ?? Palette, _expression));
        return this;
    }

    public Blog Post<T>(ReadOnlySpan<T> span, Palette? palette = default, [CallerArgumentExpression("span")] string? _expression = default) {
        _rows.Add(Renderwerks.GetLabelled(span, palette ?? Palette, _expression));
        return this;
    }

    #region static Post

    public static void Params<A>(
        A?                                      a,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default
    ) {
        using var blog = new Blog(_caller, palette);
        blog.Post(a, _a);
    }

    public static void Params<A, B>(
        A?                                      a,
        B?                                      b,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default,
        [CallerArgumentExpression("b")] string? _b      = default
    ) {
        using Blog blog = new(_caller, palette);
        blog.Post(a, _a)
            .Post(b, _b);
    }

    public static void Params<A, B, C>(
        A?                                      a,
        B?                                      b,
        C?                                      c,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default,
        [CallerArgumentExpression("b")] string? _b      = default,
        [CallerArgumentExpression("c")] string? _c      = default
    ) {
        using Blog blog = new(_caller, palette);
        blog.Post(a, _a)
            .Post(b, _b)
            .Post(c, _c);
    }

    public static void Params<A, B, C, D>(
        A?                                      a,
        B?                                      b,
        C?                                      c,
        D?                                      d,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default,
        [CallerArgumentExpression("b")] string? _b      = default,
        [CallerArgumentExpression("c")] string? _c      = default,
        [CallerArgumentExpression("d")] string? _d      = default
    ) {
        using Blog blog = new(_caller, palette);
        blog.Post(a, _a)
            .Post(b, _b)
            .Post(c, _c)
            .Post(d, _d);
    }

    public static void Params<A, B, C, D, E>(
        A?                                      a,
        B?                                      b,
        C?                                      c,
        D?                                      d,
        E?                                      e,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default,
        [CallerArgumentExpression("b")] string? _b      = default,
        [CallerArgumentExpression("c")] string? _c      = default,
        [CallerArgumentExpression("d")] string? _d      = default,
        [CallerArgumentExpression("e")] string? _e      = default
    ) {
        using Blog blog = new(_caller, palette);
        blog.Post(a, _a)
            .Post(b, _b)
            .Post(c, _c)
            .Post(d, _d)
            .Post(e, _e);
    }

    #endregion

    #endregion

    public void Dispose() {
        Brandon.Render(this);
    }

    public Measurement          Measure(RenderContext context, int maxWidth) => ToRenderable().GetMeasurement(context, maxWidth);
    public IEnumerable<Segment> Render(RenderContext  context, int maxWidth) => ToRenderable().GetSegments(context, maxWidth);
}