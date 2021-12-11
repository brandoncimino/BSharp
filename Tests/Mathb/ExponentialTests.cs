using FowlFever.BSharp;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Mathb {
    public class ExponentialTests {
        [Test]
        [TestCase(25,      4,   250_000)]
        [TestCase(123,     -3,  0.123)]
        [TestCase(0,       999, 0)]
        [TestCase(999,     0,   999)]
        [TestCase(-4.3,    -2,  -0.043)]
        [TestCase(-99.123, 6,   -99_123_000)]
        public void ShiftDecimal(double original, int decimalPlaces, double expected) {
            Assert.That(original.ShiftDecimal(decimalPlaces), Is.EqualTo(expected));
        }
    }
}