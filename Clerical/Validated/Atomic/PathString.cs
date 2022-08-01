using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Atomic;

/// <summary>
/// Represents a string that doesn't contain any <see cref="Path.GetInvalidPathChars"/> or <see cref="Path.GetInvalidFileNameChars"/>.
/// </summary>
/// <remarks>
/// Not using <see cref="ReadOnlySpan{T}"/> because we aren't modifying the <see cref="string"/>, and conversion to <see cref="ReadOnlySpan{T}"/> and <i>back</i> <see cref="ReadOnlySpan{T}.ToString"/>
/// incurs a new <see cref="string"/> creation (even if no modifications were performed).
/// </remarks>
public readonly record struct PathString : IHas<string>, IPathString {
    public static readonly PathString Empty = new();

    public string Value   { get; }
    public bool   IsEmpty => Value.IsEmpty();

    public PathString(string pathString) : this(pathString, MustRatify.Yes) { }

    internal PathString(string pathString, MustRatify mustRatify) {
        Value = mustRatify switch {
            MustRatify.Yes => IPathString.Ratify(pathString),
            MustRatify.No  => pathString,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public override string     ToString()     => Value;
    public          PathString ToPathString() => this;
}