using System;
using System.IO;
using System.Text.Json;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Clerical.Saving {
    public abstract class BaseSaveFile<TData> : ISaveFile<TData>
        where TData : ISaveData {
        public          FileSystemInfo FileSystemInfo => File;
        public          FileInfo       File           { get; }
        public abstract string         Nickname       { get; }
        public          ISaveFolder    SaveFolder     { get; }
        public          TData?         Data           { get; internal set; }
        public abstract DateTime       TimeStamp      { get; }

        protected BaseSaveFile(
            ISaveFolder folder,
            FileInfo    file,
            TData?      data
        ) {
            folder.MustBeParentOf(file);
            File       = file   ?? throw new ArgumentNullException(nameof(file));
            SaveFolder = folder ?? throw new ArgumentNullException(nameof(folder));
            Data       = data;
        }

        protected BaseSaveFile(
            ISaveFolder folder,
            string      relativePathToFile,
            TData?      data
        )
            : this(
                folder ?? throw new ArgumentNullException(nameof(folder)),
                new FileInfo(Muster.Have(folder.Directory.GetChildPath(relativePathToFile)).NotBlank()),
                data
            ) { }

        public ISaveFile<TData> Save(DuplicateFileResolution duplicateFileResolution, JsonSerializerOptions? jsonSerializerOptions = default) {
            File.Serialize(Data, duplicateFileResolution);
            return this;
        }

        public ISaveFile<TData> Save(SaveManagerSettings? saveSettings = default) {
            saveSettings ??= new SaveManagerSettings();
            Save(saveSettings.DuplicateFileResolution, saveSettings.JsonSerializerOptions);
            return this;
        }

        public ISaveFile<TData> Load(SaveManagerSettings? saveSettings = default) {
            saveSettings ??= new SaveManagerSettings();
            Data         =   File.Deserialize<TData>(saveSettings.JsonSerializerOptions);
            return this;
        }

        public override string ToString() {
            return $"📄 {File.ToUri()}";
        }
    }
}