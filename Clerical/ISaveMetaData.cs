using System.Text.Json.Serialization;

namespace FowlFever.Clerical;

public interface ISaveMetaData {
    [JsonInclude]
    public DateTime LastSaveTime { get; }

    [JsonInclude]
    public DateTime LastLoadTime { get; }
}