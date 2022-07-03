using System;

using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Strings;

public class GraphemeClusterTests {
    public static string[] GraphemeClusters = new[] {
        "क्",
        "a",
        "ʥ",
        "🕵️‍♀️",
        "⛔",
        "👮‍♀️",
        "🧵",
    };

    [Test]
    public void RendersProperly([ValueSource(nameof(GraphemeClusters))] string asString) {
        var asCluster = GraphemeCluster.TryCreate(asString);
        Console.WriteLine($"string input:   {asString}");
        Console.WriteLine($"cluster:        {asCluster}");
        Asserter.Against(asCluster)
                .And(Is.Not.Null)
                .And(it => it?.ToString(), Is.EqualTo(asString))
                .Invoke();
    }
}