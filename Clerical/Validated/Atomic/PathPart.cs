using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Atomic;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly record struct PathPart : IHas<string>, IPathPart {
    public string Value { get; }

    public PathPart(string pathPart) : this(pathPart, MustRatify.Yes) { }

    private static ReadOnlySpan<char> TrimDirectorySeparators(ReadOnlySpan<char> pathPart) {
        if (pathPart[0].IsDirectorySeparator()) {
            pathPart = pathPart[1..];
        }

        if (pathPart[^1].IsDirectorySeparator()) {
            pathPart = pathPart[..^1];
        }

        return pathPart;
    }

    internal PathPart(string pathPart, MustRatify mustRatify) {
        Value = mustRatify switch {
            MustRatify.Yes => IPathPart.Ratify(TrimDirectorySeparators(pathPart.AsSpan())).ToString(),
            MustRatify.No  => pathPart,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public override string     ToString()     => Value;
    public          PathString ToPathString() => new(Value, MustRatify.No);
    public          PathPart   ToPathPart()   => this;
}