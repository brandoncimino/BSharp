using Ratified;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public record PathPart : PathString {
    public PathPart(string pathPart) : this(pathPart, MustRatify.Yes) { }

    internal PathPart(string pathPart, MustRatify mustRatify) : base(pathPart, MustRatify.No) {
        if (mustRatify == MustRatify.Yes) {
            BadCharException.Assert(pathPart, Clerk.InvalidPathPartChars);
        }

        Value = pathPart;
    }

    public override string ToString() => Value;
}