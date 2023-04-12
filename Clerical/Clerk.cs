using System.Diagnostics.Contracts;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;

namespace FowlFever.Clerical;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileExtension"/>.
/// </summary>
public static partial class Clerk {
    /// <summary>
    /// Splits apart each <see cref="PathPart"/> from the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">the original string</param>
    /// <returns>a <see cref="ValueArray{T}"/> containing each <see cref="PathPart"/> in the input <paramref name="path"/></returns>
    [Pure]
    public static ValueArray<PathPart> SplitPath(ReadOnlySpan<char> path) {
        return path.IsEmpty ? default(ValueArray<PathPart>) : EnumeratePathParts(path).ToImmutableArray(static str => PathPart.Of(str));
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
    public static PathPart GetRandomFileName() => FindFileName(Path.GetRandomFileName()).MustNotBeNull();

    /// <param name="path">a file path</param>
    /// <returns><c>true</c> if the <paramref name="path"/> <see cref="EndsInDirectorySeparator(System.ReadOnlySpan{char})"/> or is a <see cref="SpecialPathPart"/></returns>
    [Pure]
    public static bool IsDirectory(string path) => EndsInDirectorySeparator(path) || PathPart.IsSpecialPathPart(path);

    /// <inheritdoc cref="IsDirectory(string)"/>
    [Pure]
    public static bool IsDirectory(ReadOnlySpan<char> path) => EndsInDirectorySeparator(path) || PathPart.IsSpecialPathPart(path);
}