using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory.SpanExtensions;

public class CountWhileTests {
    public record IsLetterCount(string Source, int Starting, int Ending) {
        public readonly Func<char, bool> Predicate = static c => char.IsLetter(c);
    }

    public static IsLetterCount[] IsLetterCounts = {
        new("abc", 3, 3),
        new("abc-", 3, 0),
        new("-abc", 0, 3),
        new("---", 0, 0),
        new("", 0, 0),
        new("abc-xz", 3, 2)
    };

    [Test]
    public void Span_CountWhile_IsLetter([ValueSource(nameof(IsLetterCounts))] IsLetterCount isLetterCount) {
        var span  = isLetterCount.Source.AsSpan();
        var count = span.CountWhile(isLetterCount.Predicate);
        Assert.That(count, Is.EqualTo(isLetterCount.Starting));
    }

    [Test]
    public void Span_CountWhileLast_IsLetter([ValueSource(nameof(IsLetterCounts))] IsLetterCount isLetterCount) {
        var span  = isLetterCount.Source.AsSpan();
        var count = span.CountLastWhile(isLetterCount.Predicate);
        Assert.That(count, Is.EqualTo(isLetterCount.Ending));
    }

    public static IEnumerable<int> Limits => Enumerable.Range(-5, 5);

    [Test]
    public void Span_CountWhile_IsLetter_Limited([ValueSource(nameof(IsLetterCounts))] IsLetterCount isLetterCount, [ValueSource(nameof(Limits))] int limit) {
        var span          = isLetterCount.Source.AsSpan();
        var actualCount   = span.CountWhile(isLetterCount.Predicate, limit);
        var expectedCount = isLetterCount.Starting.Clamp(0, limit.Max(0));
        Assert.That(actualCount, Is.EqualTo(expectedCount));
    }

    [Test]
    public void Span_CountWhileLast_IsLetter_Limited([ValueSource(nameof(IsLetterCounts))] IsLetterCount data, [ValueSource(nameof(Limits))] int limit) {
        var span          = data.Source.AsSpan();
        var actualCount   = span.CountLastWhile(data.Predicate, limit);
        var expectedCount = data.Starting.Clamp(0, limit.Max(0));
        Assert.That(actualCount, Is.EqualTo(expectedCount));
    }
}