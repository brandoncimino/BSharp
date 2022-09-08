using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    public void Deconstruct(out ReadOnlySpan<T> a) {
        a = this[0];
    }

    public void Deconstruct(out ReadOnlySpan<T> a, out ReadOnlySpan<T> b) {
        a = this[0];
        b = this[1];
    }

    public void Deconstruct(out ReadOnlySpan<T> a, out ReadOnlySpan<T> b, out ReadOnlySpan<T> c) {
        (a, b) = this;
        c      = this[2];
    }

    public void Deconstruct(out ReadOnlySpan<T> a, out ReadOnlySpan<T> b, out ReadOnlySpan<T> c, out ReadOnlySpan<T> d) {
        (a, b, c) = this;
        d         = this[3];
    }

    public void Deconstruct(
        out ReadOnlySpan<T> a,
        out ReadOnlySpan<T> b,
        out ReadOnlySpan<T> c,
        out ReadOnlySpan<T> d,
        out ReadOnlySpan<T> e
    ) {
        (a, b, c, d) = this;
        e            = this[4];
    }

    public void Deconstruct(
        out ReadOnlySpan<T> a,
        out ReadOnlySpan<T> b,
        out ReadOnlySpan<T> c,
        out ReadOnlySpan<T> d,
        out ReadOnlySpan<T> e,
        out ReadOnlySpan<T> f
    ) {
        (a, b, c, d, e) = this;
        f               = this[5];
    }

    public void Deconstruct(
        out ReadOnlySpan<T> a,
        out ReadOnlySpan<T> b,
        out ReadOnlySpan<T> c,
        out ReadOnlySpan<T> d,
        out ReadOnlySpan<T> e,
        out ReadOnlySpan<T> f,
        out ReadOnlySpan<T> g
    ) {
        (a, b, c, d, e, f) = this;
        g                  = this[6];
    }

    public void Deconstruct(
        out ReadOnlySpan<T> a,
        out ReadOnlySpan<T> b,
        out ReadOnlySpan<T> c,
        out ReadOnlySpan<T> d,
        out ReadOnlySpan<T> e,
        out ReadOnlySpan<T> f,
        out ReadOnlySpan<T> g,
        out ReadOnlySpan<T> h
    ) {
        (a, b, c, d, e, f, g) = this;
        h                     = this[7];
    }
}