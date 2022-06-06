using System.Collections.Generic;

using FowlFever.Clerical.Validated;
using FowlFever.Testing;

using NUnit.Framework;

using Is = FowlFever.Testing.Is;

namespace BSharp.Tests.Clerical2;

public class PathPartTests : BaseClericalTest {
    public static IEnumerable<char> GetInvalidPathPartChars() => PathPart.InvalidChars;

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

    [Test]
    [TestCase("a")]
    [TestCase("Program Files")]
    [TestCase(".ssh")]
    public void PathPart_Valid(string part) {
        Asserter.Against(() => new PathPart(part))
                .And(it => it.ToString(), Is.EqualTo(part))
                .Invoke();
    }
}