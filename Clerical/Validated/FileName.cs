using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>-safe name.
///
/// TODO: Add validation when things like <see cref="BaseName"/> or <see cref="Extensions"/> are set
/// TODO: Always strip periods from <see cref="Extensions"/>
/// </summary>
public record FileName(
    FileNamePart    BaseName,
    FileExtension[] Extensions
) {
    public string NameWithExtensions => Extensions.Prepend(BaseName).JoinString(".");

    public FileName(FileNamePart fileName, IEnumerable<FileExtension> extensions) : this(fileName, extensions.ToArray()) {

    }
}