using System;
using System.Numerics;

namespace FowlFever.BSharp.Memory;

public static partial class PrimitiveMath {
    internal enum ArithmeticOperation : byte {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Exponentiation,
        RootExtraction,
        Logarithmization,
    }
}

internal static class ArithmeticOperationExtensions {
    public static int Symbol(this PrimitiveMath.ArithmeticOperation operation) {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => '➕',
            PrimitiveMath.ArithmeticOperation.Subtraction      => '➖',
            PrimitiveMath.ArithmeticOperation.Multiplication   => '✖',
            PrimitiveMath.ArithmeticOperation.Division         => '➗',
            PrimitiveMath.ArithmeticOperation.Exponentiation   => '^',
            PrimitiveMath.ArithmeticOperation.RootExtraction   => '√',
            PrimitiveMath.ArithmeticOperation.Logarithmization => 0x1FAB5, // 🪵
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static string LeftSideName(this PrimitiveMath.ArithmeticOperation operation) {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => "augend",
            PrimitiveMath.ArithmeticOperation.Subtraction      => "minuend",
            PrimitiveMath.ArithmeticOperation.Multiplication   => "multiplicand",
            PrimitiveMath.ArithmeticOperation.Division         => "dividend",
            PrimitiveMath.ArithmeticOperation.Exponentiation   => "base",
            PrimitiveMath.ArithmeticOperation.RootExtraction   => throw new NotImplementedException(),
            PrimitiveMath.ArithmeticOperation.Logarithmization => throw new NotImplementedException(),
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static string RightSideName(this PrimitiveMath.ArithmeticOperation operation) {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => "addend",
            PrimitiveMath.ArithmeticOperation.Subtraction      => "subtrahend",
            PrimitiveMath.ArithmeticOperation.Multiplication   => "multiplier",
            PrimitiveMath.ArithmeticOperation.Division         => "divisor",
            PrimitiveMath.ArithmeticOperation.Exponentiation   => "exponent",
            PrimitiveMath.ArithmeticOperation.RootExtraction   => "radicand",
            PrimitiveMath.ArithmeticOperation.Logarithmization => throw new NotImplementedException(),
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static string ResultName(this PrimitiveMath.ArithmeticOperation operation) {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => "sum",
            PrimitiveMath.ArithmeticOperation.Subtraction      => "difference",
            PrimitiveMath.ArithmeticOperation.Multiplication   => "product",
            PrimitiveMath.ArithmeticOperation.Division         => "quotient",
            PrimitiveMath.ArithmeticOperation.Exponentiation   => "power",
            PrimitiveMath.ArithmeticOperation.RootExtraction   => "root",
            PrimitiveMath.ArithmeticOperation.Logarithmization => throw new NotImplementedException(),
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static T Apply<T>(this PrimitiveMath.ArithmeticOperation operation, T a, T b) where T : unmanaged {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => PrimitiveMath.Add(a, b),
            PrimitiveMath.ArithmeticOperation.Subtraction      => PrimitiveMath.Subtract(a, b),
            PrimitiveMath.ArithmeticOperation.Multiplication   => PrimitiveMath.Multiply(a, b),
            PrimitiveMath.ArithmeticOperation.Division         => PrimitiveMath.Divide(a, b),
            PrimitiveMath.ArithmeticOperation.Exponentiation   => throw new NotSupportedException(),
            PrimitiveMath.ArithmeticOperation.RootExtraction   => throw new NotSupportedException(),
            PrimitiveMath.ArithmeticOperation.Logarithmization => throw new NotSupportedException(),
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static bool SupportsVectors(this PrimitiveMath.ArithmeticOperation operation) {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => true,
            PrimitiveMath.ArithmeticOperation.Subtraction      => true,
            PrimitiveMath.ArithmeticOperation.Multiplication   => true,
            PrimitiveMath.ArithmeticOperation.Division         => true,
            PrimitiveMath.ArithmeticOperation.Exponentiation   => false,
            PrimitiveMath.ArithmeticOperation.RootExtraction   => false,
            PrimitiveMath.ArithmeticOperation.Logarithmization => false,
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static Vector<T> Apply<T>(this PrimitiveMath.ArithmeticOperation operation, Vector<T> a, Vector<T> b) where T : unmanaged {
        return operation switch {
            PrimitiveMath.ArithmeticOperation.Addition         => a + b,
            PrimitiveMath.ArithmeticOperation.Subtraction      => a - b,
            PrimitiveMath.ArithmeticOperation.Multiplication   => a * b,
            PrimitiveMath.ArithmeticOperation.Division         => a / b,
            PrimitiveMath.ArithmeticOperation.Exponentiation   => throw new NotSupportedException(),
            PrimitiveMath.ArithmeticOperation.RootExtraction   => throw new NotSupportedException(),
            PrimitiveMath.ArithmeticOperation.Logarithmization => throw new NotSupportedException(),
            _                                                  => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
}