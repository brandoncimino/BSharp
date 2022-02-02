using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Represents a group of <see cref="PathPart"/>s
/// </summary>
/// <param name="Parts"></param>
/// <param name="Separator"></param>
public record DirectoryPath(
    IEnumerable<PathPart>? Parts,
    DirectorySeparator    Separator = default
) : IHasDirectoryInfo {
    public IEnumerable<PathPart> Parts { get; init; } = Parts.OrEmpty();

    public FileSystemInfo FileSystemInfo => Directory;
    public DirectoryInfo  Directory      => new DirectoryInfo(ToString());

    public override string ToString() {
        return string.Join(Separator.ToCharString(), Parts);
    }
}