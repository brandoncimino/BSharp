using System.Diagnostics.Contracts;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Clerical.Validated.Composed;

namespace FowlFever.Clerical;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileName"/>.
/// </summary>
public static partial class Clerk {
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
    public static IEnumerable<PathPart> SplitPath(string? path) {
        return EnumeratePathParts(path)
            .ToImmutableArray(static span => new PathPart(span.ToString()));
    }

    [Pure] public static SpanSpliterator<char> SplitPath(ReadOnlySpan<char> path) => EnumeratePathParts(path);

    [Pure] private static SpanSpliterator<char> EnumeratePathParts(ReadOnlySpan<char> path) => new(path, DirectorySeparatorChars.AsSpan(), SplitterMatchStyle.AnyEntry, StringSplitOptions.RemoveEmptyEntries | (StringSplitOptions)2);

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
    public static FileName GetRandomFileName() => FindFileName(Path.GetRandomFileName()).MustNotBeNull();
}