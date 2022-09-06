using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.Conjugal.Affixing;

using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Renders a bunch of <see cref="IRenderable"/>s into a list on one line, e.g. <c>[1, 2, 3]</c>
/// </summary>
public class OneLineList : IRenderable {
    private readonly IImmutableList<IRenderable> _stuff;
    public           ICircumfix                  Bookends       { get; init; } = BSharp.Enums.Bookends.SquareBrackets;
    public           Stylist                     BookendStyle   { get; init; }
    public           Stylist                     DelimiterStyle { get; init; }
    private          Segment?                    _prefixSegment;
    private          Segment                     PrefixSegment => _prefixSegment ??= Bookends.GetPrefix().ToSegment(BookendStyle);
    private          Segment?                    _suffixSegment;
    private          Segment                     SuffixSegment => _suffixSegment ??= Bookends.GetSuffix().ToSegment(BookendStyle);
    private          Segment                     JoinerSegment => new(", ", DelimiterStyle);

    public OneLineList(IEnumerable<IRenderable> stuff) {
        _stuff = stuff.ToImmutableList();
    }

    public OneLineList(IEnumerable<IRenderable> stuff, Palette palette) : this(stuff) {
        BookendStyle   = palette.Borders;
        DelimiterStyle = palette.Delimiters;
    }

    private SegmentLine GetSegmentLine(RenderContext context, int maxWidth) {
        var line = new SegmentLine { PrefixSegment };

        var first = true;

        foreach (var renderable in _stuff) {
            if (first) {
                first = false;
            }
            else {
                line.Add(JoinerSegment);
            }

            line.AddRange(renderable.Render(context, maxWidth));
        }

        line.Add(SuffixSegment);
        return line;
    }

    public Measurement Measure(RenderContext context, int maxWidth) {
        return new Measurement(maxWidth, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderContext context, int maxWidth) {
        return GetSegmentLine(context, maxWidth);
    }
}