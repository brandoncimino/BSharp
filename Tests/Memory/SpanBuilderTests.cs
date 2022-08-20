using System;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

using Spectre.Console;

namespace BSharp.Tests.Memory;

[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType", Justification = "NUnit static extensions")]
public class SpanBuilderTests {
    [Test]
    public void SpanBuilder_Default_State() {
        SpanBuilder<int> builder = default;
        Asserter.WithHeading()
                .And(builder.State,        Is.EqualTo(SpanBuilderState.Unallocated))
                .And(builder.Span.IsEmpty, Is.True)
                .And(builder.Capacity,     Is.EqualTo(0))
                .And(builder.SpanPos,      Is.EqualTo(0))
                .Invoke();
    }

    [Test]
    [TestCase(3)]
    [TestCase(0)]
    public void SpanBuilder_NewWithAllocation(int capacity) {
        SpanBuilder<int> builder       = new(stackalloc int[capacity]);
        var              expectedState = capacity == 0 ? SpanBuilderState.Unallocated : SpanBuilderState.Fillable;

        Asserter.WithHeading()
                .And(builder.State,        Is.EqualTo(expectedState))
                .And(builder.Span.IsEmpty, Is.True)
                .And(builder.Capacity,     Is.EqualTo(capacity))
                .And(builder.SpanPos,      Is.EqualTo(0))
                .Invoke();
    }

    [Test]
    [TestCase("abcd")]
    public void SpanBuilder_FillExactly(string source) {
        SpanBuilder<char> builder = new(stackalloc char[0]);
        var               span    = source.AsSpan();

        AnsiConsole.Write(builder.Describe());
        Assert.That(builder.State, Is.EqualTo(SpanBuilderState.Unallocated));

        builder.TryAdd(span[0]);
        AnsiConsole.Write(builder.Describe());

        builder.Allocate(stackalloc char[source.Length]);

        for (int i = 0; i < span.Length; i++) {
            AnsiConsole.Write(builder.Describe());

            var added = builder.TryAdd(span[i]);
            Assert.That(added, Is.True);
            Assert.That(builder.Span.SequenceEqual(span[..(i + 1)]));

            var expectedState = i >= span.Length - 1 ? SpanBuilderState.Full : SpanBuilderState.Fillable;
            Assert.That(builder.State, Is.EqualTo(expectedState));
        }
    }

    [Test]
    public void SpanBuilder_Full() {
        Span<int> span = stackalloc int[3];
        var       sb   = new SpanBuilder<int>(span);
        sb.Add(1);
        sb.Add(2);
        sb.Add(3);

        Asserter.WithHeading()
                .And(sb.State,          Is.EqualTo(SpanBuilderState.Full))
                .And(sb.SpanPos,        Is.EqualTo(3))
                .And(sb.Span.ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }))
                .And(sb.TryAdd(99),     Is.False)
                .Invoke();
    }

    [Test]
    public void SpanBuilder_Add_WhenFull_Throws() {
        Span<int> span = stackalloc int[3];
        var       sb   = new SpanBuilder<int>(span);
        sb.Add(1);
        sb.Add(2);
        sb.Add(3);

        // 📎 We have to use an explicit try-catch block because we can't use ref structs inside of lambdas
        Exception exc = null;
        try {
            sb.Add(1);
        }
        catch (Exception e) {
            // Assert.Pass($"Got expected exception: {e}");
            exc = e;
        }

        // Assert.Fail("Expected an exception!");
        Assert.That(exc, Is.Not.Null);
    }
}