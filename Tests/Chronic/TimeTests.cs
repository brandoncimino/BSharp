using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Chronic;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests.Chronic {
    public class TimeTests {
        private static double[]   ValuesInSeconds = { 5d, 0.53, 2, 10, 264576.523, 7801.623, 15.623, 0.123, 234678.234, 345.4 * 645.2, Math.PI, 0.1, 0.01, 0.001, 0.00001, 0 };
        private static TimeSpan[] Spans           = ValuesInSeconds.Select(TimeSpan.FromSeconds).ToArray();

        [Test]
        [Combinatorial]
        public void TestDivision(
            [ValueSource(nameof(ValuesInSeconds)), Values(0)]
            double dividendSeconds,
            [ValueSource(nameof(ValuesInSeconds))]
            double divisorSeconds
        ) {
            dividendSeconds = TimeUtils.NormalizeSeconds(dividendSeconds);
            divisorSeconds  = TimeUtils.NormalizeSeconds(divisorSeconds);

            Assume.That(divisorSeconds, Is.Not.EqualTo(0), "Checking for division by zero is a different test!");

            var dividend = TimeSpan.FromSeconds(dividendSeconds);
            var divisor  = TimeSpan.FromSeconds(divisorSeconds);

            var expectedDivisionResult = (double)dividend.Ticks / divisor.Ticks;

            Assert.That(dividend.Divide(divisor), Is.EqualTo(expectedDivisionResult));
        }

        [Test]
        public void TestDivisionByZero(
            [ValueSource(nameof(ValuesInSeconds)), Values(0)]
            double dividendSeconds
        ) {
            var dic = new Dictionary<object, Func<object>>() {
                ["1s / 0s"]      = () => TimeSpan.FromSeconds(1) / TimeSpan.Zero,
                ["1s.Div(0s)"]   = () => TimeSpan.FromSeconds(1).Divide(TimeSpan.Zero),
                ["1s / 0 int"]   = () => TimeSpan.FromSeconds(1) / 0,
                ["1s / 0d"]      = () => TimeSpan.FromSeconds(1) / 0d,
                ["1s.Div(0int)"] = () => TimeSpan.FromSeconds(1).Divide(0),
                ["1s.Div(0d)"]   = () => TimeSpan.FromSeconds(1).Divide(0d),
            };

            var failures = dic.ToDictionary(
                it => it.Key,
                it => {
                    var failable = it.Value.Try();
                    return failable.Failed ? failable.Excuse : failable.Value;
                }
            );

            Console.WriteLine(failures.Prettify());

            // Assert.That(() => TimeUtils.Divide(TimeSpan.FromSeconds(dividendSeconds), TimeSpan.Zero), Throws.InstanceOf<DivideByZeroException>());

            var dividend = TimeSpan.FromSeconds(dividendSeconds);
            Assert.That(TimeUtils.Divide(dividend, TimeSpan.Zero), Is.EqualTo(dividend / TimeSpan.Zero));
        }

        [Test]
        [Combinatorial]
        public void TestQuotient(
            [ValueSource(nameof(Spans))]
            TimeSpan dividend,
            [ValueSource(nameof(Spans))]
            TimeSpan divisor
        ) {
            Assert.That(TimeUtils.Quotient(dividend, divisor), Is.EqualTo(Math.Floor(dividend / divisor)));
        }

        [Test]
        [Combinatorial]
        public void TestQuotient_old(
            [ValueSource(nameof(ValuesInSeconds)), Values(0)]
            double dividendSeconds,
            [ValueSource(nameof(ValuesInSeconds))]
            double divisorSeconds
        ) {
            dividendSeconds = TimeUtils.NormalizeSeconds(dividendSeconds);
            divisorSeconds  = TimeUtils.NormalizeSeconds(divisorSeconds);

            Assume.That(divisorSeconds, Is.Not.EqualTo(0), "Checking for division by zero is a different test!");

            var dividend = TimeSpan.FromSeconds(dividendSeconds);
            var divisor  = TimeSpan.FromSeconds(divisorSeconds);

            Assert.That(dividend.Quotient(divisor), Is.EqualTo(dividend.Ticks / divisor.Ticks));
        }

        [Test]
        public void TestQuotientByZero(
            [ValueSource(nameof(ValuesInSeconds)), Values(0)]
            double dividendSeconds
        ) {
            Assert.Throws<DivideByZeroException>(() => TimeSpan.FromSeconds(dividendSeconds).Quotient(TimeSpan.Zero));
        }

        [Test]
        [Combinatorial]
        public void TestModulusCombinatorial(
            [ValueSource(nameof(ValuesInSeconds)), Values(0)]
            double dividendSeconds,
            [ValueSource(nameof(ValuesInSeconds))]
            double divisorSeconds
        ) {
            dividendSeconds = TimeUtils.NormalizeSeconds(dividendSeconds);
            divisorSeconds  = TimeUtils.NormalizeSeconds(divisorSeconds);

            Assume.That(divisorSeconds, Is.Not.EqualTo(0), "Checking for division by zero is a different test!");

            var dividend         = TimeSpan.FromSeconds(dividendSeconds);
            var divisor          = TimeSpan.FromSeconds(divisorSeconds);
            var dividendTicks    = dividend.Ticks;
            var divisorTicks     = divisor.Ticks;
            var expectedModTicks = dividendTicks % divisorTicks;
            var expectedModSpan  = TimeSpan.FromTicks(expectedModTicks);

            Assert.That(dividend.Modulus(divisor), Is.EqualTo(expectedModSpan), $"The modulus of {dividend} % {divisor} = {expectedModSpan}\n\tIn Ticks: {dividendTicks} % {divisorTicks} = {expectedModTicks}");
        }

        [Test]
        public void TestModulusByZero(
            [ValueSource(nameof(ValuesInSeconds)), Values(0)]
            double dividendSeconds
        ) {
            Assert.Throws<DivideByZeroException>(() => TimeSpan.FromSeconds(dividendSeconds).Modulus(TimeSpan.Zero));
        }

        [Test]
        public void TestMultiply(
            [ValueSource(nameof(Spans))]
            TimeSpan multiplicand,
            [ValueSource(nameof(ValuesInSeconds))]
            double multiplier
        ) {
            Asserter.Against(() => TimeUtils.Multiply(multiplicand, multiplier))
                    .And(Is.EqualTo(multiplicand * multiplier))
                    .And(Is.EqualTo(multiplicand.Multiply(multiplier)))
                    .Invoke();
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

            var ls = new List<TimeSpan> { a_span, b_span, c_span };

            Assert.That(ls.Sum(), Is.EqualTo(a_span + b_span + c_span));
        }
    }
}