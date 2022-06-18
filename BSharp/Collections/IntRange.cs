using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct IntRange : IEnumerable<int> {
    public           int                         Offset { get; }
    public           int                         Length { get; }
    public           int                         Start  => Offset;
    public           int                         End    => Offset + Length;
    private readonly Lazy<ImmutableList<int>>    _list;
    private          ImmutableList<int>          AsList => _list.Value;
    private readonly Lazy<ImmutableHashSet<int>> _set;
    private          ImmutableHashSet<int>       AsSet            => _set.Value;
    private          IList                       AsNonGenericList => AsList;

    public IntRange(int offset, int length) {
        (Offset, Length) = (offset, length);
        _list            = new Lazy<ImmutableList<int>>(Enumerable.Range(Offset,    Length).ToImmutableList);
        _set             = new Lazy<ImmutableHashSet<int>>(Enumerable.Range(Offset, Length).ToImmutableHashSet);
    }

    public IntRange(Range range, int count) {
        (Offset, Length) = range.GetOffsetAndLength(count);
        _list            = new Lazy<ImmutableList<int>>(Enumerable.Range(Offset,    Length).ToImmutableList);
        _set             = new Lazy<ImmutableHashSet<int>>(Enumerable.Range(Offset, Length).ToImmutableHashSet);
    }

    public IEnumerator<int> GetEnumerator() => Enumerable.Range(Offset, Length).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int              Count           => Length;
}