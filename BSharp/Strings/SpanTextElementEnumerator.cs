#if !NET6_0_OR_GREATER
#define USE_LAME_ENUMERATOR
#endif

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Theoretically a <see cref="ReadOnlySpan{T}"/>-friendly version of <see cref="TextElementEnumerator"/>.
///
/// Unfortunately, <a href="https://docs.microsoft.com/en-us/dotnet/api/system.globalization.stringinfo.getnexttextelementlength?view=net-6.0">GetNextTextElementLength</a> isn't available
/// until .NET 6, and it's difficult to steal it from the source code without access to the <a href="https://docs.microsoft.com/en-us/dotnet/api/system.text.rune?view=net-6.0">Rune</a> type,
/// so for pre-.NET 6 versions this just falls back to <see cref="TextElementEnumerator"/> 😭
/// </summary>
[SuppressMessage("ReSharper", "StructCanBeMadeReadOnly", Justification = "Cannot be readonly in .net 5+, so keeping it non-readonly in older versions for consistency")]
public ref struct SpanTextElementEnumerator {
#if USE_LAME_ENUMERATOR
    private readonly TextElementEnumerator _lameEnumerator;
    public           ReadOnlySpan<char>    Current => _lameEnumerator.GetTextElement();

    public SpanTextElementEnumerator(ReadOnlySpan<char> source) {
        this._lameEnumerator = StringInfo.GetTextElementEnumerator(source.ToString());
    }

    public bool MoveNext() => this._lameEnumerator.MoveNext();

#else
    private readonly ReadOnlySpan<char> _source;
    private          ReadOnlySpan<char> _remaining;
    private          bool               _isFinished = false;
    private          ReadOnlySpan<char> _current    = default;
    public           ReadOnlySpan<char> Current => _current;

    public SpanTextElementEnumerator(ReadOnlySpan<char> source) {
        this._source    = source;
        this._remaining = source;
    }

    public bool MoveNext() {
        if (_remaining.IsEmpty) {
            _isFinished = false;
        }

        if (_isFinished) {
            return false;
        }

        var nextLength = StringInfo.GetNextTextElementLength(_remaining);
        if (nextLength == 0) {
            _isFinished = true;
            return false;
        }

        (_current, _remaining) = _source.TakeLeftovers(nextLength);
        return true;
    }
#endif

    public ImmutableArray<GraphemeCluster> ToGraphemeClusters() {
        var builder = ImmutableArray.CreateBuilder<GraphemeCluster>();

        while (this.MoveNext()) {
            builder.Add(new GraphemeCluster(Current));
        }

        return builder.MoveToImmutableSafely();
    }
}