using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Exceptions

    private static string SpanName<T>(this ReadOnlySpan<T> span) {
        return $"{nameof(ReadOnlySpan<T>)}<{typeof(T).Name}>";
    }

    /// <exception cref="InvalidOperationException">if this <paramref name="span"/> <see cref="ReadOnlySpan{T}.IsEmpty"/></exception>
    private static ReadOnlySpan<T> RequireNotEmpty<T>(this ReadOnlySpan<T> span, [CallerMemberName] string? caller = default) {
        if (span.IsEmpty) {
            throw WasEmpty(span, caller);
        }

        return span;
    }

    private static ReadOnlySpan<T> RequireLength<T>(this ReadOnlySpan<T> span, int length, [CallerMemberName] string? _caller = default) {
        if (span.Length < length) {
            throw new InvalidOperationException($"🙅 {_caller} - {span.SpanName()} length {span.Length} is shorter than the required {length}!");
        }

        return span;
    }

    private static T RequireFound<T, T2>(
        this ReadOnlySpan<T>       span,
        int                        index,
        string?                    _selector,
        T2                         expected,
        [CallerMemberName] string? _caller = default
    ) {
        return index switch {
            < 0 => throw NotFound(span, _selector, expected, _caller),
            _   => span[index],
        };
    }

    private static InvalidOperationException NotFound<T, T2>(
        this ReadOnlySpan<T>       span,
        string?                    _selector,
        T2                         expected,
        [CallerMemberName] string? _caller = default
    ) {
        return new InvalidOperationException($"🙅 {_caller} - there were no entries in the {span.SpanName()} satisfying {_selector} == {expected}!");
    }

    private static InvalidOperationException WasEmpty<T>(ReadOnlySpan<T> span, [CallerMemberName] string? _caller = default) {
        return new InvalidOperationException($"🙅 {_caller} - the {span.SpanName()} was empty!");
    }

    #endregion
}