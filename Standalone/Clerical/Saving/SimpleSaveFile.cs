using System;
using System.IO;

namespace FowlFever.BSharp.Clerical.Saving {
    public class SimpleSaveFile<TData> : BaseSaveFile<TData> where TData : ISaveData {
        public override string   Nickname  { get; }
        public override DateTime TimeStamp => File.LastAccessTime;

        public SimpleSaveFile(
            ISaveFolder saveFolder,
            FileInfo    fileInfo,
            TData?      data = default
        ) : base(
            saveFolder,
            fileInfo,
            data
        ) { }

        public SimpleSaveFile(
            ISaveFolder saveFolder,
            string      nickname,
            string      extension = SaveFileName.DefaultExtension,
            TData?      data      = default
        ) : this(
            saveFolder,
            new FileInfo($"{nickname}{extension}"),
            data
        ) {
            Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
        }
    }
}