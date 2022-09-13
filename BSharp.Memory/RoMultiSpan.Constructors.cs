using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    #region Constructors

    private RoMultiSpan(
        int             spanCount,
        ReadOnlySpan<T> a = default,
        ReadOnlySpan<T> b = default,
        ReadOnlySpan<T> c = default,
        ReadOnlySpan<T> d = default,
        ReadOnlySpan<T> e = default,
        ReadOnlySpan<T> f = default,
        ReadOnlySpan<T> g = default,
        ReadOnlySpan<T> h = default
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

    public RoMultiSpan() : this(0) { }
    public RoMultiSpan(ReadOnlySpan<T> a) : this(1, a) { }
    public RoMultiSpan(ReadOnlySpan<T> a, ReadOnlySpan<T> b) : this(2, a, b) { }
    public RoMultiSpan(ReadOnlySpan<T> a, ReadOnlySpan<T> b, ReadOnlySpan<T> c) : this(3, a, b, c) { }
    public RoMultiSpan(ReadOnlySpan<T> a, ReadOnlySpan<T> b, ReadOnlySpan<T> c, ReadOnlySpan<T> d) : this(4, a, b, c, d) { }

    public RoMultiSpan(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e
    ) : this(5, a, b, c, d, e) { }

    public RoMultiSpan(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f
    ) : this(6, a, b, c, d, e, f) { }

    public RoMultiSpan(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f,
        ReadOnlySpan<T> g
    ) : this(7, a, b, c, d, e, f, g) { }

    public RoMultiSpan(
        ReadOnlySpan<T> a,
        ReadOnlySpan<T> b,
        ReadOnlySpan<T> c,
        ReadOnlySpan<T> d,
        ReadOnlySpan<T> e,
        ReadOnlySpan<T> f,
        ReadOnlySpan<T> g,
        ReadOnlySpan<T> h
    ) : this(8, a, b, c, d, e, f, g, h) { }

    #endregion
}