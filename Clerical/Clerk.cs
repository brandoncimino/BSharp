using System.Collections.Immutable;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory.Enumerators;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// Contains factory methods for <see cref="Validated"/> objects like <see cref="PathPart"/> and <see cref="FileExtension"/>.
/// </summary>
public static partial class Clerk {
    [Pure]
    public static ValueArray<PathPart> SplitPath(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return ValueArray<PathPart>.Empty;
        }

        var erator       = new IndexOfAnyEnumerator<char>(path, DirectorySeparatorChars);
        var arrayBuilder = ImmutableArray.CreateBuilder<PathPart>();
        foreach (var index in erator) {
            var segment = new StringSegment(path, index, path.Length - index);
            var part    = PathPart.Parser.CreateUnsafe(segment);
            arrayBuilder.Add(part);
        }

        return arrayBuilder.MoveToImmutableSafely();
    }

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
}