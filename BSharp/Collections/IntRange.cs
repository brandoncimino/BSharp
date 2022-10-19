using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public readonly partial record struct IntRange : IRange<int> {
    public                    int Offset { get; init; } = 0;
    [NonNegativeValue] public int Length { get; init; } = 0;
    public int Start {
        get => Offset;
        init => Offset = value;
    }
    public int End {
        get => Offset + Length;
        init => Length = value - Offset;
    }

    private ImmutableList<int>    AsList           => this.ToImmutableList();
    private ImmutableHashSet<int> AsSet            => this.ToImmutableHashSet();
    private IList                 AsNonGenericList => AsList;

    public IntRange(int offset, int length) {
        (Offset, Length) = (offset, length);
    }

    public IntRange((int from, int to) range) {
        Offset = range.from;
        Length = range.to - Offset;
    }

    public IntRange(Range range, int length) {
        (Offset, Length) = range.GetOffsetAndLength(length);
    }

    public IndexRangeEnumerator                     GetEnumerator() => new(Start, End);
    IEnumerator<int> IEnumerable<int>.              GetEnumerator() => Enumerable.Range(Offset, Length).GetEnumerator();
    IEnumerator IEnumerable.                        GetEnumerator() => Enumerable.Range(Offset, Length).GetEnumerator();
    [NonNegativeValue] int ICollection.             Count           => Length;
    [NonNegativeValue] int ICollection<int>.        Count           => Length;
    [NonNegativeValue] int IReadOnlyCollection<int>.Count           => Length;
    public             MinBound<int>?               Min             => new(Start, Clusivity.Inclusive);
    public             MaxBound<int>?               Max             => new(End, Clusivity.Exclusive);
}