using System;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Linq-style extensions for <see cref="ReadOnlySpan{T}"/> - but with a super-cute name!
/// </summary>
public static partial class Spanq {
    #region Containment

#if !NET6_0_OR_GREATER
    [Pure]
    public static bool Contains<T>(this ReadOnlySpan<T> span, T entry)
        where T : IEquatable<T> => span.IndexOf(entry) >= 0;
#endif

    [Pure]
    public static bool Contains<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSpan)
        where T : IEquatable<T> => span.IndexOf(subSpan) >= 0;

    [Pure]
    public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> soughtAfter)
        where T : IEquatable<T> => span.IndexOfAny(soughtAfter) >= 0;

    #endregion
}