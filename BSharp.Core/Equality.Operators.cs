using System.Numerics;

namespace FowlFever.BSharp.Core;

public static partial class Equality {
#if NET7_0_OR_GREATER

    #region Operators

    private sealed class EqualsOperatorComparer<T> : BaseEquality<T> where T : IEqualityOperators<T, T, bool> {
        public static readonly EqualsOperatorComparer<T> Instance = new();
        protected override     bool                      _Equals(T x, T y) => x == y;
    }

    private sealed class NotEqualsOperatorComparer<T> : BaseEquality<T> where T : IEqualityOperators<T, T, bool> {
        public static readonly NotEqualsOperatorComparer<T> Instance = new();
        protected override     bool                         _Equals(T x, T y) => !(x != y);
    }

    /// <returns>an <see cref="IEqualityComparer{T}"/> that uses the <see cref="IEqualityOperators{TSelf,TOther,TResult}.op_Equality"/></returns>
    [Pure]
    public static IEqualityComparer<T> ByEqualsOperator<T>() where T : IEqualityOperators<T, T, bool> => EqualsOperatorComparer<T>.Instance;

    /// <returns>an <see cref="IEqualityComparer{T}"/> that uses the <see cref="IEqualityOperators{TSelf,TOther,TResult}.op_Inequality"/></returns>
    /// <remarks>
    /// <see cref="IEqualityOperators{TSelf,TOther,TResult}.op_Inequality"/> <i>should</i> always be implemented as <c>!(x == y)</c>, making this comparer redundant.
    /// It is intended primarily for testing purposes, so that you can make sure <see cref="IEqualityOperators{TSelf,TOther,TResult}.op_Inequality"/> was properly implemented as the inverse of <see cref="IEqualityOperators{TSelf,TOther,TResult}.op_Equality"/>.
    /// </remarks>
    [Pure]
    public static IEqualityComparer<T> ByNotEqualsOperator<T>() where T : IEqualityOperators<T, T, bool> => NotEqualsOperatorComparer<T>.Instance;

    #endregion

#endif
}