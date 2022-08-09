using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public class SkipTakeTests {
    private const string           Source   = "abcdefg";
    public static IEnumerable<int> HotTakes = Enumerable.Range(-5, 20);

    [Test]
    public void SkipTest([ValueSource(nameof(HotTakes))] int count) {
        var expected = Source.Skip(count).ToArray().AsSpan().ToString();
        var actual   = Source.AsSpan().Skip(count).ToString();

        Brandon.Print(expected);
        Brandon.Print(actual);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TakeTest([ValueSource(nameof(HotTakes))] int count) {
        var expected = Source.Take(count).ToArray().AsSpan().ToString();
        var actual   = Source.AsSpan().Take(count).ToString();

        Brandon.Print(expected);
        Brandon.Print(actual);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void SkipLast([ValueSource(nameof(HotTakes))] int skipCount) {
        var expected = Source.SkipLast(skipCount).ToArray().AsSpan().ToString();
        var actual   = Source.AsSpan().SkipLast(skipCount).ToString();

        Brandon.Print(expected);
        Brandon.Print(actual);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TakeLast([ValueSource(nameof(HotTakes))] int takeCount) {
        var expected = Source.TakeLast(takeCount).ToArray().AsSpan().ToString();
        var actual   = Source.AsSpan().TakeLast(takeCount).ToString();

        Brandon.Print(expected);
        Brandon.Print(actual);

        Assert.That(actual, Is.EqualTo(expected));
    }
}