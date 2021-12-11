using System.Collections.Generic;
using System.IO;
using System.Security;

namespace FowlFever.BSharp.Clerical {
    /// <summary>
    /// A pass-through wrapper around <see cref="System.IO.DirectoryInfo"/> that essentially allows <see cref="System.IO.DirectoryInfo"/> to be inherited.
    /// </summary>
    /// <remarks>
    /// This is probably a really really bad idea for reasons far beyond mortal comprehension.
    /// </remarks>
    /// <seealso cref="CustomFileInfo"/>
    public abstract class CustomDirectoryInfo : FileSystemInfo, IHasDirectoryInfo {
        public abstract DirectoryInfo  Directory      { get; }
        public          FileSystemInfo FileSystemInfo => Directory;

        /// <summary>Gets the name of this <see cref="T:System.IO.DirectoryInfo" /> instance.</summary>
        /// <returns>The directory name.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Name?view=netframework-4.7.1">`DirectoryInfo.Name` on docs.microsoft.com</a></footer>
        public override string Name => Directory.Name;

        /// <summary>Gets the full path of the directory.</summary>
        /// <returns>A string containing the full path.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.FullName?view=netframework-4.7.1">`DirectoryInfo.FullName` on docs.microsoft.com</a></footer>
        public override string FullName => Directory.FullName;

        /// <summary>Gets the parent directory of a specified subdirectory.</summary>
        /// <returns>The parent directory, or <see langword="null" /> if the path is null or if the file path denotes a root (such as "\", "C:", or * "\\server\share").</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Parent?view=netframework-4.7.1">`DirectoryInfo.Parent` on docs.microsoft.com</a></footer>
        public DirectoryInfo Parent => Directory.Parent;

        /// <summary>Creates a subdirectory or subdirectories on the specified path. The specified path can be relative to this instance of the <see cref="T:System.IO.DirectoryInfo" /> class.</summary>
        /// <param name="path">The specified path. This cannot be a different disk volume or Universal Naming Convention (UNC) name. </param>
        /// <returns>The last directory specified in <paramref name="path" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> does not specify a valid file path or contains invalid <see langword="DirectoryInfo" /> characters. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The subdirectory cannot be created.-or- A file or directory already has the name specified by <paramref name="path" />. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. The specified path, file name, or both are too long.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have code access permission to create the directory.-or-The caller does not have code access permission to read the directory described by the returned <see cref="T:System.IO.DirectoryInfo" /> object.  This can occur when the <paramref name="path" /> parameter describes an existing directory.</exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.CreateSubdirectory?view=netframework-4.7.1">`DirectoryInfo.CreateSubdirectory` on docs.microsoft.com</a></footer>
        public DirectoryInfo CreateSubdirectory(string path) => Directory.CreateSubdirectory(path);

        /// <summary>Creates a directory.</summary>
        /// <exception cref="T:System.IO.IOException">The directory cannot be created. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Create?view=netframework-4.7.1">`DirectoryInfo.Create` on docs.microsoft.com</a></footer>
        public void Create() => Directory.Create();

        /// <summary>Gets a value indicating whether the directory exists.</summary>
        /// <returns>
        /// <see langword="true" /> if the directory exists; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Exists?view=netframework-4.7.1">`DirectoryInfo.Exists` on docs.microsoft.com</a></footer>
        public override bool Exists => Directory.Exists;

        /// <summary>Returns a file list from the current directory matching the given search pattern.</summary>
        /// <param name="searchPattern">The search string to match against the names of files.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns>An array of type <see cref="T:System.IO.FileInfo" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetFiles?view=netframework-4.7.1">`DirectoryInfo.GetFiles` on docs.microsoft.com</a></footer>
        public FileInfo[] GetFiles(string searchPattern) => Directory.GetFiles(searchPattern);

        /// <summary>Returns a file list from the current directory matching the given search pattern and using a value to determine whether to search subdirectories.</summary>
        /// <param name="searchPattern">The search string to match against the names of files.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories.</param>
        /// <returns>An array of type <see cref="T:System.IO.FileInfo" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetFiles?view=netframework-4.7.1">`DirectoryInfo.GetFiles` on docs.microsoft.com</a></footer>
        public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption) => Directory.GetFiles(searchPattern, searchOption);

        /// <summary>Returns a file list from the current directory.</summary>
        /// <returns>An array of type <see cref="T:System.IO.FileInfo" />.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid, such as being on an unmapped drive. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetFiles?view=netframework-4.7.1">`DirectoryInfo.GetFiles` on docs.microsoft.com</a></footer>
        public FileInfo[] GetFiles() => Directory.GetFiles();

        /// <summary>Returns the subdirectories of the current directory.</summary>
        /// <returns>An array of <see cref="T:System.IO.DirectoryInfo" /> objects.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetDirectories?view=netframework-4.7.1">`DirectoryInfo.GetDirectories` on docs.microsoft.com</a></footer>
        public DirectoryInfo[] GetDirectories() => Directory.GetDirectories();

        /// <summary>Retrieves an array of strongly typed <see cref="T:System.IO.FileSystemInfo" /> objects representing the files and subdirectories that match the specified search criteria.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories and files.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns>An array of strongly typed <see langword="FileSystemInfo" /> objects matching the search criteria.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetFileSystemInfos?view=netframework-4.7.1">`DirectoryInfo.GetFileSystemInfos` on docs.microsoft.com</a></footer>
        public FileSystemInfo[] GetFileSystemInfos(string searchPattern) => Directory.GetFileSystemInfos(searchPattern);

        /// <summary>Retrieves an array of <see cref="T:System.IO.FileSystemInfo" /> objects that represent the files and subdirectories matching the specified search criteria.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories and files.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.</param>
        /// <returns>An array of file system entries that match the search criteria.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetFileSystemInfos?view=netframework-4.7.1">`DirectoryInfo.GetFileSystemInfos` on docs.microsoft.com</a></footer>
        public FileSystemInfo[] GetFileSystemInfos(
            string       searchPattern,
            SearchOption searchOption
        ) => Directory.GetFileSystemInfos(searchPattern, searchOption);

        /// <summary>Returns an array of strongly typed <see cref="T:System.IO.FileSystemInfo" /> entries representing all the files and subdirectories in a directory.</summary>
        /// <returns>An array of strongly typed <see cref="T:System.IO.FileSystemInfo" /> entries.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid (for example, it is on an unmapped drive). </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetFileSystemInfos?view=netframework-4.7.1">`DirectoryInfo.GetFileSystemInfos` on docs.microsoft.com</a></footer>
        public FileSystemInfo[] GetFileSystemInfos() => Directory.GetFileSystemInfos();

        /// <summary>Returns an array of directories in the current <see cref="T:System.IO.DirectoryInfo" /> matching the given search criteria.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns>An array of type <see langword="DirectoryInfo" /> matching <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see langword="DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetDirectories?view=netframework-4.7.1">`DirectoryInfo.GetDirectories` on docs.microsoft.com</a></footer>
        public DirectoryInfo[] GetDirectories(string searchPattern) => Directory.GetDirectories(searchPattern);

        /// <summary>Returns an array of directories in the current <see cref="T:System.IO.DirectoryInfo" /> matching the given search criteria and using a value to determine whether to search subdirectories.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories.</param>
        /// <returns>An array of type <see langword="DirectoryInfo" /> matching <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see langword="DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.GetDirectories?view=netframework-4.7.1">`DirectoryInfo.GetDirectories` on docs.microsoft.com</a></footer>
        public DirectoryInfo[] GetDirectories(
            string       searchPattern,
            SearchOption searchOption
        ) => Directory.GetDirectories(searchPattern, searchOption);

        /// <summary>Returns an enumerable collection of directory information in the current directory.</summary>
        /// <returns>An enumerable collection of directories in the current directory.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateDirectories?view=netframework-4.7.1">`DirectoryInfo.EnumerateDirectories` on docs.microsoft.com</a></footer>
        public IEnumerable<DirectoryInfo> EnumerateDirectories() => Directory.EnumerateDirectories();

        /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns>An enumerable collection of directories that matches <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateDirectories?view=netframework-4.7.1">`DirectoryInfo.EnumerateDirectories` on docs.microsoft.com</a></footer>
        public IEnumerable<DirectoryInfo> EnumerateDirectories(
            string searchPattern
        ) => Directory.EnumerateDirectories(searchPattern);

        /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option. </summary>
        /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.</param>
        /// <returns>An enumerable collection of directories that matches <paramref name="searchPattern" /> and <paramref name="searchOption" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateDirectories?view=netframework-4.7.1">`DirectoryInfo.EnumerateDirectories` on docs.microsoft.com</a></footer>
        public IEnumerable<DirectoryInfo> EnumerateDirectories(
            string       searchPattern,
            SearchOption searchOption
        ) => Directory.EnumerateDirectories(searchPattern, searchOption);

        /// <summary>Returns an enumerable collection of file information in the current directory.</summary>
        /// <returns>An enumerable collection of the files in the current directory.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateFiles?view=netframework-4.7.1">`DirectoryInfo.EnumerateFiles` on docs.microsoft.com</a></footer>
        public IEnumerable<FileInfo> EnumerateFiles() => Directory.EnumerateFiles();

        /// <summary>Returns an enumerable collection of file information that matches a search pattern.</summary>
        /// <param name="searchPattern">The search string to match against the names of files.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns>An enumerable collection of files that matches <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateFiles?view=netframework-4.7.1">`DirectoryInfo.EnumerateFiles` on docs.microsoft.com</a></footer>
        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern) => Directory.EnumerateFiles(searchPattern);

        /// <summary>Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.</summary>
        /// <param name="searchPattern">The search string to match against the names of files.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.</param>
        /// <returns>An enumerable collection of files that matches <paramref name="searchPattern" /> and <paramref name="searchOption" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateFiles?view=netframework-4.7.1">`DirectoryInfo.EnumerateFiles` on docs.microsoft.com</a></footer>
        public IEnumerable<FileInfo> EnumerateFiles(
            string       searchPattern,
            SearchOption searchOption
        ) => Directory.EnumerateFiles(searchPattern, searchOption);

        /// <summary>Returns an enumerable collection of file system information in the current directory.</summary>
        /// <returns>An enumerable collection of file system information in the current directory. </returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateFileSystemInfos?view=netframework-4.7.1">`DirectoryInfo.EnumerateFileSystemInfos` on docs.microsoft.com</a></footer>
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() => Directory.EnumerateFileSystemInfos();

        /// <summary>Returns an enumerable collection of file system information that matches a specified search pattern.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns>An enumerable collection of file system information objects that matches <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateFileSystemInfos?view=netframework-4.7.1">`DirectoryInfo.EnumerateFileSystemInfos` on docs.microsoft.com</a></footer>
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(
            string searchPattern
        ) => Directory.EnumerateFileSystemInfos(searchPattern);

        /// <summary>Returns an enumerable collection of file system information that matches a specified search pattern and search subdirectory option.</summary>
        /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.</param>
        /// <returns>An enumerable collection of file system information objects that matches <paramref name="searchPattern" /> and <paramref name="searchOption" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.EnumerateFileSystemInfos?view=netframework-4.7.1">`DirectoryInfo.EnumerateFileSystemInfos` on docs.microsoft.com</a></footer>
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(
            string       searchPattern,
            SearchOption searchOption
        ) => Directory.EnumerateFileSystemInfos(searchPattern, searchOption);

        /// <summary>Gets the root portion of the directory.</summary>
        /// <returns>An object that represents the root of the directory.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Root?view=netframework-4.7.1">`DirectoryInfo.Root` on docs.microsoft.com</a></footer>
        public DirectoryInfo Root => Directory.Root;

        /// <summary>Moves a <see cref="T:System.IO.DirectoryInfo" /> instance and its contents to a new path.</summary>
        /// <param name="destDirName">The name and path to which to move this directory. The destination cannot be another disk volume or a directory with the identical name. It can be an existing directory to which you want to add this directory as a subdirectory. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destDirName" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="destDirName" /> is an empty string (''"). </exception>
        /// <exception cref="T:System.IO.IOException">An attempt was made to move a directory to a different volume. -or-
        /// <paramref name="destDirName" /> already exists.-or-You are not authorized to access this path.-or- The directory being moved and the destination directory have the same name.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The destination directory cannot be found.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.MoveTo?view=netframework-4.7.1">`DirectoryInfo.MoveTo` on docs.microsoft.com</a></footer>
        [SecuritySafeCritical]
        public void MoveTo(string destDirName) => Directory.MoveTo(destDirName);

        /// <summary>Deletes this <see cref="T:System.IO.DirectoryInfo" /> if it is empty.</summary>
        /// <exception cref="T:System.UnauthorizedAccessException">The directory contains a read-only file.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory described by this <see cref="T:System.IO.DirectoryInfo" /> object does not exist or could not be found.</exception>
        /// <exception cref="T:System.IO.IOException">The directory is not empty. -or-The directory is the application's current working directory.-or-There is an open handle on the directory, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories. For more information, see How to: Enumerate Directories and Files.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Delete?view=netframework-4.7.1">`DirectoryInfo.Delete` on docs.microsoft.com</a></footer>
        [SecuritySafeCritical]
        public override void Delete() => Directory.Delete();

        /// <summary>Deletes this instance of a <see cref="T:System.IO.DirectoryInfo" />, specifying whether to delete subdirectories and files.</summary>
        /// <param name="recursive">
        /// <see langword="true" /> to delete this directory, its subdirectories, and all files; otherwise, <see langword="false" />. </param>
        /// <exception cref="T:System.UnauthorizedAccessException">The directory contains a read-only file.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory described by this <see cref="T:System.IO.DirectoryInfo" /> object does not exist or could not be found.</exception>
        /// <exception cref="T:System.IO.IOException">The directory is read-only.-or- The directory contains one or more files or subdirectories and <paramref name="recursive" /> is <see langword="false" />.-or-The directory is the application's current working directory. -or-There is an open handle on the directory or on one of its files, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and Files.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.Delete?view=netframework-4.7.1">`DirectoryInfo.Delete` on docs.microsoft.com</a></footer>
        [SecuritySafeCritical]
        public void Delete(bool recursive) => Directory.Delete(recursive);

        /// <summary>Returns the original path that was passed by the user.</summary>
        /// <returns>Returns the original path that was passed by the user.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo.ToString?view=netframework-4.7.1">`DirectoryInfo.ToString` on docs.microsoft.com</a></footer>
        public override string ToString() => Directory.ToString();
    }
}