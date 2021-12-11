using System.IO;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Clerical {
    /// <summary>
    /// Indicates that the implementer contains a meaningful <see cref="DirectoryInfo"/> reference, stored in <see cref="Directory"/>.
    /// </summary>
    public interface IHasDirectoryInfo : IHasFileSystemInfo {
        /// <summary>
        /// The <see cref="DirectoryInfo"/> that this object cares about.
        /// </summary>
        /// <remarks>
        /// This is named "Directory", and annotated with <see cref="CanBeNullAttribute"/>, in order to match <see cref="FileInfo"/>.<see cref="FileInfo.Directory"/>.
        /// </remarks>

        public DirectoryInfo Directory { get; }
    }
}