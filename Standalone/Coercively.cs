using System;
using System.Runtime.CompilerServices;

using BrandonUtils.Standalone.Chronic;
using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Strings;

[assembly: InternalsVisibleTo("BrandonUtils.Tests.Standalone")]

namespace BrandonUtils.Standalone {
    /// <summary>
    /// Performs basic <a href="https://en.wikipedia.org/wiki/Operation_(mathematics)">operations</a> against <see cref="object"/>s.
    /// </summary>
    /// <remarks>
    /// This class is much cleaner and more idiomatic than the previously-existing <see cref="CoercionUtils"/>.
    ///
    /// As of 7/3/2021, the coercive operators are <b>not commutative</b> - they "prefer" the left-hand, or "a", type.
    /// <code><![CDATA[
    /// Coercively(1,    2.5f); // returns [3]:    1   + 2.5 -> 1   + 2   -> 3
    /// Coercively(2.5f, 1);    // returns [3.5]:  2.5 + 1   -> 2.5 + 1.0 -> 3.5
    ///
    /// ]]></code>
    /// </remarks>
    public static class Coercively {
        private static Coercive.Operator Addition { get; } = new Coercive.Operator() {
            Noun       = "Addition",
            Verb       = "Add",
            Symbol     = "+",
            SymbolName = "plus",
            Byte       = (a, b) => a + b,
            SByte      = (a, b) => a + b,
            Short      = (a, b) => a + b,
            UShort     = (a, b) => a + b,
            Int        = (a, b) => a + b,
            UInt       = (a, b) => a + b,
            Long       = (a, b) => a + b,
            ULong      = (a, b) => a + b,
            Float      = (a, b) => a + b,
            Double     = (a, b) => a + b,
            Decimal    = (a, b) => a + b,
            String     = (a, b) => a + b,
            DateTime   = (a, b) => a.Add(b),
            TimeSpan   = (a, b) => a.Add(TimeUtils.TimeSpanOf(b)),
        };

        /// <summary>
        /// <a href="https://en.wikipedia.org/wiki/Addition">Adds</a> <paramref name="a"/> and <paramref name="b"/> together if possible.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static object Add(object a, object b) {
            return Addition.Apply(a, b);
        }

        private static Coercive.Operator Subtraction { get; } = new Coercive.Operator() {
            Noun       = "Subtraction",
            Verb       = "Subtract",
            Symbol     = "-",
            SymbolName = "minus",
            Byte       = (a, b) => a - b,
            SByte      = (a, b) => a - b,
            Short      = (a, b) => a - b,
            UShort     = (a, b) => a - b,
            Int        = (a, b) => a - b,
            UInt       = (a, b) => a - b,
            Long       = (a, b) => a - b,
            ULong      = (a, b) => a - b,
            Float      = (a, b) => a - b,
            Double     = (a, b) => a - b,
            Decimal    = (a, b) => a - b,
            DateTime   = (a, b) => a.Subtract(b),
            TimeSpan   = (a, b) => a.Subtract(TimeUtils.TimeSpanOf(b)),
        };

        public static object Subtract(object a, object b) {
            return Subtraction.Apply(a, b);
        }

        private static Coercive.Operator Multiplication { get; } = new Coercive.Operator() {
            Noun       = "Multiplication",
            Verb       = "Multiply",
            Symbol     = "x",
            SymbolName = "times",
            Byte       = (a, b) => a * b,
            SByte      = (a, b) => a * b,
            Short      = (a, b) => a * b,
            UShort     = (a, b) => a * b,
            Int        = (a, b) => a * b,
            UInt       = (a, b) => a * b,
            Long       = (a, b) => a * b,
            ULong      = (a, b) => a * b,
            Float      = (a, b) => a * b,
            Double     = (a, b) => a * b,
            Decimal    = (a, b) => a * b,
            String     = (a, b) => a.Repeat(Convert.ToInt32(b)), // this might be inefficient, since it means that b will be converted to a string AND then to an int after - even if it was an int to begin with
            TimeSpan   = (a, b) => TimeUtils.Multiply(a, Convert.ToDouble(b)),
        };

        public static object Multiply(object multiplicand, object multiplier) {
            return Multiplication.Apply(multiplicand, multiplier);
        }

        private static Coercive.Operator Division { get; } = new Coercive.Operator() {
            Noun       = "Division",
            Verb       = "Divide",
            Symbol     = "/",
            SymbolName = "divided by",
            Byte       = (a, b) => a / b,
            SByte      = (a, b) => a / b,
            Short      = (a, b) => a / b,
            UShort     = (a, b) => a / b,
            Int        = (a, b) => a / b,
            UInt       = (a, b) => a / b,
            Long       = (a, b) => a / b,
            ULong      = (a, b) => a / b,
            Float      = (a, b) => a / b,
            Double     = (a, b) => a / b,
            Decimal    = (a, b) => a / b,
            TimeSpan = (a, b) => {
                if (b.IsNumber()) {
                    return a.Divide(Convert.ToDouble(b));
                }

                return a.Divide(TimeUtils.TimeSpanOf(b));
            }
        };

        public static object Divide(object dividend, object divisor) {
            return Division.Apply(dividend, divisor);
        }

        public static object Compute(object a, Coercive.Operation operation, object b) {
            return operation.Operator().Apply(a, b);
        }

        internal static Coercive.Operator Operator(this Coercive.Operation operation) {
            return operation switch {
                Coercive.Operation.Plus      => Addition,
                Coercive.Operation.Minus     => Subtraction,
                Coercive.Operation.Times     => Multiplication,
                Coercive.Operation.DividedBy => Division,
                _                            => throw EnumUtils.InvalidEnumArgumentException(nameof(operation), operation)
            };
        }
    }
}