using FowlFever.Clerical;

using Assert = FowlFever.Testing.NUnitExtensionPoints.Assert;

namespace Clerical.Tests;

// [Ignore("Not sure there's value in the PathInfo type, and it has a stack overflow in it somewhere anyways.")]
public class PathInfo_Tests {
    [TestCase("a/b.c",   "a",   "b", "c")]
    [TestCase("a/b/c.d", "a/b", "c", "d")]
    [TestCase("a/b/c",   "a/b", "c", "")]
    public void PathInfo_Parse_HappyPath(string input, string parent, string baseName, string extension) {
        Assert.Multiple(
            () => {
                var info = PathInfo.Parse(input);
                Assert.That(info.BaseName,  Is.EqualTo(PathPart.Parse(baseName)));
                Assert.That(info.Extension, Is.EqualTo(FileExtension.Parse(extension)));
                Assert.That(info.FileName,  Is.EqualTo(new FileName(PathPart.Parse(baseName), FileExtension.Parse(extension))));
            }
        );
    }

    [Test]
    public void TryJoinPath() {
        var        a      = "a/";
        var        b      = "/b";
        Span<char> buffer = stackalloc char[10];
        Path.TryJoin(a, b, buffer, out int written);
        Console.WriteLine(buffer[..written].ToString());

        var c = "c";
        Path.TryJoin(b, c, buffer, out written);
        Console.WriteLine(buffer[..written].ToString());

        Path.TryJoin(b, b, buffer, out written);
        Console.WriteLine(buffer[..written].ToString());

        Path.TryJoin("w\\", @"\w2\", buffer, out written);
        Console.WriteLine(buffer[..written].ToString());
    }
}