using System;

using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory;

public class SkipWhileTests {
    [TestCase("abc123abc", "123abc")]
    [TestCase("  1a",      "  1a")]
    [TestCase("abc",       "")]
    public void SkipLetters(string input, string expected) {
        var actual = input.AsSpan()
                          .SkipWhile(static c => char.IsLetter(c))
                          .ToString();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("abc123abc", "abc123")]
    [TestCase("  1a",      "  1")]
    [TestCase("abc",       "")]
    [TestCase("1",         "1")]
    [TestCase("123",       "123")]
    public void SkipLastLetters(string input, string expected) {
        var actual = input.AsSpan()
                          .SkipLastWhile(static c => char.IsLetter(c))
                          .ToString();

        Assert.That(actual, Is.EqualTo(expected));
    }
}