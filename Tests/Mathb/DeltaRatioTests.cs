using System;

using NUnit.Framework;

namespace BSharp.Tests.Mathb;

public class DeltaRatioTests {
    [Test]
    [TestCase(6,                       9,                       .5)]
    [TestCase(10,                      7.5,                     -.25)]
    [TestCase(0,                       0,                       0)]
    [TestCase(0,                       100,                     double.PositiveInfinity)]
    [TestCase(0,                       -double.Epsilon,         double.NegativeInfinity)]
    [TestCase(100,                     0,                       -1)]
    [TestCase(5,                       5,                       0)]
    [TestCase(5,                       -5,                      -2)]
    [TestCase(3,                       double.NegativeInfinity, double.NegativeInfinity)]
    [TestCase(5,                       double.PositiveInfinity, double.PositiveInfinity)]
    [TestCase(double.PositiveInfinity, double.PositiveInfinity, double.NaN)]
    [TestCase(double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity)]
    [TestCase(double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity)]
    [TestCase(double.NegativeInfinity, double.NegativeInfinity, double.NaN)]
    [TestCase(double.PositiveInfinity, 0,                       double.NegativeInfinity)]
    [TestCase(double.NegativeInfinity, 0,                       double.PositiveInfinity)]
    [TestCase(double.Epsilon,          0,                       -1)]
    [TestCase(Math.PI,                 0,                       -1)]
    [TestCase(-8,                      -12,                     .5)]
    public void DeltaRatio(double a, double b, double expectedDeltaRatio) {
        Assert.That(FowlFever.BSharp.Mathb.DeltaRatio(a, b), Is.EqualTo(expectedDeltaRatio));
    }
}