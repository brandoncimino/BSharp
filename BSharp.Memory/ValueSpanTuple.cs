using System;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// Combines a non-span <see cref="Value"/> with a span <see cref="A"/>.
/// </summary>
/// <remarks>
/// You can store multiple non-span values in <see cref="Value"/> by using a <see cref="ValueTuple"/> type.
/// </remarks>
/// <typeparam name="TValue">the non-span <see cref="Value"/> type</typeparam>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
public readonly ref struct ValueSpanTuple<TValue, TA> {
    public TValue Value { get; init; }

    #region Spans

    public RoSpanTuple<TA> Spans { get; init; }

    public ReadOnlySpan<TA> A {
        get => Spans.A;
        init => Spans = new RoSpanTuple<TA>(value);
    }

    #endregion

    #region 'structors

    public ValueSpanTuple(TValue value, RoSpanTuple<TA> spans) {
        Value = value;
        Spans = spans;
    }

    public ValueSpanTuple(TValue value, ReadOnlySpan<TA> a) : this(value, new RoSpanTuple<TA>(a)) { }

    public void Deconstruct(out TValue value, out ReadOnlySpan<TA> a) {
        value = Value;
        a     = A;
    }

    #endregion

    #region object methods

    [Obsolete, DoesNotReturn] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete, DoesNotReturn] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();

    #endregion
}

/// <summary>
/// Combines a non-span <see cref="Value"/> with spans <see cref="A"/> and <see cref="B"/>.
/// </summary>
/// <remarks><inheritdoc cref="ValueSpanTuple{TValue,TA}"/></remarks>
/// <typeparam name="TValue">the non-span <see cref="Value"/> type</typeparam>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
public readonly ref struct ValueSpanTuple<TValue, TA, TB> {
    public TValue Value { get; init; }

    #region Spans

    public RoSpanTuple<TA, TB> Spans { get; init; }

    public ReadOnlySpan<TA> A {
        get => Spans.A;
        init => Spans = Spans with { A = value };
    }

    public ReadOnlySpan<TB> B {
        get => Spans.B;
        init => Spans = Spans with { B = value };
    }

    #endregion

    #region 'structors

    public ValueSpanTuple(TValue value, RoSpanTuple<TA, TB> spans) {
        Value = value;
        Spans = spans;
    }

    public ValueSpanTuple(TValue value, ReadOnlySpan<TA> a, ReadOnlySpan<TB> b) : this(value, new RoSpanTuple<TA, TB>(a, b)) { }

    public void Deconstruct(out TValue value, out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b) {
        value = Value;
        a     = A;
        b     = B;
    }

    #endregion

    #region object methods

    [Obsolete, DoesNotReturn] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete, DoesNotReturn] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();

    #endregion
}

/// <summary>
/// Combines a non-span <see cref="Value"/> with spans <see cref="A"/>, <see cref="B"/> and <see cref="C"/>.
/// </summary>
/// /// <remarks><inheritdoc cref="ValueSpanTuple{TValue,TA}"/></remarks>
/// <typeparam name="TValue">the non-span <see cref="Value"/> type</typeparam>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
/// <typeparam name="TC"><see cref="C"/>'s element type</typeparam>
public readonly ref struct ValueSpanTuple<TValue, TA, TB, TC> {
    public TValue Value { get; init; }

    #region Spans

    public RoSpanTuple<TA, TB, TC> Spans { get; init; }

    public ReadOnlySpan<TA> A {
        get => Spans.A;
        init => Spans = Spans with { A = value };
    }

    public ReadOnlySpan<TB> B {
        get => Spans.B;
        init => Spans = Spans with { B = value };
    }

    public ReadOnlySpan<TC> C {
        get => Spans.C;
        init => Spans = Spans with { C = value };
    }

    #endregion

    #region 'structors

    public ValueSpanTuple(TValue value, RoSpanTuple<TA, TB, TC> spans) {
        Value = value;
        Spans = spans;
    }

    public ValueSpanTuple(TValue value, ReadOnlySpan<TA> a, ReadOnlySpan<TB> b, ReadOnlySpan<TC> c) : this(value, new RoSpanTuple<TA, TB, TC>(a, b, c)) { }

    public void Deconstruct(out TValue value, out ReadOnlySpan<TA> a, out ReadOnlySpan<TB> b, out ReadOnlySpan<TC> c) {
        value = Value;
        a     = A;
        b     = B;
        c     = C;
    }

    #endregion

    #region object methods

    [Obsolete, DoesNotReturn] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete, DoesNotReturn] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();

    #endregion
}

/// <summary>
/// Combines a non-span <see cref="Value"/> with spans <see cref="A"/>, <see cref="B"/>, <see cref="C"/> and <see cref="D"/>.
/// </summary>
/// /// <remarks><inheritdoc cref="ValueSpanTuple{TValue,TA}"/></remarks>
/// <typeparam name="TValue">the non-span <see cref="Value"/> type</typeparam>
/// <typeparam name="TA"><see cref="A"/>'s element type</typeparam>
/// <typeparam name="TB"><see cref="B"/>'s element type</typeparam>
/// <typeparam name="TC"><see cref="C"/>'s element type</typeparam>
/// <typeparam name="TD"><see cref="D"/>'s element type</typeparam>
public readonly ref struct ValueSpanTuple<TValue, TA, TB, TC, TD> {
    public TValue Value { get; init; }

    #region Spans

    public RoSpanTuple<TA, TB, TC, TD> Spans { get; init; }

    public ReadOnlySpan<TA> A {
        get => Spans.A;
        init => Spans = Spans with { A = value };
    }

    public ReadOnlySpan<TB> B {
        get => Spans.B;
        init => Spans = Spans with { B = value };
    }

    public ReadOnlySpan<TC> C {
        get => Spans.C;
        init => Spans = Spans with { C = value };
    }

    public ReadOnlySpan<TD> D {
        get => Spans.D;
        init => Spans = Spans with { D = value };
    }

    #endregion

    #region 'structors

    public ValueSpanTuple(TValue value, RoSpanTuple<TA, TB, TC, TD> spans) {
        Value = value;
        Spans = spans;
    }

    public ValueSpanTuple(
        TValue           value,
        ReadOnlySpan<TA> a,
        ReadOnlySpan<TB> b,
        ReadOnlySpan<TC> c,
        ReadOnlySpan<TD> d
    ) : this(value, new RoSpanTuple<TA, TB, TC, TD>(a, b, c, d)) { }

    public void Deconstruct(
        out TValue           value,
        out ReadOnlySpan<TA> a,
        out ReadOnlySpan<TB> b,
        out ReadOnlySpan<TC> c,
        out ReadOnlySpan<TD> d
    ) {
        value = Value;
        a     = A;
        b     = B;
        c     = C;
        d     = D;
    }

    #endregion

    #region object methods

    [Obsolete, DoesNotReturn] public override bool Equals(object obj) => throw RoSpanTuple.BecauseSpanDoesnt();
    [Obsolete, DoesNotReturn] public override int  GetHashCode()      => throw RoSpanTuple.BecauseSpanDoesnt();

    #endregion
}