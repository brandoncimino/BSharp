using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.Implementors;

using Ratified;

namespace FowlFever.Clerical.Validated.Atomic;

/// <summary>
/// Represents one part of a <see cref="FileInfo"/>.<see cref="FileInfo.Name"/> - either the base name or a <see cref="FileExtension"/>.
///
/// Specifically, <see cref="FileNamePart"/>s subdivide <see cref="PathPart"/>s by periods. 
/// </summary>
/// <example>
/// <code><![CDATA[
///                            [         FileName          ]
/// C:\Users\bcimino\dev\BSharp\BSharp.sln.DotSettings.user
///                            [BSharp]   [DotSettings]                 
///                                   [sln]           [user]
/// ]]></code>
/// </example>
public readonly record struct FileNamePart : IFileNamePart {
    public static readonly FileNamePart Empty = new();

    private readonly string? _value;
    public           string  Value => _value ?? "";

    public FileNamePart(ReadOnlySpan<char> fileNamePart) : this(fileNamePart.ToString()) { }
    public FileNamePart(string             fileNamePart) : this(fileNamePart, MustRatify.Yes) { }

    internal FileNamePart(string fileNamePart, MustRatify mustRatify) {
        _value = mustRatify switch {
            MustRatify.Yes => IFileNamePart.Ratify(fileNamePart),
            MustRatify.No  => fileNamePart,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public bool Equals(string?        other)                          => Value.Equals(other);
    public bool Equals<T>(T?          other) where T : IHas<string?>? => Equals(other?.Value);
    public bool Equals(IHas<string?>? other)                          => Equals(other?.Value);

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public override string ToString() => Value;

    public bool         IsEmpty          => Value.IsEmpty();
    public PathString   ToPathString()   => new(Value, MustRatify.No);
    public PathPart     ToPathPart()     => new(Value, MustRatify.No);
    public FileNamePart ToFileNamePart() => this;

    // public static bool IsValid(ReadOnlySpan<char> fileNamePart) {
    //     if (PathPart.IsSpecialPathPart(fileNamePart)) {
    //         return false;
    //     }
    //     
    //     if()
    // }
}