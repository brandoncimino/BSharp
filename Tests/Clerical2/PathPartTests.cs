using System.Collections.Generic;

using FowlFever.Clerical;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Testing;

using NUnit.Framework;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace BSharp.Tests.Clerical2;

public class PathPartTests : BaseClericalTest {
    public static IEnumerable<char> GetInvalidPathPartChars() => Clerk.InvalidPathPartChars;

    [Test]
    public void PathPart_InvalidChar_AtStart([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Ignore.If(badChar, Is.In(Clerk.DirectorySeparatorChars));
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

    [Test]
    public static void PathPart_OfSpecialPathPart([Values] SpecialPathPart specialPathPart) {
        var str      = specialPathPart.PathString();
        var pathPart = new PathPart(str);
        Assert.That(pathPart, Is.EqualTo(str));
    }
}