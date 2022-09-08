using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Static factory methods for creating <see cref="RoSpanTuple{TA,TB,TC}"/> instances.
/// </summary>
/// <remarks>
/// This is a <see cref="ValueType"/> instead of a <c>static</c> class to match the built-in <see cref="ValueTuple"/>.
/// </remarks>
public readonly ref struct RoSpanTuple {
    public static RoSpanTuple          Of()                                                                 => default;
    public static RoSpanTuple<A>       Of<A>(ReadOnlySpan<A>       a)                                       => new(a);
    public static RoSpanTuple<A, B>    Of<A, B>(ReadOnlySpan<A>    a, ReadOnlySpan<B> b)                    => new(a, b);
    public static RoSpanTuple<A, B, C> Of<A, B, C>(ReadOnlySpan<A> a, ReadOnlySpan<B> b, ReadOnlySpan<C> c) => new(a, b, c);

    internal static NotSupportedException BecauseSpanDoesnt([CallerMemberName] string? _caller = default) => new NotSupportedException($"{_caller}() isn't supported because {nameof(ReadOnlySpan<byte>)} doesn't support it!");
}

/// <summary>
/// A <see cref="ValueTuple"/>-inspired wrapper around some <see cref="ReadOnlySpan{T}"/>s.
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
public readonly ref struct RoSpanTuple<TA> {
    public ReadOnlySpan<TA> A { get; init; }
    public RoSpanTuple(ReadOnlySpan<TA>          a) => A = a;
    public void Deconstruct(out ReadOnlySpan<TA> a) => a = A;

    public static bool operator ==(RoSpanTuple<TA>  a, RoSpanTuple<TA>  b) => a.A == b.A;
    public static bool operator !=(RoSpanTuple<TA>  a, RoSpanTuple<TA>  b) => !(a == b);
    public static bool operator ==(RoSpanTuple<TA>  a, ReadOnlySpan<TA> b) => a.A == b;
    public static bool operator !=(RoSpanTuple<TA>  a, ReadOnlySpan<TA> b) => !(a == b);
    public static bool operator ==(ReadOnlySpan<TA> a, RoSpanTuple<TA>  b) => a == b.A;
    public static bool operator !=(ReadOnlySpan<TA> a, RoSpanTuple<TA>  b) => !(a == b);

    public static implicit operator RoSpanTuple<TA>(ReadOnlySpan<TA> span)      => new(span);
    public static implicit operator ReadOnlySpan<TA>(RoSpanTuple<TA> spanTuple) => spanTuple.A;

    public RoSpanTuple<TA, TB>         Append<TB>(ReadOnlySpan<TB>                other) => new(A, other);
    public RoSpanTuple<TA, TB>         Append<TB>(RoSpanTuple<TB>                 other) => new(A, other.A);
    public RoSpanTuple<TA, TB, TC>     Append<TB, TC>(RoSpanTuple<TB, TC>         other) => new(A, other.A, other.B);
    public RoSpanTuple<TA, TB, TC, TD> Append<TB, TC, TD>(RoSpanTuple<TB, TC, TD> other) => new(A, other.A, other.B, other.C);

    [Obsolete] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();
}

/// <summary>
/// <inheritdoc cref="RoSpanTuple{T}"/> 
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
public readonly ref struct RoSpanTuple<TA, TB> {
    public ReadOnlySpan<TA> A { get; init; }
    public ReadOnlySpan<TB> B { get; init; }

    public RoSpanTuple(ReadOnlySpan<TA> a, ReadOnlySpan<TB> b) {
        A = a;
        B = b;
    }

    public void Deconstruct(out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b) {
        a = A;
        b = B;
    }

    public static bool operator ==(RoSpanTuple<TA, TB> a, RoSpanTuple<TA, TB> b) => a.A == b.A && a.B == b.B;
    public static bool operator !=(RoSpanTuple<TA, TB> a, RoSpanTuple<TA, TB> b) => !(a == b);

    public RoSpanTuple<TA, TB, TC>     Append<TC>(ReadOnlySpan<TC>        other) => new(A, B, other);
    public RoSpanTuple<TA, TB, TC>     Append<TC>(RoSpanTuple<TC>         other) => new(A, B, other.A);
    public RoSpanTuple<TA, TB, TC, TD> Append<TC, TD>(RoSpanTuple<TC, TD> other) => new(A, B, other.A, other.B);

    [Obsolete] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();
}

/// <summary>
/// <inheritdoc cref="RoSpanTuple{T}"/> 
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
/// <typeparam name="TC"><see cref="C"/>'s element type</typeparam>
public readonly ref struct RoSpanTuple<TA, TB, TC> {
    public ReadOnlySpan<TA> A { get; init; }
    public ReadOnlySpan<TB> B { get; init; }
    public ReadOnlySpan<TC> C { get; init; }

    public RoSpanTuple(ReadOnlySpan<TA> a, ReadOnlySpan<TB> b, ReadOnlySpan<TC> c) {
        A = a;
        B = b;
        C = c;
    }

    public void Deconstruct(out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b, out ReadOnlySpan<TC> c) {
        a = A;
        b = B;
        c = C;
    }

    public static bool operator ==(RoSpanTuple<TA, TB, TC> a, RoSpanTuple<TA, TB, TC> b) => a.A == b.A && a.B == b.B && a.C == b.C;
    public static bool operator !=(RoSpanTuple<TA, TB, TC> a, RoSpanTuple<TA, TB, TC> b) => !(a == b);

    public RoSpanTuple<TA, TB, TC, TD> Append<TD>(ReadOnlySpan<TD> other) => new(A, B, C, other);
    public RoSpanTuple<TA, TB, TC, TD> Append<TD>(RoSpanTuple<TD>  other) => new(A, B, C, other.A);

    [Obsolete] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();
}

/// <summary>
/// <inheritdoc cref="RoSpanTuple{T}"/> 
/// </summary>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
/// <typeparam name="TC"><see cref="C"/>'s element type</typeparam>
/// <typeparam name="TD"><see cref="D"/>'s element type</typeparam>
public readonly ref struct RoSpanTuple<TA, TB, TC, TD> {
    public ReadOnlySpan<TA> A { get; init; }
    public ReadOnlySpan<TB> B { get; init; }
    public ReadOnlySpan<TC> C { get; init; }
    public ReadOnlySpan<TD> D { get; init; }

    public RoSpanTuple(ReadOnlySpan<TA> a, ReadOnlySpan<TB> b, ReadOnlySpan<TC> c, ReadOnlySpan<TD> d) {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public void Deconstruct(out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b, out ReadOnlySpan<TC> c, out ReadOnlySpan<TD> d) {
        a = A;
        b = B;
        c = C;
        d = D;
    }

    public static bool operator ==(RoSpanTuple<TA, TB, TC, TD> a, RoSpanTuple<TA, TB, TC, TD> b) => a.A == b.A && a.B == b.B && a.C == b.C;
    public static bool operator !=(RoSpanTuple<TA, TB, TC, TD> a, RoSpanTuple<TA, TB, TC, TD> b) => !(a == b);

    [Obsolete] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();
}