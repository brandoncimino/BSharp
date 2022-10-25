namespace FowlFever.Clerical.Validated.Composed;

public interface IDirectoryPath : IFileSystemPath {
    DirectoryPath   ToDirectoryPath();
    Atomic.PathPart DirectoryName { get; }
}