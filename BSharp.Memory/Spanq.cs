using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Linq-style extensions for <see cref="ReadOnlySpan{T}"/> - but with a super-cute name!
/// </summary>
public static partial class Spanq {
    #region Containment

#if !NET5_0_OR_GREATER
    [Pure] public static bool Contains<T>(this ReadOnlySpan<T> span, T entry) where T : IEquatable<T> => span.IndexOf(entry) >= 0;
#endif

    [Pure] public static bool Contains<T>(this    ReadOnlySpan<T> span, ReadOnlySpan<T> subSpan) where T : IEquatable<T>     => span.IndexOf(subSpan)        >= 0;
    [Pure] public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> soughtAfter) where T : IEquatable<T> => span.IndexOfAny(soughtAfter) >= 0;

    #endregion

    internal static string FormatString<T>(this ReadOnlySpan<T> span) {
        var sb = new StringBuilder();
        sb.Append(span.SpanName());
        sb.Append('[');

        for (int i = 0; i < span.Length; i++) {
            if (i > 0) {
                sb.Append(", ");
            }

            sb.Append(span[i]);
        }

        sb.Append(']');
        return sb.ToString();
    }

    internal static string FormatString<T>(this Span<T> span) {
        return FormatString((ReadOnlySpan<T>)span);
    }

    /// <summary>
    /// Attempts to get the underlying <see cref="ReadOnlySpan{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="stuff">a sequence of stuff</param>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryGetReadOnlySpan<T>([NoEnumeration] IEnumerable<T> stuff, out ReadOnlySpan<T> span) {
        switch (stuff) {
            case T[] arr:
                span = arr;
                return true;
            case List<T> ls:
                span = SpanHelpers.GetListSpan(ls);
                return true;
            case ImmutableArray<T> imm:
                span = imm.AsSpan();
                return true;
            case ArraySegment<T> seg:
                span = seg;
                return true;
            // ReSharper disable once SuspiciousTypeConversion.Global
            case IAsReadOnlySpan<T> asRo:
                span = asRo.AsReadOnlySpan();
                return true;
            default:
                span = default;
                return false;
        }
    }

    /// <summary>
    /// Attempts to get the underlying <see cref="Span{T}"/> from this <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="stuff">a sequence of stuff</param>
    /// <param name="span">the underlying <b><i>mutable</i></b> <see cref="Span{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>true if a span could be retrieved</returns>
    internal static bool TryGetSpan<T>(IEnumerable<T> stuff, out Span<T> span) {
        switch (stuff) {
            case T[] arr:
                span = arr;
                return true;
            case List<T> ls:
                span = SpanHelpers.GetListSpan(ls);
                return true;
            // ReSharper disable once SuspiciousTypeConversion.Global
            case IAsSpan<T> asSpan:
                span = asSpan.AsSpan();
                return true;
            default:
                span = default;
                return false;
        }
    }
}