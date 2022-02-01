using System.IO;
using System.Linq;

namespace FowlFever.BSharp.Clerical;

public static class DirectoryInfoExtensions {
    /// <param name="directoryInfo">this <see cref="DirectoryInfo"/></param>
    /// <returns><c>true</c> if this <see cref="DirectoryInfo.Exists"/> and <see cref="DirectoryInfo.EnumerateFileSystemInfos()"/> <see cref="CollectionUtils.IsNotEmpty{T}"/></returns>
    public static bool IsNotEmpty(this DirectoryInfo directoryInfo) {
        return directoryInfo.Exists && directoryInfo.EnumerateFileSystemInfos().Any();
    }

    /// <param name="directoryInfo">this <see cref="DirectoryInfo"/></param>
    /// <returns><c>true</c> unless this <see cref="DirectoryInfo.Exists"/> with <see cref="DirectoryInfo.EnumerateFileSystemInfos()"/> <see cref="CollectionUtils.IsNotEmpty{T}"/></returns>
    public static bool IsEmptyOrMissing(this DirectoryInfo directoryInfo) {
        return !directoryInfo.IsNotEmpty();
    }
}