using System;

using Newtonsoft.Json;

namespace FowlFever.BSharp.Clerical.Saving {
    public interface ISaveData {
        [JsonProperty]
        public DateTime LastSaveTime { get; }

        [JsonProperty]
        public DateTime LastLoadTime { get; }
    }
}