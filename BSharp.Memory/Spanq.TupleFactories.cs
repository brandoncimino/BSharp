using System;
using System.Diagnostics.Contracts;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Tuple factories

    [Pure] public static RoSpanTuple                Tuple()                                                                                           => default;
    [Pure] public static RoSpanTuple<A>             Tuple<A>(ReadOnlySpan<A>          a)                                                              => new(a);
    [Pure] public static RoSpanTuple<A, B>          Tuple<A, B>(ReadOnlySpan<A>       a,     ReadOnlySpan<B> b)                                       => new(a, b);
    [Pure] public static RoSpanTuple<A, B, C>       Tuple<A, B, C>(ReadOnlySpan<A>    a,     ReadOnlySpan<B> b, ReadOnlySpan<C> c)                    => new(a, b, c);
    [Pure] public static RoSpanTuple<A, B, C, D>    Tuple<A, B, C, D>(ReadOnlySpan<A> a,     ReadOnlySpan<B> b, ReadOnlySpan<C> c, ReadOnlySpan<D> d) => new(a, b, c, d);
    [Pure] public static ValueSpanTuple<T, A>       Tuple<T, A>(T                     value, ReadOnlySpan<A> a)                                       => new(value, a);
    [Pure] public static ValueSpanTuple<T, A, B>    Tuple<T, A, B>(T                  value, ReadOnlySpan<A> a, ReadOnlySpan<B> b)                    => new(value, a, b);
    [Pure] public static ValueSpanTuple<T, A, B, C> Tuple<T, A, B, C>(T               value, ReadOnlySpan<A> a, ReadOnlySpan<B> b, ReadOnlySpan<C> c) => new(value, a, b, c);

    [Pure]
    public static ValueSpanTuple<T, A, B, C, D> Tuple<T, A, B, C, D>(
        T               value,
        ReadOnlySpan<A> a,
        ReadOnlySpan<B> b,
        ReadOnlySpan<C> c,
        ReadOnlySpan<D> d
    ) => new(value, a, b, c, d);

    #endregion
}