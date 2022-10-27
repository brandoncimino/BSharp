using System;

namespace FowlFever.BSharp.Memory;

public static class RoSpanTupleExtensions {
    public static bool SequenceEqual<T>(this     RoSpanTuple<T> self, ReadOnlySpan<T> other) where T : IEquatable<T>  => self.A.SequenceEqual(other);
    public static bool SequenceEqual<T>(this     RoSpanTuple<T> self, RoSpanTuple<T>  other) where T : IEquatable<T>  => self.A.SequenceEqual(other.A);
    public static int  SequenceCompareTo<T>(this RoSpanTuple<T> self, ReadOnlySpan<T> other) where T : IComparable<T> => self.A.SequenceCompareTo(other);
    public static int  SequenceCompareTo<T>(this RoSpanTuple<T> self, RoSpanTuple<T>  other) where T : IComparable<T> => self.A.SequenceCompareTo(other.A);

    public static bool SequenceEqual<A, B>(this RoSpanTuple<A, B> self, RoSpanTuple<A, B> other) where A : IEquatable<A> where B : IEquatable<B>                          => self.A.SequenceEqual(other.A) && self.B.SequenceEqual(other.B);
    public static bool SequenceEqual<A, B>(this RoSpanTuple<A, B> self, ReadOnlySpan<A>   otherA, ReadOnlySpan<B> otherB) where A : IEquatable<A> where B : IEquatable<B> => self.A.SequenceEqual(otherA)  && self.B.SequenceEqual(otherB);

    public static bool SequenceEqual<A, B, C>(this RoSpanTuple<A, B, C> self, RoSpanTuple<A, B, C> other) where A : IEquatable<A> where B : IEquatable<B> where C : IEquatable<C>                                                  => self.A.SequenceEqual(other.A) && self.B.SequenceEqual(other.B) && self.C.SequenceEqual(other.C);
    public static bool SequenceEqual<A, B, C>(this RoSpanTuple<A, B, C> self, ReadOnlySpan<A>      otherA, ReadOnlySpan<B> otherB, ReadOnlySpan<C> otherC) where A : IEquatable<A> where B : IEquatable<B> where C : IEquatable<C> => self.A.SequenceEqual(otherA)  && self.B.SequenceEqual(otherB)  && self.C.SequenceEqual(otherC);

    public static bool SequenceEqual<A, B, C, D>(this RoSpanTuple<A, B, C, D> self, RoSpanTuple<A, B, C, D> other) where A : IEquatable<A> where B : IEquatable<B> where C : IEquatable<C> where D : IEquatable<D> => self.A.SequenceEqual(other.A) && self.B.SequenceEqual(other.B) && self.C.SequenceEqual(other.C) && self.D.SequenceEqual(other.D);

    public static bool SequenceEqual<A, B, C, D>(
        this RoSpanTuple<A, B, C, D> self,
        ReadOnlySpan<A>              otherA,
        ReadOnlySpan<B>              otherB,
        ReadOnlySpan<C>              otherC,
        ReadOnlySpan<D>              otherD
    ) where A : IEquatable<A> where B : IEquatable<B> where C : IEquatable<C> where D : IEquatable<D> => self.A.SequenceEqual(otherA) && self.B.SequenceEqual(otherB) && self.C.SequenceEqual(otherC) && self.D.SequenceEqual(otherD);

    #region ToMultiSpan

    public static RoMultiSpan<T> ToMultiSpan<T>(this RoSpanTuple<T>          spans) => spans;
    public static RoMultiSpan<T> ToMultiSpan<T>(this RoSpanTuple<T, T>       spans) => spans;
    public static RoMultiSpan<T> ToMultiSpan<T>(this RoSpanTuple<T, T, T>    spans) => spans;
    public static RoMultiSpan<T> ToMultiSpan<T>(this RoSpanTuple<T, T, T, T> spans) => spans;

    #endregion

    #region GetEnumerator

    public static RoMultiSpan<T>.SpanEnumerator GetEnumerator<T>(this RoSpanTuple<T>          spans) => new(spans);
    public static RoMultiSpan<T>.SpanEnumerator GetEnumerator<T>(this RoSpanTuple<T, T>       spans) => new(spans);
    public static RoMultiSpan<T>.SpanEnumerator GetEnumerator<T>(this RoSpanTuple<T, T, T>    spans) => new(spans);
    public static RoMultiSpan<T>.SpanEnumerator GetEnumerator<T>(this RoSpanTuple<T, T, T, T> spans) => new(spans);

    #endregion
}