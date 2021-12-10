using System;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public abstract class SaveData : ISaveData {
        public DateTime LastSaveTime { get; }
        public DateTime LastLoadTime { get; }
    }
}