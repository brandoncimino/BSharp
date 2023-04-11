using FowlFever.Clerical;
using FowlFever.Testing.Extensions;

namespace Clerical.Tests;

public class Clerk_FileExtension_Tests {
    [TestCase("",    false)]
    [TestCase("/",   true)]
    [TestCase("/ ",  false)]
    [TestCase("a\\", true)]
    public void EndsInDirectorySeparator(string input, bool expected) {
        Clerk.EndsInDirectorySeparator(input).AssertEquals(expected);
        Clerk.EndsInDirectorySeparator(input.AsSpan()).AssertEquals(expected);
    }

    [TestCase("",      "/")]
    [TestCase("/a",    "/a/")]
    [TestCase("/a/",   "/a/")]
    [TestCase("a/ ",   "a/")]
    [TestCase(" ",     "/")]
    [TestCase("//a//", "//a//")]
    [TestCase("a\\",   "a\\")]
    public void EnsureEndingDirectorySeparator(string input, string expected) {
        Clerk.EnsureEndingDirectorySeparator(input).AssertEquals(expected);
        Clerk.EnsureEndingDirectorySeparator(input.AsSpan()).AssertEquals(expected);
    }

    [TestCase("",              "")]
    [TestCase("/",             "")]
    [TestCase(" ",             "")]
    [TestCase(" / ",           "")]
    [TestCase("//",            "")]
    [TestCase("///",           "")]
    [TestCase("a////  //",     "a")]
    [TestCase(" /a/b/c/",      " /a/b/c")]
    [TestCase("a/b  // \\ \\", "a/b")]
    public void TrimEndingDirectorySeparators(string input, string expected) {
        Clerk.TrimEndingDirectorySeparators(input).AssertEquals(expected);
        Clerk.TrimEndingDirectorySeparators(input.AsSpan()).AssertEquals(expected);
    }
}