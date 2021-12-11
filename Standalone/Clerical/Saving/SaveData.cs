using System;

namespace FowlFever.BSharp.Clerical.Saving {
    public abstract class SaveData : ISaveData {
        public DateTime LastSaveTime { get; }
        public DateTime LastLoadTime { get; }
    }
}