using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated.Atomic;

public enum SpecialPathPart { CurrentDirectory, ParentDirectory, HomeDirectory }

public interface IPathPart : IPathString {
    public PathPart ToPathPart();

    private const string DoublePeriod = "..";

    public static SpecialPathPart? GetSpecialPathPart(ReadOnlySpan<char> pathPart) {
        return pathPart switch {
            { Length: 1 } when pathPart[0] == '.'        => SpecialPathPart.CurrentDirectory,
            { Length: 1 } when pathPart[0] == '~'        => SpecialPathPart.HomeDirectory,
            { Length: 2 } when pathPart.StartsWith("..") => SpecialPathPart.ParentDirectory,
            _                                            => null,
        };
    }

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> pathPart) {
        if (GetSpecialPathPart(pathPart).HasValue) {
            return pathPart;
        }

        Must.Have(pathPart.Length > 0);
        Must.Have(pathPart[0].IsWhitespace(),                 false, "cannot start with whitespace");
        Must.Have(pathPart[^1].IsWhitespace(),                false, "cannot end with whitespace");
        Must.Have(pathPart[^1] != '.',                        true,  "cannot end in a period");
        Must.Have(pathPart.Length == 1 && pathPart[0] == ':', false, "cannot be a single colon (colons are special thingies for drive indicators)");
        Must.Have(pathPart.IndexOf(DoublePeriod) < 0,         true,  details: $"cannot contain double-periods ({DoublePeriod})");
        BadCharException.Assert(pathPart, Clerk.InvalidPathPartChars);
        return pathPart;
    }

    public new static string Ratify(string pathPart) {
        Ratify(pathPart.AsSpan());
        return pathPart;
    }
}

public static class PathPartExtensions {
    public static SpecialPathPart? AsSpecialPathPart<T>(this T pathPart)
        where T : IPathPart {
        return IPathPart.GetSpecialPathPart(pathPart.ToPathPart().Value);
    }

    public static PathPart ToPathPart(this SpecialPathPart specialPathPart) => specialPathPart switch {
        SpecialPathPart.CurrentDirectory => PathPart.CurrentDirectory,
        SpecialPathPart.ParentDirectory  => PathPart.ParentDirectory,
        SpecialPathPart.HomeDirectory    => PathPart.HomeDirectory,
    };
}