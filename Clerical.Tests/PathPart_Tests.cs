using FowlFever.Clerical;

namespace Clerical.Tests;

public class PathPart_Tests {
    [TestCase("ab/c")]
    [TestCase("//a")]
    [TestCase(@"\\a")]
    [TestCase(@"ab\c")]
    public void PathPart_RejectsInternalSeparators(string input) {
        Assert_NoParse<PathPart>(input);
    }

    [TestCase("",         "",         Description = "Empty string")]
    [TestCase("/",        "",         Description = "Just a separator (/)")]
    [TestCase("\\",       "",         Description = "Just a separator (\\)")]
    [TestCase("a",        "a",        Description = "Basic use case")]
    [TestCase("A",        "A",        Description = "should NOT mess with case")]
    [TestCase(".NOMEDIA", ".NOMEDIA", Description = "Should preserve leading periods, often used for so-called 'hidden files'")]
    [TestCase(" a ",      "a",        Description = "Should allow leading and trailing white space")]
    [TestCase("/a",       "a",        Description = "Should allow up to 1 leading separator (/)")]
    [TestCase(@"\a",      "a",        Description = "Should allow up to 1 leading separator (\\)")]
    [TestCase("a/",       "a",        Description = "Should allow up to 1 trailing separator (/)")]
    [TestCase(@"a\",      "a",        Description = "Should allow up to 1 trailing separator (\\)")]
    [TestCase("/a/",      "a",        Description = "Should allow up to 1 each of leading and trailing separators (/)")]
    [TestCase(@"\a\",     "a",        Description = "Should allow up to 1 each of leading and trailing separators (\\)")]
    [TestCase(@"/a\",     "a",        Description = "Should allow up to 1 each of leading and trailing separators (mixed)")]
    [TestCase(@"\a/",     "a",        Description = "Should allow up to 1 each of leading and trailing separators (mixed)")]
    [TestCase("a.b",      "a.b",      Description = "Allowed to contain inner periods")]
    // [TestCase("//a//", "a")] // ⚠ Too many directory separators!
    public void PathPart_HappyPath(string input, string expected) {
        Assert_Parses(input, PathPart.CreateUnsafe(expected));
    }

    [TestCase("a",  "b",  "a/b")]
    [TestCase("a/", "/b", "a/b")]
    [TestCase("a/", "b/", "a/b")]
    public void PathPart_Addition(string a, string b, string expected) {
        var aPart = PathPart.Parse(a);
        var bPart = PathPart.Parse(b);

        var added = aPart + bPart;
        // var expectedPath = new DirectoryPath(ImmutableArray.Create(aPart, bPart));
        // Assert.That(added, Is.EqualTo(expectedPath));
        Assert.That(added.ToString(), Is.EqualTo(expected));
    }
}