using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BrandonUtils.Standalone.Clerical;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Clerical.Saving {
    [PublicAPI]
    public readonly struct SaveFolder : ISaveFolder {
        public const string SaveFolderName = "SaveData";

        public string PersistentDataPath             { get; }
        public string RelativePathFromPersistentData => GameName;
        public string GameName                       { get; }

        public DirectoryInfo  Directory      => new DirectoryInfo(FolderPath);
        public FileSystemInfo FileSystemInfo => Directory;


        public SaveFolder(
            string persistentDataPath,
            string gameName
        ) {
            if (string.IsNullOrWhiteSpace(persistentDataPath)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(persistentDataPath));
            }

            if (string.IsNullOrWhiteSpace(gameName)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(gameName));
            }

            PersistentDataPath = persistentDataPath;
            GameName           = gameName;
        }


        private string FolderPath => BPath.JoinPath(
            PersistentDataPath,
            SaveFolderName,
            GameName
        );


        public IEnumerable<ISaveFile<T>> EnumerateSaveFiles<T>(string searchPattern) where T : ISaveData {
            var self = this;
            if (Directory.Exists) {
                return Directory.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly)
                                .Select(it => new SaveFile<T>(self, it))
                                .OrderByDescending(it => it.TimeStamp);
            }
            else {
                return Enumerable.Empty<SaveFile<T>>();
            }
        }

        public override string ToString() {
            return $"📁 {Directory.ToUri()}";
        }
    }
}