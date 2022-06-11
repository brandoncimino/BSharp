using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FowlFever.BSharp.Clerical;
using FowlFever.Clerical.Validated;

namespace BSharp.Tests.Clerical2;

public abstract partial class BaseClericalTest {
    public record TestFileData(string Nickname, Guid ID = default) {
        public TestFileData() : this(Guid.NewGuid().ToString(), Guid.NewGuid()) { }
    }

    public enum PathType { Directory, File }

    public record SplitPath(
        string             String,
        string[]           Parts,
        PathType           PathType,
        DirectorySeparator InferredSeparator
    );

    public static SplitPath[] AllPaths() {
        return new SplitPath[] {
            new(
                @"C:\Users\bcimino\dev\payments",
                new[] { "C:", "Users", "bcimino", "dev", "payments" },
                PathType.Directory,
                DirectorySeparator.Windows
            ),
            new(
                "/c/Users/bcimino/dev/payments",
                new[] { "c", "Users", "bcimino", "dev", "payments" },
                PathType.Directory,
                DirectorySeparator.Universal
            ),
            new("~/.ssh/config", new[] { "~", ".ssh", "config" }, PathType.File, DirectorySeparator.Universal),
        };
    }

    public static IEnumerable<SplitPath> Data_FilePaths() => AllPaths().Where(it => it is { PathType: PathType.File });
    public static IEnumerable<SplitPath> Data_DirPaths()  => AllPaths().Where(it => it is { PathType: PathType.Directory });

    protected static readonly FileNamePart  TestFolderName = new FileNamePart(nameof(BaseClericalTest));
    protected static readonly DirectoryPath TestFolder     = Clerk.GetTempDirectoryPath();
    // protected static readonly ISaveFolder TestFolder = new SaveFolder(Path.GetTempPath(),TestFolderName);

    public string GetNonExistentFilePath() {
        var tempDir    = Path.GetTempPath();
        var randomName = Path.GetRandomFileName();
        return $"{tempDir}{randomName}";
    }

    public FileInfo GetTempFile(bool exists = true) {
        if (exists) {
            return new FileInfo(Path.GetTempFileName());
        }
        else {
            return new FileInfo(Path.GetTempPath() + Path.PathSeparator + Path.GetRandomFileName());
        }
    }

    public (FileInfo File, TestFileData Data) GetFileWithData() {
        var data = new TestFileData();
        var file = GetTempFile();
        file.Serialize(data);
        return (file, data);
    }
}