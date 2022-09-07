using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Atomic;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly record struct PathPart : IPathPart {
    public static readonly PathPart Empty = new();

    private readonly string? _value;
    public           string  Value => _value ?? "";

    public PathPart(string pathPart) : this(pathPart, MustRatify.Yes) { }

    public PathPart(ReadOnlySpan<char> pathPart) : this(pathPart.ToString()) { }

    private static ReadOnlySpan<char> TrimDirectorySeparators(ReadOnlySpan<char> pathPart) {
        var skipped = pathPart.SkipWhile(static c => c.IsDirectorySeparator(), 2);
        pathPart = pathPart.SkipWhile(static c => c.IsDirectorySeparator(), 2)
                           .SkipLastWhile(static c => c.IsDirectorySeparator(), 1);
        return pathPart;
    }

    internal PathPart(string pathPart, MustRatify mustRatify) {
        _value = mustRatify switch {
            MustRatify.Yes => IPathPart.Ratify(TrimDirectorySeparators(pathPart.AsSpan())).ToString(),
            MustRatify.No  => pathPart,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public bool Equals<T>(T? other)
        where T : IHas<string?>? =>
        Equals(other?.Value);

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public bool Equals(IHas<string?>? other) => Equals(other?.Value);
    public bool Equals(string?        other) => Value.Equals(other);

    public override string     ToString()     => Value;
    public          bool       IsEmpty        => Value.IsEmpty();
    public          PathString ToPathString() => new(Value, MustRatify.No);
    public          PathPart   ToPathPart()   => this;

    #region SpecialPathParts

    public static readonly PathPart CurrentDirectory = new(".", MustRatify.No);
    public static readonly PathPart ParentDirectory  = new("..", MustRatify.No);
    public static readonly PathPart HomeDirectory    = new("~", MustRatify.No);

    public static implicit operator PathPart(SpecialPathPart specialPathPart) => specialPathPart.ToPathPart();

    #endregion
}