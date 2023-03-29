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

    [Test]
    [TestCase("a-b-c",        '-', StringSplitOptions.None,                                                "a",  "b",  "c")]
    [TestCase("aa--bb--cc",   '-', StringSplitOptions.None,                                                "aa", "",   "bb", "", "cc")]
    [TestCase("aa--bb--cc",   '-', StringSplitOptions.RemoveEmptyEntries,                                  "aa", "bb", "cc")]
    [TestCase(" x- - y --z ", '-', StringSplitOptions.None,                                                " x", " ",  " y ", "", "z ")]
    [TestCase(" x- - y --z ", '-', StringSplitOptions.RemoveEmptyEntries,                                  " x", " ",  " y ", "z ")]
    [TestCase(" x- - y --z ", '-', StringSplitOptions.TrimEntries,                                         "x",  "",   "y",   "", "z")]
    [TestCase(" x- - y --z ", '-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries, "x",  "y",  "z")]
    public void SpliterateString_Simple(string source, char splitter, StringSplitOptions options, params string[] expectedParts) {
        var span        = source.AsSpan();
        var spliterator = span.Spliterate(stackalloc[] { splitter }) with { Options = options };
        var parts       = spliterator.ToStringList().ToArray();
        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [Test]
    [TestCase("a-b!c-d*e", "-!", "a", "b", "c", "d*e")]
    public void SpliterateString_Any(string source, string splitters, params string[] expectedParts) {
        var span        = source.AsSpan();
        var spliterator = span.Spliterate(splitters.AsSpan()) with { MatchStyle = SplitterMatchStyle.AnyEntry };
        var parts       = spliterator.ToStringList().ToArray();
        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    [Test]
    [TestCase("a1b12c21d12e", "12", "a1b", "c21d", "e")]
    public void SpliterateString_SubSeq(string source, string splitSeq, params string[] expectedParts) {
        var span        = source.AsSpan();
        var spliterator = span.Spliterate(splitSeq) with { MatchStyle = SplitterMatchStyle.SubSequence };
        var parts       = spliterator.ToStringList().ToArray();
        Assert.That(parts, Is.EquivalentTo(expectedParts));
    }

    #endregion
}