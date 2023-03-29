using System;
using System.ComponentModel;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Determines how <see cref="SpanSpliterator{T}.Splitters"/> should be matched against a <see cref="ReadOnlySpan{T}"/>'s entries.
/// </summary>
public enum SplitterMatchStyle : byte {
    /// <summary>
    /// All of <see cref="SpanSpliterator{T}.Splitters"/> should be treated as a single sequence that hast be found, in the manner of <see cref="MemoryExtensions.IndexOf{T}(System.ReadOnlySpan{T}, System.ReadOnlySpan{T})"/>
    /// </summary>
    SubSequence,
    /// <summary>
    /// Each entry in <see cref="SpanSpliterator{T}.Splitters"/> should be treated a single, possible match, in the manner of <see cref="MemoryExtensions.IndexOfAny{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>.
    /// </summary>
    AnyEntry,
}

/// <summary>
/// Determines where we start from and in what direction we move through a sequence when we're searching, i.e.
/// <ul>
/// <li><see cref="Forward"/> → <see cref="MemoryExtensions.IndexOf{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/></li>
/// <li><see cref="Backward"/> → <see cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/></li>
/// </ul> 
/// </summary>
public enum SearchDirection : byte {
    /// <summary>
    /// Search from <see cref="Index.Start"/> → <see cref="Index.End"/>.
    /// </summary>
    /// <remarks>
    /// Analogous to <see cref="MemoryExtensions.IndexOf{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>.
    /// </remarks>
    Forward,
    /// <summary>
    /// Search from <see cref="Index.End"/> → <see cref="Index.Start"/>.
    /// </summary>
    /// <remarks>
    /// Analogous to <see cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},System.ReadOnlySpan{T})"/>.
    /// </remarks>
    Backward
}

/// <summary>
/// Splits a <see cref="ReadOnlySpan{T}"/>, enumerating the resulting parts.
/// </summary>
/// <remarks>
/// Additional options, like <see cref="MatchStyle"/> and <see cref="PartitionLimit"/>, can be specified via object initializers
/// <i>(if you are constructing a <see cref="SpanSpliterator{T}"/> directly)</i>
/// or via <c>with</c> expressions <i>(if you are constructing a <see cref="SpanSpliterator{T}"/> via a method call,
/// like <see cref="Spanq.Spliterate{T}(System.ReadOnlySpan{T},T[])"/>)</i>.
/// </remarks>
/// <typeparam name="T">the span element type. 📎 Must implement <see cref="IEquatable{T}"/> for efficient use of <see cref="MemoryExtensions.IndexOf{T}(ReadOnlySpan{T}, T)"/> and friends</typeparam>
public ref struct SpanSpliterator<T> where T : IEquatable<T> {
    #region "Configuration"

    /// <summary>
    /// The <typeparamref name="T"/> entries used to split the source.
    /// Dependent on <see cref="MatchStyle"/>, <see cref="Splitters"/> will be treated as multiple possible splitters <i>(<see cref="SplitterMatchStyle.AnyEntry"/>)</i>
    /// or a sub-sequence <i>(<see cref="SplitterMatchStyle.SubSequence"/>)</i>.
    /// </summary>
    public readonly ReadOnlySpan<T> Splitters { private get; init; }

    /// <summary>
    /// Determines how we should treat the <see cref="Splitters"/>.
    /// </summary>
    public readonly SplitterMatchStyle MatchStyle { private get; init; }

    /// <summary>
    /// Hijacks <see cref="StringSplitOptions"/> to determine whether we trim the partitions and/or remove empty ones.
    /// </summary>
    /// <remarks>
    /// Though <see cref="F:System.StringSplitOptions.TrimEntries"/> isn't technically supported until .NET 5+, a <see cref="StringSplitOptions"/> that <see cref="Enum.HasFlag"/> <c>2</c>
    /// will still be respected. 
    /// </remarks>
    public readonly StringSplitOptions Options { private get; init; }

    /// <summary>
    /// The maximum number of <see cref="ReadOnlySpan{T}"/>s that this <see cref="SpanSpliterator{T}"/> will produce.
    /// <br/>
    /// If the <see cref="PartitionLimit"/> is reached, the final <see cref="ReadOnlySpan{T}"/> will contain the remainder of the entries.
    /// </summary>
    public readonly int PartitionLimit { private get; init; }

    #endregion

    private ReadOnlySpan<T> _remaining;
    private ReadOnlySpan<T> _current = default;
    private bool            _isEnumeratorActive;

    private readonly int _splitterSize => MatchStyle switch {
        SplitterMatchStyle.AnyEntry    => 1,
        SplitterMatchStyle.SubSequence => Splitters.Length,
        _                              => throw new ArgumentOutOfRangeException(nameof(MatchStyle), MatchStyle, $"Unknown {nameof(SplitterMatchStyle)}!")
    };

    /// <summary>
    /// The current number of times that <see cref="_remaining"/> has been split.
    /// </summary>
    private int _splitCount = 0;

    public SpanSpliterator(
        ReadOnlySpan<T>    source,
        ReadOnlySpan<T>    splitters,
        SplitterMatchStyle matchStyle     = SplitterMatchStyle.SubSequence,
        StringSplitOptions options        = StringSplitOptions.None,
        int                partitionLimit = int.MaxValue
    ) {
        _remaining          = source;
        _isEnumeratorActive = source.IsEmpty == false;
        Splitters           = splitters;
        MatchStyle          = matchStyle;
        Options             = options;
        PartitionLimit      = partitionLimit;
    }

    /// <summary>
    /// Gets the current chunk.
    /// </summary>
    public readonly ReadOnlySpan<T> Current => _current;

    /// <summary>
    /// Returns this instance as an enumerator.
    /// </summary>
    /// <remarks>
    /// I guess this lets you use the <see cref="SpanSpliterator{T}"/> itself in a <c>foreach</c> loop...which you can't normally do, apparently.
    /// </remarks>
    public readonly SpanSpliterator<T> GetEnumerator() => this;

    /// <summary>
    /// Advances the enumerator to the next <see cref="Splitters"/> of the span.
    /// </summary>
    /// <returns>
    /// True if the enumerator successfully advanced to the next <see cref="Splitters"/>; false if
    /// the enumerator has advanced past the end of the span <b><i>OR</i></b> we've reached <see cref="PartitionLimit"/>.
    /// </returns>
    public bool MoveNext() {
        if (!_isEnumeratorActive) {
            return false;
        }

        _splitCount += 1;
        if (_splitCount >= PartitionLimit) {
            Finish();
        }

        var idx = NextSplitIndex();
        if (idx >= 0) {
            Advance(idx);
        }
        else {
            Finish();
        }

        #region Handle _options flags

        while (Options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && _current.IsEmpty) {
            if (MoveNext() == false) {
                return false;
            }
        }

        #endregion

        return true;
    }

    /// <summary>
    /// Grabs <see cref="_remaining"/> up to <paramref name="idx"/>, puts it into <see cref="_current"/>, skips <see cref="_splitterSize"/>, and puts the leftovers into <see cref="_remaining"/>.
    /// </summary>
    /// <param name="idx"></param>
    private void Advance(int idx) {
        _current   = _remaining[..idx];
        _remaining = _remaining.Skip(idx + _splitterSize);
    }

    /// <summary>
    /// We've decided to finish iterating, but we still need to return whatever we have left,
    /// so we need to set <see cref="_isEnumeratorActive"/> to <c>false</c> and dump the rest of <see cref="_remaining"/> into <see cref="_current"/>.
    /// <p/>
    /// After <see cref="Finish"/> has been called, any future calls to <see cref="MoveNext"/> will return <c>false</c>.
    /// </summary>
    private void Finish() {
        _current            = _remaining;
        _remaining          = default;
        _isEnumeratorActive = false;
    }

    private readonly int NextSplitIndex() {
        return MatchStyle switch {
            SplitterMatchStyle.AnyEntry    => _remaining.IndexOfAny(Splitters),
            SplitterMatchStyle.SubSequence => _remaining.IndexOf(Splitters),
            _                              => throw new InvalidEnumArgumentException(nameof(MatchStyle), (int)MatchStyle, typeof(SplitterMatchStyle))
        };
    }
}