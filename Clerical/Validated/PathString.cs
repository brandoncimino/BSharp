using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Represents a string that doesn't contain any <see cref="Path.GetInvalidPathChars"/> or <see cref="Path.GetInvalidFileNameChars"/>.
/// </summary>
/// <remarks>
/// Not using <see cref="ReadOnlySpan{T}"/> because we aren't modifying the <see cref="string"/>, and conversion to <see cref="ReadOnlySpan{T}"/> and <i>back</i> <see cref="ReadOnlySpan{T}.ToString"/>
/// incurs a new <see cref="string"/> creation (even if no modifications were performed).
/// </remarks>
public record PathString : IHas<string> {
    public string Value { get; init; }

    private static void Ratify(ReadOnlySpan<char> pathString) {
        BadCharException.Assert(pathString, Clerk.InvalidPathChars);
    }

    public PathString(string pathString) : this(pathString, MustRatify.Yes) { }

    protected PathString(string pathString, MustRatify mustRatify) {
        if (mustRatify == MustRatify.Yes) {
            Ratify(pathString);
        }

        Value = pathString;
    }

    internal static PathString Force(string pathString) => new(pathString, MustRatify.No);

    public override string ToString() => Value;
}