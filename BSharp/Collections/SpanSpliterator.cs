using System;

namespace FowlFever.BSharp.Collections;

public ref struct SpanSpliterator<T>
    where T : IEquatable<T> {
    private readonly ReadOnlySpan<T> _splitters;
    private          ReadOnlySpan<T> _remaining;
    private          ReadOnlySpan<T> _current;
    private          bool            _isEnumeratorActive;

    public SpanSpliterator(ReadOnlySpan<T> buffer, ReadOnlySpan<T> splitters) {
        _remaining          = buffer;
        _current            = default;
        _isEnumeratorActive = true;
        _splitters          = splitters;
    }

    public SpanSpliterator(ReadOnlySpan<T> buffer, params T[] splitters) {
        _remaining          = buffer;
        _current            = default;
        _isEnumeratorActive = true;
        _splitters          = splitters;
    }

    /// <summary>
    /// Gets the current file extension.
    /// </summary>
    public ReadOnlySpan<T> Current => _current;

    /// <summary>
    /// Returns this instance as an enumerator.
    /// </summary>
    public SpanSpliterator<T> GetEnumerator() => this;

    /// <summary>
    /// Advances the enumerator to the next <see cref="_splitters"/> of the span.
    /// </summary>
    /// <returns>
    /// True if the enumerator successfully advanced to the next <see cref="_splitters"/>; false if
    /// the enumerator has advanced past the end of the span.
    /// </returns>
    public bool MoveNext() {
        if (!_isEnumeratorActive) {
            return false; // EOF previously reached or enumerator was never initialized
        }

        var idx = _remaining.IndexOfAny(_splitters);
        if (idx >= 0) {
            _current   = _remaining[..idx];
            _remaining = _remaining[idx..];
        }
        else {
            // We've reached EOF, but we still need to return 'true' for this final
            // iteration so that the caller can query the Current property once more.

            _current            = _remaining;
            _remaining          = default;
            _isEnumeratorActive = false;
        }

        return true;
    }
}