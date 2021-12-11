using System;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Enums;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests {
    public class CoercivelyTests {
        private static readonly Type[] OtherCoerciveTypes = {
            typeof(string),
            typeof(DateTime),
            typeof(TimeSpan)
        };

        private static readonly Type[] CoerciveTypes = Enumerable.Union(PrimitiveUtils.NumericTypes, OtherCoerciveTypes).ToArray();

        private static Type GetExpectedResultType(Type aType) {
            return PrimitiveUtils.PseudoIntTypes.Contains(aType) ? typeof(int) : aType;
        }

        #region Coercively, or: "CoercionUtils 2.0"

        private readonly struct OperationExpectation {
            private readonly object             A;
            private readonly Type               AType;
            private readonly Coercive.Operation Operation;
            private readonly object             B;
            private readonly Type               BType;
            private readonly object             E;
            private readonly Type               EType;

            public OperationExpectation(Type a_type, Coercive.Operation operation, Type b_type) {
                object a_val;
                object b_val;
                object e_val;

                AType     = a_type;
                Operation = operation;
                BType     = b_type;
                EType     = GetExpectedResultType(AType);

                switch (operation) {
                    case Coercive.Operation.Plus:
                        a_val = 10;
                        b_val = 20;
                        e_val = 30;
                        break;
                    case Coercive.Operation.Minus:
                        a_val = 55;
                        b_val = 21;
                        e_val = 34;
                        break;
                    case Coercive.Operation.Times:
                        a_val = 8;
                        b_val = 10;
                        e_val = 80;
                        break;
                    case Coercive.Operation.DividedBy:
                        a_val = 20;
                        b_val = 5;
                        e_val = 4;
                        break;
                    default:
                        throw EnumUtils.InvalidEnumArgumentException(nameof(operation), operation);
                }

                A = Convert.ChangeType(a_val, AType);
                B = Convert.ChangeType(b_val, BType);
                E = Convert.ChangeType(e_val, EType);
            }

            public void Test_Compute() {
                AssertAll.Of(
                    $"{nameof(Coercively)}.{nameof(Coercively.Compute)}: [{AType.Name}]{A} {Operation} [{BType.Name}]{B} == [{EType.Name}]{E}",
                    Coercively.Compute(A, Operation, B),
                    Is.EqualTo(E),
                    Is.TypeOf(EType)
                );
            }

            public void Test_Verb() {
                Func<object, object, object> method = Operation switch {
                    Coercive.Operation.Plus      => Coercively.Add,
                    Coercive.Operation.Minus     => Coercively.Subtract,
                    Coercive.Operation.Times     => Coercively.Multiply,
                    Coercive.Operation.DividedBy => Coercively.Divide,
                    _                            => throw EnumUtils.InvalidEnumArgumentException(nameof(Operation), Operation)
                };

                AssertAll.Of(
                    $"Coercively.{Operation.Operator().Verb}()-ing [{AType.Name}]{A}, [{BType.Name}]{B}",
                    method.Invoke(A, B),
                    Is.EqualTo(E),
                    Is.TypeOf(EType)
                );
            }
        }

        [Test]
        public void Coercively_Compute_Numeric(
            [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
            Type aType,
            [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
            Type bType,
            [Values] Coercive.Operation operation
        ) {
            new OperationExpectation(aType, operation, bType).Test_Compute();
        }

        [Test]
        public void Coercively_Verb_Numeric(
            [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
            Type aType,
            [ValueSource(typeof(PrimitiveUtils), nameof(PrimitiveUtils.NumericTypes))]
            Type bType,
            [Values] Coercive.Operation operation
        ) {
            new OperationExpectation(aType, operation, bType).Test_Verb();
        }

        #endregion
    }
}