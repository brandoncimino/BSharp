using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class PrimitiveMath {
    public enum ComparisonOperator : byte {
        GreaterThan,
        LessThan,
        EqualTo,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        NotEqualTo,
    }
}

internal static class ComparisonOperatorExtensions {
    [Pure]
    public static char Symbol(this PrimitiveMath.ComparisonOperator comparisonOperator) {
        return comparisonOperator switch {
            PrimitiveMath.ComparisonOperator.GreaterThan          => '>',
            PrimitiveMath.ComparisonOperator.LessThan             => '<',
            PrimitiveMath.ComparisonOperator.EqualTo              => '=',
            PrimitiveMath.ComparisonOperator.GreaterThanOrEqualTo => '≥',
            PrimitiveMath.ComparisonOperator.LessThanOrEqualTo    => '≤',
            PrimitiveMath.ComparisonOperator.NotEqualTo           => '≠',
            _                                                     => throw new ArgumentOutOfRangeException(nameof(comparisonOperator), comparisonOperator, null)
        };
    }

    [Pure]
    public static bool Apply<T>(this PrimitiveMath.ComparisonOperator comparisonOperator, T right, T left) where T : unmanaged {
        return comparisonOperator switch {
            PrimitiveMath.ComparisonOperator.GreaterThan          => PrimitiveMath.GreaterThan(right, left),
            PrimitiveMath.ComparisonOperator.LessThan             => PrimitiveMath.LessThan(right, left),
            PrimitiveMath.ComparisonOperator.EqualTo              => PrimitiveMath.EqualTo(right, left),
            PrimitiveMath.ComparisonOperator.GreaterThanOrEqualTo => PrimitiveMath.GreaterThanOrEqualTo(right, left),
            PrimitiveMath.ComparisonOperator.LessThanOrEqualTo    => PrimitiveMath.LessThanOrEqualTo(right, left),
            PrimitiveMath.ComparisonOperator.NotEqualTo           => PrimitiveMath.EqualTo(right, left) == false,
            _                                                     => throw new ArgumentOutOfRangeException(nameof(comparisonOperator), comparisonOperator, null)
        };
    }

    [Pure]
    public static Vector<T> Apply<T>(this PrimitiveMath.ComparisonOperator comparisonOperator, Vector<T> right, Vector<T> left) where T : unmanaged {
        return comparisonOperator switch {
            PrimitiveMath.ComparisonOperator.GreaterThan          => Vector.GreaterThan(right, left),
            PrimitiveMath.ComparisonOperator.LessThan             => Vector.LessThan(right, left),
            PrimitiveMath.ComparisonOperator.EqualTo              => Vector.Equals(right, left),
            PrimitiveMath.ComparisonOperator.GreaterThanOrEqualTo => Vector.GreaterThanOrEqual(right, left),
            PrimitiveMath.ComparisonOperator.LessThanOrEqualTo    => Vector.LessThanOrEqual(right, left),
            PrimitiveMath.ComparisonOperator.NotEqualTo           => Vector.OnesComplement(Vector.Equals(right, left)) /*TODO: This is very likely incorrect*/,
            _                                                     => throw new ArgumentOutOfRangeException(nameof(comparisonOperator), comparisonOperator, null),
        };
    }
}