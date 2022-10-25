using FowlFever.BSharp.Clerical;
using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated;

public static class ClericalExtensions {
    public static string ToPathString<T>(this IEnumerable<T> parts, DirectorySeparator separator = DirectorySeparator.Universal)
        where T : IPathPart {
        return string.Join(separator.ToChar(), parts);
    }

    public static string ToPathString<T>(this T parts, DirectorySeparator separator = DirectorySeparator.Universal)
        where T : IEnumerable<PathPart> {
        return string.Join(separator.ToChar(), parts);
    }
}