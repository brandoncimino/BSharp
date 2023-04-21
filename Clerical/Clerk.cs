using System.Diagnostics.Contracts;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Memory.Enumerators;

namespace FowlFever.Clerical;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileExtension"/>.
/// </summary>
public static partial class Clerk {
    [Pure]
    public static ValueArray<PathPart> SplitPath(string? path) {
        static PathPart CreatePathPart(ReadOnlySpan<char> span, string path) {
            return new PathPart(Substring.CreateFromSpan(span, path));
        }

        return path is not { Length: > 0 } ? ValueArray<PathPart>.Empty : EnumeratePathParts(path).ToImmutableArray(path, CreatePathPart);
    }

    [Pure] internal static SpanSpliterator<char> EnumeratePathParts(ReadOnlySpan<char> path) => path.SpliterateAny(DirectorySeparatorChars.AsSpan());

    /// <summary>
    /// <inheritdoc cref="Path.GetTempPath"/>
    /// </summary>
    /// <remarks>
    /// 📎 The built-in method, <see cref="Path.GetTempPath"/>, is ambiguously named - it specifically always returns a folder <i>(and uses the term "folder" in its doc comments, not directory!)</i>.
    /// </remarks>
    /// <returns>a sequence of <see cref="PathPart"/>s equivalent of <see cref="Path.GetTempPath"/></returns>
    [Pure]
    public static ValueArray<PathPart> GetTempFolder() => SplitPath(Path.GetTempPath());

    /// <summary>
    /// <inheritdoc cref="Path.GetTempFileName"/>
    /// </summary>
    /// <remarks>
    /// ⚠ The built-in method, <see cref="Path.GetTempFileName"/> is <b>incorrectly named</b>! It actually <b>creates a new file!</b>
    /// </remarks>
    /// <returns>a sequence of <see cref="PathPart"/>s equivalent of <see cref="Path.GetTempFileName"/></returns>
    [Pure]
    public static ValueArray<PathPart> CreateTempFile() => SplitPath(Path.GetTempFileName());

    /// <summary>
    /// <inheritdoc cref="Path.GetRandomFileName"/>
    /// </summary>
    /// <returns>a sequence of <see cref="PathPart"/>s equivalent to <see cref="Path.GetRandomFileName"/></returns>
    [Pure]
    public static FileName GetRandomFileName() {
        return FileName.Parse(Path.GetRandomFileName());
    }

    /// <param name="path">a file path</param>
    /// <returns><c>true</c> if the <paramref name="path"/> <see cref="EndsInDirectorySeparator(System.ReadOnlySpan{char})"/> or is a <see cref="SpecialPathPart"/></returns>
    [Pure]
    public static bool IsDirectory(string path) => EndsInDirectorySeparator(path) || PathPart.IsSpecialPathPart(path);

    /// <inheritdoc cref="IsDirectory(string)"/>
    [Pure]
    public static bool IsDirectory(ReadOnlySpan<char> path) => EndsInDirectorySeparator(path) || PathPart.IsSpecialPathPart(path);
}