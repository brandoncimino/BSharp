using System;
using System.ComponentModel;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Determines how <see cref="SpanSpliterator{T}._splitters"/> should be matched against a <see cref="ReadOnlySpan{T}"/>'s entries.
/// </summary>
internal enum SplitterMatchStyle : byte {
    /// <summary>
    /// All of <see cref="SpanSpliterator{T}._splitters"/> should be treated as a single sequence that hast be found, in the manner of <see cref="MemoryExtensions.IndexOf{T}(System.ReadOnlySpan{T}, System.ReadOnlySpan{T})"/>
    /// </summary>
    SubSequence,
    /// <summary>
    /// Each entry in <see cref="SpanSpliterator{T}._splitters"/> should be treated a single, possible match, in the manner of <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>.
    /// </summary>
    AnyEntry,
}

/// <summary>
/// Splits a <see cref="ReadOnlySpan{T}"/>, enumerating the resulting parts.
/// </summary>
/// <remarks>
/// Additional options, like <see cref="_matchStyle"/> and <see cref="_partitionLimit"/>, can be specified via object initializers
/// <i>(if you are constructing a <see cref="SpanSpliterator{T}"/> directly)</i>
/// or via <c>with</c> expressions <i>(if you are constructing a <see cref="SpanSpliterator{T}"/> via a method call,
/// like <see cref="Spanq.Spliterate{T}(System.ReadOnlySpan{T},T[])"/>)</i>.
/// <p/>
/// This differs slightly from <see cref="string.Split(char,int,System.StringSplitOptions)"/> in that it, if the input is empty, the result will also be empty:
/// <code><![CDATA[
/// "".Split('a');                  // => [ "" ]
/// "".AsSpan().Spliterate('a');    // => [ ]
/// "".AsSpan().EnumerateLines();   // => [ "" ] 
/// ]]></code>
/// </remarks>
/// <typeparam name="T">the span element type. 📎 Must implement <see cref="IEquatable{T}"/> for efficient use of <see cref="MemoryExtensions.IndexOf{T}(ReadOnlySpan{T}, T)"/> and friends</typeparam>
public ref struct SpanSpliterator<T> where T : IEquatable<T> {
    #region "Configuration"

    /// <summary>
    /// The <typeparamref name="T"/> entries used to split the source.
    /// Dependent on <see cref="_matchStyle"/>, <see cref="_splitters"/> will be treated as multiple possible splitters <i>(<see cref="SplitterMatchStyle.AnyEntry"/>)</i>
    /// or a sub-sequence <i>(<see cref="SplitterMatchStyle.SubSequence"/>)</i>.
    /// </summary>
    private readonly ReadOnlySpan<T> _splitters;
    private readonly T _singleSplitter;

    /// <summary>
    /// Determines how we should treat the <see cref="_splitters"/>.
    /// </summary>
    /// <remarks>
    /// TODO: Replace this with a static abstract interface when upgrading to .NET 7.
    /// </remarks>
    private readonly SplitterMatchStyle? _matchStyle;

    /// <summary>
    /// The maximum number of <see cref="ReadOnlySpan{T}"/>s that this <see cref="SpanSpliterator{T}"/> will produce.
    /// <br/>
    /// If the <see cref="_partitionLimit"/> is reached, the final <see cref="ReadOnlySpan{T}"/> will contain the remainder of the entries.
    /// </summary>
    private readonly int _partitionLimit;

    #endregion

    private          ReadOnlySpan<T> _remaining;
    private          bool            _isEnumeratorActive;
    private readonly int             _splitterSize;

    /// <summary>
    /// The current number of times that <see cref="_remaining"/> has been split.
    /// </summary>
    private int _splitCount = 0;

    /// <summary>
    /// Gets the current chunk.
    /// </summary>
    public ReadOnlySpan<T> Current { get; private set; } = default;

    internal SpanSpliterator(
        ReadOnlySpan<T>    source,
        ReadOnlySpan<T>    splitters,
        SplitterMatchStyle matchStyle     = SplitterMatchStyle.SubSequence,
        int                partitionLimit = int.MaxValue
    ) {
        _remaining          = source;
        _isEnumeratorActive = source.IsEmpty == false;
        _splitters          = splitters;
        _matchStyle         = matchStyle;
        _partitionLimit     = partitionLimit;
        _singleSplitter     = default!;
        _splitterSize = _matchStyle switch {
            SplitterMatchStyle.AnyEntry    => 1,
            SplitterMatchStyle.SubSequence => _splitters.Length,
            _                              => throw new ArgumentOutOfRangeException(nameof(_matchStyle), _matchStyle, $"Unknown {nameof(SplitterMatchStyle)}!")
        };
    }

    internal SpanSpliterator(
        ReadOnlySpan<T> source,
        T               splitter,
        int             partitionLimit = int.MaxValue
    ) {
        _remaining          = source;
        _singleSplitter     = splitter;
        _splitters          = default;
        _matchStyle         = null;
        _splitterSize       = 1;
        _partitionLimit     = partitionLimit;
        _isEnumeratorActive = source.IsEmpty == false;
    }

    /// <summary>
    /// Returns this instance as an enumerator.
    /// </summary>
    /// <remarks>
    /// I guess this lets you use the <see cref="SpanSpliterator{T}"/> itself in a <c>foreach</c> loop...which you can't normally do, apparently.
    /// </remarks>
    public readonly SpanSpliterator<T> GetEnumerator() => this;

    /// <summary>
    /// Advances the enumerator to the next <see cref="_splitters"/> of the span.
    /// </summary>
    /// <returns>
    /// True if the enumerator successfully advanced to the next <see cref="_splitters"/>; false if
    /// the enumerator has advanced past the end of the span <b><i>OR</i></b> we've reached <see cref="_partitionLimit"/>.
    /// </returns>
    public bool MoveNext() {
        if (!_isEnumeratorActive) {
            return false;
        }

        _splitCount += 1;
        if (_splitCount >= _partitionLimit) {
            Finish();
        }

        var idx = NextSplitIndex();
        if (idx >= 0) {
            Advance(idx);
        }
        else {
            Finish();
        }

        return true;
    }

    /// <summary>
    /// Grabs <see cref="_remaining"/> up to <paramref name="idx"/>, puts it into <see cref="Current"/>, skips <see cref="_splitterSize"/>, and puts the leftovers into <see cref="_remaining"/>.
    /// </summary>
    /// <param name="idx"></param>
    private void Advance(int idx) {
        Current    = _remaining[..idx];
        _remaining = _remaining.Skip(idx + _splitterSize);
    }

    /// <summary>
    /// We've decided to finish iterating, but we still need to return whatever we have left,
    /// so we need to set <see cref="_isEnumeratorActive"/> to <c>false</c> and dump the rest of <see cref="_remaining"/> into <see cref="Current"/>.
    /// <p/>
    /// After <see cref="Finish"/> has been called, any future calls to <see cref="MoveNext"/> will return <c>false</c>.
    /// </summary>
    private void Finish() {
        Current             = _remaining;
        _remaining          = default;
        _isEnumeratorActive = false;
    }

    /// <summary>
    /// TODO: This should be replaced with a static-abstract interface method in .NET 7+.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    private readonly int NextSplitIndex() {
        return _matchStyle switch {
            null                           => _remaining.IndexOf(_singleSplitter),
            SplitterMatchStyle.AnyEntry    => _remaining.IndexOfAny(_splitters),
            SplitterMatchStyle.SubSequence => _remaining.IndexOf(_splitters),
            _                              => throw new InvalidEnumArgumentException(nameof(_matchStyle), (int)_matchStyle, typeof(SplitterMatchStyle))
        };
    }
}