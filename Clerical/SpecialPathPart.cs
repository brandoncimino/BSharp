using FowlFever.BSharp.Enums;
using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical;

public enum SpecialPathPart : byte { CurrentDirectory, ParentDirectory, HomeDirectory }

public static class SpecialPathPartExtensions {
    public static string PathString(this SpecialPathPart specialPathPart) => specialPathPart switch {
        SpecialPathPart.CurrentDirectory => ".",
        SpecialPathPart.ParentDirectory  => "..",
        SpecialPathPart.HomeDirectory    => "~",
        _                                => throw BEnum.UnhandledSwitch(specialPathPart),
    };

    public static PathPart GetPathPart(this SpecialPathPart specialPathPart) => specialPathPart switch {
        SpecialPathPart.CurrentDirectory => PathPart.CurrentDirectory,
        SpecialPathPart.ParentDirectory  => PathPart.ParentDirectory,
        SpecialPathPart.HomeDirectory    => PathPart.HomeDirectory,
        _                                => throw BEnum.UnhandledSwitch(specialPathPart),
    };

    public static SpecialPathPart? AsSpecialPathPart<T>(this T pathPart) where T : IPathPart => PathPart.GetSpecialPathPart(pathPart.ToPathPart().Value);
}