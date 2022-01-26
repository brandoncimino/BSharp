using System;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
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
                        .Satisfies(it => _ = it!.Value,  Throws.Exception)
                        .Satisfies(it => _ = it!.Excuse, Throws.Nothing)
                        .Satisfies(it => _ = it!.ValueOrExcuse)
                        .Invoke();
            }

            public static void PassedFailable<T>(FailableFunc<T> failableFunc) {
                Asserter.Against(failableFunc)
                        .WithHeading("A failable that PASSED")
                        .And(Has.Property(nameof(failableFunc.HasValue)).True)
                        .And(Has.Property(nameof(failableFunc.Failed)).False)
                        //NOTE: there is some weirdness with the special _ symbol...
                        .And(it => _ = it!.Value)
                        .And(Has.Property(nameof(failableFunc.Excuse)).Null)
                        .And(it => it!.ValueOrExcuse, Is.EqualTo(failableFunc.Value))
                        .Invoke();
            }

            public static IMultipleAsserter Equality<T>(FailableFunc<T> failableFunc, IOptional<T> optional, Should should) {
                return Asserter.Against(failableFunc)
                               .And(it => it.Equals(optional),                   should.Constrain())
                               .And(it => Optional.AreEqual(it,       optional), should.Constrain())
                               .And(it => Optional.AreEqual(optional, it),       should.Constrain());
            }

            public static IMultipleAsserter Equality<T>(FailableFunc<T> a, FailableFunc<T> b, Should should) {
                return Asserter.WithHeading($"Equality of {a} and {b}")
                               .And(a.Equals(b),             should.Constrain())
                               .And(b.Equals(a),             should.Constrain())
                               .And(Optional.AreEqual(a, b), should.Constrain())
                               .And(Optional.AreEqual(b, a), should.Constrain());
            }

            public static IMultipleAsserter Equality<T>(FailableFunc<T> failableFunc, T expectedValue, Should should) {
                return Asserter.Against(failableFunc)
                               .WithHeading($"Equality of {failableFunc.GetType().Prettify()} {failableFunc} and {typeof(T).Prettify()} {expectedValue}")
                               .And(it => it.Equals(expectedValue),                        should.Constrain(), $".Equals {should.Constrain().Prettify()}")
                               .And(it => Optional.AreEqual(it,            expectedValue), should.Constrain(), $"Optional.AreEqual {should.Constrain().Prettify()}")
                               .And(it => Optional.AreEqual(expectedValue, it),            should.Constrain(), $"Optional.AreEqual(reverse) {should.Constrain().Prettify()}");
            }

            public static IMultipleAsserter ObjectEquality<T>(FailableFunc<T> failableFunc, object? obj, Should should) {
                // Console.WriteLine($"Optional.AreEqual({failableFunc}, {obj}) => {Optional.AreEqual(failableFunc, obj)}");
                return Asserter.Against(failableFunc)
                               .WithHeading($"[{failableFunc}] should {(should.Boolean() ? "be" : "not be")} equal to [{obj}]; {failableFunc.Equals(obj!)}")
                               .And(it => it!.Equals(obj!), should.Constrain(), $"{failableFunc}.Equals({obj})")
                               .And(it => Equals(it, obj),  should.Constrain(), $"Equals({failableFunc}, {obj})");
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
            Asserter.Against(failable)
                    .And(Validate.Equality(failable, optional,       Should.Pass))
                    .And(Validate.Equality(failable, Expected_Value, Should.Pass))
                    .Invoke();
        }

        [Test]
        public void SuccessfulEqualsSelf() {
            var failable = Optional.Try(Succeed);
            Validate.Equality(failable, failable, Should.Pass).Invoke();
        }

        [Test]
        public void SuccessfulEqualsDuplicate() {
            var a = Optional.Try(Succeed);
            var b = Optional.Try(Succeed);
            Validate.Equality(a, b, Should.Pass).Invoke();
        }

        [Test]
        public void SuccessfulFailableInequality() {
            var failable = Optional.Try(Succeed);
            var optional = Optional.Of(Unexpected_Value);
            Asserter.Against(failable)
                    .And(Validate.Equality(failable, optional,         Should.Fail))
                    .And(Validate.Equality(failable, Unexpected_Value, Should.Fail))
                    .Invoke();
        }

        [Test]
        public void FailedFailableInequality() {
            var failable = Optional.Try(Fail);
            var optional = Optional.Of(Expected_Value);
            Asserter.Against(failable)
                    .And(Validate.Equality(failable, optional,              Should.Fail))
                    .And(Validate.Equality(failable, Expected_Value,        Should.Fail))
                    .And(Validate.Equality(failable, Optional.Try(Succeed), Should.Fail))
                    .Invoke();
        }

        [Test]
        public void FailedFailableEquality() {
            var failable = Optional.Try(Fail);
            var optional = new Optional<int>();
            Asserter.Against(failable)
                    .And(Validate.Equality(failable, optional,           Should.Pass))
                    .And(Validate.Equality(failable, failable,           Should.Pass))
                    .And(Validate.Equality(failable, Optional.Try(Fail), Should.Pass))
                    .Invoke();
        }

        [Test]
        public void FailableSuccessObjectEquality() {
            var failable  = Optional.Try(Succeed);
            var failable2 = Optional.Try(Succeed);
            Console.WriteLine($"int: {failable.Equals(5)}");
            Console.WriteLine($"obj: {failable.Equals((object)5)}");
            Console.WriteLine($"int-int: {5.Equals(5)}");
            Console.WriteLine($"int-obj: {5.Equals((object)5)}");
            Console.WriteLine($"obj-int: {((object)5).Equals(5)}");
            Asserter.Against(failable)
                    // .And(Validate.Equality(failable, Expected_Value, Should.Pass))
                    .And(Validate.ObjectEquality(failable, failable2, Should.Pass))
                    .Invoke();
        }

        [Test]
        public void FailableSuccessObjectInequality() {
            var failable  = Optional.Try(Succeed);
            var failable2 = Optional.Try(UnexpectedSuccess);
            Asserter.Against(failable)
                    .And(Validate.ObjectEquality(failable, Unexpected_Value, Should.Fail))
                    .And(Validate.ObjectEquality(failable, failable2,        Should.Fail))
                    .And(Validate.ObjectEquality(failable, null,             Should.Fail))
                    .Invoke();
        }
    }
}