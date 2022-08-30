using System;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.Clerical.Validated;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Clerical.Validated.Composed;
using FowlFever.Testing;

using NUnit.Framework;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace BSharp.Tests.Clerical2;

public class FileNameTests : BaseClericalTest {
    public record Full_FileNameExpectation(string FullName, string BaseName, string[] Extensions, string? Description = null);

    public static Full_FileNameExpectation[] FileNameExpectations = {
        new("a.txt", "a", new[] { ".txt" }, "one extension"),
        new(".ssh", "", new[] { ".ssh" }, "no base name"),
        new("a.b.c", "a", new[] { ".b", ".c" }, "two extensions"),
        new("   -   ", "   -   ", Array.Empty<string>()),
        new("a.", "a", Array.Empty<string>()),
    };

    [Test]
    public void Clerk_GetBaseName([ValueSource(nameof(FileNameExpectations))] Full_FileNameExpectation expectation) {
        Assert.That(Clerk.GetBaseName(expectation.FullName), Is.EqualTo(expectation.BaseName));
    }

    [Test]
    public void Clerk_GetFileExtensions([ValueSource(nameof(FileNameExpectations))] Full_FileNameExpectation expectation) {
        Assert.That(Clerk.GetExtensions(expectation.FullName), Is.EquivalentTo(expectation.Extensions));
    }

    [Test]
    public void Clerk_GetFileNamePart([ValueSource(nameof(FileNameExpectations))] Full_FileNameExpectation expectation) {
        Asserter.Against(() => new FileNamePart(expectation.BaseName))
                .And(it => it,       Throws.Nothing)
                .And(it => it.Value, Is.EqualTo(expectation.BaseName))
                .Invoke();
    }

    [Test]
    [TestCase(@"C:\Users\bcimino\dev\BSharp\BSharp\Mathb.Wholesomeness.cs", "Mathb")]
    [TestCase(".ssh",                                                       "")]
    [TestCase("a",                                                          "a")]
    [TestCase(".ssh/config",                                                "config")]
    [TestCase("yolo.swag",                                                  "yolo")]
    public void GetBaseName(string path, string? expectedBaseName) {
        Asserter.Against(() => Clerk.GetBaseName(path))
                .And(it => it.Value, Is.EqualTo(expectedBaseName))
                .Invoke();
    }

    [Test]
    [TestCase("a.txt", "a", ".txt")]
    [TestCase("a.b.c", "a", ".b", ".c")]
    [TestCase(".ssh",  "",  ".ssh")]
    [TestCase("z",     "z")]
    public void FileName_Pass(string fileName, string baseName, params string[] extensions) {
        Asserter.Against(() => new FileName(fileName))
                .And(Has.Property(nameof(FileName.Value)).EqualTo(fileName))
                .And(it => it.BaseName.ToString(),                            Is.EqualTo(baseName))
                .And(it => it.Extensions,                                     Has.Exactly(extensions.Length).Items)
                .And(it => it.Extensions,                                     Is.EquivalentTo(extensions))
                .And(it => it.Extensions.Select(e => e.ToString()).ToArray(), Is.EqualTo(extensions))
                .Invoke();
    }

    [Test]
    [TestCase("C:/a/", Description = "Contains directory separator")]
    [TestCase("a/b",   Description = "Contains directory separator")]
    [TestCase("/d",    Description = "Contains directory separator")]
    [TestCase("d/",    Description = "Contains directory separator")]
    [TestCase("a.b ",  Description = "Extension contains whitespace")]
    [TestCase("a..")]
    [TestCase("a..b")]
    [TestCase("a.",  Description = "Ends in period")]
    [TestCase(" ",   Description = "Whitespace")]
    [TestCase(" . ", Description = "Empty after trimming whitespaces and periods")]
    public void FileName_Fail(string fileName) {
        Brandon.Print(fileName);
        Assert.Multiple(
            () => {
                Assert.That(() => IFileName.Ratify(fileName), Throws.Exception);
                Assert.That(() => new FileName(fileName),     Throws.Exception);
            }
        );
    }
}