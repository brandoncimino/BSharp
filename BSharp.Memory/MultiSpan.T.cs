using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// A mutable, <see cref="Span{T}"/>-based equivalent of <see cref="RoMultiSpan{T}"/>.
/// </summary>
/// <typeparam name="T">the span element type</typeparam>
/// <seealso cref="RoMultiSpan{T}"/>
public readonly ref struct MultiSpan<T> {
    /// <summary>
    /// The max number of <see cref="Span{T}"/>s that a <see cref="MultiSpan{T}"/> can support.
    /// </summary>
    private const int MaxSpans = RoMultiSpan.MaxSpans;

    /// <inheritdoc cref="SpanCount"/>
    [UsedImplicitly]
    [ValueRange(0, MaxSpans)]
    public int Count => SpanCount;

    /// <summary>
    /// The number of <b><see cref="Span{T}"/>s</b> in this <see cref="MultiSpan{T}"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="Count"/> property is supported for consistency with standard collections and for <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges#implicit-range-support">implicit Range and Index support</a>,
    /// but it is generally better to use the explicit <see cref="SpanCount"/> to prevent confusion with <see cref="ElementCount"/>. 
    /// </remarks>
    [ValueRange(0, MaxSpans)]
    public int SpanCount { get; private init; }

    /// <summary>
    /// The total <see cref="Span{T}.Length"/> of all of the contained <see cref="Span{T}"/>s.
    /// </summary>
    [NonNegativeValue]
    public int ElementCount => _a.Length +
                               _b.Length +
                               _c.Length +
                               _d.Length +
                               _e.Length +
                               _f.Length +
                               _g.Length +
                               _h.Length;

    #region Storage

    private Span<T> _a { get; init; }
    private Span<T> _b { get; init; }
    private Span<T> _c { get; init; }
    private Span<T> _d { get; init; }
    private Span<T> _e { get; init; }
    private Span<T> _f { get; init; }
    private Span<T> _g { get; init; }
    private Span<T> _h { get; init; }

    #endregion

    #region Constructors

    private MultiSpan(
        int     spanCount,
        Span<T> a = default,
        Span<T> b = default,
        Span<T> c = default,
        Span<T> d = default,
        Span<T> e = default,
        Span<T> f = default,
        Span<T> g = default,
        Span<T> h = default
    ) {
        SpanCount = spanCount;
        _a        = a;
        _b        = b;
        _c        = c;
        _d        = d;
        _e        = e;
        _f        = f;
        _g        = g;
        _h        = h;
    }

    public MultiSpan() : this(0) { }
    public MultiSpan(Span<T> a) : this(1, a) { }
    public MultiSpan(Span<T> a, Span<T> b) : this(2, a, b) { }
    public MultiSpan(Span<T> a, Span<T> b, Span<T> c) : this(3, a, b, c) { }
    public MultiSpan(Span<T> a, Span<T> b, Span<T> c, Span<T> d) : this(4, a, b, c, d) { }

    public MultiSpan(
        Span<T> a,
        Span<T> b,
        Span<T> c,
        Span<T> d,
        Span<T> e
    ) : this(5, a, b, c, d, e) { }

    public MultiSpan(
        Span<T> a,
        Span<T> b,
        Span<T> c,
        Span<T> d,
        Span<T> e,
        Span<T> f
    ) : this(6, a, b, c, d, e, f) { }

    public MultiSpan(
        Span<T> a,
        Span<T> b,
        Span<T> c,
        Span<T> d,
        Span<T> e,
        Span<T> f,
        Span<T> g
    ) : this(7, a, b, c, d, e, f, g) { }

    public MultiSpan(
        Span<T> a,
        Span<T> b,
        Span<T> c,
        Span<T> d,
        Span<T> e,
        Span<T> f,
        Span<T> g,
        Span<T> h
    ) : this(8, a, b, c, d, e, f, g, h) { }

    #endregion

    /// <summary>
    /// Converts this <see cref="MultiSpan{T}"/> into an equivalent <see cref="RoMultiSpan{T}"/>.
    /// </summary>
    /// <returns>an equivalent <see cref="RoMultiSpan{T}"/></returns>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="SpanCount"/> is less than 0 or greater than <see cref="MaxSpans"/></exception>
    [Pure]
    public RoMultiSpan<T> AsReadOnly() {
        return SpanCount switch {
            0 => default,
            1 => new RoMultiSpan<T>(_a),
            2 => new RoMultiSpan<T>(_a, _b),
            3 => new RoMultiSpan<T>(_a, _b, _c),
            4 => new RoMultiSpan<T>(_a, _b, _c, _d),
            5 => new RoMultiSpan<T>(_a, _b, _c, _d, _e),
            6 => new RoMultiSpan<T>(_a, _b, _c, _d, _e, _f),
            7 => new RoMultiSpan<T>(_a, _b, _c, _d, _e, _f, _g),
            8 => new RoMultiSpan<T>(_a, _b, _c, _d, _e, _f, _g, _h),
            _ => throw new ArgumentOutOfRangeException(nameof(SpanCount), SpanCount, "This should have been unreachable!"),
        };
    }

    [Pure, UsedImplicitly]
    public RoMultiSpan<T>.SpanEnumerator GetEnumerator() {
        return AsReadOnly().GetEnumerator();
    }

    public static implicit operator RoMultiSpan<T>(MultiSpan<T> spans) => spans.AsReadOnly();
}