using System;
using System.Linq;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using JetBrains.Annotations;

using NUnit.Framework;

namespace BSharp.Tests.Memory.RoMultiSpanTests;

public static class AssertionExtensions {
    private static string[] NullToEmpty(this string?[]? strings) {
        return strings == null ? Array.Empty<string>() : strings.Select(it => it ?? "").ToArray();
    }

    [MustUseReturnValue]
    public static T AndBuild<T>(this IMultipleAsserter<T> asserter, RoMultiSpan<char>.Builder builder, params string?[] expected) where T : IMultipleAsserter<T> {
        try {
            expected = expected.NullToEmpty();
            return asserter
                   .And(builder.Build().ToStringArray(), Is.EqualTo(expected))
                   .Self;
        }
        catch (Exception e) {
            return asserter
                .And(e, Is.Null);
        }
    }

    [MustUseReturnValue]
    public static T IsEmpty<T>(this IMultipleAsserter<T> asserter, RoMultiSpan<char>.Builder builder) where T : IMultipleAsserter<T> {
        return asserter.And(builder.Count, Is.EqualTo(0))
                       .And(builder.RemainingSpans, Is.EqualTo(FowlFever.BSharp.Memory.RoMultiSpan.MaxSpans))
                       .AndBuild(builder)
                       .Self;
    }
}

public class BuilderTests {
    [Test]
    public void DefaultStateTest() {
        Asserter.WithHeading()
                .IsEmpty(default(RoMultiSpan<char>.Builder))
                .IsEmpty(new RoMultiSpan<char>.Builder())
                .Invoke();
    }

    [Test]
    public void CannotAddBeyondMaxSpans() {
        var builder = FowlFever.BSharp.Memory.RoMultiSpan.CreateBuilder<char>();
        for (int i = 0; i < FowlFever.BSharp.Memory.RoMultiSpan.MaxSpans; i++) {
            builder.Add($"[{i}]");
        }

        try {
            builder.Add("FAIL");
            Assert.Fail("expected an exception");
        }
        catch (InvalidOperationException e) {
            Assert.Pass();
        }
    }

    [Test]
    public void AddTest([Values("abc", null, "", " ")] string? addedString) {
        var builder = FowlFever.BSharp.Memory.RoMultiSpan.CreateBuilder<char>();
        using var ass = Asserter.WithHeading()
                                .And(builder.Count,                  Is.EqualTo(0))
                                .And(builder.Add(addedString).Count, Is.EqualTo(1))
                                .And(builder[0].ToString(),          Is.EqualTo(addedString))
                                .AndBuild(builder, addedString);
    }

    [Test]
    [TestCase("abc", "a", " ", "", null, "yolo")]
    public void AddRepeatedTest(params string?[] strings) {
        var builder = FowlFever.BSharp.Memory.RoMultiSpan.CreateBuilder<char>();
        using var ass = Asserter.WithHeading()
                                .And(builder.Count, Is.EqualTo(0))
                                .AndBuild(builder);

        for (int i = 0; i < strings.Length; i++) {
            ass.And(builder.Add(strings[i]).Count, Is.EqualTo(i + 1))
               .AndBuild(builder, strings[..(i + 1)]);
        }
    }

    [Test]
    [TestCase("abc", "a", " ", "", null, "yolo")]
    public void AddRangeTest(
        string a,
        string b,
        string c,
        string d,
        string e,
        string f
    ) {
        var builder = FowlFever.BSharp.Memory.RoMultiSpan.CreateBuilder<char>();
        var strings = FowlFever.BSharp.Memory.RoMultiSpan.Of(a, b, c, d, e, f);

        using var ass = Asserter.WithHeading()
                                .And(builder.AddRange(strings).Count, Is.EqualTo(strings.SpanCount))
                                .AndBuild(builder, strings.ToStringArray());
    }

    [Test]
    public void CannotRemoveWhenEmpty() {
        var builder = RoMultiSpan<char>.CreateBuilder();
        try {
            builder.Remove();
            Assert.Fail("expected an exception");
        }
        catch (InvalidOperationException e) {
            Assert.Pass();
        }
    }

    [Test]
    public void RemoveTest() {
        var builder = FowlFever.BSharp.Memory.RoMultiSpan.CreateBuilder<char>()
                               .Add("one")
                               .Add("two")
                               .Add("three");

        Asserter.WithHeading()
                .And(builder.Count,          Is.EqualTo(3))
                .And(builder.Remove().Count, Is.EqualTo(2))
                .And(builder[0].ToString(),  Is.EqualTo("one"))
                .And(builder[1].ToString(),  Is.EqualTo("two"))
                .Invoke();
    }

    [Test]
    public void RemoveOutOfRange() {
        var builder = RoMultiSpan<char>.CreateBuilder()
                                       .Add("one")
                                       .Add("two")
                                       .Add("three");

        try {
            builder.RemoveAt(4);
            Assert.Fail("expected an exception");
        }
        catch (ArgumentOutOfRangeException e) {
            Assert.Pass();
        }
    }
}