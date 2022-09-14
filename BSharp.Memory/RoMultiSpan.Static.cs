using System;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Static factory and extension methods for <see cref="RoMultiSpan{T}"/> and <see cref="RoMultiSpan{T}.Builder"/>.
/// </summary>
public static class RoMultiSpan {
    /// <summary>
    /// The max number of <see cref="ReadOnlySpan{T}"/>s that a <see cref="RoMultiSpan{T}"/> can support.
    /// </summary>
    internal const int MaxSpans = 8;

    [Pure] public static RoMultiSpan<T> Of<T>()                                                                           => new();
    [Pure] public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a)                                                          => new(a);
    [Pure] public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b)                                       => new(a, b);
    [Pure] public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, ReadOnlySpan<T> c)                    => new(a, b, c);
    [Pure] public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, ReadOnlySpan<T> c, ReadOnlySpan<T> d) => new(a, b, c, d);

    [Pure]
    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e
    ) => new(a, b, c, d, e);

    [Pure]
    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f
    ) => new(a, b, c, d, e, f);

    [Pure]
    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f,
        ReadOnlySpan<T> g
    ) => new(a, b, c, d, e, f, g);

    [Pure]
    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f,
        ReadOnlySpan<T> g,
        ReadOnlySpan<T> h
    ) => new(a, b, c, d, e, f, g, h);

    [Pure]
    public static RoMultiSpan<T> Of<T>(params T[][] arrays) {
        if (arrays.Length > MaxSpans) {
            throw new ArgumentOutOfRangeException($"🙅‍♀️ Cannot create a {nameof(RoMultiSpan<T>)} out of {arrays.Length} arrays: it would exceed the {nameof(MaxSpans)} limit of {MaxSpans}!");
        }

        var builder = CreateBuilder<T>();
        foreach (var ar in arrays) {
            builder.Add(ar);
        }

        return builder.Build();
    }

    [Pure]
    public static RoMultiSpan<char> Of(params string[] strings) {
        if (strings.Length > MaxSpans) {
            throw new ArgumentOutOfRangeException($"🙅‍♀️ Cannot create a {nameof(RoMultiSpan<char>)} out of {strings.Length} strings: it would exceed the {nameof(MaxSpans)} limit of {MaxSpans}!");
        }

        var builder = CreateBuilder<char>();
        foreach (var str in strings) {
            builder.Add(str);
        }

        return builder.Build();
    }

    #region AddRange

    public static RoMultiSpan<T>.Builder AddRange<T>(this RoMultiSpan<T>.Builder builder, params T[][] arrays) {
        builder.Count.RequireSpace(arrays.Length);

        foreach (var ar in arrays) {
            builder.Add(ar);
        }

        return builder;
    }

    [Pure]
    public static RoMultiSpan<T> AddRange<T>(this RoMultiSpan<T> spans, params T[][] arrays) {
        spans.SpanCount.RequireSpace(arrays.Length);
        return spans.ToBuilder().AddRange(arrays);
    }

    public static RoMultiSpan<char>.Builder AddRange(this RoMultiSpan<char>.Builder builder, params string[] strings) {
        builder.Count.RequireSpace(strings.Length);

        foreach (var str in strings) {
            builder.Add(str);
        }

        return builder;
    }

    [Pure]
    public static RoMultiSpan<char> AddRange(this RoMultiSpan<char> builder, params string[] strings) {
        builder.Count.RequireSpace(strings.Length);
        return builder.ToBuilder().AddRange(strings);
    }

    #endregion

    /// <inheritdoc cref="RoMultiSpan{T}.Builder"/>
    public static RoMultiSpan<T>.Builder CreateBuilder<T>() => new();
}