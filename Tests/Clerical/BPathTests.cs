using System;
using System.IO;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

using static FowlFever.Testing.Should;

namespace BSharp.Tests.Clerical;

public class BPathTests {
    [Test]
    [TestCase(".json",            "",     ".json")]
    [TestCase("yolo.swag",        "yolo", ".swag")]
    [TestCase("yo.lo.swag.ins",   "yo",   ".lo", ".swag", ".ins")]
    [TestCase("abc",              "abc")]
    [TestCase("a/b.c/.d.e",       "", ".d", ".e")]
    [TestCase("a/b.c/d",          "d")]
    [TestCase(@"a//b\\c../\d..e", "d", ".e")]
    public void GetExtensions(string path, string expectedFileName, params string[] expectedExtensions) {
        AssertAll.Of(
            () => Console.WriteLine($"{nameof(GetExtensions)}: {BPath.GetExtensions(path).Prettify()}"),
            () => Assert.That(BPath.GetExtensions(path),                Is.EqualTo(expectedExtensions)),
            () => Assert.That(BPath.GetFullExtension(path),             Is.EqualTo(expectedExtensions.JoinString())),
            () => Assert.That(BPath.GetFileNameWithoutExtensions(path), Is.EqualTo(expectedFileName)),
            () => AssertAll.Of(
                $"{nameof(FileSystemInfo)} extensions",
                () => Console.WriteLine($"{nameof(FileInfo)}: {new FileInfo(path)}, {new FileInfo(path).Name}, {new FileInfo(path).FullName}"),
                () => Assert.That(new FileInfo(path).Extensions(),                Is.EqualTo(expectedExtensions)),
                () => Assert.That(new FileInfo(path).FullExtension(),             Is.EqualTo(expectedExtensions.JoinString())),
                () => Assert.That(new FileInfo(path).FileNameWithoutExtensions(), Is.EqualTo(expectedFileName))
            )
        );
    }

    [TestCase("a/b",      "a", "b")]
    [TestCase(@"a/\b/",   "a", "", "b/")]
    [TestCase("/",        "/")]
    [TestCase("/a/b",     "/a", "b")]
    [TestCase("//a//b//", "/",  "a", "", "b", "/")]
    [TestCase("/a//b/",   "/a", "",  "b/")]
    [TestCase("/a//",     "/a", "/")]
    public void SplitPath(string path, params string[] expectedParts) {
        Asserter.Against(path)
                .And(BPath.SplitPath, Is.EqualTo(expectedParts))
                .Invoke();
    }

    [TestCase("this is really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really long", Fail)]
    [Test]
    [TestCase("abc",          Pass)]
    [TestCase(".ssh",         Pass)]
    [TestCase("a|b",          Fail)]
    [TestCase("%$@#%!@:$#%[", Fail)]
    [TestCase(null,           Fail)]
    [TestCase("",             Fail)]
    [TestCase("\n",           Fail)]
    [TestCase("C:/",          Pass)]
    [TestCase("C:D:E",        Fail)]
    [TestCase("//yolo",       Fail)]
    [TestCase(@"\\yolo",      Fail)]
    [TestCase("a/b",          Pass)]
    [TestCase(@"a\b",         Pass)]
    [TestCase(@":\\c",        Fail)]
    [TestCase("/a/b//c",      Fail)]
    public void IsValidFilename(string? path, Should should) {
        const string msg = "📎 Take this test with a grain of salt...file system validation is confusing...";
        try {
            Console.WriteLine(msg);
            var vp = BPath.ValidatePath(path);
            if (vp.Failed) {
                Console.WriteLine(vp.Excuse);
            }

            Asserter.Against(path)
                    .WithHeading($"{nameof(IsValidFilename)}: {path}")
                    .And(BPath.IsValidPath,  should.Constrain())
                    .And(BPath.ValidatePath, Has.Property(nameof(Failable.Failed)).EqualTo(should.Inverse().Boolean()))
                    .Invoke();
        }
        catch (Exception e) {
            Assert.Ignore(msg, e);
        }
    }

    [TestCase("a")]
    [TestCase("a/b/c")]
    [TestCase("/")]
    [TestCase("/./..//a")]
    public void Directory_To_Uri(string directory) {
        var di = new DirectoryInfo(directory);
        AssertAll.Of(
            () => Assert.That(di.ToUri(), Has.Property(nameof(Uri.IsFile)).True),
            () => Assert.That(
                di.ToUri().AbsolutePath,
                Is.EqualTo(
                    BPath.NormalizeSeparators(BPath.EnsureTrailingSeparator(di.FullName))
                )
            )
        );
    }

    [TestCase("a")]
    [TestCase("a/b/c")]
    [TestCase("/")]
    [TestCase("/./..//a")]
    public void File_To_Uri(string file) {
        var fi = new FileInfo(file);
        AssertAll.Of(
            () => Assert.That(fi.ToUri(),              Has.Property(nameof(Uri.IsFile)).True),
            () => Assert.That(fi.ToUri().AbsolutePath, Is.EqualTo(BPath.NormalizeSeparators(fi.FullName)))
        );
    }

    [Test]
    [TestCase("a",     "b",           Fail)]
    [TestCase("a",     "a/b",         Pass)]
    [TestCase("a/b/c", @"a\b/c\d",    Pass)]
    [TestCase("a/",    "a/c",         Pass)]
    [TestCase("/a",    "a",           Fail)]
    [TestCase("a",     "a",           Fail)]
    [TestCase(@"\a",   "/a.txt",      Fail)]
    [TestCase(@".ssh", ".ssh/id_rsa", Pass)]
    public void IsParentOf(string parentDirPath, string childFilePath, Should should) {
        Assume.That(Path.GetFileName(childFilePath), Is.Not.Empty);
        var parentDir = new DirectoryInfo(parentDirPath);
        var childFile = new FileInfo(childFilePath);
        Assert.That(parentDir.IsParentOf(childFile), should.Constrain());
    }

    [Test]
    [TestCase("a/b",    "a",     "b")]
    [TestCase("/a/b",   "/a/",   "/b")]
    [TestCase(@"a/b/c", @"a\",   "b", "c")]
    [TestCase(null,     "a",     "",  "c")]
    [TestCase(null,     null,    "")]
    [TestCase(null,     null,    "",   "a")]
    [TestCase(null,     "/a//",  "/b", "/c")]
    [TestCase(null,     "/a/",   "",   "")]
    [TestCase(null,     "",      "")]
    [TestCase("/",      "/",     "/")]
    [TestCase(null,     "//",    "/")]
    [TestCase(null,     null,    null, "", "\n", "\t", null, "a", null, "  ", "\n \t", "b")]
    [TestCase(null,     "//a",   "//b")]
    [TestCase("/a/b/c", "/a\\b", "c")]
    public void JoinPath(string? expectedPath, params string[] parts) {
        Assert.That(() => BPath.JoinPath(parts), expectedPath == null ? Throws.Exception : Is.EqualTo(expectedPath), () => $"{parts.Prettify()} => {BPath.JoinPath(parts)}");
    }

    [TestCase(null,                                      "b",        null)]
    [TestCase("a",                                       null,       null)]
    [TestCase(null,                                      null,       null)]
    [TestCase("/",                                       null,       null)]
    [TestCase("/",                                       @"\",       "/")]
    [TestCase("//a",                                     "//b",      null)]
    [TestCase("a",                                       "",         null)]
    [TestCase("",                                        "b",        null)]
    [TestCase("",                                        "",         null)]
    [TestCase(@"\/\/\/a/\/\/\/\/\/\/\\////\\\///\/\/\/", @"b\\\///", null)]
    [TestCase("a",                                       "b",        "a/b")]
    [TestCase("a/",                                      @"\b",      "a/b")]
    [TestCase("/a/b/",                                   "/c/d/",    "/a/b/c/d/")]
    public void JoinPath_Simple(string parent, string child, string? expected) {
        Assert.That(() => BPath.JoinPath(parent, child), expected == null ? Throws.Exception : Is.EqualTo(expected), () => $"[{parent}, {child}] => {BPath.JoinPath(parent, child)}");
    }

    [Test]
    [TestCase("a",        "a/",     "a")]
    [TestCase("a/",       "a/",     "a/")]
    [TestCase("a//",      "a/",     "a//")]
    [TestCase("a/b/c",    "a/b/c/", "a/b/c")]
    [TestCase(null,       "/",      "")]
    [TestCase("",         "/",      "")]
    [TestCase(@"a\",      "a/",     "a/")]
    [TestCase(@"\a/\b",   "/a//b/", "a//b/")]
    [TestCase(" ",        "/",      "")]
    [TestCase(" / ",      "/",      "")]
    [TestCase("a  ",      "a/",     "a/")]
    [TestCase(@"\/\/",    "/",      "")]
    [TestCase(@"\\a/b//", "//a/b/", "a/b/")]
    public void Fix_Separators(string input, string expected_trail, string expected_strip) {
        Assert.That(BPath.EnsureTrailingSeparator(input), Is.EqualTo(expected_trail));
    }
}