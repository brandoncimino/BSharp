using System;

namespace FowlFever.BSharp.Memory;

public static partial class SpanExtensions {
    #region AfterFirst

    [Pure]
    public static ReadOnlySpan<T> AfterFirst<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Skip(span.IndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> AfterFirstAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Skip(span.IndexOfAny(splitters));

    [Pure]
    public static ReadOnlySpan<T> AfterFirst<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSequence)
        where T : IEquatable<T> => span.Skip(span.IndexOf(subSequence)).Take(subSequence.Length);

    #endregion

    #region BeforeFirst

    [Pure]
    public static ReadOnlySpan<T> BeforeFirst<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Take(span.IndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> BeforeFirstAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Take(span.IndexOfAny(splitters));

    [Pure]
    public static ReadOnlySpan<T> BeforeFirst<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSequence)
        where T : IEquatable<T> => span.Take(span.IndexOf(subSequence));

    #endregion

    #region AfterLast

    [Pure]
    public static ReadOnlySpan<T> AfterLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Skip(span.LastIndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> AfterLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Skip(span.LastIndexOfAny(splitters));

    [Pure]
    public static ReadOnlySpan<T> AfterLast<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSequence)
        where T : IEquatable<T> {
        var index = span.LastIndexOf(subSequence);
        return index >= 0 ? span[(index + subSequence.Length)..] : default;
    }

    #endregion

    #region BeforeLast

    [Pure]
    public static ReadOnlySpan<T> BeforeLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Take(span.LastIndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> BeforeLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Take(span.LastIndexOfAny(splitters));

    [Pure]
    public static ReadOnlySpan<T> BeforeLast<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSequence)
        where T : IEquatable<T> => span.SkipLast(span.IndexOf(subSequence) + subSequence.Length);

    #endregion
}