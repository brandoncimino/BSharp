using System.Globalization;

using FowlFever.Clerical;

namespace Clerical.Tests;

[TestOf(nameof(FileExtension))]
public class FileExtension_Tests {
    [TestCase("exe")]
    public void FileExtension_Parse_PeriodIsOptional(string extensionStringWithoutPeriod) {
        var withPeriod    = FileExtension.Parse($".{extensionStringWithoutPeriod}");
        var withoutPeriod = FileExtension.Parse(extensionStringWithoutPeriod);

        Assert_Equality(withPeriod, withoutPeriod, true);
    }

    [TestCase("",      "", Description = "Empty (i.e. no extension)")]
    [TestCase(".json", ".json")]
    public void FileExtension_ParseExact_AcceptsValidInput(string validFileExtension, string expected) {
        var expectedFileExtension = FileExtension.Parser.CreateUnsafe(expected);
        Assert.Multiple(
            () => {
                Parse.Stylish<FileExtension, FileExtension.Parser>(validFileExtension, ClericalStyles.Any)
                     .Assert_Success(expectedFileExtension);

                Parse.Stylish<FileExtension, FileExtension.Parser>(validFileExtension, ClericalStyles.None)
                     .Assert_Success(expectedFileExtension);
            }
        );
    }

    [TestCase("a.b",     Description = "File name with extension")]
    [TestCase(".a.",     Description = "Ends with period")]
    [TestCase("a.",      Description = "Ends with period")]
    [TestCase("..a",     Description = "Two leading periods")]
    [TestCase("a/.ssh",  Description = "Contains directory separators")]
    [TestCase(".a/b",    Description = "Contains directory separators")]
    [TestCase("a\\.ssh", Description = "Contains directory separators")]
    [TestCase(".a\\b",   Description = "Contains directory separators")]
    [TestCase("/.exe",   Description = "Starts with directory separator")]
    [TestCase("\\.exe",  Description = "Starts with directory separator")]
    [TestCase(".exe/",   Description = "Ends with directory separator")]
    [TestCase(".exe\\",  Description = "Ends with directory separator")]
    [TestCase(".",       Description = "Just a period")]
    [TestCase(".js on",  Description = "Inner space")]
    [TestCase("\n.a",    Description = "Contains a line break")]
    [TestCase("\0.b",    Description = "Contains a control character")]
    public void FileExtension_Parse_RejectsInvalidInput(string notFileExtension) {
        // 📎 These strings should NEVER be considered valid file extensions - i.e. they will fail with `ClericalStyles.Any`.
        //    Anything that fails with `ClericalStyles.Any` should also fail with `ClericalStyles.None`.
        Assert.Multiple(
            () => {
                Parse.Stylish<FileExtension, FileExtension.Parser>(notFileExtension, ClericalStyles.Any)
                     .Assert_Failure();

                Parse.Stylish<FileExtension, FileExtension.Parser>(notFileExtension, ClericalStyles.None)
                     .Assert_Failure();
            }
        );
    }

    [TestCase(" .json", ClericalStyles.AllowLeadingWhiteSpace,          ".json", Description = "Starts with space")]
    [TestCase(".json ", ClericalStyles.AllowTrailingWhiteSpace,         ".json", Description = "Ends with space")]
    [TestCase("json",   ClericalStyles.AllowExtensionsWithoutPeriods,   ".json", Description = "No leading period")]
    [TestCase("JSoN",   ClericalStyles.AllowUpperCaseExtensions,        ".json", Description = "Mixed casing")]
    [TestCase("/.ssh",  ClericalStyles.AllowLeadingDirectorySeparator,  ".ssh")]
    [TestCase(".ssh/",  ClericalStyles.AllowTrailingDirectorySeparator, ".ssh")]
    public void FileExtension_Parse_RequiresStyle(string notFileExtension, ClericalStyles requiredStyles, string expected) {
        int.Parse("1", NumberStyles.Integer);
        Assert.Multiple(
            () => {
                Parse.Stylish<FileExtension, FileExtension.Parser>(notFileExtension, ClericalStyles.None)
                     .Assert_Failure();

                Parse.Stylish<FileExtension, FileExtension.Parser>(notFileExtension, requiredStyles)
                     .Assert_Success(FileExtension.Parser.CreateUnsafe(expected));
            }
        );
    }

    [TestCase("jpg",   ClericalStyles.AllowExtensionsWithoutPeriods, ".jpeg")]
    [TestCase("JpG",   ClericalStyles.AllowUpperCaseExtensions,      ".jpeg")]
    [TestCase("jpeg",  ClericalStyles.AllowExtensionsWithoutPeriods, ".jpeg")]
    [TestCase(".jpeg", ClericalStyles.None,                          ".jpeg")]
    [TestCase(".jpg",  ClericalStyles.None,                          ".jpeg")]
    [TestCase(".yml",  ClericalStyles.None,                          ".yaml")]
    [TestCase(".mpg",  ClericalStyles.None,                          ".mpeg")]
    public void FileExtension_Parse_UnifiesCommonAliases(string rawInput, ClericalStyles styles, string expectedAlias) {
        var expected = FileExtension.Parser.CreateUnsafe(expectedAlias);
        Parse.Stylish<FileExtension, FileExtension.Parser>(rawInput, styles)
             .Assert_Success(expected);
    }
}