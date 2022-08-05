using System;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Memory;

public ref struct SpanSpliterator<T>
    where T : IEquatable<T> {
    private readonly ReadOnlySpan<T>    _splitters;
    private readonly SplitterStyle      _splitterStyle;
    private readonly StringSplitOptions _options;
    private          ReadOnlySpan<T>    _remaining;
    private          ReadOnlySpan<T>    _current;
    private          bool               _isEnumeratorActive;
    private readonly int                _splitterSize;

    /// <summary>
    /// 📝 <see cref="StringSplitOptions"/>.<a href="https://docs.microsoft.com/en-us/dotnet/api/system.stringsplitoptions?view=net-6.0#system-stringsplitoptions-trimentries">TrimEntries</a> doesn't exist until .NET 5,
    /// but we can pretend it does by hard-casting its <see cref="int"/> value, <c>2</c>, directly to <see cref="StringSplitOptions"/>.
    /// </summary>
    private const StringSplitOptions TrimEntries = (StringSplitOptions)2;

    public SpanSpliterator(ReadOnlySpan<T> buffer, ReadOnlySpan<T> splitters, SplitterStyle splitterStyle, StringSplitOptions options) {
        _remaining          = buffer;
        _current            = default;
        _isEnumeratorActive = true;
        _splitters          = splitters;
        _splitterStyle      = splitterStyle;
        _options            = options;
        _splitterSize = splitterStyle switch {
            SplitterStyle.AnyEntry    => 1,
            SplitterStyle.SubSequence => splitters.Length,
            _                         => throw BEnum.UnhandledSwitch(splitterStyle)
        };
    }

    public SpanSpliterator(ReadOnlySpan<T> buffer, T[] splitters, SplitterStyle splitterStyle, StringSplitOptions options) : this(buffer, splitters.AsSpan(), splitterStyle, options) { }

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
            return false;
        }

        var idx = NextSplitIndex();
        if (idx >= 0) {
            _current   = _remaining[..idx];
            _remaining = _remaining.Skip(idx + _splitterSize);

            if (_options.HasFlag(TrimEntries)) {
                _current = _current.SkipWhile(static equatable => IsTrimmable(equatable));
            }

            if (_options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && _current.IsEmpty) {
                // ReSharper disable once TailRecursiveCall
                return MoveNext();
            }
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

    private static bool IsTrimmable(T entry) {
        return entry switch {
            char c => c.IsWhitespace(),
#if NET5_0_OR_GREATER
            Rune r => Rune.IsWhiteSpace(r),
#endif
            null => true,
            _    => false
        };
    }

    private int NextSplitIndex() {
        return _splitterStyle switch {
            SplitterStyle.AnyEntry    => _remaining.IndexOfAny(_splitters),
            SplitterStyle.SubSequence => _remaining.IndexOf(_splitters),
            _                         => throw BEnum.UnhandledSwitch(_splitterStyle),
        };
    }
}

public enum SplitterStyle { AnyEntry, SubSequence }