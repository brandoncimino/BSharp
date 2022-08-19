using System;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Contains 2 <see cref="ReadOnlySpan{T}"/>s, sorta like a <see cref="ValueTuple{T1,T2}"/>.
/// </summary>
/// <typeparam name="TA">the type of <see cref="A"/></typeparam>
/// <typeparam name="TB">the type of <see cref="B"/></typeparam>
public readonly ref struct SpanTuple2<TA, TB> {
    public ReadOnlySpan<TA> A { get; init; }
    public ReadOnlySpan<TB> B { get; init; }

    public int  Length  => A.Length + B.Length;
    public bool IsEmpty => Length == 0;

    public static SpanTuple2<TA, TB> Empty => default;

    public SpanTuple2(ReadOnlySpan<TA> a, ReadOnlySpan<TB> b) {
        A = a;
        B = b;
    }

    public void Deconstruct(out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b) {
        a = A;
        b = B;
    }
}

/// <inheritdoc cref="SpanTuple2{TA,TB}"/>
public readonly ref struct SpanTuple2<T> {
    public ReadOnlySpan<T> A       { get; init; }
    public ReadOnlySpan<T> B       { get; init; }
    public int             Length  => A.Length + B.Length;
    public bool            IsEmpty => Length == 0;

    public SpanTuple2(ReadOnlySpan<T> a, ReadOnlySpan<T> b) {
        A = a;
        B = b;
    }

    public void Deconstruct(out ReadOnlySpan<T> a, out ReadOnlySpan<T> b) {
        a = A;
        b = B;
    }
}

public readonly ref struct Span2Extra<TA, TB, TExtra> {
    public ReadOnlySpan<TA>   A     { get; init; }
    public ReadOnlySpan<TB>   B     { get; init; }
    public TExtra             Extra { get; init; }
    public SpanTuple2<TA, TB> Spans => new(A, B);

    public void Deconstruct(out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b, out TExtra extra) {
        a     = A;
        b     = B;
        extra = Extra;
    }

    public void Deconstruct(out SpanTuple2<TA, TB> spans, out TExtra extra) {
        spans = Spans;
        extra = Extra;
    }
}

public readonly ref struct Span2Extra<T, TExtra> {
    public ReadOnlySpan<T> A     { get; init; }
    public ReadOnlySpan<T> B     { get; init; }
    public SpanTuple2<T>   Spans => new(A, B);
    public TExtra          Extra { get; init; }

    public Span2Extra(ReadOnlySpan<T> a, ReadOnlySpan<T> b, TExtra extra) {
        A     = a;
        B     = b;
        Extra = extra;
    }

    public Span2Extra(TExtra extra, ReadOnlySpan<T> a, ReadOnlySpan<T> b) : this(a, b, extra) { }

    public void Deconstruct(out ReadOnlySpan<T> a, out ReadOnlySpan<T> b, out TExtra extra) {
        a     = A;
        b     = B;
        extra = Extra;
    }
}