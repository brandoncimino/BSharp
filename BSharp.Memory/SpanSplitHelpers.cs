using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

internal static class SpanSplitHelpers {
    public const StringSplitOptions TrimEntriesOption = (StringSplitOptions)2;

    /// <summary>
    /// Determines if <see cref="F:System.StringSplitOptions.TrimEntries"/> should apply.
    /// </summary>
    /// <remarks>
    /// TODO: Add the ability to provide a <see cref="Func{T, T}"/> predicate to <see cref="SpanSpliterator{T}"/> that will override this default behavior
    /// </remarks>
    /// <param name="entry">a <typeparamref name="T"/> instance</param>
    /// <returns><c>true</c> if we should trim the entry</returns>
    internal static bool IsTrimmable<T>(T? entry) {
        return entry switch {
            char c => char.IsWhiteSpace(c),
#if NET5_0_OR_GREATER
            System.Text.Rune r => System.Text.Rune.IsWhiteSpace(r),
#endif
            string s => string.IsNullOrWhiteSpace(s),
            null     => true,
            _        => false
        };
    }

    internal static ReadOnlySpan<T> GenericTrim<T>(this ReadOnlySpan<T> span) {
        return span.SkipWhile(static it => IsTrimmable(it))
                   .SkipLastWhile(static it => IsTrimmable(it));
    }

    internal static int RequireIndex(
        this int                                     length,
        int                                          index,
        [CallerArgumentExpression("length")] string? _length = default,
        [CallerArgumentExpression("index")]  string? _index  = default,
        [CallerMemberName]                   string? _caller = default
    ) {
        if (index < 0 || index >= length) {
            throw new ArgumentOutOfRangeException($"🙅 {_caller}: {_index} {index} is out-of-bounds for a collection of size {_length} {length}!");
        }

        return index;
    }

    internal static int RequireIndex(
        this int                                     length,
        Index                                        index,
        [CallerArgumentExpression("length")] string? _length = default,
        [CallerArgumentExpression("index")]  string? _index  = default,
        [CallerMemberName]                   string? _caller = default
    ) {
        var off = index.GetOffset(length);

        if (off < 0 || off >= length) {
            throw new ArgumentOutOfRangeException($"🙅 {_caller}: {_index} {index} is out-of-bounds for {_length} {length}!");
        }

        return off;
    }

    internal static (int off, int len) RequireRange(
        this int                                     length,
        Range                                        range,
        [CallerArgumentExpression("length")] string? _length = default,
        [CallerArgumentExpression("range")]  string? _range  = default,
        [CallerMemberName]                   string? _caller = default
    ) {
        var start = length.RequireIndex(range.Start, _length: _length, _caller: _caller);
        var end   = length.RequireIndex(range.End,   _length: _length, _caller: _caller);
        return (start, end - start);
    }
}