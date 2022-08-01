using System.Collections.Immutable;

using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFileSystemPath : IPathString {
    public ImmutableArray<PathPart> Parts  { get; }
    public DirectoryPath            Parent { get; }
}