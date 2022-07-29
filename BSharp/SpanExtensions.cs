using System;

namespace FowlFever.BSharp;

[Pure]
public static class SpanExtensions {
    [Pure]
    public static ReadOnlySpan<T> AfterIndex<T>(ReadOnlySpan<T> span, int index) => index switch {
        < 0 => default,
        _   => span[index..]
    };

    [Pure]
    public static ReadOnlySpan<T> BeforeIndex<T>(ReadOnlySpan<T> span, int index) => index switch {
        < 0 => default,
        _   => span[..index]
    };

    #region AfterFirst

    [Pure]
    public static ReadOnlySpan<T> AfterFirst<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => AfterIndex(span, span.IndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> AfterFirstAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => AfterIndex(span, span.IndexOfAny(splitters));

    #endregion

    #region BeforeFirst

    [Pure]
    public static ReadOnlySpan<T> BeforeFirst<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => BeforeIndex(span, span.IndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> BeforeFirstAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => BeforeIndex(span, span.IndexOfAny(splitters));

    #endregion

    #region AfterLast

    [Pure]
    public static ReadOnlySpan<T> AfterLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => AfterIndex(span, span.LastIndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> AfterLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => AfterIndex(span, span.LastIndexOfAny(splitters));

    #endregion

    #region BeforeLast

    [Pure]
    public static ReadOnlySpan<T> BeforeLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => BeforeIndex(span, span.LastIndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> BeforeLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => BeforeIndex(span, span.LastIndexOfAny(splitters));

    #endregion
}