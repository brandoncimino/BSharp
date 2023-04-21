using FowlFever.Clerical;

using Assert = FowlFever.Testing.NUnitExtensionPoints.Assert;

namespace Clerical.Tests;

public class FileName_Tests {
    [TestCase("a.b",                 "a",              ".b")]
    [TestCase("a",                   "a",              "")]
    [TestCase(".ssh",                "",               ".ssh")]
    [TestCase("",                    "",               "")]
    [TestCase("a.",                  "a",              ".")]
    [TestCase(".",                   "",               ".")]
    [TestCase("environment.qa.json", "environment.qa", ".json")]
    public void FileName_Parse_HappyPath(string input, string baseName, string extension) {
        var fn = FileName.Parse(input);
        Assert.Multiple(
            () => {
                Assert.That(fn.BaseName,  Is.EqualTo(new PathPart(baseName)));
                Assert.That(fn.Extension, Is.EqualTo(new FileExtension(extension)));
                Assert.That(fn,           Is.EqualTo(new FileName(baseName, new FileExtension(extension))));
            }
        );
    }

    [TestCase("/a")]
    [TestCase("a/b")]
    [TestCase("a/")]
    public void FileName_Parse_RejectsDirectorySeparators(string input) {
        Assert.That(FileName.TryParse(input, out var result), Is.False);
        Assert.That(result == default);
        Assert.That(() => FileName.Parse(input), Throws.InstanceOf<FormatException>());
    }
}