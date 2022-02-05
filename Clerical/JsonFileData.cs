using FowlFever.BSharp.Clerical;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

/// <summary>
/// An <see cref="ImmutableFileData{T}"/> that uses <see cref="System.Text.Json"/> for serialization.
/// </summary>
/// <inheritdoc/>
public class JsonFileData<T> : ImmutableFileData<T> {
    public JsonFileData(FileInfo fileInfo) : base(fileInfo) { }
    public JsonFileData(T        data, FileInfo fileInfo) : base(data, fileInfo) { }

    protected override void ExecuteDeserialization() {
        Data = File.Deserialize<T>();
    }

    protected override void ExecuteSerialization() {
        File.Serialize(Data);
    }
}