using System;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Memory.RoMultiSpanTests;

public class SkipTests {
    [TestCase(new[] { "yolo", "swag" },                 2, new[] { "lo", "swag" })]
    [TestCase(new[] { "yolo", "swag" },                 4, new[] { "swag" })]
    [TestCase(new[] { "yolo", "swag" },                 6, new[] { "ag" })]
    [TestCase(new[] { "yolo", "swag" },                 8, new string[] { })]
    [TestCase(new[] { "", "", "yolo", "", "swag", "" }, 2, new[] { "lo", "", "swag", "" })]
    public void Skip(string[] strings, int amountToSkip, string[] expected) {
        var skipped = RoMultiSpan.Of(strings).SkipElements(amountToSkip);
        Console.WriteLine($"skipped: {skipped.ToStringArray().JoinString(", ")}");
        Asserter.WithHeading()
                .And(skipped.ToStringArray(), Is.EqualTo(expected))
                .Invoke();
    }

    [TestCase(new[] { "yolo", "swag" },                 2, new[] { "yolo", "sw" })]
    [TestCase(new[] { "yolo", "swag" },                 4, new[] { "yolo" })]
    [TestCase(new[] { "yolo", "swag" },                 6, new[] { "yo" })]
    [TestCase(new[] { "yolo", "swag" },                 8, new string[] { })]
    [TestCase(new[] { "", "", "yolo", "", "swag", "" }, 2, new[] { "", "", "yolo", "", "sw" })]
    public void SkipLast(string[] strings, int amountToSkip, string[] expected) {
        var skipped = RoMultiSpan.Of(strings).SkipLastElements(amountToSkip);
        Asserter.WithHeading()
                .And(skipped.ToStringArray(), Is.EqualTo(expected))
                .Invoke();
    }
}