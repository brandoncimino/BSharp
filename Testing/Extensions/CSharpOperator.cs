using System;
using System.Numerics;
using System.Reflection;

using FowlFever.BSharp.Attributes;

namespace FowlFever.Testing.Extensions;

[Experimental("This seems useful, and should probably be promoted")]
internal enum CSharpOperator {
    /// <seealso cref="IEqualityOperators{TSelf,TOther,TResult}.op_Equality"/>
    Equality,
    /// <seealso cref="IEqualityOperators{TSelf,TOther,TResult}.op_Inequality"/>
    Inequality,

    /// <seealso cref="IAdditionOperators{TSelf,TOther,TResult}.op_Addition"/>
    Addition,
    /// <seealso cref="IAdditionOperators{TSelf,TOther,TResult}.op_CheckedAddition"/>
    CheckedAddition,

    /// <seealso cref="ISubtractionOperators{TSelf,TOther,TResult}.op_Subtraction"/>
    Subtraction,
    /// <seealso cref="ISubtractionOperators{TSelf,TOther,TResult}.op_CheckedSubtraction"/>
    CheckedSubtraction,

    /// <seealso cref="IMultiplyOperators{TSelf,TOther,TResult}.op_Multiply"/>
    Multiply,
    /// <seealso cref="IMultiplyOperators{TSelf,TOther,TResult}.op_CheckedMultiply"/>
    CheckedMultiply,

    /// <seealso cref="IDivisionOperators{TSelf,TOther,TResult}.op_Division"/>
    Division,
    /// <seealso cref="IDivisionOperators{TSelf,TOther,TResult}.op_CheckedDivision"/>
    CheckedDivision,

    /// <seealso cref="IIncrementOperators{TSelf}.op_Increment"/>
    Increment,
    /// <seealso cref="IIncrementOperators{TSelf}.op_CheckedIncrement"/>
    CheckedIncrement,

    /// <seealso cref="IDecrementOperators{TSelf}.op_Decrement"/>
    Decrement,
    /// <seealso cref="IDecrementOperators{TSelf}.op_CheckedDecrement"/>
    CheckedDecrement,

    /// <seealso cref="Vector{T}.op_BitwiseAnd"/>
    Explicit,
    CheckedExplicit,
    /// <seealso cref="string.op_Implicit"/>
    Implicit,
    CheckedImplicit,

    /// <seealso cref="Vector{T}.op_BitwiseAnd"/>
    /// <seealso cref="IBitwiseOperators{TSelf,TOther,TResult}.op_BitwiseAnd"/>
    BitwiseAnd,
    /// <seealso cref="Vector{T}.op_BitwiseOr"/>
    /// <seealso cref="IBitwiseOperators{TSelf,TOther,TResult}.op_BitwiseOr"/>
    BitwiseOr,
    /// <seealso cref="Vector{T}.op_ExclusiveOr"/>
    /// <seealso cref="IBitwiseOperators{TSelf,TOther,TResult}.op_ExclusiveOr"/>
    ExclusiveOr,
    /// <seealso cref="Vector{T}.op_OnesComplement"/>
    /// <seealso cref="IBitwiseOperators{TSelf,TOther,TResult}.op_OnesComplement"/>
    OnesComplement,

    /// <seealso cref="Half.op_UnaryPlus"/>
    /// <seealso cref="IUnaryPlusOperators{TSelf,TResult}.op_UnaryPlus"/>
    UnaryPlus,

    /// <seealso cref="Vector{T}.op_UnaryNegation"/>
    /// <seealso cref="IUnaryNegationOperators{TSelf,TResult}.op_UnaryNegation"/>
    UnaryNegation,
    /// <seealso cref="IUnaryNegationOperators{TSelf,TResult}.op_CheckedUnaryNegation"/>
    CheckedUnaryNegation,

    /// <seealso cref="Half.op_Modulus"/>
    /// <seealso cref="IModulusOperators{TSelf,TOther,TResult}.op_Modulus"/>
    Modulus,

    /// <seealso cref="Half.op_GreaterThan"/>
    /// <seealso cref="IComparisonOperators{TSelf,TOther,TResult}.op_GreaterThan"/>
    GreaterThan,
    /// <seealso cref="Half.op_LessThan"/>
    /// <seealso cref="IComparisonOperators{TSelf,TOther,TResult}.op_LessThan"/>
    LessThan,
    /// <seealso cref="Half.op_GreaterThanOrEqual"/>
    /// <seealso cref="IComparisonOperators{TSelf,TOther,TResult}.op_GreaterThanOrEqual"/>
    GreaterThanOrEqual,
    /// <seealso cref="Half.op_LessThanOrEqual"/>
    /// <seealso cref="IComparisonOperators{TSelf,TOther,TResult}.op_LessThanOrEqual"/>
    LessThanOrEqual,

    /// <seealso cref="IShiftOperators{TSelf,TOther,TResult}.op_LeftShift"/>
    LeftShift,
    /// <seealso cref="IShiftOperators{TSelf,TOther,TResult}.op_RightShift"/>
    RightShift,
    /// <seealso cref="IShiftOperators{TSelf,TOther,TResult}.op_UnsignedRightShift"/>
    UnsignedRightShift,
}

internal enum OperatorInterface {
    Equality,
    Addition,
    Subtraction,
    Multiply,
    Division,
    Increment,
    Decrement,
    Bitwise,
    UnaryPlus,
    UnaryNegation,
    Modulus,
    Comparison,
    Shift,
}

internal static class CSharpOperatorExtensions {
    public static string MethodName(this CSharpOperator op) {
        return string.Intern($"op_{op}");
    }

    public static string Symbol(this CSharpOperator op) {
        return op switch {
            CSharpOperator.Equality                                                                                                => "==",
            CSharpOperator.Inequality                                                                                              => "!=",
            CSharpOperator.Addition or CSharpOperator.CheckedAddition                                                              => "+",
            CSharpOperator.Subtraction or CSharpOperator.CheckedSubtraction                                                        => "-",
            CSharpOperator.Multiply or CSharpOperator.CheckedMultiply                                                              => "*",
            CSharpOperator.Division or CSharpOperator.CheckedDivision                                                              => "/",
            CSharpOperator.Increment or CSharpOperator.CheckedIncrement                                                            => "++",
            CSharpOperator.Decrement or CSharpOperator.CheckedDecrement                                                            => "--",
            CSharpOperator.Explicit or CSharpOperator.CheckedExplicit or CSharpOperator.Implicit or CSharpOperator.CheckedImplicit => throw new ArgumentOutOfRangeException(nameof(op), op, $"The operator {op} doesn't have an associated symbol!"),
            CSharpOperator.BitwiseAnd                                                                                              => "&",
            CSharpOperator.BitwiseOr                                                                                               => "|",
            CSharpOperator.ExclusiveOr                                                                                             => "^",
            CSharpOperator.OnesComplement                                                                                          => "~",
            CSharpOperator.UnaryPlus                                                                                               => "+",
            CSharpOperator.UnaryNegation or CSharpOperator.CheckedUnaryNegation                                                    => "-",
            CSharpOperator.Modulus                                                                                                 => "%",
            CSharpOperator.GreaterThan                                                                                             => ">",
            CSharpOperator.LessThan                                                                                                => "<",
            CSharpOperator.GreaterThanOrEqual                                                                                      => ">=",
            CSharpOperator.LessThanOrEqual                                                                                         => "<=",
            CSharpOperator.LeftShift                                                                                               => "<<",
            CSharpOperator.RightShift                                                                                              => ">>",
            CSharpOperator.UnsignedRightShift                                                                                      => ">>>",
            _                                                                                                                      => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public static OperatorInterface Interface(this CSharpOperator op) {
        return op switch {
            CSharpOperator.Equality or CSharpOperator.Inequality                                                                         => OperatorInterface.Equality,
            CSharpOperator.Addition or CSharpOperator.CheckedAddition                                                                    => OperatorInterface.Addition,
            CSharpOperator.Subtraction or CSharpOperator.CheckedSubtraction                                                              => OperatorInterface.Subtraction,
            CSharpOperator.Multiply or CSharpOperator.CheckedMultiply                                                                    => OperatorInterface.Multiply,
            CSharpOperator.Division or CSharpOperator.CheckedDivision                                                                    => OperatorInterface.Division,
            CSharpOperator.Increment or CSharpOperator.CheckedIncrement                                                                  => OperatorInterface.Increment,
            CSharpOperator.Decrement or CSharpOperator.CheckedDecrement                                                                  => OperatorInterface.Decrement,
            CSharpOperator.Explicit or CSharpOperator.CheckedExplicit or CSharpOperator.Implicit or CSharpOperator.CheckedImplicit       => throw new ArgumentOutOfRangeException(nameof(op), op, "I don't think the conversion operators have an interface..."),
            CSharpOperator.BitwiseAnd or CSharpOperator.BitwiseOr or CSharpOperator.ExclusiveOr or CSharpOperator.OnesComplement         => OperatorInterface.Bitwise,
            CSharpOperator.UnaryPlus                                                                                                     => OperatorInterface.UnaryPlus,
            CSharpOperator.UnaryNegation or CSharpOperator.CheckedUnaryNegation                                                          => OperatorInterface.UnaryNegation,
            CSharpOperator.Modulus                                                                                                       => OperatorInterface.Modulus,
            CSharpOperator.GreaterThan or CSharpOperator.LessThan or CSharpOperator.GreaterThanOrEqual or CSharpOperator.LessThanOrEqual => OperatorInterface.Comparison,
            CSharpOperator.LeftShift or CSharpOperator.RightShift or CSharpOperator.UnsignedRightShift                                   => OperatorInterface.Shift,
            _                                                                                                                            => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public static string InterfaceName(this OperatorInterface op) {
        return string.Intern($"I{op}Operators");
    }

    public static TResult? Invoke<TSelf, TOther, TResult>(this CSharpOperator op, TSelf self, TOther other) {
        const BindingFlags operatorBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        Type[] paramTypes = { typeof(TSelf), typeof(TOther) };
        // 📎 The nice signature, without the binder and modifiers, isn't available in .NET Standard 2.1 😢
        var method = typeof(TSelf).GetMethod(op.MethodName(), BindingFlags.Default, binder: null, paramTypes, modifiers: null);

        if (method == null) {
            throw new MissingMethodException(typeof(TSelf).Name, op.MethodName());
        }

        if (method.ReturnType != typeof(TResult)) {
            throw new MissingMethodException($"The {typeof(TSelf).Name} operator {typeof(TSelf).Name} {op.Symbol()} {typeof(TOther)} didn't have the expected result type, {typeof(TResult).Name} (actual result type: {method.ReturnType.Name})");
        }

        var result = method.Invoke(null, new object?[] { self, other });

        try {
            return (TResult?)result;
        }
        catch (InvalidCastException e) {
            throw new InvalidCastException($"The result of the operator {typeof(TSelf).Name} {op.Symbol()} {typeof(TOther)} didn't return the expected result type, {typeof(TResult).Name}!", e);
        }
    }

    public static bool InvokeBoolean<TSelf>(this CSharpOperator op, TSelf self, TSelf other) => op.Invoke<TSelf, TSelf, bool>(self, other);

    public static TSelf? Invoke<TSelf>(this CSharpOperator op, TSelf self, TSelf other) => op.Invoke<TSelf, TSelf, TSelf?>(self, other);
}