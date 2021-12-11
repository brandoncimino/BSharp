using System.IO;

using BrandonUtils.Standalone.Strings;

namespace FowlFever.BSharp.Clerical {
    public static class ClericalErrors {
        #region Already Exists

        private static IOException _itemAlreadyExistsException(object item) {
            return new IOException($"The {item.GetType().Prettify()} {item.Prettify()} already exists!");
        }

        public static IOException ItemAlreadyExistsException(FileSystemInfo item) {
            return _itemAlreadyExistsException(item);
        }

        public static IOException ItemAlreadyExistsException(IHasFileSystemInfo item) {
            return _itemAlreadyExistsException(item);
        }

        #endregion

        #region Does Not Exist

        private static IOException _itemDoesNotExistException(object item) {
            var message = $"The {item.GetType().Prettify()} {item.Prettify()} does not exist!";

            return item switch {
                FileInfo fi      => new FileNotFoundException(message, fi.FullName),
                DirectoryInfo di => new DirectoryNotFoundException(message),
                _                => new IOException(message)
            };
        }

        public static IOException ItemDoesNotExistException(FileSystemInfo item) {
            return _itemDoesNotExistException(item);
        }

        public static IOException ItemDoesNotExistException(IHasFileSystemInfo item) {
            return _itemAlreadyExistsException(item);
        }

        #endregion
    }
}