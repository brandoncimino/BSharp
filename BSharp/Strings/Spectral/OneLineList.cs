using System;
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
    private          Segment                     PrefixSegment  => Bookends.GetPrefix().EscapeSegment(BookendStyle);
    private          Segment                     SuffixSegment  => Bookends.GetSuffix().EscapeSegment(BookendStyle);
    private          Segment                     JoinerSegment  => new(", ", DelimiterStyle);

    public OneLineList(IEnumerable<IRenderable> stuff) {
        _stuff = stuff.ToImmutableList();
    }

    public OneLineList(IEnumerable<IRenderable> stuff, Palette palette) : this(stuff) {
        BookendStyle   = palette.Borders;
        DelimiterStyle = palette.Delimiters;
    }

    private SegmentLine GetSegmentLine(RenderContext context, int maxWidth) {
        var line = new SegmentLine { PrefixSegment };
        FlatJoin(line, _stuff, it => it.Render(context, maxWidth), JoinerSegment);
        line.Add(SuffixSegment);
        return line;
    }

    private static TList FlatJoin<TList, T, T2>(TList list, IEnumerable<T2> stuff, Func<T2, IEnumerable<T>> selector, T joiner)
        where TList : List<T> {
        using var erator = stuff.GetEnumerator();

        if (erator.MoveNext() == false) {
            return list;
        }

        list.AddRange(selector(erator.Current));

        while (erator.MoveNext()) {
            list.Add(joiner);
            list.AddRange(selector(erator.Current));
        }

        return list;
    }

    public Measurement Measure(RenderContext context, int maxWidth) {
        return new Measurement(maxWidth, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderContext context, int maxWidth) {
        return GetSegmentLine(context, maxWidth);
    }
}