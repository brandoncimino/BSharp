using System.Collections.Generic;

using BrandonUtils.Standalone.Clerical;

namespace FowlFever.BSharp.Clerical.Saving {
    public interface ISaveFolder : IHasDirectoryInfo {
        public string PersistentDataPath             { get; }
        public string RelativePathFromPersistentData { get; }

        public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles<TData>(string searchPattern) where TData : ISaveData;
    }
}