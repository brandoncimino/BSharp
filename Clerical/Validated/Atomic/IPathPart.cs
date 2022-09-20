using System.Diagnostics;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated.Atomic;

public interface IPathPart : IPathString {
    public PathPart ToPathPart();

    private const string DoublePeriod = "..";

    [StackTraceHidden]
    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> pathPart) {
        if (PathPart.GetSpecialPathPart(pathPart).HasValue) {
            return pathPart;
        }

        Must.Have(pathPart.Length > 0);
        Must.Have(pathPart[0].IsWhitespace(),                 false, "cannot start with whitespace");
        Must.Have(pathPart[^1].IsWhitespace(),                false, "cannot end with whitespace");
        Must.Have(pathPart[^1] != '.',                        true,  $"cannot end in a period (unless the entire {nameof(PathPart)} is '.' or '..')");
        Must.Have(pathPart.Length == 1 && pathPart[0] == ':', false, "cannot be a single colon (colons are special thingies for drive indicators)");
        Must.Have(pathPart.IndexOf(DoublePeriod) < 0,         true,  details: $"cannot contain double-periods ('..') (unless the entire {nameof(PathPart)} is '..'");
        BadCharException.Assert(pathPart, Clerk.InvalidPathPartChars);
        return pathPart;
    }

    public new static string Ratify(string pathPart) {
        Ratify(pathPart.AsSpan());
        return pathPart;
    }
}