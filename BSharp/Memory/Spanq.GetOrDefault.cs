using System;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region GetOrDefault

    public static bool TryGet<T>(this ReadOnlySpan<T> span, int index, [MaybeNullWhen(false)] out T got) {
        if (span.Length.ContainsIndex(index)) {
            got = span[index];
            return true;
        }

        got = default;
        return false;
    }

    public static bool TryGet<T>(this ReadOnlySpan<T> span, Index index, [MaybeNullWhen(false)] out T got) {
        if (span.Length.ContainsIndex(index)) {
            got = span[index];
            return true;
        }

        got = default;
        return false;
    }

    public static T? GetOrDefault<T>(this ReadOnlySpan<T> span, int   index)             => span.Length.ContainsIndex(index) ? span[index] : default;
    public static T? GetOrDefault<T>(this ReadOnlySpan<T> span, Index index)             => span.Length.ContainsIndex(index) ? span[index] : default;
    public static T  GetOrDefault<T>(this ReadOnlySpan<T> span, int   index, T fallback) => span.Length.ContainsIndex(index) ? span[index] : fallback;
    public static T  GetOrDefault<T>(this ReadOnlySpan<T> span, Index index, T fallback) => span.Length.ContainsIndex(index) ? span[index] : fallback;

    #endregion
}