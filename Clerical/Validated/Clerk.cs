using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Strings;

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

    /// <summary>
    /// Extracts the <see cref="Path.GetFileName"/> without <b>ANY</b> <see cref="FileExtension"/>s from the given <paramref name="path"/>.
    /// <p/>
    /// If the <paramref name="path"/> doesn't contain a base file name (for example, it <see cref="string.IsNullOrEmpty"/>, or begins with a period like <c>.ssh</c>),
    /// then <c>null</c> is returned instead.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// a           => a  
    /// a.txt       => a
    /// a.b.txt     => a
    /// a/b.txt     => b
    /// a/.ssh      => null
    /// ]]></code>
    /// </example>
    /// <param name="path">the full <see cref="Path"/></param>
    /// <returns>the "base name" for the path, </returns>
    public static FileNamePart? GetBaseName(string? path) {
        [return: NotNullIfNotNull("_path")]
        static string? _GetBaseName(string? _path) {
            if (_path == null) {
                return null;
            }

            var fileName    = Path.GetFileName(_path);
            var firstPeriod = fileName.IndexOf(".", StringComparison.Ordinal);
            return firstPeriod < 0 ? fileName : fileName[..firstPeriod];
        }

        var bn = _GetBaseName(path);
        return bn.IsEmpty() ? null : new FileNamePart(bn);
    }

    public static IEnumerable<PathPart>      SplitPath(string         path) => BPath.SplitPath(path).Select(PathPart.From);
    public static IEnumerable<FileExtension> GetFileExtensions(string path) => BPath.GetExtensions(path).Select(it => new FileExtension(it));
    public static FileName                   GetFileName(string       path) => new FileName(Path.GetFileName(path));

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