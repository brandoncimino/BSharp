using System;
using System.IO;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFile<TData> : BaseSaveFile<TData> where TData : ISaveData {
        private readonly SaveFileName _saveFileName;

        public override string Nickname => _saveFileName.Nickname;

        public override DateTime TimeStamp => _saveFileName.TimeStamp;

        public SaveFile(
            ISaveFolder folder,
            string      nickname,
            DateTime    timeStamp,
            string?     extension = SaveFileName.DefaultExtension,
            TData       data      = default
        )
            : this(
                folder,
                new SaveFileName {
                    Nickname      = nickname,
                    TimeStamp     = timeStamp,
                    FullExtension = extension ?? SaveFileName.DefaultExtension
                },
                data
            ) { }

        internal SaveFile(
            ISaveFolder  folder,
            SaveFileName saveFileName,
            TData?       data = default
        ) : base(
            folder,
            saveFileName.Rendered.MustNotBeBlank(),
            data
        ) {
            _saveFileName = saveFileName ?? throw new ArgumentNullException(nameof(saveFileName));
        }

        public SaveFile(
            ISaveFolder folder,
            FileInfo    fileInfo,
            TData?      data = default
        ) : base(
            folder,
            fileInfo,
            data
        ) {
            _saveFileName = SaveFileName.Parse(fileInfo);
        }
    }
}