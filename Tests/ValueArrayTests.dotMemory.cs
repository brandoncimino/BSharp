using System;
using System.Collections.Generic;

using BenchmarkDotNet.Engines;

using FluentAssertions;

using FowlFever.BSharp;
using FowlFever.BSharp.Exceptions;
using FowlFever.Testing;

using JetBrains.dotMemoryUnit;
using JetBrains.dotMemoryUnit.Kernel;

using NUnit.Framework;

namespace BSharp.Tests;

public class ValueArrayTests_dotMemory {
    private static readonly ValueTuple Nothing  = default;
    private static readonly Consumer   Consumer = new();

    private static (TResult result, Traffic traffic) GetTraffic<TArg, TResult>(TrafficQuery query, TArg arg, Func<TArg, TResult> code) {
        Must.Have(dotMemoryApi.IsEnabled);

        var previousEnabled = dotMemoryApi.CollectAllocations;
        dotMemoryApi.CollectAllocations = true;

        var before = dotMemoryApi.GetSnapshot();
        var result = code.Invoke(arg);
        var after  = dotMemoryApi.GetSnapshot();

        var traffic = dotMemoryApi.GetTrafficBetween(before, after).Where(query);
        dotMemoryApi.CollectAllocations = previousEnabled;

        return (result, traffic);
    }

    [Test]
    [TestCase(1, 2, 3)]
    [DotMemoryUnit(CollectAllocations = true)]
    public void ForEach_DoesNotAllocate(params int[] input) {
        Ignore.Unless(dotMemoryApi.IsEnabled, Is.True);

        var query = new TrafficQuery().Where(it => it.Interface.Is<IEnumerable<int>>());

        var (array, traffic) = GetTraffic(query, input, static inp => ValueArray.Of(inp));

        traffic.AllocatedMemory.ObjectsCount.Should().Be(0);

        GetTraffic(
                query,
                array,
                static inp => {
                    foreach (var i in inp) {
                        Consumer.Consume(i);
                    }

                    return Nothing;
                }
            )
            .traffic
            .AllocatedMemory
            .ObjectsCount
            .Should()
            .Be(0);
    }
}