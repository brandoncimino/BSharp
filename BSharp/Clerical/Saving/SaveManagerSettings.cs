using JetBrains.Annotations;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Clerical.Saving {
    [PublicAPI]
    public class SaveManagerSettings {
        public JsonSerializerSettings  JsonSerializerSettings  { get; } = new JsonSerializerSettings();
        public string                  AutoSaveName            { get; } = "AutoSave";
        public string                  SaveFileExtension       { get; } = ".sav.json";
        public int                     BackupSaveSlots         { get; } = 10;
        public DuplicateFileResolution DuplicateFileResolution { get; } = default;
    }
}