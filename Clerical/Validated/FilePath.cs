using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Combines a <see cref="DirectoryPath"/> with a <see cref="FileName"/>.
/// </summary>
/// <param name="DirectoryPath"></param>
/// <param name="FileName"></param>
/// <param name="Separator"></param>
public record FilePath(
    DirectoryPath      DirectoryPath,
    FileName           FileName,
    DirectorySeparator Separator = default
) {
    public override string ToString() {
        return string.Join(Separator.ToCharString(), DirectoryPath, FileName);
    }
}