using System.Collections.Generic;

using FowlFever.Clerical.Validated;
using FowlFever.Conjugal.Affixing;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Clerical2;

public class PathPartTests : BaseClericalTest {
    public static IEnumerable<char>  GetInvalidPathPartChars() => PathPart.InvalidChars;

    [Test]
    public void PathPart_InvalidChar_AtStart([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Assert.That(() => new PathPart($"{badChar}abc"), Throws.Exception);
    }

    [Test]
    public void PathPart_InvalidChar_AtEnd([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Assert.That(() => new PathPart($"abc{badChar}"), Throws.Exception);
    }

    [Test]
    public void PathPart_InvalidChar_Inside([ValueSource(nameof(GetInvalidPathPartChars))] char badChar) {
        Assert.That(() => new PathPart($"ab{badChar}cd"), Throws.Exception);
    }
}