using BrandonUtils.Standalone.Clerical;
using BrandonUtils.Standalone.Optional;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Clerical.Saving {
    [PublicAPI]
    public class SaveManagerSettings {
        public Fallback<JsonSerializerSettings?> JsonSerializerSettings  { get; } = new Fallback<JsonSerializerSettings>(new JsonSerializerSettings());
        public Fallback<string?>                 AutoSaveName            { get; } = new Fallback<string>("AutoSave");
        public Fallback<string?>                 SaveFileExtension       { get; } = new Fallback<string>(".sav.json");
        public Fallback<int>                     BackupSaveSlots         { get; } = new Fallback<int>(10);
        public Fallback<DuplicateFileResolution> DuplicateFileResolution { get; } = new Fallback<DuplicateFileResolution>();
    }
}