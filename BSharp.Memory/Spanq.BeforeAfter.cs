using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
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

    [Pure]
    public static ReadOnlySpan<char> AfterFirst(this ReadOnlySpan<char> span, ReadOnlySpan<char> subString, StringComparison comparisonType = StringComparison.Ordinal)
        => span.Skip(span.IndexOf(subString, comparisonType)).Take(subString.Length);

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

    [Pure]
    public static ReadOnlySpan<char> BeforeFirst(this ReadOnlySpan<char> span, ReadOnlySpan<char> subString, StringComparison comparisonType = StringComparison.Ordinal)
        => span.Take(span.IndexOf(subString, comparisonType));

    #endregion

    #region AfterLast

    [Pure]
    public static ReadOnlySpan<T> AfterLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> {
        // TODO: If the separator isn't found, then the full input is returned. This probably isn't intuitive...but it's very useful behavior, so it should probably be moved into a more appropriately-named method.
        //  For example:
        //      "abc".AfterLast('z') => "abc", when it should probably return "".
        //  The useful case for this behavior:
        //      "a/b.c".AfterLast('.') => 'c'
        //      ".c".AfterLast('.') => 'c'
        //      "c".AfterLast('.') => 'c'
        //  So, maybe something like..."skip-to-last"?
        return span.Skip(span.LastIndexOf(splitter));
    }

    [Pure]
    public static ReadOnlySpan<T> AfterLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Skip(span.LastIndexOfAny(splitters));

    [Pure]
    public static ReadOnlySpan<T> AfterLast<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> subSequence)
        where T : IEquatable<T> {
        var index = span.LastIndexOf(subSequence);
        return index >= 0 ? span[(index + subSequence.Length)..] : default;
    }

#if NET5_0_OR_GREATER
    [Pure]
    public static ReadOnlySpan<char> AfterLast(this ReadOnlySpan<char> span, ReadOnlySpan<char> subString, StringComparison comparisonType = StringComparison.Ordinal) {
        var index = span.LastIndexOf(subString, comparisonType);
        return index >= 0 ? span[(index + subString.Length)..] : default;
    }
#endif

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
        where T : IEquatable<T> => span.SkipLast(span.LastIndexOf(subSequence) + subSequence.Length);

#if NET5_0_OR_GREATER
    [Pure]
    public static ReadOnlySpan<char> BeforeLast(this ReadOnlySpan<char> span, ReadOnlySpan<char> subString, StringComparison comparisonType = StringComparison.Ordinal)
        => span.SkipLast(span.LastIndexOf(subString, comparisonType) + subString.Length);
#endif

    #endregion
}