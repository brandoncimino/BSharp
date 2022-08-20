using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Clerical.Validated.Composed;

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
    /// Extracts the <see cref="Path.GetFileName(System.ReadOnlySpan{char})"/> without <b>ANY</b> <see cref="FileExtension"/>s from the given <paramref name="path"/>.
    /// <p/>
    /// If the <paramref name="path"/> doesn't contain a base file name (for example, it <see cref="string.IsNullOrEmpty"/>, or begins with a period like <c>.ssh</c>),
    /// then <see cref="FileNamePart.Empty"/> is returned instead.
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
        return new FileNamePart(bn);
    }

    [Pure]
    public static ReadOnlySpan<char> GetBaseName(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 ? fileName : fileName[..firstPeriod];
    }

    [Pure]
    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 ? ReadOnlySpan<char>.Empty : path[firstPeriod..];
    }

    [Pure]
    public static IEnumerable<PathPart> SplitPath(string? path) {
        return EnumeratePathParts(path)
            .ToImmutableArray(static span => new PathPart(span.ToString()));
    }

    [Pure] private static SpanSpliterator<char> EnumeratePathParts(ReadOnlySpan<char> path) => new(path, DirectorySeparatorChars.AsSpan(), SplitterStyle.AnyEntry, StringSplitOptions.RemoveEmptyEntries | (StringSplitOptions)2);

    /// <summary>
    /// Extracts each <b>individual</b> <see cref="FileExtension"/> from a path.
    /// </summary>
    /// <remarks>This uses the <see cref="SingleExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    /// <remarks>This method is similar to <see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c></remarks>
    [Pure]
    public static ImmutableArray<FileExtension> GetExtensions(string? path) {
        return EnumerateExtensions(path)
            .ToImmutableArray(static span => new FileExtension('.' + span.ToString()));
    }

    [Pure] private static SpanSpliterator<char> EnumerateExtensions(ReadOnlySpan<char> path) => GetFullExtension(path).Spliterate(StringSplitOptions.RemoveEmptyEntries, '.');

    [Pure] public static FileName GetFileName(string             path) => new(path);
    [Pure] public static FileName GetFileName(ReadOnlySpan<char> path) => GetFileName(path.ToString());

    [Pure]
    public static bool EndsInDirectorySeparator(string path) {
#if NET6_0_OR_GREATER
        return Path.EndsInDirectorySeparator(path);
#else
        return DirectorySeparatorChars.Contains(path.Last());
#endif
    }

    /// <summary>
    /// <inheritdoc cref="Path.GetTempPath"/>
    /// </summary>
    /// <returns>a <see cref="DirectoryPath"/> equivalent of <see cref="Path.GetTempPath"/></returns>
    [Pure]
    public static DirectoryPath GetTempFolder() => new(Path.GetTempPath());

    /// <summary>
    /// <inheritdoc cref="Path.GetTempFileName"/>
    /// </summary>
    /// <remarks>
    /// ⚠ The built-in method, <see cref="Path.GetTempFileName"/> is <b>incorrectly named</b>! It actually <b>creates a new file!</b>
    /// </remarks>
    /// <returns>a <see cref="FileName"/> equivalent of <see cref="Path.GetTempFileName"/></returns>
    [Pure]
    public static FileName CreateTempFile() => GetFileName(Path.GetTempFileName());

    /// <summary>
    /// <inheritdoc cref="Path.GetRandomFileName"/>
    /// </summary>
    /// <returns>a <see cref="FileName"/> equivalent of <see cref="Path.GetRandomFileName"/></returns>
    [Pure]
    public static FileName GetRandomFileName() => GetFileName(Path.GetRandomFileName());
}