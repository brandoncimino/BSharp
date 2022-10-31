using FowlFever.BSharp.Enums;

namespace FowlFever.Clerical;

public enum SpecialPathPart : byte { CurrentDirectory, ParentDirectory, HomeDirectory }

public static class SpecialPathPartExtensions {
    public static string ToPathString(this SpecialPathPart specialPathPart) => specialPathPart switch {
        SpecialPathPart.CurrentDirectory => ".",
        SpecialPathPart.ParentDirectory  => "..",
        SpecialPathPart.HomeDirectory    => "~",
        _                                => throw BEnum.UnhandledSwitch(specialPathPart),
    };

    public static PathPart ToPathPart(this SpecialPathPart specialPathPart) => specialPathPart switch {
        SpecialPathPart.CurrentDirectory => PathPart.CurrentDirectory,
        SpecialPathPart.ParentDirectory  => PathPart.ParentDirectory,
        SpecialPathPart.HomeDirectory    => PathPart.HomeDirectory,
        _                                => throw BEnum.UnhandledSwitch(specialPathPart),
    };
}