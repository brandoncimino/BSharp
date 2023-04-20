using FowlFever.BSharp.Strings;
using FowlFever.Clerical;

namespace Clerical.Tests;

[TestOf(nameof(FileExtension))]
public class FileExtension_Tests {
    private static void Assert_Parses(string input, FileExtension expected) {
        Assert.Multiple(
            () => {
                // string
                Assert.That(FileExtension.TryParse(input, out var stringy), Is.True,              $"TryParse(\"{input}\")");
                Assert.That(stringy,                                        Is.EqualTo(expected), $"TryParse(\"{input}\")");
                Assert.That(FileExtension.Parse(input),                     Is.EqualTo(expected), $"Parse(\"{input}\")");

                // span
                Assert.That(FileExtension.TryParse(input.AsSpan(), out var spanny), Is.True,              $"TryParse(Span[{input}])");
                Assert.That(spanny,                                                 Is.EqualTo(expected), $"TryParse(Span[{input}])");
                Assert.That(FileExtension.Parse(input.AsSpan()),                    Is.EqualTo(expected), $"Parse(Span[{input}])");
            }
        );
    }

    private static void Assert_NoParse(string input) {
        Assert.Multiple(
            () => {
                // string
                Assert.That(FileExtension.TryParse(input, out _), Is.False,                             $"TryParse(\"{input}\")");
                Assert.That(() => FileExtension.Parse(input),     Throws.InstanceOf<FormatException>(), $"Parse(\"{input}\")");

                // span
                Assert.That(FileExtension.TryParse(input, out _), Is.False,                             $"TryParse(Span[{input}])");
                Assert.That(() => FileExtension.Parse(input),     Throws.InstanceOf<FormatException>(), $"Parse(Span[{input}])");
            }
        );
    }

    [TestCase("exe")]
    public void FileExtension_Parse_PeriodIsOptional(string extensionStringWithoutPeriod) {
        var withoutPeriod = FileExtension.Parse(extensionStringWithoutPeriod);
        var withPeriod    = FileExtension.Parse($".{extensionStringWithoutPeriod}");
        Assert.That(withoutPeriod.ToString(), Is.EqualTo(withPeriod.ToString()));
    }

    [TestCase("a.b")]
    public void FileExtension_Parse_RejectsBaseNames(string fileNameWithExtension) {
        Assert_NoParse(fileNameWithExtension);
    }

    [Test]
    public void FileExtension_Parse_AcceptsMaxLengthExtensions() {
        var maxAllowed = 'a'.Repeat(FileExtension.MaxExtensionLengthIncludingPeriod - 1);
        var expected   = FileExtension.CreateUnsafe($".{maxAllowed}");
        Assert_Parses(maxAllowed,       expected);
        Assert_Parses($".{maxAllowed}", expected);

        Assert_NoParse($"a{maxAllowed}");
        Assert_NoParse($".a{maxAllowed}");
    }
}