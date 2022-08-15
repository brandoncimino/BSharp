using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Strings;

/// <inheritdoc cref="ITextElements"/>
public sealed record TextElementString : ITextElements {
    private readonly Lazy<string>                          _stringSource;
    public           string                                Source => _stringSource.Value;
    private readonly Lazy<ImmutableArray<GraphemeCluster>> _graphemeClusters;
    public           ImmutableArray<GraphemeCluster>       Clusters => _graphemeClusters.Value;

    public GraphemeCluster this[int index] => Clusters[index];

    public ITextElements Slice(int start, int length) {
        return this switch {
            { _graphemeClusters.IsValueCreated: true } => new TextElementString(Clusters.Slice(start, length)),
            _                                          => new TextElementString(Source.Substring(start, length))
        };
    }

    public TextElementString(string? stringSource) {
        _stringSource     = new Lazy<string>(stringSource ?? "");
        _graphemeClusters = new Lazy<ImmutableArray<GraphemeCluster>>(() => stringSource.EnumerateTextElements().ToImmutableArray());
    }

    public TextElementString(IEnumerable<GraphemeCluster> graphemeClusters) {
        // 📎 .ToImmutableList() already checks if the underlying instance is an ImmutableList() to prevent duplication, so we don't have to do it ourselves.
        _graphemeClusters = new Lazy<ImmutableArray<GraphemeCluster>>(graphemeClusters.ToImmutableArray);
        _stringSource     = new Lazy<string>(() => string.Join(null, Clusters));
    }

    public static   TextElementString Empty                            => new("");
    public override int               GetHashCode()                    => Source.GetHashCode();
    public          bool              Equals(TextElementString? other) => Source == other?.Source;
}