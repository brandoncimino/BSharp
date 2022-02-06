using System.IO;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileName"/>.
/// </summary>
[PublicAPI]
public static class Clerk {
    public static PathPart GetPathPart(string pathPart) {
        return new PathPart(pathPart);
    }

    public static IEnumerable<PathPart> SplitPath(string path) {
        return BPath.SplitPath(path).Select(GetPathPart);
    }

    public static FileExtensionGroup GetFileExtensions(string path) {
        return new FileExtensionGroup(BPath.GetExtensions(path));
    }

    public static FileName GetFileName(string path) {
        var baseName     = BPath.GetFileNameWithoutExtensions(path);
        var baseNamePart = new FileNamePart(baseName);
        var extensions   = GetFileExtensions(path);
        return new FileName(baseNamePart, extensions);
    }

    public static DirectoryPath GetDirectoryPath(string path, DirectorySeparator separator = default) {
        var dirName = Path.GetDirectoryName(path);
        var splitDir = dirName switch {
            null => Enumerable.Empty<string>(),
            _    => dirName.Split(BPath.DirectorySeparatorPattern),
        };
        var dirParts = splitDir.Select(it => new PathPart(it));
        return new DirectoryPath(dirParts, separator);
    }

    public static FilePath GetFilePath(string path, DirectorySeparator separator = default) {
        if (BPath.EndsWithSeparator(path)) {
            throw new ArgumentException("Path ended with a separator, implying that it is a DIRECTORY!");
        }

        return new FilePath(
            GetDirectoryPath(path),
            GetFileName(path),
            separator
        );
    }

    /// <summary>
    /// The <see cref="DirectoryPath"/>-flavored version of <see cref="Path.GetTempPath"/>.
    /// </summary>
    /// <returns>a <see cref="DirectoryPath"/> equivalent of <see cref="Path.GetTempPath"/></returns>
    [Pure] public static DirectoryPath GetTempDirectoryPath() => GetDirectoryPath(Path.GetTempPath());

    /// <summary>
    /// The <see cref="FileName"/>-flavored version of <see cref="Path.GetTempFileName"/>.
    /// </summary>
    /// <returns>a <see cref="FileName"/> equivalent of <see cref="Path.GetTempFileName"/></returns>
    [Pure] public static FileName CreateTempFileName() => GetFileName(Path.GetTempFileName());

    /// <summary>
    /// <see cref="FileName"/>-flavored version of <see cref="Path.GetRandomFileName"/>.
    /// </summary>
    /// <returns>a <see cref="FileName"/> equivalent of <see cref="Path.GetRandomFileName"/></returns>
    [Pure] public static FileName GetRandomFileName() => GetFileName(Path.GetRandomFileName());
}