using System.Diagnostics.Contracts;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.Clerical.Validated.Atomic;
using FowlFever.Clerical.Validated.Composed;

namespace FowlFever.Clerical;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileName"/>.
/// </summary>
public static partial class Clerk {
    [Pure]
    public static IEnumerable<PathPart> SplitPath(string? path) {
        if (path == null) {
            yield break;
        }

        int pos = 0;

        static int SeparatorIndex(ReadOnlySpan<char> str) {
            return str.IndexOfAny(DirectorySeparatorChars.AsSpan());
        }

        while (pos < path.Length) {
            var remaining = path.AsSpan(pos);
            var sep       = SeparatorIndex(path.AsSpan(pos));

            // no more separators; yield whatever's is left and then break
            if (sep <= -1) {
                yield return new PathPart(remaining);
                yield break;
            }

            // at least 1 char before the next separator, so yield that part
            if (sep >= 1) {
                yield return new PathPart(remaining[..sep]);
            }

            // advance `pos` by the size of the part + 1 (to account for the separator itself)
            pos += sep + 1;
        }
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

    /// <param name="path">a file path</param>
    /// <returns><c>true</c> if the <paramref name="path"/> <see cref="EndsInDirectorySeparator(System.ReadOnlySpan{char})"/> or is a <see cref="SpecialPathPart"/></returns>
    [Pure]
    public static bool IsDirectory(string path) => EndsInDirectorySeparator(path) || PathPart.IsSpecialPathPart(path);

    /// <inheritdoc cref="IsDirectory(string)"/>
    [Pure]
    public static bool IsDirectory(ReadOnlySpan<char> path) => EndsInDirectorySeparator(path) || PathPart.IsSpecialPathPart(path);
}