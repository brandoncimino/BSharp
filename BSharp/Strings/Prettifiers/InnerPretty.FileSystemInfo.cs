using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string PrettifyFileSystemInfo(FileSystemInfo item, PrettificationSettings? settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            return settings.PreferredLineStyle == LineStyle.Single
                       ? PrettifySingleItem(item, settings)
                       : PrettifyWithChildren(item, settings).JoinLines();
        }


        private static IEnumerable<string> SummarizeChildren(DirectoryInfo directoryInfo) {
            var childCount = directoryInfo.EnumerateFileSystemInfos().Count();
            return new[] { $"{StringUtils.Ellipsis}({childCount} children)" };
        }


        private static IEnumerable<string> EnumerateChildren(DirectoryInfo dir, PrettificationSettings? settings, int depthLimit, int currentDepth) {
            return dir.EnumerateFileSystemInfos()
                      .SelectMany(it => PrettifyWithChildren(it, settings, depthLimit, currentDepth));
        }


        private static string GetIcon(FileSystemInfo item) {
            return item switch {
                FileInfo _      => BPath.FileIcon,
                DirectoryInfo _ => BPath.ClosedFolderIcon,
                _               => throw new ArgumentException(nameof(item))
            };
        }

        private static string PrettifySingleItem(FileSystemInfo item, PrettificationSettings settings, int currentDepth = 0) {
            var itemName = currentDepth == 0 ? item.ToUri().ToString() : item.Name;
            return itemName.Prefix(GetIcon(item), " ");
        }


        private static IEnumerable<string> PrettifyWithChildren(FileSystemInfo item, PrettificationSettings? settings, [NonNegativeValue] int depthLimit = 2, int currentDepth = 0) {
            /*
             * 📂 Folder
             *   | File
             *   | File
             *  📂 Folder
             *   …(5 children)
             */
            IEnumerable<string> lines;
            var                 itemName = currentDepth == 0 ? item.ToUri().ToString() : item.Name;

            if (item is DirectoryInfo dir) {
                var directoryName = itemName.Prefix(BPath.ClosedFolderIcon, " ");
                var children      = currentDepth >= depthLimit ? SummarizeChildren(dir) : EnumerateChildren(dir, settings, depthLimit, currentDepth);
                lines = children.Prepend(directoryName);
            }
            else {
                lines = new[] { $"| {itemName}" };
            }

            return lines.SplitLines().Indent();
        }
    }
}