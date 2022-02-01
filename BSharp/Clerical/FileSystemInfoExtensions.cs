using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Clerical {
    [PublicAPI]
    public static class FileSystemInfoExtensions {
        #region Uri

        [ContractAnnotation("null => null")]
        [ContractAnnotation("notnull => notnull")]
        public static Uri? ToUri(this FileSystemInfo? fileSystemInfo) {
            return fileSystemInfo switch {
                null              => null,
                DirectoryInfo dir => new Uri(dir.FullName.AppendIfMissing(Path.DirectorySeparatorChar.ToString())),
                FileInfo file     => new Uri(file.FullName),
                _                 => throw new ArgumentException($"Must be a {nameof(FileInfo)} or {nameof(DirectoryInfo)}, but was {fileSystemInfo.GetType().Prettify()}", nameof(fileSystemInfo))
            };
        }

        [ContractAnnotation("null => null")]
        [ContractAnnotation("notnull => notnull")]
        public static Uri? ToUri(this IHasFileSystemInfo? hasFileSystemInfo) {
            return hasFileSystemInfo?.FileSystemInfo.ToUri();
        }

        #endregion

        #region Parent-Child Relationships

        #region IsParentOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        [Pure]
        public static bool IsParentOf(this DirectoryInfo parent, FileSystemInfo child) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            if (child == null) {
                throw new ArgumentNullException(nameof(child));
            }

            return parent.ToUri().IsBaseOf(child.ToUri());
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsParentOf(this DirectoryInfo parent, IHasFileSystemInfo child) => parent.IsParentOf(child.FileSystemInfo);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsParentOf(this IHasDirectoryInfo parent, FileSystemInfo child) => parent.Directory!.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsParentOf(this IHasDirectoryInfo parent, IHasFileSystemInfo child) => parent.IsParentOf(child.FileSystemInfo);

        #endregion

        #region MustBeParentOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf(this DirectoryInfo? parent, FileSystemInfo? child) {
            CheckNull(parent, child);

            if (parent.IsParentOf(child) == false) {
                throw NotMyChildException(parent, child);
            }
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf(this DirectoryInfo? parent, IHasFileSystemInfo? child) => MustBeParentOf(parent, child!.FileSystemInfo);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf(this IHasDirectoryInfo? parent, FileSystemInfo? child) => MustBeParentOf(parent!.Directory!, child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf(this IHasDirectoryInfo? parent, IHasFileSystemInfo? child) => MustBeParentOf(parent!.Directory!, child!.FileSystemInfo);

        #endregion

        #region IsChildOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf(this FileSystemInfo child, DirectoryInfo parent) => parent.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf(this FileSystemInfo child, IHasDirectoryInfo parent) => parent.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf(this IHasFileSystemInfo child, DirectoryInfo parent) => parent.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf(this IHasFileSystemInfo child, IHasDirectoryInfo parent) => parent.IsParentOf(child);

        #endregion

        #region MustBeChildOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf(this FileSystemInfo? child, DirectoryInfo? parent) {
            CheckNull(parent, child);

            if (child.IsChildOf(parent) == false) {
                throw NotMyMomException(child, parent);
            }
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf(this FileSystemInfo? child, IHasDirectoryInfo? parent) => child.MustBeChildOf(parent!.Directory!);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf(this IHasFileSystemInfo? child, DirectoryInfo? parent) => child!.FileSystemInfo.MustBeChildOf(parent);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf(this IHasFileSystemInfo? child, IHasDirectoryInfo? parent) => child.MustBeChildOf(parent!.Directory!);

        #endregion


        public static string GetChildPath(this DirectoryInfo parent, string? relativePath) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            return BPath.JoinPath(parent, relativePath);
        }

        public static FileInfo GetChildFile(this DirectoryInfo parent, string? relativePath) {
            return new FileInfo(parent.GetChildPath(relativePath));
        }

        private static InvalidOperationException NotMyChildException(DirectoryInfo parent, FileSystemInfo child) {
            return new InvalidOperationException(
                $"The {nameof(parent)} {parent.GetType().Prettify()} does not contain the {nameof(child)} {child.GetType().Prettify()}!\n{FamilyString(parent, child)}"
            );
        }

        private static InvalidOperationException NotMyMomException(FileSystemInfo child, DirectoryInfo parent) {
            return new InvalidOperationException(
                $"The {nameof(child)} {child.GetType().Prettify()} isn't contained by the {nameof(parent)} {parent.GetType().Prettify()}!\n{FamilyString(parent, child)}"
            );
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        private static void CheckNull(DirectoryInfo parent, FileSystemInfo child) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent), $"{nameof(parent)} was null, so it cannot possibly contain the {nameof(child)} {child.Prettify()}!");
            }

            if (child == null) {
                throw new ArgumentNullException(nameof(child), $"{nameof(child)} was null, so it cannot possibly be contained by the {nameof(parent)} {parent.Prettify()}!");
            }
        }


        private static string FamilyString(DirectoryInfo parent, FileSystemInfo child) {
            Console.WriteLine(parent.Prettify());

            return new Dictionary<object, object>() {
                [nameof(parent)] = InnerPretty.PrettifyFileSystemInfo(parent),
                [nameof(child)]  = InnerPretty.PrettifyFileSystemInfo(child)
            }.Prettify();
        }

        #endregion

        #region Existential

        public static void MustNotExist(this FileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.Refresh();
            if (fileSystemInfo.Exists) {
                throw ClericalErrors.ItemAlreadyExistsException(fileSystemInfo);
            }
        }

        public static void MustNotExist(this IHasFileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.FileSystemInfo.Refresh();
            if (fileSystemInfo.FileSystemInfo.Exists) {
                throw ClericalErrors.ItemAlreadyExistsException(fileSystemInfo);
            }
        }

        public static void MustExist(this FileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.Refresh();
            if (fileSystemInfo.Exists == false) {
                throw ClericalErrors.ItemDoesNotExistException(fileSystemInfo);
            }
        }

        public static void MustExist(this IHasFileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.FileSystemInfo.Refresh();
            if (fileSystemInfo.FileSystemInfo.Exists == false) {
                throw ClericalErrors.ItemDoesNotExistException(fileSystemInfo);
            }
        }

        #endregion
    }
}