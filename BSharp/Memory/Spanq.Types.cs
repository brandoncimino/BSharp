using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Types

    /// <summary>
    /// Equivalent to <see cref="object.GetType"/>, but for <see cref="Span{T}"/>s, which can't use <see cref="object"/> methods.
    /// </summary>
    /// <param name="span">this <see cref="Span{T}"/></param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns><c>typeof(</c><see cref="Span{T}"/><c>)</c></returns>
    [Pure]
    public static Type SpanType<T>(this Span<T> span) => typeof(Span<T>);

    /// <summary>
    /// Equivalent to <see cref="object.GetType"/>, but for <see cref="ReadOnlySpan{T}"/>s, which can't use <see cref="object"/> methods.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns><c>typeof(</c><see cref="ReadOnlySpan{T}"/><c>)</c></returns>
    [Pure]
    public static Type SpanType<T>(this ReadOnlySpan<T> span) => typeof(ReadOnlySpan<T>);

    #endregion
}