using FowlFever.BSharp.Clerical;
using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated;

public static class ClericalExtensions {
    public static string JoinPathString<T>(this IEnumerable<T> parts, DirectorySeparator separator = DirectorySeparator.Universal)
        where T : IPathPart {
        return string.Join(separator.ToChar(), parts);
    }
}