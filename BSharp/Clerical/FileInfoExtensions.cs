using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FowlFever.BSharp.Clerical {
    [PublicAPI]
    public static class FileInfoExtensions {
        public const string BackupExtension = ".bak";

        /// <summary>
        /// An extension method made to mimic Powershell's <see cref="FileInfo"/>.<c>BaseName</c> <a href="https://docs.microsoft.com/en-us/powershell/scripting/developer/cmdlet/extending-properties-for-objects?view=powershell-7.1#script-properties">Script Property</a>.
        /// </summary>
        /// <param name="fileSystemInfo"></param>
        /// <returns></returns>
        public static string BaseName(this FileSystemInfo fileSystemInfo) {
            return Path.GetFileNameWithoutExtension(fileSystemInfo.Name);
        }

        #region BPath Extensions

        public static string[] Extensions(this FileSystemInfo fileSystemInfo) {
            return BPath.GetExtensions(fileSystemInfo.Name);
        }

        public static string FullExtension(this FileSystemInfo fileSystemInfo) {
            return BPath.GetFullExtension(fileSystemInfo.Name);
        }

        public static string FileNameWithoutExtensions(this FileSystemInfo fileSystemInfo) {
            return BPath.GetFileNameWithoutExtensions(fileSystemInfo.Name);
        }

        #endregion

        #region System.IO.File Extensions

        #region CreateDirectory

        /// <summary>
        /// Creates <paramref name="fileInfo"/>'s <see cref="FileInfo.Directory"/>.
        /// </summary>
        /// <remarks>
        /// The only difference between <see cref="CreateDirectory"/> and <see cref="EnsureDirectory"/> is what they return:
        /// <ul>
        /// <li><see cref="CreateDirectory"/> → <paramref name="fileInfo"/>.<see cref="FileInfo.Directory"/></li>
        /// <li><see cref="EnsureDirectory"/> → <paramref name="fileInfo"/></li>
        /// </ul>
        /// </remarks>
        /// <param name="fileInfo">a <see cref="FileInfo"/></param>
        /// <returns><paramref name="fileInfo"/>'s <see cref="FileInfo.Directory"/>, whether or not it already existed.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="fileInfo"/> is null</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="fileInfo"/>.<see cref="Directory"/> is null</exception>
        /// <exception cref="IOException">if the <see cref="DirectoryInfo"/> couldn't be created</exception>
        [ContractAnnotation("null => stop")]
        public static DirectoryInfo CreateDirectory(this FileInfo fileInfo) {
            if (fileInfo == null) {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            var dir = fileInfo.Directory ?? throw new ArgumentNullException(nameof(fileInfo.Directory), $"The {fileInfo.GetType().Prettify()}.{nameof(fileInfo.Directory)} was null, so we couldn't create it! ({nameof(fileInfo.FullName)}: \"{fileInfo.FullName}\")");
            return Directory.CreateDirectory(dir.FullName);
        }

        /// <inheritdoc cref="CreateDirectory"/>
        /// <returns>this <paramref name="fileInfo"/>, for method chaining</returns>
        [ContractAnnotation("null => stop")]
        private static FileInfo EnsureDirectory(this FileInfo fileInfo) {
            fileInfo.CreateDirectory();
            return fileInfo;
        }

        #endregion

        #region Read

        /// <seealso cref="File.ReadAllLines(string)"/>
        public static string[] ReadLines(this FileInfo fileInfo) => File.ReadAllLines(fileInfo.FullName);

        /// <seealso cref="File.ReadAllLines(string,Encoding)"/>
        public static string[] ReadLines(this FileInfo fileInfo, Encoding encoding) => File.ReadAllLines(fileInfo.FullName, encoding);

        /// <seealso cref="File.ReadAllText(string)"/>
        public static string Read(this FileInfo fileInfo) => File.ReadAllText(fileInfo.FullName);

        /// <seealso cref="File.ReadAllText(string, Encoding)"/>
        public static string Read(this FileInfo fileInfo, Encoding encoding) => File.ReadAllText(fileInfo.FullName, encoding);

        #endregion

        #region Write

        /// <summary>
        /// A nice wrapper for <see cref="File.WriteAllText(string,string)"/>.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around a static <see cref="File"/> method that:
        /// <ul>
        /// <li>Creates <paramref name="fileInfo"/>'s <see cref="FileInfo.Directory"/> if necessary</li>
        /// <li>Uses this <see cref="FileInfo"/>'s <see cref="FileSystemInfo.FullName"/> as the path</li>
        /// <li>Returns this <see cref="FileInfo"/>, for method chaining</li>
        /// </ul>
        /// </remarks>
        /// <param name="fileInfo">the <see cref="FileInfo"/> being written to</param>
        /// <param name="contents">the text being written to <paramref name="fileInfo"/></param>
        /// <returns>this <see cref="FileInfo"/>, for method chaining</returns>
        public static FileInfo Write(this FileInfo fileInfo, string contents) {
            fileInfo.EnsureDirectory();
            File.WriteAllText(fileInfo.FullName, contents);
            fileInfo.Refresh();
            return fileInfo;
        }

        /// <summary>
        /// A nice wrapper for <see cref="File.WriteAllText(string,string,Encoding)"/>.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="Write(System.IO.FileInfo,string)"/>
        /// </remarks>
        /// <param name="fileInfo"><inheritdoc cref="Write(System.IO.FileInfo,string)"/></param>
        /// <param name="contents"><inheritdoc cref="Write(System.IO.FileInfo,string)"/></param>
        /// <param name="encoding">an optional text <see cref="Encoding"/></param>
        /// <returns><inheritdoc cref="Write(System.IO.FileInfo,string)"/></returns>
        public static FileInfo Write(this FileInfo fileInfo, string contents, Encoding encoding) {
            fileInfo.CreateDirectory();
            File.WriteAllText(fileInfo.FullName, contents, encoding);
            fileInfo.Refresh();
            return fileInfo;
        }

        /// <summary>
        /// A nice wrapper for <see cref="File.WriteAllLines(string,System.Collections.Generic.IEnumerable{string})"/>
        /// </summary>
        /// <param name="fileInfo"><inheritdoc cref="Write(System.IO.FileInfo,string)"/></param>
        /// <param name="lines">the <see cref="string"/>s that will be written to <paramref name="fileInfo"/> on <b>separate lines</b></param>
        /// <returns><inheritdoc cref="Write(System.IO.FileInfo,string)"/></returns>
        public static FileInfo WriteLines(this FileInfo fileInfo, IEnumerable<string> lines) {
            fileInfo.CreateDirectory();
            File.WriteAllLines(fileInfo.FullName, lines);
            fileInfo.Refresh();
            return fileInfo;
        }

        /**
         * <inheritdoc cref="WriteLines(System.IO.FileInfo,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static FileInfo WriteLines(this FileInfo fileInfo, IEnumerable<string> lines, Encoding encoding) {
            fileInfo.CreateDirectory();
            File.WriteAllLines(fileInfo.FullName, lines);
            fileInfo.Refresh();
            return fileInfo;
        }

        #region Append

        /// <seealso cref="File.AppendAllText(string,string)"/>
        public static void Append(this FileInfo fileInfo, string contents) => File.AppendAllText(fileInfo.FullName, contents);

        /// <seealso cref="File.AppendAllText(string,string, Encoding)"/>
        public static void Append(this FileInfo fileInfo, string contents, Encoding encoding) => File.AppendAllText(fileInfo.FullName, contents, encoding);

        /// <seealso cref="File.AppendAllLines(string,System.Collections.Generic.IEnumerable{string})"/>
        public static void AppendLines(this FileInfo fileInfo, IEnumerable<string> lines) => File.AppendAllLines(fileInfo.FullName, lines);

        /// <seealso cref="File.AppendAllLines(string,System.Collections.Generic.IEnumerable{string}, Encoding)"/>
        public static void AppendLines(this FileInfo fileInfo, IEnumerable<string> lines, Encoding encoding) => File.AppendAllLines(fileInfo.FullName, lines, encoding);

        #endregion

        #endregion

        #endregion

        #region Backup

        /// <summary>
        /// If this <see cref="FileInfo.Exists"/>, create a copy of it with <see cref="BackupExtension"/> appended to the <see cref="FileSystemInfo.FullName"/>.
        /// </summary>
        /// <param name="fileInfo">The original <see cref="FileInfo"/> that will be copied</param>
        /// <param name="overwrite">
        /// <see langword="true" /> to allow an existing <b>backup</b> file to be overwritten; otherwise, <see langword="false" />. </param>
        /// <returns>A new file, or an overwrite of an existing file if <paramref name="overwrite" /> is <see langword="true" />. If the file exists and <paramref name="overwrite" /> is <see langword="false" />, an <see cref="T:System.IO.IOException" /> is thrown.</returns>
        /// <exception cref="ArgumentNullException">This <paramref name="fileInfo"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.IOException">An error occurs, or the destination file already exists and <paramref name="overwrite" /> is <see langword="false" />. </exception>
        public static FileInfo? Backup(this FileInfo fileInfo, bool overwrite = false) {
            if (fileInfo == null) {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            fileInfo.Refresh();
            return fileInfo.Exists ? fileInfo.CopyTo($"{fileInfo.FullName}{BackupExtension}", overwrite) : null;
        }

        #endregion

        #region System.Text.Json Extensions

        /// <summary>
        /// <see cref="Read(System.IO.FileInfo)"/>s a <paramref name="fileInfo"/> and then <see cref="JsonSerializer.Deserialize{TValue}(System.Text.Json.JsonDocument,System.Text.Json.JsonSerializerOptions?)"/>s its content.
        /// </summary>
        /// <param name="fileInfo">the <see cref="FileInfo"/> being <see cref="Read(System.IO.FileInfo)"/> from</param>
        /// <param name="options">optional <see cref="JsonSerializerOptions"/></param>
        /// <typeparam name="T">the <see cref="Type"/> that the content will be deserialized as</typeparam>
        /// <returns>a new <typeparamref name="T"/> instance</returns>
        public static T? Deserialize<T>(
            this FileInfo          fileInfo,
            JsonSerializerOptions? options = default
        ) {
            fileInfo.MustExist();
            return JsonSerializer.Deserialize<T>(
                fileInfo.Read(),
                options
            );
        }

        public static void Serialize<T>(
            this FileInfo           fileInfo,
            T?                      obj,
            DuplicateFileResolution duplicateFileResolution = DuplicateFileResolution.Error,
            JsonSerializerOptions? options                = default
        ) {
            switch (duplicateFileResolution) {
                case DuplicateFileResolution.Error:
                    fileInfo.SerializeCautiously(obj, options);
                    break;
                case DuplicateFileResolution.Overwrite:
                    fileInfo.SerializeForcefully(obj, options);
                    break;
                case DuplicateFileResolution.Backup:
                    fileInfo.SerializeWithBackup(obj, options);
                    break;
                default:
                    throw BEnum.InvalidEnumArgumentException(nameof(duplicateFileResolution), duplicateFileResolution);
            }

            Console.WriteLine($"💾 → {fileInfo.ToUri()}");
        }

        public static void SerializeCautiously<T>(
            this FileInfo          fileInfo,
            T?                     obj,
            JsonSerializerOptions? options = default
        ) {
            fileInfo.MustNotExist();
            fileInfo.Write(JsonSerializer.Serialize(obj, options));
        }

        public static void SerializeForcefully<T>(
            this FileInfo           fileInfo,
            T?                      obj,
            JsonSerializerOptions? options = default
        ) {
            fileInfo.Write(JsonSerializer.Serialize(obj, options));
        }

        public static void SerializeWithBackup<T>(
            this FileInfo           fileInfo,
            T?                      obj,
            JsonSerializerOptions? options = default
        ) {
            fileInfo.Backup(true);
            fileInfo.SerializeForcefully(obj, options);
        }

        #endregion

        #region Matters of Size

        /// <summary>
        /// Gets the <see cref="FileInfo.Length"/>, in bytes, <b>IF</b> this <see cref="FileInfo.Exists"/>.
        /// Otherwise, returns <c>null</c>.
        /// </summary>
        /// <param name="fileInfo">this <see cref="FileInfo"/></param>
        /// <returns>Gets the <see cref="FileInfo.Length"/>, in bytes, <b>IF</b> this <see cref="FileInfo.Exists"/></returns>
        public static long? Size(this FileInfo fileInfo) {
            return fileInfo.Exists ? fileInfo.Length : null;
        }

        #endregion

        /// <param name="fileInfo">this <see cref="FileInfo"/></param>
        /// <returns><c>true</c> if this <see cref="FileInfo.Exists"/> and has a <see cref="FileInfo.Length"/> > 0</returns>
        public static bool IsNotEmpty(this FileInfo fileInfo) {
            return fileInfo is { Exists: true, Length: > 0 };
        }

        /// <param name="fileInfo">this <see cref="FileInfo"/></param>
        /// <returns><c>true</c> if this doesn't <see cref="FileInfo.Exists"/> with a <see cref="FileInfo.Length"/> > 0</returns>
        public static bool IsEmptyOrMissing(this FileInfo fileInfo) {
            return !fileInfo.IsNotEmpty();
        }
    }
}