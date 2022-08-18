using System;

namespace FowlFever.BSharp;

public static partial class Comparisons {
    #region Tuples

    /// <summary>
    /// Gets the member-wise <see cref="ComparisonResult"/> for each entry in a pair of <see cref="ValueTuple{T1,T2}"/>s.
    /// </summary>
    /// <param name="self">this <see cref="ValueTuple{T1,T2}"/></param>
    /// <param name="other">another <see cref="ValueTuple{T1,T2}"/></param>
    /// <typeparam name="A">the type of <see cref="ValueTuple{A,B}.Item1"/></typeparam>
    /// <typeparam name="B">the type of <see cref="ValueTuple{A,B}.Item2"/></typeparam>
    /// <returns>(<see cref="ComparisonResult"/>, <see cref="ComparisonResult"/>)</returns>
    public static (ComparisonResult a, ComparisonResult b) CompareEach<A, B>(this (A, B) self, (A, B) other)
        where A : IComparable<A>
        where B : IComparable<B> {
        return (self.Item1.ComparedWith(other.Item1), self.Item2.ComparedWith(other.Item2));
    }

    /// <summary>
    /// Gets the member-wise <see cref="ComparisonResult"/> for each entry in a pair of <see cref="ValueTuple{A,B,C}"/>s.
    /// </summary>
    /// <param name="self">this <see cref="ValueTuple{A,B,C}"/></param>
    /// <param name="other">another <see cref="ValueTuple{A,B,C}"/></param>
    /// <typeparam name="A">the type of <see cref="ValueTuple{A,B,C}.Item1"/></typeparam>
    /// <typeparam name="B">the type of <see cref="ValueTuple{A,B,C}.Item2"/></typeparam>
    /// <typeparam name="C">the type of <see cref="ValueTuple{A,B,C}.Item3"/></typeparam>
    /// <returns>(<see cref="ComparisonResult"/>, <see cref="ComparisonResult"/>)</returns>
    public static (ComparisonResult a, ComparisonResult b, ComparisonResult c) CompareEach<A, B, C>(this (A, B, C) self, (A, B, C) other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C> {
        return (self.Item1.ComparedWith(other.Item1), self.Item2.ComparedWith(other.Item2), self.Item3.ComparedWith(other.Item3));
    }

    /// <summary>
    /// Combines the <see cref="CompareEach{A,B}"/> results into a single <see cref="ComparisonResult"/>:
    /// <ul>
    /// <li>If each entry in the <see cref="CompareEach{A,B}"/> results is the same, that <see cref="ComparisonResult"/> is returned.</li>
    /// <li>Otherwise, <c>null</c> is returned.</li>
    /// </ul>
    /// </summary>
    /// <param name="self">this <see cref="ValueTuple{A,B}"/></param>
    /// <param name="other">another <see cref="ValueTuple{A,B}"/></param>
    /// <typeparam name="A">the type of <see cref="ValueTuple{A,B}.Item1"/></typeparam>
    /// <typeparam name="B">the type of <see cref="ValueTuple{A,B}.Item2"/></typeparam>
    /// <returns>the <see cref="ComparisonResult"/> if each <see cref="CompareEach{A,B}"/> entry is the same; otherwise, <c>null</c></returns>
    public static ComparisonResult? CompareAll<A, B>(this (A a, B b) self, (A a, B b) other)
        where A : IComparable<A>
        where B : IComparable<B> {
        var (a, b) = self.CompareEach(other);
        return a == b ? a : null;
    }

    /// <summary>
    /// Combines the <see cref="CompareEach{A,B,C}"/> results into a single <see cref="ComparisonResult"/>:
    /// <ul>
    /// <li>If each entry in the <see cref="CompareEach{A,B,C}"/> results is the same, that <see cref="ComparisonResult"/> is returned.</li>
    /// <li>Otherwise, <c>null</c> is returned.</li>
    /// </ul>
    /// </summary>
    /// <param name="self">this <see cref="ValueTuple{A,B,C}"/></param>
    /// <param name="other">another <see cref="ValueTuple{A,B,C}"/></param>
    /// <typeparam name="A">the type of <see cref="ValueTuple{A,B,C}.Item1"/></typeparam>
    /// <typeparam name="B">the type of <see cref="ValueTuple{A,B,C}.Item2"/></typeparam>
    /// <typeparam name="C">the type of <see cref="ValueTuple{A,B,C}.Item3"/></typeparam>
    /// <returns>the <see cref="ComparisonResult"/> if each <see cref="CompareEach{A,B}"/> entry is the same; otherwise, <c>null</c></returns>
    public static ComparisonResult? CompareAll<A, B, C>(this (A a, B b, C c) self, (A a, B b, C c) other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C> {
        var ab = (self.a, self.b).CompareAll((other.a, other.b));
        if (ab != null && ab == self.c.ComparedWith(other.c)) {
            return ab;
        }

        return null;
    }

    #endregion
}