using Ratified;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public record PathPart : PathString {
    private static void Ratify(ReadOnlySpan<char> pathPart) {
        BadCharException.Assert(pathPart, Clerk.InvalidPathPartChars);
    }

    public PathPart(string value) : this(value, MustRatify.Yes) { }

    private PathPart(string value, MustRatify mustRatify) : base(value, MustRatify.No) {
        if (mustRatify == MustRatify.Yes) {
            Ratify(value);
        }

        Value = value;
    }

    internal new static PathPart Force(string value) => new(value, MustRatify.No);

    public override string ToString() => Value;
}