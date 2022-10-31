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

    #endregion
}