using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp;

namespace BSharp.Tests;

public static class TestData {
    [Pure] private static int CharSum(string str)  => str.Sum(static it => it);
    [Pure] private static int GuidSum(Guid   guid) => guid.ToByteArray().Sum(static it => it);

    [Pure] public static Random Randomizer(Guid                       guid)              => new(GuidSum(guid));
    [Pure] public static Random Randomizer([CallerMemberName] string? _caller = default) => new(CharSum(_caller));

    [Pure]
    public static IEnumerable<int> RandomInts(int count, int max = int.MaxValue, [CallerMemberName] string? _caller = default) {
        var randomizer = Randomizer(_caller);
        return count.Repeat(() => randomizer.Next(max));
    }
}