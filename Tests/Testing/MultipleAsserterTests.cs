using System;

using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Testing {
    public class MultipleAsserterTests {
        [Test]
        public void MultipleAsserter_Asserter() {
            ValidateMultiAss<Asserter<object>, AssertionException>();
        }

        [Test]
        public void MultipleAsserter_Assumer() {
            ValidateMultiAss<Assumer<object>, InconclusiveException>();
        }

        [Test]
        public void MultipleAsserter_Ignorer() {
            ValidateMultiAss<Ignorer<object>, IgnoreException>();
        }

        private static void ValidateMultiAss<TAsserter, TException>()
            where TAsserter : MultipleAsserter<TAsserter, object>, new()
            where TException : Exception {
            var asserter = new TAsserter()
                           .Against(9)
                           .And(Is.GreaterThan(100))
                           .And(() => Assert.Ignore("IGNORED"))
                           .And(() => throw new NullReferenceException())
                           .And(Is.Null);

            Assert.Throws<TException>(asserter.Invoke);
        }

        [Test]
        public void MultipleAsserter_Asserter_NoFailures() {
            ValidateMultiAss_NoFailures<Asserter<object>>();
        }

        [Test]
        public void MultipleAsserter_Assumer_NoFailures() {
            ValidateMultiAss_NoFailures<Assumer<object>>();
        }

        [Test]
        public void MultipleAsserter_Ignorer_NoFailures() {
            ValidateMultiAss_NoFailures<Ignorer<object>>();
        }

        private static void ValidateMultiAss_NoFailures<TAsserter>() where TAsserter : MultipleAsserter<TAsserter, object>, new() {
            var asserter = new TAsserter()
                           .Against("yolo")
                           .And(Is.Not.EqualTo("swag"))
                           .And(() => Console.WriteLine("yolo action"))
                           .And(Assert.Pass);

            Assert.DoesNotThrow(asserter.Invoke);
        }

        #region {x}All.Of() classes

        [Test]
        public void AssertAll_WithFailures() {
            Assert.Throws<AssertionException>(
                () =>
                    AssertAll.Of(
                        5,
                        Is.EqualTo(9),
                        Is.LessThan(double.PositiveInfinity),
                        Is.InstanceOf(typeof(DayOfWeek))
                    )
            );
        }

        [Test]
        public void AssumeAll_WithFailures() {
            Assert.Throws<InconclusiveException>(
                () =>
                    AssumeAll.Of(
                        5,
                        Is.EqualTo("b"),
                        Is.Zero,
                        Is.GreaterThan(double.MinValue),
                        Has.Member("yolo"),
                        Is.EqualTo(2)
                    )
            );
        }

        [Test]
        public void IgnoreAll_WithFailures() {
            Assert.Throws<IgnoreException>(
                () =>
                    Ignore.Unless(
                        5,
                        Is.False,
                        Is.Unique,
                        Is.EqualTo(double.PositiveInfinity)
                    )
            );
        }

        #endregion
    }
}