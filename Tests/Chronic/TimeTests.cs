using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Chronic;
using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BSharp.Tests.Chronic;

public class TimeTests {
    private static double[] ValuesInSeconds = {
        5d,
        0.53,
        2,
        10,
        264576.523,
        7801.623,
        15.623,
        0.123,
        234678.234,
        345.4 * 645.2,
        Math.PI,
        0.1,
        0.01,
        0.001,
        0.00001,
        0
    };
    private static TimeSpan[] Spans = ValuesInSeconds.Select(TimeSpan.FromSeconds).ToArray();

    [Test]
    [Combinatorial]
    public void TestDivision(
        [ValueSource(nameof(Spans))]
        TimeSpan dividend,
        [ValueSource(nameof(Spans))]
        TimeSpan divisor
    ) {
#if NETSTANDARD2_1_OR_GREATER
        Assert.That(TimeUtils.Divide(dividend, divisor), Is.EqualTo(dividend / divisor));
#else
        Ignore.This("Only a valid test in .NET Standard 2.1+");
#endif
    }

    [Test]
    [Combinatorial]
    public void TestQuotient(
        [ValueSource(nameof(Spans))]
        TimeSpan dividend,
        [ValueSource(nameof(Spans))]
        TimeSpan divisor
    ) {
#if NETSTANDARD2_1_OR_GREATER
        Assert.That(TimeUtils.Quotient(dividend, divisor), Is.EqualTo(Math.Floor(dividend / divisor)));
#else
        Ignore.This("Only a valid test in .NET Standard 2.1+");
#endif
    }

    [Test]
    [Combinatorial]
    public void TestModulus(
        [ValueSource(nameof(Spans))]
        TimeSpan dividend,
        [ValueSource(nameof(Spans))]
        TimeSpan divisor
    ) {
        TimeSpan ExpectedModulus() {
            var millis = (dividend.TotalMilliseconds - ((dividend.TotalMilliseconds / divisor.TotalMilliseconds).Floor() * divisor.TotalMilliseconds));
            return TimeSpan.FromMilliseconds(millis);
        }

        Console.WriteLine($"{dividend} % {divisor}");

        IResolveConstraint expectation = divisor == TimeSpan.Zero ? Throws.ArgumentException : Throws.Nothing;

        // NOTE: my MultipleAsserter doesn't handle expected exceptions very well, so this uses the classic style
        Assert.That(() => dividend.Modulus(divisor), expectation);
    }

    [Test, Pairwise]
    public void TestSum(
        [ValueSource(nameof(ValuesInSeconds))]
        double a_seconds,
        [ValueSource(nameof(ValuesInSeconds))]
        double b_seconds,
        [ValueSource(nameof(ValuesInSeconds))]
        double c_seconds
    ) {
        var a_span = TimeSpan.FromSeconds(a_seconds);
        var b_span = TimeSpan.FromSeconds(b_seconds);
        var c_span = TimeSpan.FromSeconds(c_seconds);

        var ls = new List<TimeSpan> {
            a_span, b_span, c_span
        };

        Assert.That(ls.Sum(), Is.EqualTo(a_span + b_span + c_span));
    }
}