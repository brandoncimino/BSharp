using System.Collections.Immutable;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Clerical;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileName"/>.
/// </summary>
[PublicAPI]
public static class Clerk {
    #region Constants

    /// <summary>
    /// The valid <see cref="DirectorySeparator"/>s, <c>/</c> and <c>\</c>.
    /// </summary>
    public static readonly ImmutableHashSet<char> DirectorySeparatorChars = new HashSet<char> { '\\', '/' }.ToImmutableHashSet();

    /// <summary>
    /// Combines <see cref="Path.GetInvalidPathChars"/> and <see cref="Path.GetInvalidFileNameChars"/> into a single <see cref="ImmutableHashSet{T}"/>.
    /// </summary>
    public static readonly ImmutableHashSet<char> InvalidPathChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).ToImmutableHashSet();

    /// <summary>
    /// A <see cref="Regex"/> pattern matching <see cref="DirectorySeparatorChars"/>.
    /// </summary>
    public static readonly Regex DirectorySeparatorPattern = new(@"[\\\/]");

    #endregion

    public static PathPart                   GetPathPart(string       pathPart) => new PathPart(pathPart);
    public static IEnumerable<PathPart>      SplitPath(string         path)     => BPath.SplitPath(path).Select(GetPathPart);
    public static IEnumerable<FileExtension> GetFileExtensions(string path)     => BPath.GetExtensions(path).Select(it => new FileExtension(it));
    public static FileName                   GetFileName(string       path)     => new FileName(Path.GetFileName(path));

    /// <summary>
    /// Validates and creates a <see cref="DirectoryPath"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static DirectoryPath GetDirectoryPath(string path, DirectorySeparator separator = default) {
        return new DirectoryPath(separator, path);
    }

    public static FilePath GetFilePath(string path, DirectorySeparator separator = default) {
        if (BPath.EndsWithSeparator(path)) {
            throw new ArgumentException("Path ended with a separator, implying that it is a DIRECTORY!");
        }

        return new FilePath(
            "",
            // GetDirectoryPath(path, separator),
            // GetFileName(path),
            separator
        );
    }

    /// <summary>
    /// The <see cref="DirectoryPath"/>-flavored version of <see cref="Path.GetTempPath"/>.
    /// </summary>
    /// <returns>a <see cref="DirectoryPath"/> equivalent of <see cref="Path.GetTempPath"/></returns>
    [Pure]
    public static DirectoryPath GetTempDirectoryPath() => GetDirectoryPath(Path.GetTempPath());

    /// <summary>
    /// The <see cref="FileName"/>-flavored version of <see cref="Path.GetTempFileName"/>.
    /// </summary>
    /// <returns>a <see cref="FileName"/> equivalent of <see cref="Path.GetTempFileName"/></returns>
    [Pure]
    public static FileName CreateTempFileName() => GetFileName(Path.GetTempFileName());

    /// <summary>
    /// <see cref="FileName"/>-flavored version of <see cref="Path.GetRandomFileName"/>.
    /// </summary>
    /// <returns>a <see cref="FileName"/> equivalent of <see cref="Path.GetRandomFileName"/></returns>
    [Pure]
    public static FileName GetRandomFileName() => GetFileName(Path.GetRandomFileName());
}