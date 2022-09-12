using System;

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
}