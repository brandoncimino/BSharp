using System;

using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace BSharp.Tests.Testing;

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

    /// <summary>
    /// NOTE: <see cref="SuccessException"/> is no longer properly caught inside of delegates in NUnit 😿
    /// </summary>
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

    private static void ValidateMultiAss_NoFailures<TAsserter>()
        where TAsserter : MultipleAsserter<TAsserter, object>, new() {
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
                Assumer.Against(5)
                       .And(Is.EqualTo("b"))
                       .And(Is.Zero)
                       .And(Is.GreaterThan(double.MinValue))
                       .And(Has.Member("yolo"))
                       .And(Is.EqualTo(2))
                       .Invoke()
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

    [Test]
    public void ActualValueDelegateIsOnlyCalledOnce() {
        var fnCount = 0;

        int Fn() {
            fnCount++;
            Console.WriteLine($"{nameof(fnCount)} = {fnCount}");
            return fnCount;
        }

        Assert.That(fnCount, Is.EqualTo(0));

        var ass = Asserter.Against(Fn)
                          .And(Is.EqualTo(1).And.GreaterThan(0).And.EqualTo(1).And.LessThan(2))
                          .And(it => it * 2, Is.EqualTo(2))
                          .And(Is.EqualTo(1))
                          .And(DoubleMe, Is.EqualTo(2))
                          .And(it => { Console.WriteLine("🦥"); });

        ass.Invoke();
        ass.Invoke();
        ass.Invoke();

        Assert.That(fnCount, Is.EqualTo(1));
    }

    private static int DoubleMe(int i) {
        return i * 2;
    }

    [Test]
    public void AndAgainstDoesNotNeedActualUntilInvoked() {
        var asserter = Asserter.WithHeading("hi")
                               .AndAgainst(it => it, ass => ass.And(Is.Not.Null));

        // NOTE: Assert.Throws<Exception> requires that EXACTLY System.Exception is thrown, which is useless
        Assert.That(asserter.Invoke, Throws.Exception);
    }

    [Test]
    public void ConstraintAgainstActual() {
        Asserter.Against(5).And(Is.EqualTo(5)).Invoke();
    }

    [Test]
    public void AndAgainstFailsIfInvokedWithoutActual() {
        var asserter = Asserter.WithHeading<int>(nameof(AndAgainstFailsIfInvokedWithoutActual))
                               .AndAgainst(it => it * 2, ass => ass.And(Is.EqualTo(3)));


        using (new TestExecutionContext.IsolatedContext()) {
            Assert.That(asserter.Invoke, Throws.Exception);
        }
    }

    [Test]
    public void AndAgainstWorksWhenActualWasSetLate() {
        var asserter = Asserter.WithHeading<int>(nameof(AndAgainstWorksWhenActualWasSetLate))
                               .AndAgainst(it => it, ass => ass.And(Is.EqualTo(3)))
                               .Against(3);

        Assert.That(asserter.Invoke, Throws.Nothing);
    }
}