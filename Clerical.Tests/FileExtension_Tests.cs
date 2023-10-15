using FowlFever.Clerical;

namespace Clerical.Tests;

[TestOf(nameof(FileExtension))]
public class FileExtension_Tests {
    public enum ParseStyle { Strict, Forgiving }

    private static void Assert_Parses(string input, FileExtension expected, ParseStyle parseStyle) {
        switch (parseStyle) {
            case ParseStyle.Strict:
                Assert.Multiple(
                    () => {
                        // string
                        AssertThat(FileExtension.TryParseExact(input, out var stringy), Is.True);
                        Assert_Equality(stringy, expected, true);
                        AssertThat(stringy, Is.EqualTo(expected));
                        Assert_Equality(FileExtension.ParseExact(input), expected, true);

                        // span
                        AssertThat(FileExtension.TryParseExact(input.AsSpan(), out var spanny), Is.True);
                        Assert_Equality(spanny,                                   expected, true);
                        Assert_Equality(FileExtension.ParseExact(input.AsSpan()), expected, true);
                    }
                );
                break;
            case ParseStyle.Forgiving:
                Assert.Multiple(
                    () => {
                        // string
                        AssertThat(FileExtension.TryParse(input, out var stringy), Is.True);
                        Assert_Equality(stringy,                    expected, true);
                        Assert_Equality(FileExtension.Parse(input), expected, true);

                        // span
                        AssertThat(FileExtension.TryParse(input.AsSpan(), out var spanny), Is.True);
                        Assert_Equality(spanny,                              expected, true);
                        Assert_Equality(FileExtension.Parse(input.AsSpan()), expected, true);
                    }
                );
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(parseStyle), parseStyle, null);
        }
    }

    private static void Assert_NoParse(string input, ParseStyle parseStyle) {
        switch (parseStyle) {
            case ParseStyle.Strict:
                Assert.Multiple(
                    () => {
                        // string
                        AssertThat(FileExtension.TryParseExact(input, out _), Is.False);
                        AssertThat(() => FileExtension.ParseExact(input),     Throws.InstanceOf<FormatException>());

                        // span
                        AssertThat(FileExtension.TryParseExact(input.AsSpan(), out _), Is.False);
                        AssertThat(() => FileExtension.ParseExact(input),              Throws.InstanceOf<FormatException>());
                    }
                );
                break;
            case ParseStyle.Forgiving:
                Assert.Multiple(
                    () => {
                        // string
                        AssertThat(FileExtension.TryParse(input, out _), Is.False);
                        AssertThat(() => FileExtension.Parse(input),     Throws.InstanceOf<FormatException>());

                        // span
                        AssertThat(FileExtension.TryParse(input.AsSpan(), out _), Is.False);
                        AssertThat(() => FileExtension.Parse(input.AsSpan()),     Throws.InstanceOf<FormatException>());
                    }
                );
                break;
            default: throw new ArgumentOutOfRangeException(nameof(parseStyle), parseStyle, null);
        }
    }

    [TestCase("exe")]
    public void FileExtension_Parse_PeriodIsOptional(string extensionStringWithoutPeriod) {
        var withPeriod    = FileExtension.Parse($".{extensionStringWithoutPeriod}");
        var withoutPeriod = FileExtension.Parse(extensionStringWithoutPeriod);

        Assert_Equality(withPeriod, withoutPeriod, true);
    }

    [TestCase("",              "",      Description = "Empty (i.e. no extension)")]
    [TestCase("json",          ".json", Description = "No period")]
    [TestCase("a",             ".a",    Description = "One (valid) character")]
    [TestCase(".json",         ".json", Description = "With period")]
    [TestCase("JSoN",          ".json", Description = "Mixed casing")]
    [TestCase(" \n .json",     ".json", Description = "Leading whitespace")]
    [TestCase(".json \n ",     ".json", Description = "Trailing whitespace")]
    [TestCase("\tJpG  \t\t\n", ".jpeg", Description = "Hot mess")]
    public void FileExtension_Parse_AcceptsValidInput(string validFileExtension, string expected) {
        Assert_Parses(validFileExtension, FileExtension.CreateUnsafe(expected), ParseStyle.Forgiving);
    }

    [TestCase("",      "", Description = "Empty (i.e. no extension)")]
    [TestCase(".json", ".json")]
    public void FileExtension_ParseExact_AcceptsValidInput(string validFileExtension, string expected) {
        Assert_Parses(validFileExtension, FileExtension.CreateUnsafe(expected), ParseStyle.Strict);
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
    public void FileExtension_Parse_RejectsInvalidInput(string notFileExtension) {
        // 📎 Anything rejected by the "forgiving" parser should also be rejected by the "strict" parser
        Assert.Multiple(
            () => {
                Assert_NoParse(notFileExtension, ParseStyle.Forgiving);
                Assert_NoParse(notFileExtension, ParseStyle.Strict);
            }
        );
    }

    [TestCase(" .json",  Description = "Starts with space")]
    [TestCase(".json ",  Description = "Ends with space")]
    [TestCase("\n.json", Description = "Starts with new-line")]
    [TestCase(".json\n", Description = "Ends with new-line")]
    [TestCase("json",    Description = "No leading period")]
    public void FileExtension_ParseExact_RejectsInvalidInput(string notFileExtension) {
        Assert_NoParse(notFileExtension, ParseStyle.Strict);
    }

    [TestCase("jpg",   ".jpeg")]
    [TestCase("JpG",   ".jpeg")]
    [TestCase("jpeg",  ".jpeg")]
    [TestCase(".jpeg", ".jpeg")]
    public void FileExtension_Parse_UnifiesCommonAliases(string rawInput, string expectedAlias) {
        var expected = FileExtension.CreateUnsafe(expectedAlias);
        Assert_Parses(rawInput, expected, ParseStyle.Forgiving);
    }

    [TestCase(".jpeg", ".jpeg")]
    [TestCase(".jpg",  ".jpeg")]
    public void FileExtension_ParseExact_UnifiesCommonAliases(string rawInput, string expectedAlias) {
        var expected = FileExtension.CreateUnsafe(expectedAlias);
        Assert_Parses(rawInput, expected, ParseStyle.Strict);
    }

    [TestCase(".json", ".json", true)]
    [TestCase(".json", "json",  true)]
    [TestCase(".json", "JSON",  true)]
    [TestCase(".jpeg", "JPeG",  true)]
    public static void FileExtension_StringEquality(string extensionString, string other, bool expectedEquality) {
        // Make sure reference equality isn't applicable
        other = new string(other);
        var ext = FileExtension.Parse(extensionString);

        AssertThat(ext.Equals(other), Is.EqualTo(expectedEquality));
    }
}