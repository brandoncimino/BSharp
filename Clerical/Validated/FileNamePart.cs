using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.Clerical.Fluffy;

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
public record FileNamePart(string Value) : PathPart(Value) {
    internal override string BeforeValidation(string value) => base.BeforeValidation(value).TrimStart(".", 1);

    [Validator] private void NoPeriods() => Must.NotContain(Value, ".");
}