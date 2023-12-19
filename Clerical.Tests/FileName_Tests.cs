using System.Collections.Immutable;

using FowlFever.Clerical;

namespace Clerical.Tests;

public class FileName_Tests {
    public record FileNameScenario(
        string         Input,
        string         BaseName,
        string         Extension,
        ClericalStyles RequiredStyles = ClericalStyles.None,
        string?        Description    = default
    ) {
        //public FileName Expected => new FileName(PathPart.CreateUnsafe(BaseName), FileExtension.Parser.CreateUnsafe(Extension));
    }

    public static ImmutableArray<FileNameScenario> Scenarios => ImmutableArray.Create(
        new FileNameScenario("a",                   "a",              "",     Description: "No extension"),
        new FileNameScenario(".ssh",                "",               ".ssh", Description: "No base name"),
        new FileNameScenario("a.b",                 "a",              ".b"),
        new FileNameScenario("environment.qa.json", "environment.qa", ".json", Description: "'Multiple extensions' aka 'period in base name'"),
        new FileNameScenario("/a.b",                "a",              ".b",    RequiredStyles: ClericalStyles.AllowLeadingDirectorySeparator),
        new FileNameScenario("\\a.b",               "a",              "b",     RequiredStyles: ClericalStyles.AllowLeadingDirectorySeparator),
        new FileNameScenario(" a",                  "a",              "",      RequiredStyles: ClericalStyles.AllowLeadingWhiteSpace),
        new FileNameScenario("a ",                  "a",              "",      RequiredStyles: ClericalStyles.AllowTrailingWhiteSpace),
        new FileNameScenario(" /a",                 "a",              "",      RequiredStyles: ClericalStyles.AllowLeadingWhiteSpace | ClericalStyles.AllowLeadingDirectorySeparator),
        new FileNameScenario(" a ",                 "a",              "",      RequiredStyles: ClericalStyles.AllowLeadingWhiteSpace | ClericalStyles.AllowTrailingWhiteSpace),
        new FileNameScenario("a.B",                 "a",              ".B",    RequiredStyles: ClericalStyles.AllowUpperCaseExtensions)
    );

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
    [TestCase("environment.qa.json", "environment.qa", ".json")]
    [TestCase("/a",                  "a",              "",   Description = nameof(ClericalStyles.AllowLeadingDirectorySeparator))]
    [TestCase("/a.b",                "a",              ".b", Description = nameof(ClericalStyles.AllowLeadingDirectorySeparator))]
    [TestCase("\\a.b",               "a",              ".b", Description = nameof(ClericalStyles.AllowLeadingDirectorySeparator))]
    [TestCase("a.b/",                "a",              ".b", Description = nameof(ClericalStyles.AllowTrailingDirectorySeparator))]
    [TestCase("a.b\\",               "a",              ".b", Description = nameof(ClericalStyles.AllowTrailingDirectorySeparator))]
    public void FileName_Parse_HappyPath(string input, string baseName, string extension) {
        var expected = new FileName(PathPart.Parse(baseName), FileExtension.Parse(extension));
        Parse.Assert_Parses(input, expected);
    }

    // [TestCase("/a")]
    [TestCase("a/b")]
    // [TestCase("a/")]
    [TestCase("a.")]
    [TestCase("/")]
    [TestCase(@"\/")]
    // [TestCase("/a.b")]
    // [TestCase("\\a.b")]
    // [TestCase("a.b/")]
    // [TestCase("a.b\\")]
    [TestCase("",  Description = "File names can never be empty")]
    [TestCase(" ", Description = "File names can't be all-whitespace (regardless or trimming styles)")]
    [TestCase("/ ")]
    [TestCase(" /")]
    [TestCase("/ /")]
    [TestCase(" / ")]
    [TestCase(@" \/ ")]
    [TestCase(@"/.")]
    [TestCase(@"./")]
    [TestCase("..")]
    [TestCase("/..")]
    [TestCase("../")]
    [TestCase("./.")]
    [TestCase("a")]
    public void FileName_Parse_RejectsInvalidInput(string input) {
        Parse.Assert_NoParse<FileName>(input);
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