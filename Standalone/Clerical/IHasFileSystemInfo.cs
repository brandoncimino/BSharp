using System.IO;

namespace FowlFever.BSharp.Clerical {
    public interface IHasFileSystemInfo {
        public FileSystemInfo FileSystemInfo { get; }
    }
}