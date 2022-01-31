using System;

using FowlFever.BSharp.Chronic;

using NUnit.Framework;

using Is = FowlFever.Testing.Is;

namespace BSharp.Tests.Chronic; 

public class RateTests {
    [Test]
    [TestCase(1,   1)]
    [TestCase(0,   double.PositiveInfinity)]
    [TestCase(10,  0.1)]
    [TestCase(2,   0.5)]
    [TestCase(-5,  -0.2)]
    [TestCase(0.1, 10)]
    public void SetInterval(double intervalInSeconds, double expectedHertz) {
        var rate = new Rate { Interval = TimeSpan.FromSeconds(intervalInSeconds) };
        Assert.That(rate, Has.Property(nameof(rate.Hertz)).EqualTo(expectedHertz));
    }

    [TestCase(1,                       1)]
    [TestCase(double.PositiveInfinity, 0)]
    [TestCase(double.NegativeInfinity, -0)]
    [TestCase(10,                      0.1)]
    public void SetHertz(double hertz, double expectedIntervalInSeconds) {
        var expectedInterval = TimeSpan.FromSeconds(expectedIntervalInSeconds);
        var rate             = new Rate() { Hertz = hertz };
        Assert.That(rate, Has.Property(nameof(rate.Interval)).EqualTo(expectedInterval));
    }

    [TestCase(1, TimeUnit.Minutes, 60)]
    [TestCase(5, TimeUnit.Hours,   5 * 60 * 60)]
    public void PerTimeUnit(double hertz, TimeUnit timeUnit, double expectedAmount) {
        var rate = new Rate() { Hertz = hertz };
        Assert.That(rate.Per(timeUnit), Is.EqualTo(expectedAmount));
    }

    [TestCase(3, 10, 30)]
    public void PerDuration(double hertz, double durationInSeconds, double expectedAmount) {
        var rate = new Rate() { Hertz = hertz };
        Assert.That(rate.Per(TimeSpan.FromSeconds(durationInSeconds)), Is.CloseTo(expectedAmount));
    }
}