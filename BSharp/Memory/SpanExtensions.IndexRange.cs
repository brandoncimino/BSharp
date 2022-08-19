using System;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Memory;

public static partial class SpanExtensions {
    #region Index & Range

    public static bool Contains<T>(this ReadOnlySpan<T> span, Index index) => Indexes.Of(span.Length).Contains(index);
    public static bool Contains<T>(this ReadOnlySpan<T> span, Range range) => Indexes.Of(span.Length).Contains(range);

    #endregion
}