using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Atomic;

/// <summary>
/// Ensures that <see cref="PathString.Value"/>:
/// <ul>
/// <li>Doesn't contain whitespace</li>
/// <li>Contains at most 1 leading period</li>
/// </ul>
/// </summary>
public readonly record struct FileExtension : IHas<string>, IFileExtension {
    public static readonly FileExtension Empty = new();

    public string Value   { get; }
    public bool   IsEmpty => Value.IsEmpty();

    /// <summary>
    /// Ensures that <see cref="PathString.Value"/>:
    /// <ul>
    /// <li>Doesn't contain whitespace</li>
    /// <li>Contains at most 1 leading period</li>
    /// </ul>
    /// </summary>
    public FileExtension(string fileExtension) : this(fileExtension, MustRatify.Yes) { }

    internal FileExtension(string fileExtension, MustRatify mustRatify) {
        Value = mustRatify switch {
            MustRatify.Yes => IFileExtension.Ratify(fileExtension),
            MustRatify.No  => fileExtension,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public override string ToString() => Value;

    public PathString    ToPathString()    => new(Value, MustRatify.No);
    public PathPart      ToPathPart()      => new(Value, MustRatify.No);
    public FileNamePart  ToFileNamePart()  => new(Value[1..], MustRatify.No);
    public FileExtension ToFileExtension() => this;
}