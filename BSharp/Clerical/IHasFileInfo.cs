using System.IO;

namespace FowlFever.BSharp.Clerical {
    /// <summary>
    /// Indicates that the implementer contains a meaningful <see cref="FileInfo"/> reference, stored in <see cref="File"/>.
    /// </summary>
    /// <remarks>
    /// "Meaningful" here meaning that the implementer is a wrapper around <see cref="FileInfo"/>, or manages a specific <see cref="FileInfo"/> instance, etc.
    /// </remarks>
    /// <seealso cref="IHasDirectoryInfo"/>
    public interface IHasFileInfo : IHasFileSystemInfo {
        /// <summary>
        /// The <see cref="FileInfo"/> that this object cares about.
        /// </summary>
        /// <remarks>
        /// I named this "File" to match <see cref="FileInfo"/>.<see cref="FileInfo.Directory"/>.
        /// </remarks>

        public FileInfo File { get; }
    }
}