using System;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public interface ISaveData {
        [JsonProperty]
        public DateTime LastSaveTime { get; }

        [JsonProperty]
        public DateTime LastLoadTime { get; }
    }
}