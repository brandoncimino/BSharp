using System.IO;

namespace BrandonUtils.Standalone.Clerical {
    public interface IHasFileSystemInfo {
        public FileSystemInfo FileSystemInfo { get; }
    }
}