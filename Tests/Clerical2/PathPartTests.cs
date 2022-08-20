using System;
using System.Collections.Generic;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.Clerical.Validated;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Testing;

using NUnit.Framework;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace BSharp.Tests.Clerical2;

public class PathPartTests : BaseClericalTest {
    public static IEnumerable<char> GetInvalidPathPartChars() => Clerk.InvalidPathPartChars;

    [Test]
    public void PathPart_InvalidChar_AtStart([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Ignore.If(Clerk.DirectorySeparatorChars, Contains.Item(badChar));
        Assert.That(() => new PathPart($"{badChar}abc"), Throws.Exception);
    }

    [Test]
    public void PathPart_InvalidChar_AtEnd([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Ignore.If(badChar, Is.In(Clerk.DirectorySeparatorChars));
        Assert.That(() => new PathPart($"abc{badChar}"), Throws.Exception);
    }

    [Test]
    public void PathPart_InvalidChar_Inside([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Assert.That(() => new PathPart($"ab{badChar}cd"), Throws.Exception);
    }

    private static void PathPart_Expectations(Expectation expectation) {
        Console.WriteLine(expectation);
        Brandon.Print($"[{expectation.Value}]");
        Brandon.Print(expectation.Value.IsEmpty());
        Brandon.Print(expectation.Value.IsNotEmpty());
        Assert.That(() => new PathPart(expectation.Value!), expectation.Should.Constrain(ShouldStyle.Exception));
    }

    [Test] public void PathPart_Positive([ValueSource(nameof(PositivePathParts))] Expectation expectation) => PathPart_Expectations(expectation);
    [Test] public void PathPart_Negative([ValueSource(nameof(NegativePathParts))] Expectation expectation) => PathPart_Expectations(expectation);
}