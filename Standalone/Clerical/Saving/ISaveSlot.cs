using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Clerical.Saving {
    public interface ISaveSlot<TData> where TData : ISaveData {
        public string      Nickname   { get; }
        public ISaveFolder SaveFolder { get; }

        public string[] RelativePath { get; }

        [NonNegativeValue]
        public int SaveFileCount { get; }

        public string SaveFileSearchPattern { get; }

        public SaveManagerSettings Settings { get; }


        public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles();

        public ISaveFile<TData>? LatestFile();


        public ISaveFile<TData> Save(TData saveData, DateTime now);
    }
}