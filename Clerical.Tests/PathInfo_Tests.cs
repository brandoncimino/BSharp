using FowlFever.Clerical;

using Assert = FowlFever.Testing.NUnitExtensionPoints.Assert;

namespace Clerical.Tests;

public class PathInfo_Tests {
    [TestCase("a/b.c", "a", "b", "c")]
    public void PathInfo_Parse_HappyPath(string input, string parent, string baseName, string extension) {
        Assert.Multiple(
            () => {
                var info = PathInfo.Parse(input);
                Assert.That(info.BaseName,  Is.EqualTo(PathPart.Of(baseName)));
                Assert.That(info.Extension, Is.EqualTo(FileExtension.Parse(extension)));
                Assert.That(info.FileName,  Is.EqualTo(new FileName(baseName, extension)));
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
    }
}