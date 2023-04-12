using FowlFever.Clerical;
using FowlFever.Testing.Extensions;

namespace Clerical.Tests;

[TestOf(typeof(Clerk))]
public class Clerk_FileExtension_Tests {
    [TestOf(nameof(Clerk.EndsInDirectorySeparator))]
    [TestCase("",    false)]
    [TestCase("/",   true)]
    [TestCase("/ ",  false)]
    [TestCase("a\\", true)]
    public void Clerk_EndsInDirectorySeparator(string path, bool expected) {
        path.AsSpan().AssertResult(Clerk.EndsInDirectorySeparator, expected);
    }

    [TestOf(nameof(Clerk.EnsureEndingDirectorySeparator))]
    [TestCase("",      "/")]
    [TestCase("/a",    "/a/")]
    [TestCase("/a/",   "/a/")]
    [TestCase("a/ ",   "a/")]
    [TestCase(" ",     "/")]
    [TestCase("//a//", "//a//")]
    [TestCase("a\\",   "a\\")]
    public void Clerk_EnsureEndingDirectorySeparator(string path, string expected) {
        Clerk.EnsureEndingDirectorySeparator(path).AssertEquals(expected);
        Clerk.EnsureEndingDirectorySeparator(path.AsSpan()).AssertEquals(expected);
    }

    [TestOf(nameof(Clerk.TrimEndingDirectorySeparators))]
    [TestCase("",              "")]
    [TestCase("/",             "")]
    [TestCase(" ",             "")]
    [TestCase(" / ",           "")]
    [TestCase("//",            "")]
    [TestCase("///",           "")]
    [TestCase("a////  //",     "a")]
    [TestCase(" /a/b/c/",      " /a/b/c")]
    [TestCase("a/b  // \\ \\", "a/b")]
    public void Clerk_TrimEndingDirectorySeparators(string input, string expected) {
        input.AsSpan().AssertResult(Clerk.TrimEndingDirectorySeparators, expected);
        Clerk.TrimEndingDirectorySeparators(input).AssertEquals(expected);
        Clerk.TrimEndingDirectorySeparators(input.AsSpan()).AssertEquals(expected);
    }
}