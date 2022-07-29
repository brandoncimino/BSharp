using Ratified;

namespace FowlFever.Clerical.Validated;

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
public record FileNamePart : PathPart {
    public FileNamePart(string fileNamePart) : this(fileNamePart, MustRatify.Yes) { }

    internal FileNamePart(string fileNamePart, MustRatify mustRatify) : base(fileNamePart, MustRatify.No) {
        if (mustRatify == MustRatify.Yes) {
            BadCharException.Assert(fileNamePart, Clerk.InvalidFileNamePartChars);
        }

        Value = fileNamePart;
    }

    public override string ToString() => Value;
}