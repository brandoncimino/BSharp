using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp;

[Experimental("Haven't looked at this in a while, and it's very silly")]
internal sealed record Blog : IDisposable, IRenderable {
    /// <summary>
    /// The <see cref="Strings.Spectral.Palette"/> used when the <see cref="Blog"/>.<see cref="Palette"/> isn't specified.
    /// </summary>
    public static Func<Palette> DefaultPalette { get; set; } = () => Palette.Fallback;

    private static Blog? _mainBlog;

    private readonly List<(IRenderable?, IRenderable?)?> _rows = new();
    private readonly string?                             _title;
    private readonly Palette?                            _palette;
    private          bool                                _isPosted = false;

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
            if (r == null) {
                grid.AddEmptyRow();
            }
            else {
                var label = r?.Item1 ?? Text.Empty;
                var value = r?.Item2 ?? Text.Empty;
                grid.AddRow(label, value);
            }
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

    private void RequireNotPosted() {
        if (_isPosted) {
            throw new InvalidOperationException($"This {GetType().Name} has already been posted!");
        }
    }

    private Blog AddRow(IRenderable? label, IRenderable? value) {
        return AddRow((label, value));
    }

    private Blog AddRow((IRenderable?, IRenderable?)? row) {
        RequireNotPosted();

        _rows.Add(row);
        return this;
    }

    public Blog Post<T, TLabel>(T? value, TLabel? label, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        return AddRow(Renderwerks.GetLabelled(value, label, palette ?? Palette, _expression));
    }

    public Blog Post<T>(T? value, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        return AddRow(Renderwerks.GetLabelled(value, palette ?? Palette, _expression));
    }

    public Blog Post<T, TLabel>(ReadOnlySpan<T> span, TLabel? label = default, Palette? palette = default, [CallerArgumentExpression("span")] string? _expression = default) {
        return AddRow(Renderwerks.GetLabelled(span, label, palette ?? Palette, _expression));
    }

    public Blog Post<T>(ReadOnlySpan<T> span, Palette? palette = default, [CallerArgumentExpression("span")] string? _expression = default) {
        return AddRow(Renderwerks.GetLabelled(span, palette ?? Palette, _expression));
    }

    #region static Post

    public static Blog Params<A>(
        A?                                      a,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default
    ) {
        return new Blog(_caller, palette).Post(a, _a);
    }

    public static Blog Params<A, B>(
        A?                                      a,
        B?                                      b,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default,
        [CallerArgumentExpression("b")] string? _b      = default
    ) {
        return new Blog(_caller, palette).Post(a, _a)
                                         .Post(b, _b);
    }

    public static Blog Params<A, B, C>(
        A?                                      a,
        B?                                      b,
        C?                                      c,
        Palette?                                palette = default,
        [CallerMemberName]              string? _caller = default,
        [CallerArgumentExpression("a")] string? _a      = default,
        [CallerArgumentExpression("b")] string? _b      = default,
        [CallerArgumentExpression("c")] string? _c      = default
    ) {
        return new Blog(_caller, palette).Post(a, _a)
                                         .Post(b, _b)
                                         .Post(c, _c);
    }

    public static Blog Params<A, B, C, D>(
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
        return new Blog(_caller, palette).Post(a, _a)
                                         .Post(b, _b)
                                         .Post(c, _c)
                                         .Post(d, _d);
    }

    public static Blog Params<A, B, C, D, E>(
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
        return new Blog(_caller, palette).Post(a, _a)
                                         .Post(b, _b)
                                         .Post(c, _c)
                                         .Post(d, _d)
                                         .Post(e, _e);
    }

    #endregion

    #endregion

    public void Dispose() {
        if (_mainBlog != null) {
            _mainBlog.AddRow(this, null);
            return;
        }
        else {
            Brandon.Render(this);
        }
    }

    public Measurement          Measure(RenderContext context, int maxWidth) => ToRenderable().GetMeasurement(context, maxWidth);
    public IEnumerable<Segment> Render(RenderContext  context, int maxWidth) => ToRenderable().GetSegments(context, maxWidth);
}