using System.Diagnostics.Contracts;

namespace FowlFever.Clerical;

public static partial class Clerk {
    #region FileName

    [Pure]
    public static PathPart? FindFileName(ReadOnlySpan<char> path) {
        var coreFileName = Path.GetFileName(path);

        if (coreFileName.IsEmpty || PathPart.IsSpecialPathPart(path)) {
            return default;
        }

        return PathPart.Of(coreFileName);
    }

    /// <inheritdoc cref="Path.GetFileName(System.ReadOnlySpan{char})"/>
    /// <remarks>
    /// Compared to the built-in <see cref="Path"/>.<see cref="Path.GetFileName(System.ReadOnlySpan{char})"/>, this uses <see cref="System.MemoryExtensions.IndexOf"/> for better performance <i>(particularly in scenarios where the input doesn't actually have an extension)</i>..
    /// </remarks>
    public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path) {
        var lastSeparator = path.LastIndexOfAny('\\', '/');
        return lastSeparator < 0 ? path : path[(lastSeparator + 1)..];
    }

    public static ReadOnlySpan<char> GetBaseName(ReadOnlySpan<char> path) {
        var fileName    = GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 ? fileName : fileName[..firstPeriod];
    }

    #endregion
}