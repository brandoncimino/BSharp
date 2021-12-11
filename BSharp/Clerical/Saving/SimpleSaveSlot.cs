using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Clerical.Saving {
    /// <summary>
    /// The most basic implementation of <see cref="ISaveSlot{TData}"/>:
    /// <ul>
    /// <li>Serializes a <b>new</b> <see cref="ISaveFile{TData}"/> whenever <see cref="Save"/> is called.</li>
    /// <li>Throws an error if the <see cref="ISaveFile{TData}"/> already exists.</li>
    /// </ul>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [PublicAPI]
    public class SimpleSaveSlot<TData> : ISaveSlot<TData> where TData : ISaveData {
        public string              Nickname              { get; }
        public SaveManagerSettings Settings              { get; }
        public ISaveFolder         SaveFolder            { get; }
        public string[]            RelativePath          { get; }
        public int                 SaveFileCount         => EnumerateSaveFiles().Count();
        public string              SaveFileSearchPattern { get; }

        public SimpleSaveSlot(
            SaveFolder           saveFolder,
            string               nickname,
            SaveManagerSettings? settings = default
        ) {
            if (string.IsNullOrWhiteSpace(nickname)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(nickname));
            }

            settings     ??= new SaveManagerSettings();
            SaveFolder   =   saveFolder;
            Nickname     =   nickname;
            Settings     =   settings;
            RelativePath =   new[] { nickname };

            SaveFileSearchPattern = new SaveFileName {
                Nickname      = nickname,
                FullExtension = settings.SaveFileExtension,
            }.GetFileSearchPattern();
        }

        public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles() {
            return SaveFolder.EnumerateSaveFiles<TData>(SaveFileSearchPattern);
        }

        public ISaveFile<TData> LatestFile() {
            return EnumerateSaveFiles().First();
        }

        public ISaveFile<TData> Save(TData saveData, DateTime now) {
            return new SimpleSaveFile<TData>(
                    SaveFolder,
                    Nickname,
                    Settings.SaveFileExtension,
                    saveData
                )
                .Save();
        }

        public override string ToString() {
            return $"📥 {GetType().Prettify()}: \"{Nickname}\"";
        }
    }
}