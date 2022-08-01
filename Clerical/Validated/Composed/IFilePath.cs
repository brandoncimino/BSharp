using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFilePath : IFileSystemPath, IFileName {
    public DirectoryPath      Directory { get; }
    public FileName           FileName  { get; }
    public DirectorySeparator Separator { get; }
}