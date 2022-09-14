using System;
using System.Collections.Generic;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Memory;

public class RoMultiSpanTests {
    private static RoMultiSpan<char> ToSpans(IEnumerable<string> strings) {
        var rospan = new RoMultiSpan<char>();

        foreach (var s in strings) {
            rospan = rospan.Add(s);
        }

        return rospan;
    }

    [Test]
    public void RoMultiSpan_Default_Counts() {
        RoMultiSpan<char> spans = default;
        Asserter.WithHeading()
                .And(spans.SpanCount,    Is.EqualTo(0))
                .And(spans.ElementCount, Is.EqualTo(0))
                .Invoke();
    }

    [Test]
    public void RoMultiSpan_NewEmpty_Counts() {
        RoMultiSpan<char> spans = new();
        Asserter.WithHeading()
                .And(spans.SpanCount,    Is.EqualTo(0))
                .And(spans.ElementCount, Is.EqualTo(0))
                .Invoke();
    }

    [Test]
    [TestCase("a", "bb")]
    public void RoMultiSpan_2Spans_Counts(string a, string b) {
        var spans = new RoMultiSpan<char>(a, b);
        Asserter.WithHeading(spans.ToString())
                .And(spans[0].ToString(), Is.EqualTo(a))
                .And(spans[1].ToString(), Is.EqualTo(b))
                .And(spans.SpanCount,     Is.EqualTo(2))
                .And(spans.ElementCount,  Is.EqualTo(a.Length + b.Length))
                .Invoke();
    }

    [Test]
    [TestCase("a", "b", "c")]
    public void RoMultiSpan_Add(params string[] strings) {
        var       spans    = new RoMultiSpan<char>();
        var       lenSoFar = 0;
        using var ass      = Asserter.WithHeading();
        for (int i = 0; i < strings.Length; i++) {
            var ass2 = Asserter.WithHeading($"Span #{i}");
            ass2.And(spans.SpanCount,    Is.EqualTo(i));
            ass2.And(spans.ElementCount, Is.EqualTo(lenSoFar));
            spans    =  spans.Add(strings[i]);
            lenSoFar += strings[i].Length;
            ass.And(ass2);
        }
    }

    [Test]
    public void RoMultiSpan_Enumerator_OnlyConsidersUsedSpans() {
        var spans     = new RoMultiSpan<char>("a", "b", "c");
        var loopCount = 0;

        foreach (var span in spans) {
            loopCount += 1;
        }

        Assert.That(loopCount, Is.EqualTo(3));
    }

    [TestCase(new[] { "a", null, "", "b" }, new[] { "a", "b" })]
    public void RoMultiSpan_Enumerator_Where_NonEmpty(string[] inputs, string[] expected) {
        var spans      = ToSpans(inputs);
        var spanerator = new RoMultiSpan<char>.SpanEnumerator(spans, static it => it.IsEmpty == false);
        var results    = new List<string>();

        while (spanerator.MoveNext()) {
            results.Add(spanerator.Current.ToString());
        }

        Assert.That(results.ToArray(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase(new[] { "a", null, "", "b" }, new[] { "a", "b" })]
    public void RoMultiSpan_Where_NonEmpty(string[] inputs, string[] expected) {
        var results = new List<string>();
        var spans   = ToSpans(inputs);
        var where   = spans.Where(static it => it.IsEmpty == false);
        foreach (var span in where) {
            Console.WriteLine($"Span: {span}");
            results.Add(span.ToString());
        }

        Assert.That(results.ToArray(), Is.EqualTo(expected));
    }

    [TestCase("abc", null, "def")]
    public void RoMultiSpan_ElementEnumerator(string a, string b, string c) {
        var spans            = RoMultiSpan.Of(a.AsSpan(), b, c);
        var expectedElements = (a + b + c).ToCharArray();
        var results          = new List<char>();
        foreach (var ch in spans.EnumerateElements()) {
            results.Add(ch);
        }

        Assert.That(results.ToArray(), Is.EqualTo(expectedElements));
    }
}