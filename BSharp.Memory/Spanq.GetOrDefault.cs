using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region GetOrDefault

    /// <summary>
    /// Attempts to retrieve <see cref="ReadOnlySpan{T}.this"/> and store it into <paramref name="got"/>, returning <c>true</c> if the <paramref name="index"/> was in range.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="index">the desired <see cref="ReadOnlySpan{T}.this"/> index</param>
    /// <param name="got">the value of <see cref="ReadOnlySpan{T}.this"/>, if any</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns><c>true</c> if the <paramref name="index"/> was in range; otherwise, <c>false</c></returns>
    public static bool TryGet<T>(this ReadOnlySpan<T> span, int index, [MaybeNullWhen(false)] out T got) {
        if (span.ContainsIndex(index)) {
            got = span[index];
            return true;
        }

        got = default;
        return false;
    }

    /// <inheritdoc cref="TryGet{T}(System.ReadOnlySpan{T},int,out T)"/>
    public static bool TryGet<T>(this ReadOnlySpan<T> span, Index index, [MaybeNullWhen(false)] out T got) {
        if (span.ContainsIndex(index)) {
            got = span[index];
            return true;
        }

        got = default;
        return false;
    }

    /// <summary>
    /// <inheritdoc cref="Enumerable.ElementAtOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource},int)"/>
    /// </summary>
    /// <remarks>
    /// Analogous to <see cref="Enumerable.ElementAtOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource},int)"/>.
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="index">the desired <see cref="ReadOnlySpan{T}.this"/> index</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns><see cref="ReadOnlySpan{T}.this"/>, if the <paramref name="index"/> is in range; otherwise, <c>default(</c><typeparamref name="T"/><c>)</c></returns>
    public static T? GetOrDefault<T>(this ReadOnlySpan<T> span, int index) => span.ContainsIndex(index) ? span[index] : default;

    /// <inheritdoc cref="GetOrDefault{T}(System.ReadOnlySpan{T},int)"/>
    /// <remarks>Analogous to <see cref="Enumerable.ElementAtOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Index)"/></remarks>
    public static T? GetOrDefault<T>(this ReadOnlySpan<T> span, Index index) => span.ContainsIndex(index) ? span[index] : default;

    /// <inheritdoc cref="GetOrDefault{T}(System.ReadOnlySpan{T},int)"/>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="index">the desired <see cref="ReadOnlySpan{T}.this"/> index</param>
    /// <param name="fallback">the returned value if <paramref name="index"/> is out-of-bounds</param>
    /// <returns><see cref="ReadOnlySpan{T}.this"/>, if the <paramref name="index"/> is in range; otherwise, <paramref name="fallback"/></returns>
    public static T GetOrDefault<T>(this ReadOnlySpan<T> span, int index, T fallback) => span.ContainsIndex(index) ? span[index] : fallback;

    /// <inheritdoc cref="GetOrDefault{T}(System.ReadOnlySpan{T},int)"/>
    public static T GetOrDefault<T>(this ReadOnlySpan<T> span, Index index, T fallback) => span.ContainsIndex(index) ? span[index] : fallback;

    #endregion
}