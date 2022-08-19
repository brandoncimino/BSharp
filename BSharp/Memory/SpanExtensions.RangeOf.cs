using System;

namespace FowlFever.BSharp.Memory;

public static partial class SpanExtensions {
    #region RangeOf

    public static Range RangeOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSpan)
        where T : IEquatable<T> {
        var index = span.IndexOf(subSpan);
        return index switch {
            < 0 => ..0,
            _   => index..(index + subSpan.Length)
        };
    }

    #endregion
}