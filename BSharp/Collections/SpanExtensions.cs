using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static class SpanExtensions {
    #region AfterFirst

    [Pure]
    public static ReadOnlySpan<T> AfterFirst<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Skip(span.IndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> AfterFirstAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Skip(span.IndexOfAny(splitters));

    #endregion

    #region BeforeFirst

    [Pure]
    public static ReadOnlySpan<T> BeforeFirst<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Take(span.IndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> BeforeFirstAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Take(span.IndexOfAny(splitters));

    #endregion

    #region AfterLast

    [Pure]
    public static ReadOnlySpan<T> AfterLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Skip(span.LastIndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> AfterLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Skip(span.LastIndexOfAny(splitters));

    #endregion

    #region BeforeLast

    [Pure]
    public static ReadOnlySpan<T> BeforeLast<T>(this ReadOnlySpan<T> span, T splitter)
        where T : IEquatable<T> => span.Take(span.LastIndexOf(splitter));

    [Pure]
    public static ReadOnlySpan<T> BeforeLastAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters)
        where T : IEquatable<T> => span.Take(span.LastIndexOfAny(splitters));

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

    /// <summary>
    /// Roughly equivalent to .NET 6's <a href="https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.take?view=net-6.0#system-linq-enumerable-take-1(system-collections-generic-ienumerable((-0))-system-range)">Enumerable.Take(source, Range)</a>
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="range">the desired <see cref="Range"/> of entries</param>
    /// <typeparam name="T">the type of entries in the <paramref name="span"/></typeparam>
    /// <returns>as much of the <paramref name="span"/> as the <see cref="Range"/> overlaps</returns>
    [Pure]
    public static ReadOnlySpan<T> SafeSlice<T>(this ReadOnlySpan<T> span, Range range) => span[range.Clamp(span.Length)];

    #region Skip

    [Pure]
    public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int toSkip) => toSkip switch {
        < 0 => span,
        _   => span.SafeSlice(toSkip..)
    };

    [Pure]
    public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int toSkip) {
        return toSkip switch {
            < 0 => span,
            _   => span.SafeSlice(..^toSkip)
        };
    }

    #endregion

    #region Skip

    [Pure]
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int toTake) {
        return toTake switch {
            < 0 => default,
            _   => span.SafeSlice(..toTake)
        };
    }

    [Pure]
    public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int toTake) {
        return toTake switch {
            < 0 => default,
            _   => span.SafeSlice(^toTake..)
        };
    }

    #endregion

    public static int StartingMatchCount<T>(this ReadOnlySpan<T> span, T toMatch, int? matchLimit = default)
        where T : IEquatable<T> {
        var ml = matchLimit ?? span.Length;
        for (int i = 0; i < ml; i++) {
            if (span[i].Equals(toMatch) == false) {
                return i;
            }
        }

        return ml;
    }

    public static int EndingMatchCount<T>(this ReadOnlySpan<T> span, T toMatch, int? matchLimit = default)
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
        return span.Skip(span.StartingMatchCount(toTrim, numberToTrim));
    }

    public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> span, T toTrim, int? numberToTrim = default)
        where T : IEquatable<T> {
        return span.SkipLast(span.StartingMatchCount(toTrim, numberToTrim));
    }

    #endregion

    #region Spliterator

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, params T[] splitters)
        where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitters, SplitterStyle.AnyEntry);
    }

    [Pure]
    public static SpanSpliterator<T> Spliterate<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitters, SplitterStyle splitterStyle = SplitterStyle.AnyEntry)
        where T : IEquatable<T> {
        return new SpanSpliterator<T>(span, splitters, splitterStyle);
    }

    public static ImmutableArray<string> ToStringArray(this SpanSpliterator<char> spliterator) {
        var arr = ImmutableArray.CreateBuilder<string>();

        foreach (var span in spliterator) {
            arr.Add(span.ToString());
        }

        return arr.MoveToImmutableSafely();
    }

    public delegate TOut ReadOnlySpanFunc<TIn, in TArg, out TOut>(ReadOnlySpan<TIn> span, TArg arg);
    public delegate TOut ReadOnlySpanFunc<TIn, out TOut>(ReadOnlySpan<TIn>          span);
    public delegate TOut SpanFunc<TIn, in TArg, out TOut>(Span<TIn>                 span, TArg arg);
    public delegate TOut SpanFunc<TIn, out TOut>(Span<TIn>                          span);

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

    public static ImmutableArray<TOut> ToImmutableArray<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut> factory)
        where TSpan : IEquatable<TSpan> {
        var arr = ImmutableArray.CreateBuilder<TOut>();
        foreach (var span in spliterator) {
            arr.Add(factory(span));
        }

        return arr.MoveToImmutableSafely();
    }

    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] Func<TOut> initialFactory, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator)
        where TSpan : IEquatable<TSpan> {
        var soFar = initialFactory();
        foreach (var span in spliterator) {
            soFar = aggregator(span, soFar);
        }

        return soFar;
    }

    #endregion
}