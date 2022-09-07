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
public readonly record struct FileExtension : IFileExtension {
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

        if (Value.StartsWith('.') == false) {
            Value = '.' + Value;
        }
    }

    public bool Equals(IHas<string?>? other)                          => Equals(other?.Value);
    public bool Equals(string?        other)                          => Value.Equals(other);
    public bool Equals<T>(T?          other) where T : IHas<string?>? => Equals(other?.Value);

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public override string ToString() => Value;

    public PathString    ToPathString()    => new(Value, MustRatify.No);
    public PathPart      ToPathPart()      => new(Value, MustRatify.No);
    public FileNamePart  ToFileNamePart()  => new(Value[1..], MustRatify.No);
    public FileExtension ToFileExtension() => this;
}