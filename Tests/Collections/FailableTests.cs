using System;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests.Collections {
    public class FailableTests {
        #region Example methods

        private static int Fail() {
            throw new BrandonException("This method always fails.");
        }

        private const int Expected_Value   = 5;
        private const int Unexpected_Value = 10;

        private static int Succeed() {
            return Expected_Value;
        }

        private static int UnexpectedSuccess() {
            return Unexpected_Value;
        }

        #endregion

        private static class Validate {
            public static void FailedFailable<T>(FailableFunc<T> failableFunc) {
                Asserter.Against(failableFunc)
                        .And(Has.Property(nameof(failableFunc.HasValue)).False)
                        .And(Has.Property(nameof(failableFunc.Failed)).True)
                        .And(it => it.Value, Throws.InvalidOperationException)
                        .Invoke();
            }

            public static void PassedFailable<T>(FailableFunc<T> failableFunc) {
                AssertAll.Of(
                    () => Assert.That(failableFunc, Has.Property(nameof(failableFunc.HasValue)).True),
                    () => Assert.That(failableFunc, Has.Property(nameof(failableFunc.Failed)).False),
                    () => Assert.DoesNotThrow(() => Console.WriteLine(failableFunc.Value)),
                    () => Assert.Throws<InvalidOperationException>(() => Console.WriteLine(failableFunc.Excuse))
                );
            }

            public static void Equality<T>(FailableFunc<T> failableFunc, Optional<T> optional, bool expectedEquality) {
                AssertAll.Of(
                    () => Assert.That(failableFunc == optional, Is.EqualTo(expectedEquality), "failable == optional"),
                    // no longer supported through ==
                    // () => Assert.That(optional     == failableFunc,                  Is.EqualTo(expectedEquality), "optional == failable"),
                    () => Assert.That(failableFunc.Equals(optional),                 Is.EqualTo(expectedEquality), "failable.Equals(optional)"),
                    () => Assert.That(optional.Equals(failableFunc),                 Is.EqualTo(expectedEquality), "optional.Equals(failable)"),
                    () => Assert.That(Optional.AreEqual(optional,     failableFunc), Is.EqualTo(expectedEquality), "Optional.AreEqual(optional, failable)"),
                    () => Assert.That(Optional.AreEqual(failableFunc, optional),     Is.EqualTo(expectedEquality), "Optional.AreEqual(failable, optional)")
                );
            }

            public static void Equality<T>(FailableFunc<T> a, FailableFunc<T> b, bool expectedEquality) {
                AssertAll.Of(
                    () => Assert.That(a == b,                  Is.EqualTo(expectedEquality), "a == b"),
                    () => Assert.That(b == a,                  Is.EqualTo(expectedEquality), "b == a"),
                    () => Assert.That(a.Equals(b),             Is.EqualTo(expectedEquality), "a.Equals(b)"),
                    () => Assert.That(b.Equals(a),             Is.EqualTo(expectedEquality), "b.Equals(a)"),
                    () => Assert.That(Optional.AreEqual(a, b), Is.EqualTo(expectedEquality), "Optional.AreEqual(a,b)"),
                    () => Assert.That(Optional.AreEqual(b, a), Is.EqualTo(expectedEquality), "Optional.AreEqual(b,a)")
                );
            }

            public static void Equality<T>(FailableFunc<T> failableFunc, T expectedValue, bool expectedEquality) {
                AssertAll.Of(
                    () => Assert.That(failableFunc  == expectedValue,                  Is.EqualTo(expectedEquality), "failable == expectedValue"),
                    () => Assert.That(expectedValue == failableFunc,                   Is.EqualTo(expectedEquality), "expectedValue == failable"),
                    () => Assert.That(failableFunc.Equals(expectedValue),              Is.EqualTo(expectedEquality), "failable.Equals(expectedValue)"),
                    () => Assert.That(Optional.AreEqual(failableFunc,  expectedValue), Is.EqualTo(expectedEquality), "Optional.AreEqual(failable, expectedValue)"),
                    () => Assert.That(Optional.AreEqual(expectedValue, failableFunc),  Is.EqualTo(expectedEquality), "Optional.AreEqual(expectedValue, failable)")
                );
            }

            public static void ObjectEquality<T>(FailableFunc<T> failableFunc, object? obj, bool expectedEquality = true) {
                Asserter.Against(failableFunc)
                        .WithHeading($"[{failableFunc}] should {(expectedEquality ? "be" : "not be")} equal to [{obj}]")
                        .And(it => it.Equals(obj),  Is.EqualTo(expectedEquality))
                        .And(it => Equals(it, obj), Is.EqualTo(expectedEquality))
                        .Invoke();
            }
        }

        [Test]
        public void FailedFailable_Lambda() {
            Validate.FailedFailable(Optional.Try(Fail));
        }

        [Test]
        public void FailedFailable_Func() {
            Func<int> func = Fail;
            Validate.FailedFailable(func.Try());
        }

        [Test]
        public void PassedFailable_Lambda() {
            Validate.PassedFailable(Optional.Try(Succeed));
        }

        [Test]
        public void PassedFailable_Func() {
            Func<int> func = Succeed;
            Validate.PassedFailable(func.Try());
        }

        [Test]
        public void SuccessfulFailableEquality() {
            var failable = Optional.Try(Succeed);
            var optional = Optional.Of(Expected_Value);
            AssertAll.Of(
                () => Validate.Equality(failable, optional,       true),
                () => Validate.Equality(failable, Expected_Value, true)
            );
        }

        [Test]
        public void SuccessfulEqualsSelf() {
            var failable = Optional.Try(Succeed);
            Validate.Equality(failable, failable, true);
        }

        [Test]
        public void SuccessfulEqualsDuplicate() {
            var a = Optional.Try(Succeed);
            var b = Optional.Try(Succeed);
            Validate.Equality(a, b, true);
        }

        [Test]
        public void SuccessfulFailableInequality() {
            var failable = Optional.Try(Succeed);
            var optional = Optional.Of(Unexpected_Value);
            AssertAll.Of(
                () => Validate.Equality(failable, optional,         false),
                () => Validate.Equality(failable, Unexpected_Value, false)
            );
        }

        [Test]
        public void FailedFailableInequality() {
            var failable = Optional.Try(Fail);
            var optional = Optional.Of(Expected_Value);
            AssertAll.Of(
                () => Validate.Equality(failable, optional,              false),
                () => Validate.Equality(failable, Expected_Value,        false),
                () => Validate.Equality(failable, Optional.Try(Succeed), false)
            );
        }

        [Test]
        public void FailedFailableEquality() {
            var failable = Optional.Try(Fail);
            var optional = new Optional<int>();
            AssertAll.Of(
                () => Validate.Equality(failable, optional,           true),
                () => Validate.Equality(failable, failable,           true),
                () => Validate.Equality(failable, Optional.Try(Fail), true)
            );
        }

        [Test]
        public void FailableDefault() {
            FailableFunc<int> failableFunc = default;
            Validate.FailedFailable(failableFunc);
        }

        [Test]
        public void FailableNoArgConstructor() {
            var failableFunc = new FailableFunc<int>();
            Validate.FailedFailable(failableFunc);
        }

        [Test]
        public void FailableSuccessObjectEquality() {
            var failable  = Optional.Try(Succeed);
            var failable2 = Optional.Try(Succeed);

            AssertAll.Of(
                () => Validate.ObjectEquality(failable, Expected_Value, true),
                () => Validate.ObjectEquality(failable, failable2,      true),
                () => Assert.That(failable       == Expected_Value, "failable == Expected_Value"),
                () => Assert.That(failable       == failable2,      "failable == failable2"),
                () => Assert.That(Expected_Value == failable,       "Expected_Value == failable")
            );
        }

        [Test]
        public void FailableSuccessObjectInequality() {
            var failable  = Optional.Try(Succeed);
            var failable2 = Optional.Try(UnexpectedSuccess);
            AssertAll.Of(
                () => Validate.ObjectEquality(failable, Unexpected_Value, false),
                () => Validate.ObjectEquality(failable, failable2,        false),
                () => Validate.ObjectEquality(failable, null,             false)
            );
        }
    }
}