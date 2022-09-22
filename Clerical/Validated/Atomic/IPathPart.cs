using System.Diagnostics;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated.Atomic;

public interface IPathPart : IPathString {
    public PathPart ToPathPart();

    private const string DoublePeriod = "..";

    public static bool IsValid(ReadOnlySpan<char> pathPart) {
        return _tryValidate(pathPart) == null;
    }

    private static Exception? _tryValidate(ReadOnlySpan<char> pathPart) {
        if (PathPart.GetSpecialPathPart(pathPart).HasValue) {
            return default;
        }

        return Must.Try(pathPart.IsEmpty, false, "cannot be empty") ??
               Must.Try(pathPart[0].IsWhitespace(), false, "cannot start with whitespace") ??
               Must.Try(pathPart[^1].IsWhitespace(), false, "cannot end with whitespace") ??
               Must.Try(pathPart[^1] != '.', true, $"cannot end in a period (unless the entire {nameof(PathPart)} is '.' or '..')") ??
               Must.Try(pathPart.Length == 1 && pathPart[0] == ':', false, "cannot be a single colon (colons are special thingies for drive indicators)") ??
               Must.Try(pathPart.IndexOf(DoublePeriod) < 0, true, details: $"cannot contain double-periods ('..') (unless the entire {nameof(PathPart)} is '..')") ??
               BadCharException.TryAssert(pathPart, Clerk.InvalidPathPartChars.AsSpan())
            ;
    }

    [StackTraceHidden]
    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> pathPart) {
        _tryValidate(pathPart)?.Throw();
        return pathPart;
    }

    public new static string Ratify(string pathPart) {
        Ratify(pathPart.AsSpan());
        return pathPart;
    }
}