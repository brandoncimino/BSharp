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
public readonly record struct PathString : IPathString {
    /// <summary>
    /// The safest max length for a file path, based on:
    /// <ul>
    /// <li>Windows's limit of <a href="https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry">260</a></li>
    /// <li>Some anecdotes about Mac's being <a href="https://stackoverflow.com/questions/7140575/mac-os-x-lion-what-is-the-max-path-length">255</a></li>
    /// </ul>
    /// </summary>
    internal const int MaxLength = 255;

    public static readonly PathString Empty = new();

    public string Value   { get; }
    public bool   IsEmpty => CollectionUtils.IsEmpty(Value);

    public PathString(string pathString) : this(pathString, MustRatify.Yes) { }

    internal PathString(string pathString, MustRatify mustRatify) {
        Value = mustRatify switch {
            MustRatify.Yes => IPathString.Ratify(pathString),
            MustRatify.No  => pathString,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public override string ToString() => Value;

    public PathString ToPathString() => this;

    #region Equals

    public bool Equals<T>(T? other)
        where T : IHas<string?>? =>
        Equals(other?.Value);

    public bool Equals(IPathString?   other) => Equals(other?.Value);
    public bool Equals(IHas<string?>? other) => Equals(other?.Value);
    public bool Equals(string?        other) => Value.Equals(other);

    #endregion
}