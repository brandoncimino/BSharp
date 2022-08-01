using System;
using System.Collections.Immutable;

namespace FowlFever.BSharp.Collections;

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

    #region Trimming

    public static ReadOnlySpan<T> Skip<T>(this     ReadOnlySpan<T> span, int toSkip) => span[toSkip..];
    public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int toSkip) => span[..^toSkip];
    public static ReadOnlySpan<T> Take<T>(this     ReadOnlySpan<T> span, int toTake) => span[..toTake];
    public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int toTake) => span[^toTake..];

    public static int StartingMatches<T>(this ReadOnlySpan<T> span, T toMatch, int? matchLimit = default)
        where T : IEquatable<T> {
        var ml = matchLimit ?? span.Length;
        for (int i = 0; i < ml; i++) {
            if (span[i].Equals(toMatch) == false) {
                return i;
            }
        }

        return ml;
    }

    public static int EndingMatches<T>(this ReadOnlySpan<T> span, T toMatch, int? matchLimit = default)
        where T : IEquatable<T> {
        var ml = matchLimit ?? span.Length;
        for (int i = 0; i < ml; i++) {
            if (span[^(i + 1)].Equals(toMatch) == false) {
                return i;
            }
        }

        return ml;
    }

    public static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> span, T toTrim, int? numberToTrim = default)
        where T : IEquatable<T> {
        return span.Skip(span.StartingMatches(toTrim, numberToTrim));
    }

    public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> span, T toTrim, int? numberToTrim = default)
        where T : IEquatable<T> {
        return span.SkipLast(span.StartingMatches(toTrim, numberToTrim));
    }

    #endregion

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, params T[] splitters)
        where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitters);
    }

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitters);
    }

    public static ImmutableArray<string> ToStringArray(this SpanSpliterator<char> spliterator) {
        var arr          = ImmutableArray.CreateBuilder<string>();
        int currentIndex = 0;
        foreach (var span in spliterator) {
            arr[currentIndex] =  span.ToString();
            currentIndex      += 1;
        }

        return arr.MoveToImmutable();
    }

    public delegate TOut ReadOnlySpanFunc<TIn, in TArg, out TOut>(ReadOnlySpan<TIn> span, TArg arg);
    public delegate TOut ReadOnlySpanFunc<TIn, out TOut>(ReadOnlySpan<TIn>          span);

    public static ImmutableArray<TOut> ToImmutableArray<TIn, TArg, TOut>(this SpanSpliterator<TIn> spliterator, ReadOnlySpanFunc<TIn, TArg, TOut> factory, TArg arg)
        where TIn : IEquatable<TIn> {
        var arr          = ImmutableArray.CreateBuilder<TOut>();
        int currentIndex = 0;
        foreach (var span in spliterator) {
            arr[currentIndex] =  factory(span, arg);
            currentIndex      += 1;
        }

        return arr.MoveToImmutable();
    }

    public static ImmutableArray<TOut> ToImmutableArray<TIn, TOut>(this SpanSpliterator<TIn> spliterator, ReadOnlySpanFunc<TIn, TOut> factory)
        where TIn : IEquatable<TIn> {
        var arr          = ImmutableArray.CreateBuilder<TOut>();
        int currentIndex = 0;
        foreach (var span in spliterator) {
            arr[currentIndex] =  factory(span);
            currentIndex      += 1;
        }

        return arr.MoveToImmutable();
    }
}