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
public static class Clerk {
    #region Constants

    /// <summary>
    /// The valid <see cref="DirectorySeparator"/>s; <b><c>/</c></b> and <b><c>\</c></b>.
    /// </summary>
    public static readonly ImmutableArray<char> DirectorySeparatorChars = ImmutableArray.Create('\\', '/');

    /// <summary>
    /// Combines <see cref="Path.GetInvalidPathChars"/> and <see cref="Path.GetInvalidFileNameChars"/>.
    /// </summary>
    public static readonly ImmutableArray<char> InvalidPathChars = Enumerable.Union(Path.GetInvalidPathChars(), Path.GetInvalidFileNameChars()).ToImmutableArray();

    /// <summary>
    /// Combines <see cref="InvalidPathChars"/> and <see cref="DirectorySeparatorChars"/>.
    /// </summary>
    public static readonly ImmutableArray<char> InvalidPathPartChars = InvalidPathChars.Union(DirectorySeparatorChars).ToImmutableArray();

    /// <inheritdoc cref="InvalidPathChars"/>
    public static ImmutableArray<char> InvalidFileNameChars => InvalidPathChars;
    /// <summary>
    /// Combines <see cref="InvalidFileNameChars"/> with <c>.</c> <i>(period)</i>.
    /// </summary>
    public static ImmutableArray<char> InvalidFileNamePartChars = InvalidPathChars.Add('.');

    /// <summary>
    /// A <see cref="Regex"/> pattern matching <see cref="DirectorySeparatorChars"/>.
    /// </summary>
    public static readonly Regex DirectorySeparatorPattern = new(@"[\\\/]");
    public static readonly Regex OuterSeparatorPattern = RegexPatterns.OuterMatch(DirectorySeparatorPattern);
    public static readonly Regex InnerSeparatorPattern = RegexPatterns.InnerMatch(DirectorySeparatorPattern);

    /// <summary>
    /// Matches <b>any</b> single <see cref="FileExtension"/>.
    /// </summary>
    internal static readonly RegexGroup SingleExtensionGroup = new(@"\.[^.]+");
    /// <summary>
    /// Matches a contiguous sequence of <see cref="FileExtension"/>s at the <b>end</b> of a <see cref="string"/>.
    /// </summary>
    internal static readonly RegexGroup ExtensionGroup = new($@"({SingleExtensionGroup})+$");

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
    /// <returns>the "base name" for the path, without any <see cref="FileExtension"/>s</returns>
    [Pure]
    public static FileNamePart GetBaseName(string? path) {
        var bn = GetBaseName(path.AsSpan());
        return new FileNamePart(bn.ToString());
    }

    public static ReadOnlySpan<char> GetBaseName(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 ? path : path[..firstPeriod];
    }

    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 ? ReadOnlySpan<char>.Empty : path[firstPeriod..];
    }

    public static IEnumerable<PathPart> SplitPath(string? path) {
        return EnumeratePathParts(path)
            .ToImmutableArray(span => new PathPart(span.ToString()));
    }

    private static SpanSpliterator<char> EnumeratePathParts(ReadOnlySpan<char> path) => new(path, DirectorySeparatorChars.AsSpan());

    /// <summary>
    /// Extracts each <b>individual</b> <see cref="FileExtension"/> from a path.
    /// </summary>
    /// <remarks>This uses the <see cref="SingleExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    /// <remarks>This method is similar to <see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c></remarks>
    public static ImmutableArray<FileExtension> GetExtensions(string? path) {
        return EnumerateExtensions(path)
            .ToImmutableArray(span => new FileExtension(span.ToString()));
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