using System.Collections.Generic;

using FowlFever.Clerical;
using FowlFever.Testing;

using NUnit.Framework;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace BSharp.Tests.Clerical2;

public class PathPartTests : BaseClericalTest {
    public static IEnumerable<char> GetInvalidPathPartChars() => Clerk.InvalidPathPartChars;

    [Test]
    public void PathPart_InvalidChar_AtStart([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Ignore.If(badChar, Is.AnyOf(Clerk.DirectorySeparatorChars));
        Assert.That(() => PathPart.Of($"{badChar}abc"), Throws.Exception);
    }

    [Test]
    public void PathPart_InvalidChar_AtEnd([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Ignore.If(badChar, Is.AnyOf(Clerk.DirectorySeparatorChars));
        Assert.That(() => PathPart.Of($"abc{badChar}"), Throws.Exception);
    }

    [Test]
    public void PathPart_InvalidChar_Inside([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Assert.That(() => PathPart.Of($"ab{badChar}cd"), Throws.Exception);
    }

    [Test]
    public static void PathPart_OfSpecialPathPart([Values] SpecialPathPart specialPathPart) {
        var str      = specialPathPart.ToPathString();
        var pathPart = PathPart.Of(str);
        Assert.That(pathPart, Is.EqualTo(str));
    }
}