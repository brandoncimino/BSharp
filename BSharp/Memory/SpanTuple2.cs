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
    private readonly SpanTuple2<T, T> _spans;

    public int  Length  => _spans.Length;
    public bool IsEmpty => _spans.IsEmpty;

    public static SpanTuple2<T> Empty => default;

    public SpanTuple2(ReadOnlySpan<T> a, ReadOnlySpan<T> b) {
        _spans = new SpanTuple2<T, T>(a, b);
    }

    public ReadOnlySpan<T> A {
        get => _spans.A;
        init => _spans = _spans with { A = value };
    }

    public ReadOnlySpan<T> B {
        get => _spans.B;
        init => _spans = _spans with { B = value };
    }

    public void Deconstruct(out ReadOnlySpan<T> a, out ReadOnlySpan<T> b) {
        a = A;
        b = B;
    }
}