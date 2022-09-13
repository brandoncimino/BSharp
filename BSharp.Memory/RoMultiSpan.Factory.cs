using System;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Static factory methods for constructing <see cref="RoMultiSpan{T}"/>s.
/// </summary>
public static class RoMultiSpan {
    /// <summary>
    /// The max number of <see cref="ReadOnlySpan{T}"/>s that a <see cref="RoMultiSpan{T}"/> can support.
    /// </summary>
    internal const int MaxSpans = 8;

    public static RoMultiSpan<T> Of<T>()                                                                           => new();
    public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a)                                                          => new(a);
    public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b)                                       => new(a, b);
    public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, ReadOnlySpan<T> c)                    => new(a, b, c);
    public static RoMultiSpan<T> Of<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, ReadOnlySpan<T> c, ReadOnlySpan<T> d) => new(a, b, c, d);

    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e
    ) => new(a, b, c, d, e);

    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f
    ) => new(a, b, c, d, e, f);

    public static RoMultiSpan<T> Of<T>(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f,
        ReadOnlySpan<T> g
    ) => new(a, b, c, d, e, f, g);

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

    /// <inheritdoc cref="RoMultiSpan{T}.Builder"/>
    public static RoMultiSpan<T>.Builder CreateBuilder<T>() => new();
}