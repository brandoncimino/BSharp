using System;
using System.Linq;

using FowlFever.BSharp.Optional;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Clerical2;

public class FileExtensionTests {
    public record FileExtensionExpectation(string FileExtensionString, Should Should);

    public static FileExtensionExpectation[] FileExtensionExpectations = {
        new("a", Should.Pass),
        new(".a", Should.Pass),
        new(".json5", Should.Pass),
        new("..", Should.Fail),
        new("a.b", Should.Fail),
        new("..a", Should.Fail),
        new(" ", Should.Fail),
        new("//", Should.Fail),
        new("a/b", Should.Fail),
        new("/", Should.Fail),
        new("/ ", Should.Fail),
        new("a ", Should.Fail),
        new(" a", Should.Fail),
        new("..", Should.Fail),
        new("a.", Should.Fail),
        new(".a.b", Should.Fail),
    };

    public static FileExtensionExpectation[] PositiveFileExtensions => FileExtensionExpectations.Where(it => it.Should == Should.Pass).ToArray();
    public static FileExtensionExpectation[] NegativeFileExtensions => FileExtensionExpectations.Where(it => it.Should == Should.Fail).ToArray();

    private static void New_FileExtension_Expectation([ValueSource(nameof(FileExtensionExpectations))] FileExtensionExpectation expectation) {
        var attempt = Failables.Try(() => new FileExtension(expectation.FileExtensionString));
        Console.WriteLine(attempt);
        Assert.That(attempt.AsFailable().Passed, Is.EqualTo(expectation.Should.Boolean()));
    }

    [Test] public void FileExtension_Negative([ValueSource(nameof(NegativeFileExtensions))] FileExtensionExpectation expectation) => New_FileExtension_Expectation(expectation);
    [Test] public void FileExtension_Positive([ValueSource(nameof(PositiveFileExtensions))] FileExtensionExpectation expectation) => New_FileExtension_Expectation(expectation);
}