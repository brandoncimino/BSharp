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
public readonly record struct FileNamePart : IHas<string>, IFileNamePart {
    public static readonly FileNamePart Empty = new();

    public string Value { get; }

    public FileNamePart(string fileNamePart) : this(fileNamePart, MustRatify.Yes) { }

    internal FileNamePart(string fileNamePart, MustRatify mustRatify) {
        Value = mustRatify switch {
            MustRatify.Yes => IFileNamePart.Ratify(fileNamePart),
            MustRatify.No  => fileNamePart,
            _              => throw BEnum.UnhandledSwitch(mustRatify)
        };
    }

    public override string ToString() => Value;

    public bool         IsEmpty          => Value.IsEmpty();
    public PathString   ToPathString()   => new(Value, MustRatify.No);
    public PathPart     ToPathPart()     => new(Value, MustRatify.No);
    public FileNamePart ToFileNamePart() => this;
}