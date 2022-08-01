using FowlFever.BSharp.Clerical;
using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated;

public static class ClericalExtensions {
    public static string JoinPath(this IEnumerable<PathPart> parts, DirectorySeparator separator = DirectorySeparator.Universal) {
        return string.Join(separator.ToChar(), parts);
    }
}