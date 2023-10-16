using FowlFever.Clerical;

namespace Clerical.Tests;

public class FileName_Tests {
    [TestCase("a", ".b", "a.b")]
    public void FileName_Constructed_HappyPath(string baseName, string extension, string expectedFileName) {
        var bn       = PathPart.Parse(baseName);
        var ext      = FileExtension.Parse(extension);
        var fileName = new FileName(bn, ext);
        AssertThat(fileName.ToString(), Is.EqualTo(expectedFileName));
    }

    [TestCase("a.b",                 "a",              ".b")]
    [TestCase("a",                   "a",              "")]
    [TestCase(".ssh",                "",               ".ssh")]
    [TestCase("",                    "",               "")]
    [TestCase("environment.qa.json", "environment.qa", ".json")]
    public void FileName_Parse_HappyPath(string input, string baseName, string extension) {
        var expected = new FileName(PathPart.Parse(baseName), FileExtension.Parse(extension));
        Assert_Parses(input, expected);
    }

    [TestCase("/a")]
    [TestCase("a/b")]
    [TestCase("a/")]
    [TestCase("a.")]
    [TestCase("a")]
    public void FileName_Parse_RejectsInvalidInput(string input) {
        Assert_NoParse<FileName>(input);
    }

    [Test]
    public void FileName_With_WorksAsExpected() {
        var fileName = FileName.Parse("a.json");
        var ext2     = FileExtension.Parse(".txt");
        var fileName2 = fileName with {
            Extension = ext2
        };

        Assert_Equality(fileName2, fileName,                false);
        Assert_Equality(fileName2, FileName.Parse("a.txt"), true);
    }
}