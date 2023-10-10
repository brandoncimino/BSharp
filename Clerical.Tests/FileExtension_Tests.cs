using System.Runtime.CompilerServices;

using FowlFever.Clerical;

using NUnit.Framework.Constraints;

namespace Clerical.Tests;

[TestOf(nameof(FileExtension))]
public class FileExtension_Tests {
    public enum ParseStyle { Strict, Forgiving }

    private static void AssertCommutative<ACTUAL, EXPECTED>(
        ACTUAL                         a,
        ACTUAL                         b,
        Func<ACTUAL, ACTUAL, EXPECTED> commutativeFunction,
        EXPECTED                       expected,
        [CallerArgumentExpression("commutativeFunction")]
        string? _commutativeFunction = default
    ) {
        var ab = commutativeFunction(a, b);
        var ba = commutativeFunction(b, a);

        Assert.That(
            new { ab, ba },
            Is.EqualTo(new { ab = expected, ba = expected }),
            $"""
             {
                 _commutativeFunction
             } is commutative:
                a: {
                    a
                }
                b: {
                    b
                }
                
                Expected: {
                    expected
                }
                  (a, b): {
                      ab
                  }
                  (b, a): {
                      ba
                  }
             """
        );
    }

    private static void Assert_Equality(FileExtension a, FileExtension b, bool expectedEquality) {
        AssertCommutative(a, b, static (x, y) => x.Equals(y),                                                 expectedEquality);
        AssertCommutative(a, b, static (x, y) => x.Equals((object)y),                                         expectedEquality);
        AssertCommutative(a, b, static (x, y) => Equals(x, y),                                                expectedEquality);
        AssertCommutative(a, b, static (x, y) => x == y,                                                      expectedEquality);
        AssertCommutative(a, b, static (x, y) => x != y,                                                      !expectedEquality);
        AssertCommutative(a, b, static (x, y) => x.ToString().Equals(y.ToString(), StringComparison.Ordinal), expectedEquality);
        AssertCommutative(a, b, EqualityComparer<FileExtension>.Default.Equals,                 expectedEquality);
        AssertCommutative(a, b, static (x, y) => EqualityComparer<object>.Default.Equals(x, y), expectedEquality);
    }

    private static void AssertThat<T>(T actual, IResolveConstraint constraint, [CallerArgumentExpression("actual")] string? _actual = default) {
        Assert.That(actual, constraint, _actual);
    }

    private static void Assert_Parses(string input, FileExtension expected, ParseStyle parseStyle) {
        switch (parseStyle) {
            case ParseStyle.Strict:
                Assert.Multiple(
                    () => {
                        // string
                        AssertThat(FileExtension.TryParseExact(input, out var stringy), Is.True);
                        AssertThat(stringy,                                             Is.EqualTo(expected));
                        AssertThat(FileExtension.ParseExact(input),                     Is.EqualTo(expected));
                        AssertThat((FileExtension)input,                                Is.EqualTo(expected));

                        // span
                        AssertThat(FileExtension.TryParseExact(input.AsSpan(), out var spanny), Is.True);
                        AssertThat(spanny,                                                      Is.EqualTo(expected));
                        AssertThat(FileExtension.ParseExact(input.AsSpan()),                    Is.EqualTo(expected));
                    }
                );
                break;
            case ParseStyle.Forgiving:
                Assert.Multiple(
                    () => {
                        // string
                        AssertThat(FileExtension.TryParse(input, out var stringy), Is.True);
                        AssertThat(stringy,                                        Is.EqualTo(expected));
                        AssertThat(FileExtension.Parse(input),                     Is.EqualTo(expected));
                        // span
                        AssertThat(FileExtension.TryParse(input.AsSpan(), out var spanny), Is.True);
                        AssertThat(spanny,                                                 Is.EqualTo(expected));
                        AssertThat(FileExtension.Parse(input.AsSpan()),                    Is.EqualTo(expected));
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
                        Assert.That(FileExtension.TryParseExact(input, out _), Is.False);
                        Assert.That(() => FileExtension.ParseExact(input),     Throws.InstanceOf<FormatException>());

                        Assert.That(FileExtension.TryParseExact(input.AsSpan(), out _), Is.False);
                        Assert.That(() => FileExtension.ParseExact(input),              Throws.InstanceOf<FormatException>());
                    }
                );
                break;
            case ParseStyle.Forgiving:
                Assert.Multiple(
                    () => {
                        // string
                        Assert.That(FileExtension.TryParse(input, out _), Is.False,                             $"TryParse(\"{input}\")");
                        Assert.That(() => FileExtension.Parse(input),     Throws.InstanceOf<FormatException>(), $"Parse(\"{input}\")");

                        // span
                        Assert.That(FileExtension.TryParse(input.AsSpan(), out _), Is.False,                             $"TryParse(Span[{input}])");
                        Assert.That(() => FileExtension.Parse(input.AsSpan()),     Throws.InstanceOf<FormatException>(), $"Parse(Span[{input}])");
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

    [TestCase("jpg",   FileExtension.Jpeg)]
    [TestCase("JpG",   FileExtension.Jpeg)]
    [TestCase("jpeg",  FileExtension.Jpeg)]
    [TestCase(".jpeg", FileExtension.Jpeg)]
    public void FileExtension_Parse_UnifiesCommonAliases(string rawInput, string expectedAlias) {
        var expected = FileExtension.CreateUnsafe(expectedAlias);
        Assert_Parses(rawInput, expected, ParseStyle.Forgiving);
    }

    [TestCase(".jpeg", FileExtension.Jpeg)]
    [TestCase(".jpg",  FileExtension.Jpeg)]
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

        Assert.That(ext.Equals(other), Is.EqualTo(expectedEquality));
    }
}