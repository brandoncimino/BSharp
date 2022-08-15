using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// An immutable version of <see cref="System.Globalization.StringInfo"/> with nutty <see cref="FowlFever.BSharp.Strings.GraphemeCluster"/> support.
/// </summary>
public interface ITextElements : IHas<string>, IEquivalent<string>, IEnumerable<GraphemeCluster> {
    /// <summary>
    /// The original <see cref="string"/>.
    /// </summary>
    string Source { get; }
    string IHas<string>.       Value      => Source;
    string IEquivalent<string>.Equivalent => Source;

    /// <summary>
    /// The parsed <see cref="GraphemeCluster"/>s.
    /// </summary>
    ImmutableArray<GraphemeCluster> Clusters { get; }

    /// <inheritdoc cref="ImmutableArray{T}.GetEnumerator"/>
    public new ImmutableArray<GraphemeCluster>.Enumerator GetEnumerator() => Clusters.GetEnumerator();

    bool IEquatable<string?>.Equals(string? other) => Source.Equals(other);

    IEnumerator<GraphemeCluster> IEnumerable<GraphemeCluster>.GetEnumerator() => (Clusters as IEnumerable<GraphemeCluster>).GetEnumerator();
    IEnumerator IEnumerable.                                  GetEnumerator() => (Clusters as IEnumerable).GetEnumerator();

    bool IsEmpty => string.IsNullOrEmpty(Source);
    bool IsBlank => string.IsNullOrWhiteSpace(Source);
    public GraphemeCluster this[int index] { get; }
    public ITextElements Slice(int  start, int length);
    /// <summary>
    /// The number of <see cref="Clusters"/>.
    /// </summary>
    public int Length => Clusters.Length;
}