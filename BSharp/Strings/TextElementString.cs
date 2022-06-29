using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// An immutable version of <see cref="StringInfo"/> with nutty <see cref="GraphemeCluster"/>s.
/// </summary>
public record TextElementString : WrappedImmutableCollection<GraphemeCluster, ImmutableList<GraphemeCluster>> {
    private            string?                              _stringSource;
    public             string                               StringSource => _stringSource ??= string.Join("", _graphemeClusters.Value);
    private readonly   Lazy<ImmutableList<GraphemeCluster>> _graphemeClusters;
    public override    ImmutableList<GraphemeCluster>       Value                                              => _graphemeClusters.Value;
    protected override ImmutableList<GraphemeCluster>       CreateSlice(IEnumerable<GraphemeCluster> elements) => elements.ToImmutableList();

    public TextElementString(string stringSource) {
        _stringSource     = stringSource;
        _graphemeClusters = new Lazy<ImmutableList<GraphemeCluster>>(() => stringSource.EnumerateTextElements().ToImmutableList());
    }

    public TextElementString(IEnumerable<GraphemeCluster> graphemeClusters) {
        _graphemeClusters = new Lazy<ImmutableList<GraphemeCluster>>(graphemeClusters.ToImmutableList());
    }

    public static TextElementString Empty => new("");
}