using System.Collections.Immutable;

using FowlFever.Clerical;

namespace Clerical.Tests;

public class PathPart_Tests {
    [TestCase("ab/c")]
    public void PathPart_RejectsInternalSeparators(string input) {
        Assert.That(() => PathPart.Of(input), Throws.InstanceOf<ArgumentException>());
    }

    [TestCase("/a",    "a")]
    [TestCase("a/",    "a")]
    [TestCase("/a/",   "a")]
    [TestCase("//a//", "a")]
    public void PathPart_HappyPath(string input, string expected) {
        Assert.Multiple(
            () => {
                var part = PathPart.Of(input);
                Assert.That(part,            Is.EqualTo(new PathPart(expected)));
                Assert.That(part.ToString(), Is.EqualTo(expected));
            }
        );
    }

    [TestCase("a",  "b",  "a/b")]
    [TestCase("a/", "/b", "a/b")]
    [TestCase("a/", "b/", "a/b/")]
    public void PathPart_Addition(string a, string b, string expected) {
        var aPart = PathPart.Of(a);
        var bPart = PathPart.Of(b);

        var added        = aPart + bPart;
        var expectedPath = new DirectoryPath(ImmutableArray.Create(aPart, bPart));
        Assert.That(added, Is.EqualTo(expectedPath));
    }
}