using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical;

/// <summary>
/// A basic implementation of <see cref="ISaveFolder"/>
/// </summary>
public class SaveFolder : ISaveFolder {
    public string        PersistentDataPath             { get; }
    public FileName      FolderName                     { get; }
    public string        RelativePathFromPersistentData => FolderName.NameWithExtensions;
    public DirectoryInfo DirectoryInfo                  => new DirectoryInfo(BPath.JoinPath(PersistentDataPath, FolderName.BaseName));

    public SaveFolder(string persistentDataPath, FileName nickname) {
        PersistentDataPath = persistentDataPath;
        FolderName         = nickname;
    }
}