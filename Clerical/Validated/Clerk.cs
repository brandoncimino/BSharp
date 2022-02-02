using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Validated;

[PublicAPI]
public static class Clerk {
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
}