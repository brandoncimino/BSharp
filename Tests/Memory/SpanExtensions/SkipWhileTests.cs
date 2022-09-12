using System;

using FowlFever.BSharp;
using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public class SkipWhileTests {
    [TestCase("abc123abc", "123abc")]
    [TestCase("  1a",      "  1a")]
    [TestCase("abc",       "")]
    [TestCase("abc",       "c", 2)]
    public void SkipLetters(string input, string expected, int limit = int.MaxValue) {
        var actual = input.AsSpan()
                          .SkipWhile(static c => char.IsLetter(c), limit)
                          .ToString();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("abc123abc", "abc123")]
    [TestCase("  1a",      "  1")]
    [TestCase("abc",       "")]
    [TestCase("1",         "1")]
    [TestCase("123",       "123")]
    [TestCase("abc",       "a", 2)]
    public void SkipLastLetters(string input, string expected, int limit = int.MaxValue) {
        var countLastWhile = input.AsSpan().CountLastWhile(static c => char.IsLetter(c));
        Brandon.Print(countLastWhile);

        var actual = input.AsSpan()
                          .SkipLastWhile(static c => char.IsLetter(c), limit)
                          .ToString();

        Assert.That(actual, Is.EqualTo(expected));
    }
}