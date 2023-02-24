using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Static factory methods for creating <see cref="SpanTuple{TA,TB,TC}"/> instances.
/// </summary>
/// <remarks>
/// This is a <see cref="ValueType"/> instead of a <c>static</c> class to match the built-in <see cref="ValueTuple"/>.
/// </remarks>
public readonly ref struct SpanTuple {
    [Pure] public static SpanTuple             Of()                                                       => default;
    [Pure] public static SpanTuple<A>          Of<A>(Span<A>          a)                                  => new(a);
    [Pure] public static SpanTuple<A, B>       Of<A, B>(Span<A>       a, Span<B> b)                       => new(a, b);
    [Pure] public static SpanTuple<A, B, C>    Of<A, B, C>(Span<A>    a, Span<B> b, Span<C> c)            => new(a, b, c);
    [Pure] public static SpanTuple<A, B, C, D> Of<A, B, C, D>(Span<A> a, Span<B> b, Span<C> c, Span<D> d) => new(a, b, c, d);

    public static implicit operator RoSpanTuple(SpanTuple self) => default;

    internal static NotSupportedException BecauseSpanDoesnt([CallerMemberName] string? _caller = default) => new($"{_caller}() isn't supported because {nameof(Span<byte>)} doesn't support it!");
    internal const  string                SpanDoesntSupport = $"{nameof(Span<byte>)} doesn't support this, so {nameof(SpanTuple<byte>)} doesn't either.";
}

/// <summary>
/// A <see cref="ValueTuple"/>-inspired wrapper around some <see cref="Span{T}"/>s.
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
public readonly ref struct SpanTuple<TA> {
    public Span<TA> A { get; init; }

    #region 'structors

    public SpanTuple(Span<TA> a) {
        A = a;
    }

    public void Deconstruct(out Span<TA> a) => a = A;

    #endregion

    #region Operators

    [Pure] public static bool operator ==(SpanTuple<TA> a, SpanTuple<TA> b) => a.A == b.A;
    [Pure] public static bool operator !=(SpanTuple<TA> a, SpanTuple<TA> b) => !(a == b);
    [Pure] public static bool operator ==(SpanTuple<TA> a, Span<TA>      b) => a.A == b;
    [Pure] public static bool operator !=(SpanTuple<TA> a, Span<TA>      b) => !(a == b);
    [Pure] public static bool operator ==(Span<TA>      a, SpanTuple<TA> b) => a == b.A;
    [Pure] public static bool operator !=(Span<TA>      a, SpanTuple<TA> b) => !(a == b);

    [Pure] public static implicit operator SpanTuple<TA>(Span<TA>        span)      => new(span);
    [Pure] public static implicit operator Span<TA>(SpanTuple<TA>        spanTuple) => spanTuple.A;
    [Pure] public static implicit operator RoSpanTuple<TA>(SpanTuple<TA> spanTuple) => new(spanTuple.A);

    #endregion

    #region Appending

    [Pure] public SpanTuple<TA, TB>         Append<TB>(Span<TB>                      other) => new(A, other);
    [Pure] public SpanTuple<TA, TB>         Append<TB>(SpanTuple<TB>                 other) => new(A, other.A);
    [Pure] public SpanTuple<TA, TB, TC>     Append<TB, TC>(SpanTuple<TB, TC>         other) => new(A, other.A, other.B);
    [Pure] public SpanTuple<TA, TB, TC, TD> Append<TB, TC, TD>(SpanTuple<TB, TC, TD> other) => new(A, other.A, other.B, other.C);

    #endregion

    #region object methods

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override bool Equals(object obj) => throw SpanTuple.BecauseSpanDoesnt();

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override int GetHashCode() => throw SpanTuple.BecauseSpanDoesnt();

    #endregion
}

/// <summary>
/// <inheritdoc cref="SpanTuple{T}"/> 
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
public readonly ref struct SpanTuple<TA, TB> {
    public Span<TA> A { get; init; }
    public Span<TB> B { get; init; }

    #region 'structors

    public SpanTuple(Span<TA> a, Span<TB> b) {
        A = a;
        B = b;
    }

    public void Deconstruct(out Span<TA> a, out Span<TB> b) {
        a = A;
        b = B;
    }

    #endregion

    #region Operators

    [Pure] public static                   bool operator ==(SpanTuple<TA, TB>    a, SpanTuple<TA, TB> b) => a.A == b.A && a.B == b.B;
    [Pure] public static                   bool operator !=(SpanTuple<TA, TB>    a, SpanTuple<TA, TB> b) => !(a == b);
    [Pure] public static implicit operator RoSpanTuple<TA, TB>(SpanTuple<TA, TB> spanTuple) => new(spanTuple.A, spanTuple.B);

    [Pure] public SpanTuple<TA, TB, TC>     Append<TC>(Span<TC>              other) => new(A, B, other);
    [Pure] public SpanTuple<TA, TB, TC>     Append<TC>(SpanTuple<TC>         other) => new(A, B, other.A);
    [Pure] public SpanTuple<TA, TB, TC, TD> Append<TC, TD>(SpanTuple<TC, TD> other) => new(A, B, other.A, other.B);

    #endregion

    #region object methods

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override bool Equals(object obj) => throw SpanTuple.BecauseSpanDoesnt();

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override int GetHashCode() => throw SpanTuple.BecauseSpanDoesnt();

    #endregion
}

/// <summary>
/// <inheritdoc cref="SpanTuple{T}"/> 
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
/// <typeparam name="TC"><see cref="C"/>'s element type</typeparam>
public readonly ref struct SpanTuple<TA, TB, TC> {
    public Span<TA> A { get; init; }
    public Span<TB> B { get; init; }
    public Span<TC> C { get; init; }

    public SpanTuple(Span<TA> a, Span<TB> b, Span<TC> c) {
        A = a;
        B = b;
        C = c;
    }

    public void Deconstruct(out Span<TA> a, out Span<TB> b, out Span<TC> c) {
        a = A;
        b = B;
        c = C;
    }

    [Pure] public static                   bool operator ==(SpanTuple<TA, TB, TC>        a, SpanTuple<TA, TB, TC> b) => a.A == b.A && a.B == b.B && a.C == b.C;
    [Pure] public static                   bool operator !=(SpanTuple<TA, TB, TC>        a, SpanTuple<TA, TB, TC> b) => !(a == b);
    [Pure] public static implicit operator RoSpanTuple<TA, TB, TC>(SpanTuple<TA, TB, TC> spanTuple) => new(spanTuple.A, spanTuple.B, spanTuple.C);

    [Pure] public SpanTuple<TA, TB, TC, TD> Append<TD>(Span<TD>      other) => new(A, B, C, other);
    [Pure] public SpanTuple<TA, TB, TC, TD> Append<TD>(SpanTuple<TD> other) => new(A, B, C, other.A);

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override bool Equals(object obj) => throw SpanTuple.BecauseSpanDoesnt();

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override int GetHashCode() => throw SpanTuple.BecauseSpanDoesnt();
}

/// <summary>
/// <inheritdoc cref="SpanTuple{T}"/> 
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
/// <typeparam name="TC"><see cref="C"/>'s element type</typeparam>
/// <typeparam name="TD"><see cref="D"/>'s element type</typeparam>
public readonly ref struct SpanTuple<TA, TB, TC, TD> {
    public Span<TA> A { get; init; }
    public Span<TB> B { get; init; }
    public Span<TC> C { get; init; }
    public Span<TD> D { get; init; }

    public SpanTuple(Span<TA> a, Span<TB> b, Span<TC> c, Span<TD> d) {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public void Deconstruct(out Span<TA> a, out Span<TB> b, out Span<TC> c, out Span<TD> d) {
        a = A;
        b = B;
        c = C;
        d = D;
    }

    [Pure] public static                   bool operator ==(SpanTuple<TA, TB, TC, TD>            a, SpanTuple<TA, TB, TC, TD> b) => a.A == b.A && a.B == b.B && a.C == b.C;
    [Pure] public static                   bool operator !=(SpanTuple<TA, TB, TC, TD>            a, SpanTuple<TA, TB, TC, TD> b) => !(a == b);
    [Pure] public static implicit operator RoSpanTuple<TA, TB, TC, TD>(SpanTuple<TA, TB, TC, TD> spanTuple) => new(spanTuple.A, spanTuple.B, spanTuple.C, spanTuple.D);

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override bool Equals(object obj) => throw SpanTuple.BecauseSpanDoesnt();

    [Obsolete(SpanTuple.SpanDoesntSupport), DoesNotReturn]
    public override int GetHashCode() => throw SpanTuple.BecauseSpanDoesnt();
}