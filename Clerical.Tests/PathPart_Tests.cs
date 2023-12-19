using System.Collections.Immutable;

using FowlFever.Clerical;

using StringSegment = Microsoft.Extensions.Primitives.StringSegment;

namespace Clerical.Tests;

public class PathPart_Tests {
    [TestCase(@"ab/c", Description = "Internal separator")]
    [TestCase(@"ab\c", Description = "Internal separator (alt)")]
    [TestCase(@"//a",  Description = "Multiple leading separators")]
    [TestCase(@"\\a",  Description = "Multiple leading separators (alt)")]
    [TestCase(@"/\a",  Description = "Multiple leading separators (mix)")]
    [TestCase(@"a//",  Description = "Multiple trailing separators")]
    [TestCase(@"a\\",  Description = "Multiple trailing separators (alt)")]
    [TestCase(@"a\/",  Description = "Multiple trailing separators (mix)")]
    [TestCase(@"/ a",  Description = "Leading separator -> white")]
    [TestCase(@"\ a",  Description = "Leading separator -> white (alt)")]
    [TestCase(@"a /",  Description = "White -> Trailing separator")]
    [TestCase(@"a \",  Description = "White -> Trailing separator (alt)")]
    public void PathPart_RejectsInternalSeparators(string invalidInput) {
        Parse.Assert_NoParse<PathPart>(invalidInput);
    }

    // [TestCase(@"",         "",         Description = "Empty string")] // ⚠ Requires `ClericalStyles.AllowEmptyPathParts`
    [TestCase(@"/",        "",         Description = "Just a separator (/)")]
    [TestCase(@"\",        "",         Description = "Just a separator (\\)")]
    [TestCase(@"a",        "a",        Description = "Basic use case")]
    [TestCase(@"A",        "A",        Description = "should NOT mess with case")]
    [TestCase(@".NOMEDIA", ".NOMEDIA", Description = "Should preserve leading periods, often used for so-called 'hidden files'")]
    [TestCase(@" a ",      "a",        Description = "Should allow leading and trailing white space")]
    [TestCase(@" /a",      "a",        Description = "Leading white -> separator")]
    [TestCase(@" \a",      "a",        Description = "Leading white -> separator (alt)")]
    [TestCase(@"a/ ",      "a",        Description = "Trailing separator -> white")]
    [TestCase(@"a\ ",      "a",        Description = "Trailing separator -> white (alt)")]
    [TestCase(@"/a",       "a",        Description = "Should allow up to 1 leading separator (/)")]
    [TestCase(@"\a",       "a",        Description = "Should allow up to 1 leading separator (\\)")]
    [TestCase(@"a/",       "a",        Description = "Should allow up to 1 trailing separator (/)")]
    [TestCase(@"a\",       "a",        Description = "Should allow up to 1 trailing separator (\\)")]
    [TestCase(@"/a/",      "a",        Description = "Should allow up to 1 each of leading and trailing separators (/)")]
    [TestCase(@"\a\",      "a",        Description = "Should allow up to 1 each of leading and trailing separators (\\)")]
    [TestCase(@"/a\",      "a",        Description = "Should allow up to 1 each of leading and trailing separators (mixed)")]
    [TestCase(@"\a/",      "a",        Description = "Should allow up to 1 each of leading and trailing separators (mixed)")]
    [TestCase(@"a.b",      "a.b",      Description = "Allowed to contain inner periods")]
    [TestCase("",          "aaaaaa")]
    public void PathPart_HappyPath_Forgiving(string input, string expected) {
        Parse.Assert_Parses(input, PathPart.Parser.CreateUnsafe((StringSegment)expected));
    }

    [TestCase("a", "b", "a/b")]
    public void PathPart_Addition(string a, string b, string expected) {
        var aPart = PathPart.Parse(a);
        var bPart = PathPart.Parse(b);

        var actual       = aPart + bPart;
        var expectedPath = new DirectoryPath(ImmutableArray.Create(aPart, bPart));

        Assert_Equality(actual, expectedPath, true);
    }

    [TestCase("a", ".b", "a.b")]
    [TestCase("a", "",   "a")]
    [TestCase("",  ".b", ".b")]
    public void PathPart_PlusExtension_EqualsFileName(string baseName, string extension, string expectedFileName) {
        var baseNamePart  = PathPart.Parse(baseName);
        var fileExtension = FileExtension.Parse(extension);
        var expected      = FileName.Parse(expectedFileName);

        var actual = baseNamePart + fileExtension;
        Assert_Equality(actual, expected, true);
    }
}