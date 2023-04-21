using System;

namespace FowlFever.BSharp.Memory.Enumerators;

/// <summary>
/// Enumerates through each <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/> result.
/// </summary>
/// <seealso cref="IndexOfEnumerator{T}"/>
/// <seealso cref="SpanSpliterator{T}"/>
/// <typeparam name="T">the span element type</typeparam>
public ref struct IndexOfAnyEnumerator<T> where T : IEquatable<T> {
    private          ReadOnlySpan<T> _remaining;
    private readonly ReadOnlySpan<T> _indexOf;
    public           int             Current { get; private set; }

    public IndexOfAnyEnumerator(ReadOnlySpan<T> source, ReadOnlySpan<T> indexOf) {
        _remaining = source;
        _indexOf   = indexOf;
        Current    = -1;
    }

    public bool MoveNext() {
        if (_remaining.IsEmpty) {
            return false;
        }

        var remainingIndex = _remaining.IndexOfAny(_indexOf);
        if (remainingIndex < 0) {
            _remaining = default;
            return false;
        }

        var nextStart = remainingIndex + 1;
        Current += nextStart;

        // if this was the last index, then don't try to slice the span again
        _remaining = nextStart == _remaining.Length ? default : _remaining[(nextStart)..];

        return true;
    }

    public IndexOfAnyEnumerator<T> GetEnumerator() => this;
}