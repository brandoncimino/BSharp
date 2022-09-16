using System;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Memory.RoMultiSpan.ElementCoord;

public class StartEndTests {
    public record Expectation(string?[] Strings, (int, int)? StartIndices, (int, int)? EndIndices) {
        public RoMultiSpan<char>              Spans => FowlFever.BSharp.Memory.RoMultiSpan.Of(Strings);
        public RoMultiSpan<char>.ElementCoord Start => StartIndices;
        public RoMultiSpan<char>.ElementCoord End   => EndIndices;
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
        Asserter.WithHeading().And(expectation.Spans.StartCoord, expectation.Start);
    }

    [Test]
    public void EndCoord([ValueSource(nameof(Expectations))] Expectation expectation) {
        Asserter.WithHeading().And(expectation.Spans.EndCoord, expectation.End);
    }
}