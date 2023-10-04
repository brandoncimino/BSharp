using System.Runtime.CompilerServices;

using FowlFever.Clerical;

namespace Clerical.Tests;

[TestOf(nameof(FileExtension))]
public class FileExtension_Tests {
    private static void AssertCommutative<ACTUAL, EXPECTED>(
        ACTUAL                         a,
        ACTUAL                         b,
        Func<ACTUAL, ACTUAL, EXPECTED> commutativeFunction,
        EXPECTED                       expected,
        [CallerArgumentExpression("commutativeFunction")]
        string? _commutativeFunction = default
    ) {
        var ab  = commutativeFunction(a, b);
        var ba  = commutativeFunction(b, a);
        var act = new { ab, ba };
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

    private static void Assert_Parses(string input, FileExtension expected) {
        Assert.Multiple(
            () => {
                // string
                Assert.That(FileExtension.TryParse(input, null, out var stringy), Is.True,              $"TryParse(\"{input}\")");
                Assert.That(stringy,                                              Is.EqualTo(expected), $"TryParse(\"{input}\")");
                Assert.That(FileExtension.Parse(input),                           Is.EqualTo(expected), $"Parse(\"{input}\")");

                // span
                Assert.That(FileExtension.TryParse(input.AsSpan(), null, out var spanny), Is.True,              $"TryParse(Span[{input}])");
                Assert.That(spanny,                                                       Is.EqualTo(expected), $"TryParse(Span[{input}])");
                Assert.That(FileExtension.Parse(input.AsSpan(), null),                    Is.EqualTo(expected), $"Parse(Span[{input}])");
            }
        );
    }

    private static void Assert_NoParse(string input) {
        Assert.Multiple(
            () => {
                // string
                Assert.That(FileExtension.TryParse(input, null, out _), Is.False,                             $"TryParse(\"{input}\")");
                Assert.That(() => FileExtension.Parse(input, null),     Throws.InstanceOf<FormatException>(), $"Parse(\"{input}\")");

                // span
                Assert.That(FileExtension.TryParse(input.AsSpan(), null, out _), Is.False,                             $"TryParse(Span[{input}])");
                Assert.That(() => FileExtension.Parse(input.AsSpan(), null),     Throws.InstanceOf<FormatException>(), $"Parse(Span[{input}])");
            }
        );
    }

    [TestCase("exe")]
    public void FileExtension_Parse_PeriodIsOptional(string extensionStringWithoutPeriod) {
        var withPeriod    = FileExtension.Parse($".{extensionStringWithoutPeriod}");
        var withoutPeriod = FileExtension.Parse(extensionStringWithoutPeriod);

        Assert_Equality(withPeriod, withoutPeriod, true);
    }

    [TestCase("",      "",      Description = "Empty (i.e. no extension)")]
    [TestCase("json",  ".json", Description = "No period")]
    [TestCase(".json", ".json", Description = "With period")]
    [TestCase("JSoN",  ".json", Description = "Mixed casing")]
    public void FileExtension_Parse_AcceptsValidInput(string validFileExtension, string expected) {
        Assert_Parses(validFileExtension, FileExtension.CreateUnsafe(expected));
    }

    [TestCase("a.b",    Description = "File name with extension")]
    [TestCase("..a",    Description = "Two leading periods")]
    [TestCase("a/.ssh", Description = "Contains directory separators")]
    [TestCase(".a/b",   Description = "Contains directory separators")]
    [TestCase(".",      Description = "Just a period")]
    public void FileExtension_Parse_RejectsInvalidInput(string notFileExtension) {
        Assert_NoParse(notFileExtension);
    }

    [TestCase("jpg",   FileExtension.Jpeg)]
    [TestCase("JpG",   FileExtension.Jpeg)]
    [TestCase("jpeg",  FileExtension.Jpeg)]
    [TestCase(".jpeg", FileExtension.Jpeg)]
    public void FileExtension_Parse_UnifiesCommonAliases(string rawInput, string expectedAlias) { }
}