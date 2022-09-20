using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Clerical.Validated.Composed;

namespace FowlFever.Clerical.Validated;

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
    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 ? ReadOnlySpan<char>.Empty : fileName[firstPeriod..];
    }

    [Pure]
    public static IEnumerable<PathPart> SplitPath(string? path) {
        return EnumeratePathParts(path)
            .ToImmutableArray(static span => new PathPart(span.ToString()));
    }

    public static SpanSpliterator<char> SplitPath(ReadOnlySpan<char> path) => EnumeratePathParts(path);

    [Pure] private static SpanSpliterator<char> EnumeratePathParts(ReadOnlySpan<char> path) => new(path, DirectorySeparatorChars.AsSpan(), SplitterMatchStyle.AnyEntry, StringSplitOptions.RemoveEmptyEntries | (StringSplitOptions)2);

    /// <summary>
    /// Extracts each <b>individual</b> <see cref="FileExtension"/> from a path.
    /// </summary>
    /// <remarks>This uses the <see cref="SingleExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    /// <remarks>This method is similar to <see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c></remarks>
    [Pure]
    public static ImmutableArray<FileExtension> GetExtensions(string? path) {
        var builder = ImmutableArray.CreateBuilder<FileExtension>();
        var ct      = 0;
        foreach (var span in EnumerateExtensions(path)) {
            ct += 1;
            builder.Add(new FileExtension('.' + span.ToString()));
        }

        builder.Capacity = ct;
        return builder.MoveToImmutable();
    }

    [Pure] internal static SpanSpliterator<char> EnumerateExtensions(ReadOnlySpan<char> path) => GetFullExtension(path).Spliterate('.') with { Options = StringSplitOptions.RemoveEmptyEntries };

    [Pure]
    public static FileName? FindFileName(string path) {
        return FindFileName(path.AsSpan());
    }

    [Pure]
    public static FileName? FindFileName(ReadOnlySpan<char> path) {
        var coreFileName = Path.GetFileName(path);

        if (coreFileName.IsEmpty) {
            return default;
        }

        return new FileName(coreFileName.ToString());
    }

    private static ArgumentException NoFileNameException(ReadOnlySpan<char> path, [CallerArgumentExpression("path")] string? _path = default) {
        return new ArgumentException($"[{path.ToString()}] did not contain a valid {nameof(FileName)}!", _path);
    }

    /// <summary>
    /// Extracts the <see cref="FileName"/> from a path, throwing an exception if it can't.
    /// </summary>
    /// <param name="path">a file path</param>
    /// <returns>a new <see cref="FileName"/></returns>
    /// <exception cref="ArgumentException">if the <paramref name="path"/> didn't contain a <see cref="FileName"/></exception>
    [Pure]
    public static FileName GetFileName(string path) => FindFileName(path) ?? throw NoFileNameException(path);

    /// <inheritdoc cref="GetFileName(string)"/>
    [Pure]
    public static FileName GetFileName(ReadOnlySpan<char> path) => FindFileName(path) ?? throw NoFileNameException(path);

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