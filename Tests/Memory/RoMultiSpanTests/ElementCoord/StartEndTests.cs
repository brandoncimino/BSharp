using System;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Memory.RoMultiSpanTests.ElementCoord;

public class StartEndTests {
    public record Expectation(string?[] Strings, (int, int) Start, (int, int) End) {
        public RoMultiSpan<char> Spans => FowlFever.BSharp.Memory.RoMultiSpan.Of(Strings);
    }

    public static Expectation[] Expectations = {
        new(new[] { "", "", "", "a" }, (3, 0), (3, 0)),
        new(new[] { "a", "", "" }, (0, 0), (0, 0)),
        new(new[] { null, "ab", null }, (1, 0), (1, 1)),
        new(Array.Empty<string>(), default, default),
        new(new[] { "" }, default, default),
        new(new[] { "", "" }, default, default),
        new(new[] { "a", "bcd" }, (0, 0), (1, 2)),
        new(new[] { "", "a", "", "bcd", "" }, (1, 0), (3, 2))
    };

    [Test]
    public void StartCoord([ValueSource(nameof(Expectations))] Expectation expectation) {
        Asserter.WithHeading().And(expectation.Spans.StartCoord, Is.EqualTo(expectation.Start)).Invoke();
    }

    [Test]
    public void EndCoord([ValueSource(nameof(Expectations))] Expectation expectation) {
        Asserter.WithHeading().And(expectation.Spans.EndCoord, Is.EqualTo(expectation.End)).Invoke();
    }
}