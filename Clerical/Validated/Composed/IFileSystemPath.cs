using System.Collections.Immutable;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFileSystemPath : IPathString {
    public ImmutableArray<PathPart> Parts  { get; }
    public DirectoryPath            Parent { get; }
}