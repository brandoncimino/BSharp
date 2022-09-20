using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using FowlFever.Clerical.Validated.Composed;

namespace FowlFever.Clerical;

public static partial class Clerk {
    #region FileName

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

    #endregion
}