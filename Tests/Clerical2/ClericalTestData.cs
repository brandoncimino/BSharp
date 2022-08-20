using System.IO;
using System.Linq;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Enums;
using FowlFever.Testing;

namespace BSharp.Tests.Clerical2;

public abstract partial class BaseClericalTest {
    public record Expectation(string? Value, Should Should, string? Description = default);

    public static Expectation[] FileSystemPaths = {
        new("/", Should.Pass, "single slash"),
        new("", Should.Pass, "empty"),
        new(".", Should.Pass, "aka 'this directory'"),
        new("..", Should.Pass, "aka 'parent directory'"),
        new("~", Should.Pass, "aka 'home directory'"),
        new("C:", Should.Pass, "windows drive in the correct position"),
        new("C:/", Should.Pass, "windows drive in the correct position"),
        new("a b", Should.Pass, "containing white space"),
        new("//a", Should.Pass, "begins with // ('root' in some contexts)"),
        new(" //a", Should.Pass, "// after whitespace"),
        new("a//b", Should.Fail, "// in the middle, i.e. an 'empty segment'"),
        new("a//", Should.Fail, "// at the end, i.e. an 'empty segment'"),
        new(@"\/", Should.Pass, "double-mixed separator styles"),
        new(@"a\b/c", Should.Pass, "mixed separators"),
    };

    public static Expectation[] PathParts {
        get {
            var separators = BEnum.GetValues<DirectorySeparator>()
                                  .Select(it => new Expectation(it.ToCharString(), Should.Fail, "directory separator"));

            var invalidPathChars = Path.GetInvalidPathChars()
                                       .Select(it => new Expectation(it.ToString(), Should.Fail, "invalid path character"));
            var invalidFileNameChars = Path.GetInvalidFileNameChars()
                                           .Select(it => new Expectation(it.ToString(), Should.Fail, "invalid file name character"));

            return new Expectation[] {
                       new("a", Should.Pass),
                       new(" ", Should.Fail, "whitespace"),
                       new(null, Should.Fail, "null"),
                       new("", Should.Fail, "empty"),
                       new(".", Should.Fail, "ends in period"),
                       new("..", Should.Fail, "ends in period"),
                       new("a..b", Should.Fail, "contains double period"),
                       new(" a", Should.Fail, "starts with whitespace"),
                       new("a ", Should.Fail, "ends in whitespace"),
                       new("a.b.", Should.Fail, "ends in period"),
                       new(".ssh", Should.Pass),
                       new(".a.b", Should.Pass),
                       new("Program Files", Should.Pass),
                       new("doc (1).txt", Should.Pass),
                       new(@"/a", Should.Pass, "starts with dir. separator"),
                       new(@"a/", Should.Pass, "starts with dir. separator"),
                       new(@"/a/", Should.Pass, "bookends with dir. separator"),
                       new(@"\a", Should.Pass, "starts with dir. separator"),
                       new(@"a\", Should.Pass, "starts with dir. separator"),
                       new(@"\a\", Should.Pass, "starts with dir. separator"),
                       new(@"//a", Should.Pass, "starts with double separator"),
                       new(@"\\a", Should.Pass, "starts with double separator"),
                       new(@"///a", Should.Fail, "starts with triple separator"),
                       new(@"\\\a", Should.Fail, "starts with triple separator"),
                       new(@"/\a", Should.Pass, "starts with mixed separators"),
                       new(@"a//", Should.Fail, "ends with double separator"),
                       new(@"a\\", Should.Fail, "ends with double separator"),
                       new("a/b", Should.Fail, "contains separator")
                   }
                   .Concat(separators)
                   .Concat(invalidPathChars)
                   .Concat(invalidFileNameChars)
                   .ToArray();
        }
    }

    public static Expectation[] PositivePathParts => PathParts.Where(it => it.Should == Should.Pass).ToArray();
    public static Expectation[] NegativePathParts => PathParts.Where(it => it.Should == Should.Fail).ToArray();
}