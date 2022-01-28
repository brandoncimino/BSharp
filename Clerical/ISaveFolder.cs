namespace FowlFever.Clerical;

/// <summary>
/// Represents the <see cref="DirectoryInfo"/> in which <see cref="ISaveFile{TData}"/>s are stored.
/// </summary>
public interface ISaveFolder {
    public string PersistentDataPath             { get; }
    public string RelativePathFromPersistentData { get; }

    public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles<TData>(string searchPattern) where TData : ISaveData;
}