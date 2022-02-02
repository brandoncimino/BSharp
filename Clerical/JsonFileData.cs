using FowlFever.BSharp.Clerical;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

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