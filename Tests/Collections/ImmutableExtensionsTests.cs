using System;
using System.Collections.Immutable;

using FowlFever.BSharp.Collections;

using NUnit.Framework;

namespace BSharp.Tests.Collections;

public class ImmutableExtensionsTests {
    [TestCase("yolo")]
    public void Span_ToImmutableArray(string input) {
        var expected = input.ToImmutableArray();
        var span     = input.AsSpan();
        var actual   = span.ToImmutableArray();
        Assert.That(actual, Is.EqualTo(expected));
    }
}