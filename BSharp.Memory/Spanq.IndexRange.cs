using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Index & Range

    public static bool ContainsIndex<T>(this ReadOnlySpan<T> span, int   index) => index >= 0 && index < span.Length;
    public static bool ContainsIndex<T>(this ReadOnlySpan<T> span, Index index) => span.ContainsIndex(index.GetOffset(span.Length));
    public static bool ContainsRange<T>(this ReadOnlySpan<T> span, Range range) => span.ContainsIndex(range.Start) && span.ContainsIndex(range.End);

    #endregion
}