using System;
using System.IO;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

using static FowlFever.Testing.Should;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Clerical {
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

        [TestCase("this is really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really long", Fail, Fail)]
        [Test]
        [TestCase("abc",          Pass, Pass)]
        [TestCase(".ssh",         Pass, Pass)]
        [TestCase("a|b",          Fail, Fail)]
        [TestCase("%$@#%!@:$#%[", Fail, Fail)]
        [TestCase(null,           Fail, Fail)]
        [TestCase("",             Fail, Fail)]
        [TestCase("\n",           Fail, Fail)]
        [TestCase("C:/",          Pass, Fail)]
        [TestCase("C:D:E",        Fail, Fail)]
        // [TestCase("//yolo",       Should.Pass, Should.Fail)]
        // [TestCase(@"\\yolo",      Should.Pass, Should.Fail)]
        // [TestCase("a/b",          Should.Pass, Should.Fail)]
        [TestCase(@"a\b",  Pass, Fail)]
        [TestCase(@":\\c", Fail, Fail)]
        public void IsValidFilename(string path, Should pathShould, Should fileNameShould) {
            AssertAll.Of(
                () => AssertAll.Of(
                    "Path Validation",
                    () => Assert.That(BPath.IsValidPath(path),  Is.EqualTo(pathShould                                    == Pass)),
                    () => Assert.That(BPath.ValidatePath(path), Has.Property(nameof(Failable.Failed)).EqualTo(pathShould == Fail))
                ),
                () => AssertAll.Of(
                    "FileName Validation",
                    () => Assert.That(BPath.IsValidFileName(path),  Is.EqualTo(fileNameShould                                    == Pass)),
                    () => Assert.That(BPath.ValidateFileName(path), Has.Property(nameof(Failable.Failed)).EqualTo(fileNameShould == Fail))
                )
            );
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
        [TestCase("a/b",    "a",   "b")]
        [TestCase("/a/b",   "/a/", "/b")]
        [TestCase(@"a/b/c", @"a\", "b",  "c")]
        [TestCase("a/c",    "a",   "",   "c")]
        [TestCase("a/b",    null,  null, "", "\n", "\t", null, "a", null, "  ", "\n \t", "b")]
        public void JoinPath(string expectedPath, params string[] parts) {
            Console.WriteLine(@"\n: " + string.IsNullOrWhiteSpace("\n"));
            Console.WriteLine(@"\t: " + string.IsNullOrWhiteSpace("\t"));
            Assert.That(BPath.JoinPath(parts), Is.EqualTo(expectedPath));
        }

        [TestCase(null,                                      "b",        "b")]
        [TestCase("a",                                       null,       "a")]
        [TestCase(null,                                      null,       "")]
        [TestCase("/",                                       null,       "")]
        [TestCase("/",                                       @"\",       "")]
        [TestCase("//a",                                     "//b",      "//a/b")]
        [TestCase("a",                                       "",         "a")]
        [TestCase("",                                        "b",        "b")]
        [TestCase("",                                        "",         "")]
        [TestCase(@"\/\/\/a/\/\/\/\/\/\/\\////\\\///\/\/\/", @"b\\\///", "//////a/b//////")]
        public void JonPath_Simple(string parent, string child, string expected) {
            AssertAll.Of(
                () => Assert.That(BPath.JoinPath(parent, child),           Is.EqualTo(expected)),
                () => Assert.That(BPath.JoinPath(new[] { parent, child }), Is.EqualTo(expected))
            );
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
}