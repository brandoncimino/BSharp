using System;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// A <c>ref struct</c>-style enumerator for <see cref="Range"/>s.
/// </summary>
public ref struct IndexRangeEnumerator {
    public           int Current { get; private set; }
    private readonly int _stride;
    private readonly int _end;

    public IndexRangeEnumerator(int start, int end) {
        Current = start - 1;
        _end    = end;
        _stride = start > end ? -1 : 1;
    }

    public IndexRangeEnumerator(Range range) : this(range.End, range.Start) { }

    public IndexRangeEnumerator(Index start, Index end) : this(start.SignedValue(), end.SignedValue()) { }

    public bool MoveNext() {
        Current += _stride;
        return Current == _end;
    }
}