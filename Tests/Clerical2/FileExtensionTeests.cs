using System;

using FowlFever.BSharp.Optional;
using FowlFever.Clerical.Validated;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Clerical2;

public class FileExtensionTests {
    public record FileExtensionExpectation(string Input, Should Should);

    public static FileExtensionExpectation[] FileExtensionExpectations = {
        new("a", Should.Pass),
        new(".a", Should.Pass),
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

    [Test]
    public void New_FileExtension_Expectation([ValueSource(nameof(FileExtensionExpectations))] FileExtensionExpectation expectation) {
        var attempt = Failables.Try(() => new FileExtension(expectation.Input));
        Console.WriteLine(attempt);
        Assert.That(attempt.AsFailable().Passed, Is.EqualTo(expectation.Should.Boolean()));
    }
}