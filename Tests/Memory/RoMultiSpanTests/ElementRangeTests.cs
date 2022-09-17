using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Memory.RoMultiSpanTests;

public class ElementRangeTests {
    public record ElementRangeExpectation(
        string[] Strings,
        Range    Range,
        string[] Sliced
    ) {
        public RoMultiSpan<char> Spans => RoMultiSpan.Of(Strings);
    }

    public static ElementRangeExpectation[] Expectations = {
        new(
            new[] { "swag" },
            1..2,
            new[] { "w" }
        ),
        new(
            new[] { "yolo" },
            1..,
            new[] { "olo" }
        ),
        new(
            new[] { "abc", "xyz" },
            2..4,
            new[] { "c", "x" }
        ),
        new(
            new[] { "abc", "xyz" },
            2..,
            new[] { "c", "xyz" }
        ),
        new(
            new[] { "", "abcd" },
            ..^1,
            new[] { "", "abc" }
        ),
    };

    [Test]
    public void SliceElements([ValueSource(nameof(Expectations))] ElementRangeExpectation expectation) {
        var flat   = expectation.Sliced.Flatten().ToArray();
        var actual = new List<char>();
        foreach (var c in expectation.Spans.SliceElements(expectation.Range).EnumerateElements()) {
            actual.Add(c);
        }

        var exp = string.Join("", expectation.Strings)[expectation.Range];
        Console.WriteLine($"flat: {flat.JoinString()}");
        Console.WriteLine($"exp:  {exp}");
        var actualStr = new string(actual.ToArray());
        Asserter.WithHeading().And(actualStr, Is.EqualTo(exp)).Invoke();
    }
}