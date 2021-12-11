using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using FowlFever.BSharp.Clerical;

namespace BrandonUtils.Standalone.Clerical {
    /// <summary>
    /// A pass-through wrapper for <see cref="System.IO.FileInfo"/> that essentially makes <see cref="System.IO.FileInfo"/> extensible.
    /// </summary>
    /// <remarks>
    /// This is probably a really really bad idea for reasons far beyond mortal comprehension.
    /// </remarks>
    /// <seealso cref="CustomDirectoryInfo"/>
    public abstract class CustomFileInfo : FileSystemInfo, IHasFileInfo {
        public abstract FileInfo       File           { get; }
        public          FileSystemInfo FileSystemInfo => File;

        /// <summary>Gets the name of the file.</summary>
        /// <returns>The name of the file.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Name?view=netframework-4.7.1">`FileInfo.Name` on docs.microsoft.com</a></footer>
        public override string Name => File.Name;

        /// <summary>Gets the size, in bytes, of the current file.</summary>
        /// <returns>The size of the current file in bytes.</returns>
        /// <exception cref="T:System.IO.IOException">
        /// <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot update the state of the file or directory. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file does not exist.-or- The <see langword="Length" /> property is called for a directory. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Length?view=netframework-4.7.1">`FileInfo.Length` on docs.microsoft.com</a></footer>
        public long Length => File.Length;

        /// <summary>Gets a string representing the directory's full path.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <see langword="null" /> was passed in for the directory name. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.DirectoryName?view=netframework-4.7.1">`FileInfo.DirectoryName` on docs.microsoft.com</a></footer>
        public string DirectoryName => File.DirectoryName;

        /// <summary>Gets an instance of the parent directory.</summary>
        /// <returns>A <see cref="T:System.IO.DirectoryInfo" /> object representing the parent directory of this file.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Directory?view=netframework-4.7.1">`FileInfo.Directory` on docs.microsoft.com</a></footer>
        public DirectoryInfo Directory => File.Directory;

        /// <summary>Gets or sets a value that determines if the current file is read only.</summary>
        /// <returns>
        /// <see langword="true" /> if the current file is read only; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.IO.FileNotFoundException">The file described by the current <see cref="T:System.IO.FileInfo" /> object could not be found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">This operation is not supported on the current platform.-or- The caller does not have the required permission.</exception>
        /// <exception cref="T:System.ArgumentException">The user does not have write permission, but attempted to set this property to <see langword="false" />.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.IsReadOnly?view=netframework-4.7.1">`FileInfo.IsReadOnly` on docs.microsoft.com</a></footer>
        public bool IsReadOnly {
            get => File.IsReadOnly;
            set => File.IsReadOnly = value;
        }

        /// <summary>Creates a <see cref="T:System.IO.StreamReader" /> with UTF8 encoding that reads from an existing text file.</summary>
        /// <returns>A new <see langword="StreamReader" /> with UTF8 encoding.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        /// <paramref name="path" /> is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.OpenText?view=netframework-4.7.1">`FileInfo.OpenText` on docs.microsoft.com</a></footer>
        [SecuritySafeCritical]
        public StreamReader OpenText() => File.OpenText();

        /// <summary>Creates a <see cref="T:System.IO.StreamWriter" /> that writes a new text file.</summary>
        /// <returns>A new <see langword="StreamWriter" />.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The file name is a directory. </exception>
        /// <exception cref="T:System.IO.IOException">The disk is read-only. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.CreateText?view=netframework-4.7.1">`FileInfo.CreateText` on docs.microsoft.com</a></footer>
        public StreamWriter CreateText() => File.CreateText();

        /// <summary>Creates a <see cref="T:System.IO.StreamWriter" /> that appends text to the file represented by this instance of the <see cref="T:System.IO.FileInfo" />.</summary>
        /// <returns>A new <see langword="StreamWriter" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.AppendText?view=netframework-4.7.1">`FileInfo.AppendText` on docs.microsoft.com</a></footer>
        public StreamWriter AppendText() => File.AppendText();

        /// <summary>Copies an existing file to a new file, disallowing the overwriting of an existing file.</summary>
        /// <param name="destFileName">The name of the new file to copy to. </param>
        /// <returns>A new file with a fully qualified path.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="destFileName" /> is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.IO.IOException">An error occurs, or the destination file already exists. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destFileName" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">A directory path is passed in, or the file is being moved to a different drive. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory specified in <paramref name="destFileName" /> does not exist.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="destFileName" /> contains a colon (:) within the string but does not specify the volume. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.CopyTo?view=netframework-4.7.1">`FileInfo.CopyTo` on docs.microsoft.com</a></footer>
        public FileInfo CopyTo(string destFileName) => File.CopyTo(destFileName);

        /// <summary>Copies an existing file to a new file, allowing the overwriting of an existing file.</summary>
        /// <param name="destFileName">The name of the new file to copy to. </param>
        /// <param name="overwrite">
        /// <see langword="true" /> to allow an existing file to be overwritten; otherwise, <see langword="false" />. </param>
        /// <returns>A new file, or an overwrite of an existing file if <paramref name="overwrite" /> is <see langword="true" />. If the file exists and <paramref name="overwrite" /> is <see langword="false" />, an <see cref="T:System.IO.IOException" /> is thrown.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="destFileName" /> is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.IO.IOException">An error occurs, or the destination file already exists and <paramref name="overwrite" /> is <see langword="false" />. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destFileName" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory specified in <paramref name="destFileName" /> does not exist.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">A directory path is passed in, or the file is being moved to a different drive. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="destFileName" /> contains a colon (:) in the middle of the string. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.CopyTo?view=netframework-4.7.1">`FileInfo.CopyTo` on docs.microsoft.com</a></footer>
        public FileInfo CopyTo(string destFileName, bool overwrite) => File.CopyTo(destFileName, overwrite);

        /// <summary>Creates a file.</summary>
        /// <returns>A new file.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Create?view=netframework-4.7.1">`FileInfo.Create` on docs.microsoft.com</a></footer>
        public FileStream Create() => File.Create();

        /// <summary>Permanently deletes a file.</summary>
        /// <exception cref="T:System.IO.IOException">The target file is open or memory-mapped on a computer running Microsoft Windows NT.-or-There is an open handle on the file, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and Files. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The path is a directory. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Delete?view=netframework-4.7.1">`FileInfo.Delete` on docs.microsoft.com</a></footer>
        [SecuritySafeCritical]
        public override void Delete() => File.Delete();

        /// <summary>Decrypts a file that was encrypted by the current account using the <see cref="M:System.IO.FileInfo.Encrypt" /> method.</summary>
        /// <exception cref="T:System.IO.DriveNotFoundException">An invalid drive was specified. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file described by the current <see cref="T:System.IO.FileInfo" /> object could not be found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.NotSupportedException">The file system is not NTFS.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The file described by the current <see cref="T:System.IO.FileInfo" /> object is read-only.-or- This operation is not supported on the current platform.-or- The caller does not have the required permission.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Decrypt?view=netframework-4.7.1">`FileInfo.Decrypt` on docs.microsoft.com</a></footer>
        [ComVisible(false)]
        public void Decrypt() => File.Decrypt();

        /// <summary>Encrypts a file so that only the account used to encrypt the file can decrypt it.</summary>
        /// <exception cref="T:System.IO.DriveNotFoundException">An invalid drive was specified. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file described by the current <see cref="T:System.IO.FileInfo" /> object could not be found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.NotSupportedException">The file system is not NTFS.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The file described by the current <see cref="T:System.IO.FileInfo" /> object is read-only.-or- This operation is not supported on the current platform.-or- The caller does not have the required permission.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Encrypt?view=netframework-4.7.1">`FileInfo.Encrypt` on docs.microsoft.com</a></footer>
        [ComVisible(false)]
        public void Encrypt() => File.Encrypt();

        /// <summary>Gets a value indicating whether a file exists.</summary>
        /// <returns>
        /// <see langword="true" /> if the file exists; <see langword="false" /> if the file does not exist or if the file is a directory.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Exists?view=netframework-4.7.1">`FileInfo.Exists` on docs.microsoft.com</a></footer>
        public override bool Exists => File.Exists;

        /// <summary>Opens a file in the specified mode.</summary>
        /// <param name="mode">A <see cref="T:System.IO.FileMode" /> constant specifying the mode (for example, <see langword="Open" /> or <see langword="Append" />) in which to open the file. </param>
        /// <returns>A file opened in the specified mode, with read/write access and unshared.</returns>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The file is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Open?view=netframework-4.7.1">`FileInfo.Open` on docs.microsoft.com</a></footer>
        public FileStream Open(FileMode mode) => File.Open(mode);

        /// <summary>Opens a file in the specified mode with read, write, or read/write access.</summary>
        /// <param name="mode">A <see cref="T:System.IO.FileMode" /> constant specifying the mode (for example, <see langword="Open" /> or <see langword="Append" />) in which to open the file. </param>
        /// <param name="access">A <see cref="T:System.IO.FileAccess" /> constant specifying whether to open the file with <see langword="Read" />, <see langword="Write" />, or <see langword="ReadWrite" /> file access. </param>
        /// <returns>A <see cref="T:System.IO.FileStream" /> object opened in the specified mode and access, and unshared.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        /// <paramref name="path" /> is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Open?view=netframework-4.7.1">`FileInfo.Open` on docs.microsoft.com</a></footer>
        public FileStream Open(FileMode mode, FileAccess access) => File.Open(mode, access);

        /// <summary>Opens a file in the specified mode with read, write, or read/write access and the specified sharing option.</summary>
        /// <param name="mode">A <see cref="T:System.IO.FileMode" /> constant specifying the mode (for example, <see langword="Open" /> or <see langword="Append" />) in which to open the file. </param>
        /// <param name="access">A <see cref="T:System.IO.FileAccess" /> constant specifying whether to open the file with <see langword="Read" />, <see langword="Write" />, or <see langword="ReadWrite" /> file access. </param>
        /// <param name="share">A <see cref="T:System.IO.FileShare" /> constant specifying the type of access other <see langword="FileStream" /> objects have to this file. </param>
        /// <returns>A <see cref="T:System.IO.FileStream" /> object opened with the specified mode, access, and sharing options.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        /// <paramref name="path" /> is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Open?view=netframework-4.7.1">`FileInfo.Open` on docs.microsoft.com</a></footer>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share) => File.Open(mode, access, share);

        /// <summary>Creates a read-only <see cref="T:System.IO.FileStream" />.</summary>
        /// <returns>A new read-only <see cref="T:System.IO.FileStream" /> object.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        /// <paramref name="path" /> is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.OpenRead?view=netframework-4.7.1">`FileInfo.OpenRead` on docs.microsoft.com</a></footer>
        public FileStream OpenRead() => File.OpenRead();

        /// <summary>Creates a write-only <see cref="T:System.IO.FileStream" />.</summary>
        /// <returns>A write-only unshared <see cref="T:System.IO.FileStream" /> object for a new or existing file.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The path specified when creating an instance of the <see cref="T:System.IO.FileInfo" /> object is read-only or is a directory.  </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified when creating an instance of the <see cref="T:System.IO.FileInfo" /> object is invalid, such as being on an unmapped drive. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.OpenWrite?view=netframework-4.7.1">`FileInfo.OpenWrite` on docs.microsoft.com</a></footer>
        public FileStream OpenWrite() => File.OpenWrite();

        /// <summary>Moves a specified file to a new location, providing the option to specify a new file name.</summary>
        /// <param name="destFileName">The path to move the file to, which can specify a different file name. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs, such as the destination file already exists or the destination device is not ready. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destFileName" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="destFileName" /> is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        /// <paramref name="destFileName" /> is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="destFileName" /> contains a colon (:) in the middle of the string. </exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.MoveTo?view=netframework-4.7.1">`FileInfo.MoveTo` on docs.microsoft.com</a></footer>
        [SecuritySafeCritical]
        public void MoveTo(string destFileName) => File.MoveTo(destFileName);

        /// <summary>Replaces the contents of a specified file with the file described by the current <see cref="T:System.IO.FileInfo" /> object, deleting the original file, and creating a backup of the replaced file.</summary>
        /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
        /// <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the <paramref name="destFileName" /> parameter.</param>
        /// <returns>A <see cref="T:System.IO.FileInfo" /> object that encapsulates information about the file described by the <paramref name="destFileName" /> parameter.</returns>
        /// <exception cref="T:System.ArgumentException">The path described by the <paramref name="destFileName" /> parameter was not of a legal form.-or-The path described by the <paramref name="destBackupFileName" /> parameter was not of a legal form.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destFileName" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file described by the current <see cref="T:System.IO.FileInfo" /> object could not be found.-or-The file described by the <paramref name="destinationFileName" /> parameter could not be found. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Replace?view=netframework-4.7.1">`FileInfo.Replace` on docs.microsoft.com</a></footer>
        [ComVisible(false)]
        public FileInfo Replace(string destinationFileName, string destinationBackupFileName) => File.Replace(destinationFileName, destinationBackupFileName);

        /// <summary>Replaces the contents of a specified file with the file described by the current <see cref="T:System.IO.FileInfo" /> object, deleting the original file, and creating a backup of the replaced file.  Also specifies whether to ignore merge errors. </summary>
        /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
        /// <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the <paramref name="destFileName" /> parameter.</param>
        /// <param name="ignoreMetadataErrors">
        /// <see langword="true" /> to ignore merge errors (such as attributes and ACLs) from the replaced file to the replacement file; otherwise <see langword="false" />. </param>
        /// <returns>A <see cref="T:System.IO.FileInfo" /> object that encapsulates information about the file described by the <paramref name="destFileName" /> parameter.</returns>
        /// <exception cref="T:System.ArgumentException">The path described by the <paramref name="destFileName" /> parameter was not of a legal form.-or-The path described by the <paramref name="destBackupFileName" /> parameter was not of a legal form.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destFileName" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file described by the current <see cref="T:System.IO.FileInfo" /> object could not be found.-or-The file described by the <paramref name="destinationFileName" /> parameter could not be found. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.Replace?view=netframework-4.7.1">`FileInfo.Replace` on docs.microsoft.com</a></footer>
        [ComVisible(false)]
        public FileInfo Replace(
            string destinationFileName,
            string destinationBackupFileName,
            bool   ignoreMetadataErrors
        ) => File.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors);

        /// <summary>Returns the path as a string.</summary>
        /// <returns>A string representing the path.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.ToString?view=netframework-4.7.1">`FileInfo.ToString` on docs.microsoft.com</a></footer>
        public override string ToString() => File.ToString();
    }
}