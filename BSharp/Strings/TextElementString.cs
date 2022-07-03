using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// An immutable version of <see cref="StringInfo"/> with nutty <see cref="GraphemeCluster"/>s.
/// </summary>
public sealed record TextElementString : WrappedImmutableCollection<GraphemeCluster, ImmutableList<GraphemeCluster>, TextElementString>, IEquatable<string?> {
    private          string?                              _stringSource;
    public           string                               StringSource => _stringSource ??= string.Join("", _graphemeClusters.Value);
    private readonly Lazy<ImmutableList<GraphemeCluster>> _graphemeClusters;
    public override  ImmutableList<GraphemeCluster>       Value => _graphemeClusters.Value;

    public override TextElementString Slice(int start, int length) => this switch {
        {
            _stringSource: not null,
            _graphemeClusters.IsValueCreated: false
        } => new TextElementString(_stringSource.Substring(start, length)), // avoid calculating the grapheme clusters and let the new slice deal with them
        _ => new TextElementString(_graphemeClusters.Value.GetRange(start, start + length)),
    };

    public TextElementString(string stringSource) {
        _stringSource     = stringSource;
        _graphemeClusters = new Lazy<ImmutableList<GraphemeCluster>>(() => stringSource.EnumerateTextElements().ToImmutableList());
    }

    public TextElementString(IEnumerable<GraphemeCluster> graphemeClusters) {
        // 📎 .ToImmutableList() already checks if the underlying instance is an ImmutableList() to prevent duplication, so we don't have to do it ourselves.
        _graphemeClusters = new Lazy<ImmutableList<GraphemeCluster>>(graphemeClusters.ToImmutableList);
    }

    public static TextElementString Empty => new("");

    public          bool Equals(string? other)            => StringSource == other;
    public override int  GetHashCode()                    => StringSource.GetHashCode();
    public          bool Equals(TextElementString? other) => StringSource == other?.StringSource;
}