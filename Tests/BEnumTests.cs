using System;
using System.Reflection;

using FowlFever.BSharp.Enums;

using NUnit.Framework;

namespace BSharp.Tests;

public class BEnumTests {
    [Test]
    [TestCase(BindingFlags.Instance | BindingFlags.Public, BindingFlags.Public, BindingFlags.Instance)]
    [TestCase(
        BindingFlags.Instance | BindingFlags.Public   | BindingFlags.Static | BindingFlags.DeclaredOnly,
        BindingFlags.Public   | BindingFlags.Instance | BindingFlags.Instance,
        BindingFlags.Static   | BindingFlags.DeclaredOnly
    )]
    public void WithoutFlag<T>(T withFlag, T toRemove, T expected)
        where T : struct, Enum {
        Console.WriteLine($"withFlag: {withFlag}");
        Console.WriteLine($"toRemove: {toRemove}");
        Console.WriteLine($"expected: {expected}");
        Console.WriteLine($"actual:   {withFlag.WithoutFlag(toRemove)}");
        Assert.That(() => withFlag.WithoutFlag(toRemove), Is.EqualTo(expected));
    }
}