using System;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Collections;

public class SpanExtensionTests {
    public record SafeSliceExpectation(string Source, Range UnsafeRange, string ExpectedSlice, Range ExpectedRange);

    public static SafeSliceExpectation[] SafeSliceExpectations = {
        new("abc", .., "abc", ..),
        new("abc", 1..99, "bc", 1..),
        new("abc", ^5..^4, "", ..0),
        new("abc", ^99..1, "a", ..1),
        new("abc", 99.., "", ^0..)
    };

    [Test]
    public void ClampedRangeTest([ValueSource(nameof(SafeSliceExpectations))] SafeSliceExpectation expectation) {
        Assert.That(expectation.UnsafeRange.Clamp(expectation.Source.Length), Is.EqualTo(expectation.ExpectedRange));
    }

    [Test]
    public void SafeSliceTest([ValueSource(nameof(SafeSliceExpectations))] SafeSliceExpectation expectation) {
        var actual = expectation.Source.AsSpan().SafeSlice(expectation.UnsafeRange);
        Assert.That(actual.ToString(), Is.EqualTo(expectation.ExpectedSlice));
    }

    [Test]
    public void SafeSliceTest_FutureTake([ValueSource(nameof(SafeSliceExpectations))] SafeSliceExpectation expectation) {
        var actual   = expectation.Source.AsSpan().SafeSlice(expectation.UnsafeRange);
        var expected = expectation.Source.Take(expectation.UnsafeRange).JoinString();
        Assert.That(actual.ToString(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("abc", 0,  "abc")]
    [TestCase("abc", 1,  "bc")]
    [TestCase("abc", 2,  "c")]
    [TestCase("abc", 3,  "")]
    [TestCase("abc", 4,  "")]
    [TestCase("abc", -1, "abc")]
    public void SkipTest(string original, int skipAmount, string expected) {
        var strSkip = original.Skip(skipAmount).JoinString();
        Console.WriteLine($"string skip: [{strSkip}]");
        var ogSpan = original.AsSpan();
        var result = ogSpan.Skip(skipAmount).ToString();
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(result, Is.EqualTo(strSkip));
    }

    public static int[] HotTakes = new[] { -1, 0, 1, 3, 10 };

    [Test]
    public void SkipLastTest([ValueSource(nameof(HotTakes))] int toSkip) {
        var str          = "abc";
        var strSkipLast  = str.SkipLast(toSkip).JoinString();
        var span         = str.AsSpan();
        var spanSkipLast = span.SkipLast(toSkip);
        Assert.That(spanSkipLast.ToString(), Is.EqualTo(strSkipLast));
    }

    [Test]
    public void TakeTest([ValueSource(nameof(HotTakes))] int toTake) {
        var str      = "abc";
        var strTake  = str.Take(toTake).JoinString();
        var span     = str.AsSpan();
        var spanTake = span.Take(toTake);
        Assert.That(spanTake.ToString(), Is.EqualTo(strTake));
    }

    [Test]
    public void TakeLastTest([ValueSource(nameof(HotTakes))] int toTake) {
        var str          = "abc";
        var strTakeLast  = str.TakeLast(toTake).JoinString();
        var span         = str.AsSpan();
        var spanTakeLast = span.TakeLast(toTake);
        Assert.That(spanTakeLast.ToString(), Is.EqualTo(strTakeLast));
    }

    [Test]
    [TestCase("a-b-c", '-', "a", "b", "c")]
    public void SpliterateString_Simple(string source, char splitter, params string[] expectedParts) {
        var span        = source.AsSpan();
        var spliterator = span.Spliterate(splitter);
        var parts       = spliterator.ToStringArray();
        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [Test]
    [TestCase("a-b!c-d*e", "-!", "a", "b", "c", "d*e")]
    public void SpliteratString_Any(string source, string splitters, params string[] expectedParts) {
        var span        = source.AsSpan();
        var spliterator = span.Spliterate(splitters.AsSpan(), SplitterStyle.AnyEntry);
        var parts       = spliterator.ToStringArray();
        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [Test]
    [TestCase("a1b12c21d12e", "12", "a1b", "c21d", "e")]
    public void SpliterateString_SubSeq(string source, string splitSeq, params string[] expectedParts) {
        var span        = source.AsSpan();
        var spliterator = span.Spliterate(splitSeq.AsSpan(), SplitterStyle.SubSequence);
        var parts       = spliterator.ToStringArray();
        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }
}