namespace FowlFever.Clerical.Validated.Composed;

public interface IDirectoryPath : IFileSystemPath {
    DirectoryPath ToDirectoryPath();
    PathPart      DirectoryName { get; }
}