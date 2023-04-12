using FowlFever.Clerical;
using FowlFever.Testing.Extensions;

namespace Clerical.Tests;

public class Clerk_FileExtensions_Tests {
    [TestOf(nameof(Clerk.GetExtension))]
    [TestCase("",          "")]
    [TestCase(".",         "")]
    [TestCase(".a",        ".a")]
    [TestCase("a.b",       ".b")]
    [TestCase("a.b.c",     ".c")]
    [TestCase("a.b  ",     ".b  ")]
    [TestCase("a",         "")]
    [TestCase("a/b/c.exe", ".exe")]
    [TestCase("...",       "")]
    [TestCase("a.exe/",    "")]
    [TestCase("/.ssh",     ".ssh")]
    [TestCase("a. b",      ". b")]
    public void Clerk_GetExtension(string path, string expected) {
        path.AsSpan().AssertResult(Clerk.GetExtension, expected);
    }
}