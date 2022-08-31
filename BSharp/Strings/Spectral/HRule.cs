using System.Collections.Generic;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A variation on <see cref="Rule"/> that is <see cref="IExpandable"/>. 
/// </summary>
public sealed record HRule : IRenderable, IExpandable, IHasBoxBorder {
    public bool Expand { get; set; }

    /// <inheritdoc cref="Rule.Title"/>
    public string? Title { get; set; }

    /// <inheritdoc cref="Rule.Style"/>
    public Style? Style { get; set; }

    /// <inheritdoc cref="Rule.Alignment"/>
    public Justify? Alignment { get; set; }

    public BoxBorder Border { get; set; } = BoxBorder.Square;

    private Rule GetRule() => new Rule {
        Title     = Title,
        Style     = Style,
        Alignment = Alignment,
        Border    = Border,
    };

    public Measurement Measure(RenderContext context, int maxWidth) {
        var min = 0;
        if (Title.IsNotBlank()) {
            min = Title.Length + 2 + 4;
        }

        var max = Expand ? maxWidth : min;
        return new Measurement(min, max);
    }

    public IEnumerable<Segment> Render(RenderContext context, int maxWidth) {
        return GetRule().GetSegments(context, maxWidth);
    }
}