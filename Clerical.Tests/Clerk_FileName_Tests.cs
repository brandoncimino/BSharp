using FowlFever.Clerical;
using FowlFever.Testing.Extensions;

namespace Clerical.Tests;

public class Clerk_FileName_Tests {
    [TestOf(nameof(Clerk.GetFileName))]
    [TestCase("a",         "a")]
    [TestCase("a/",        "")]
    [TestCase("a/b",       "b")]
    [TestCase("a/b/c.exe", "c.exe")]
    [TestCase("a.exe/",    "")]
    [TestCase("a ",        "a ")]
    [TestCase("a.exe ",    "a.exe ")]
    [TestCase("a / ",      " ")]
    [TestCase(" a / ",     " ")]
    [TestCase(" /a ",      "a ")]
    [TestCase(" a ",       " a ")]
    public void Clerk_GetFileName(string path, string expected) {
        path.AsSpan().AssertResult(Clerk.GetFileName, expected);
    }

    [TestOf(nameof(Clerk.GetBaseName))]
    [TestCase("",      "")]
    [TestCase("/a.b ", "a")]
    [TestCase("a/b",   "b")]
    [TestCase("a/",    "")]
    [TestCase("a/ ",   " ")]
    [TestCase("a .b",  "a ")]
    [TestCase("a.b.c", "a")]
    [TestCase("/.b",   "")]
    public void Clerk_GetBaseName(string path, string expected) {
        path.AsSpan().AssertResult(Clerk.GetBaseName, expected);
    }
}