using JetBrains.Annotations;

namespace FowlFever.Clerical;

/// <summary>
/// I dunno but this seemed...logical?
/// </summary>
/// <inheritdoc/>
public class SaveFileData<T> : JsonFileData<T> {
    public SaveFileData(FileInfo fileInfo) : base(fileInfo) { }
    public SaveFileData(T        data, FileInfo fileInfo) : base(data, fileInfo) { }
}