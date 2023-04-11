using System;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public partial class SpanExtensionTests {
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
        var actual = expectation.Source.AsSpan().Take(expectation.UnsafeRange);
        Assert.That(actual.ToString(), Is.EqualTo(expectation.ExpectedSlice));
    }

    [Test]
    public void SafeSliceTest_FutureTake([ValueSource(nameof(SafeSliceExpectations))] SafeSliceExpectation expectation) {
        var actual   = expectation.Source.AsSpan().Take(expectation.UnsafeRange);
        var expected = expectation.Source.Take(expectation.UnsafeRange).JoinString();
        Assert.That(actual.ToString(), Is.EqualTo(expected));
    }

    #region Spliterate

    [TestCase("",             '-')]
    [TestCase("a",            'a', "",   "")]
    [TestCase("a-b-c",        '-', "a",  "b", "c")]
    [TestCase("aa--bb--cc",   '-', "aa", "",  "bb",  "", "cc")]
    [TestCase(" x- - y --z ", '-', " x", " ", " y ", "", "z ")]
    public void SpliterateString_SingleSplitter(string source, char splitter, params string[] expectedParts) {
        var parts = source.AsSpan()
                          .Spliterate(splitter)
                          .ToStringList();

        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [TestCase("a-b-c-d", '-', -1, "a-b-c-d")]
    [TestCase("a-b-c-d", '-', 0,  "a-b-c-d")]
    [TestCase("a-b-c-d", '-', 1,  "a-b-c-d")]
    [TestCase("a-b-c-d", '-', 2,  "a", "b-c-d")]
    [TestCase("a-b-c-d", '-', 3,  "a", "b", "c-d")]
    [TestCase("a-b-c-d", '-', 4,  "a", "b", "c", "d")]
    [TestCase("a-b-c-d", '-', 5,  "a", "b", "c", "d")]
    public void SpliterateString_PartitionLimit(string source, char splitter, int limit, params string[] expectedParts) {
        var parts = source.AsSpan()
                          .Spliterate(splitter, limit)
                          .ToStringList();

        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [TestCase("",          "abc")]
    [TestCase("abc",       "z",  "abc")]
    [TestCase("abcd",      "c",  "ab", "d")]
    [TestCase("a-b!c-d*e", "-!", "a",  "b", "c", "d*e")]
    public void SpliterateString_Any(string source, string splitters, params string[] expectedParts) {
        var parts = source.AsSpan()
                          .SpliterateAny(splitters.AsSpan())
                          .ToStringList();

        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [TestCase("",             "abc")]
    [TestCase("abc",          "bc", "a",   "")]
    [TestCase("a",            "a",  "",    "")]
    [TestCase("a1b12c21d12e", "12", "a1b", "c21d", "e")]
    public void SpliterateString_SubSeq(string source, string splitSeq, params string[] expectedParts) {
        var parts = source.AsSpan()
                          .Spliterate(splitSeq)
                          .ToStringList();

        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    #endregion
}