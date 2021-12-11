using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Clerical {
    /// <summary>
    /// TODO: There is almost certainly a cross-platform library to use for this that I can get from Nuget
    /// </summary>
    [PublicAPI]
    public static class BPath {
        internal static readonly RegexGroup ExtensionGroup            = new RegexGroup(nameof(ExtensionGroup), @"(\.[^.]+?)+$");
        internal static readonly char[]     Separators                = Enum.GetValues(typeof(DirectorySeparator)).Cast<DirectorySeparator>().Select(DirectorySeparatorExtensions.ToChar).ToArray();
        internal static readonly Regex      DirectorySeparatorPattern = new Regex(@"[\\\/]");
        internal static readonly string     OpenFolderIcon            = "📂";
        internal static readonly string     ClosedFolderIcon          = "📁";
        internal static readonly string     FileIcon                  = "📄";

        public static Failable ValidatePath(string? maybePath) {
            Action action = () => _ = Path.GetFullPath(maybePath!);
            return action.Try();
        }

        public static Failable ValidateFileName(string? maybeFileName) {
            Action action = () => {
                ValidateFileNameCharacters(maybeFileName);
                _ = Path.GetFullPath(maybeFileName!);
            };
            return action.Try();
        }

        [ContractAnnotation("null => stop")]
        private static void ValidateFileNameCharacters(string? maybeFileName) {
            if (maybeFileName == null) {
                throw new ArgumentNullException($"The string [{maybeFileName.ToString(Prettification.DefaultNullPlaceholder)}] wasn't a valid filename: it was blank!");
            }

            if (maybeFileName.ContainsAny(Path.GetInvalidFileNameChars())) {
                var badCharacters = maybeFileName.Intersect(Path.GetInvalidFileNameChars());
                throw new ArgumentException($"The string [{maybeFileName}] isn't a valid filename: it contains the illegal characters {badCharacters.Prettify()}!");
            }
        }

        [ContractAnnotation("null => false")]
        public static bool IsValidPath(string? maybePath) {
            return ValidatePath(maybePath).Failed == false;
        }

        [ContractAnnotation("null => false")]
        public static bool IsValidFileName(string? maybeFileName) {
            return ValidateFileName(maybeFileName).Failed == false;
        }


        /// <summary>
        /// This method is similar to <see cref="Path.GetExtension"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c>
        /// </summary>
        /// <remarks>This uses the <see cref="ExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
        /// <param name="path">a path or file name</param>
        /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
        [ContractAnnotation("path:null => null")]
        public static string[]? GetExtensions(string? path) {
            if (path == null) {
                return null;
            }

            return GetFullExtension(path)
                   .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(it => it.PrependIfMissing("."))
                   .ToArray();
        }

        [ContractAnnotation("path:null => null")]
        public static string? GetFullExtension(string? path) {
            if (path == null) {
                return null;
            }

            return Path.GetFileName(path)
                       .Match(ExtensionGroup.Regex)
                       .Groups[ExtensionGroup.GroupName]
                       .Value;
        }

        [ContractAnnotation("path:null => null")]
        public static string? GetFileNameWithoutExtensions(string? path) {
            if (path == null) {
                return null;
            }

            var fileName    = Path.GetFileName(path);
            var firstPeriod = fileName.IndexOf(".", StringComparison.Ordinal);
            return firstPeriod < 0 ? fileName : fileName.Substring(0, firstPeriod);
        }

        [ContractAnnotation("null => false")]
        public static bool EndsWithSeparator(string? path) {
            return path.EndsWith(DirectorySeparatorPattern);
        }

        [ContractAnnotation("null => false")]
        public static bool StartsWithSeparator(string? path) {
            return path.StartsWith(DirectorySeparatorPattern);
        }


        public static string EnsureTrailingSeparator(string? path, DirectorySeparator separator = DirectorySeparator.Universal) {
            if (path == null) {
                return separator.ToCharString();
            }

            return NormalizeSeparators(
                path?.Trim()
                    .TrimEnd(DirectorySeparatorPattern)
                    .Suffix(separator.ToCharString()),
                separator
            );
        }


        public static string StripLeadingSeparator(string? path, DirectorySeparator separator = DirectorySeparator.Universal) {
            if (path == null) {
                return "";
            }

            return NormalizeSeparators(
                path?.Trim()
                    .TrimStart(separator.ToCharString()),
                separator
            );
        }


        public static string NormalizeSeparators(string? path, DirectorySeparator separator = DirectorySeparator.Universal) {
            return path.IsBlank() ? "" : DirectorySeparatorPattern.Replace(path.Trim(), separator.ToCharString());
        }


        public static string JoinPath(
            string? parent,
            string? child
        ) {
            return JoinPath(parent, child, default(DirectorySeparator));
        }


        public static string JoinPath(
            string?            parent,
            string?            child,
            DirectorySeparator separator
        ) {
            parent = parent?.Trim().TrimEnd(DirectorySeparatorPattern);
            child  = child?.Trim().TrimStart(DirectorySeparatorPattern);
            var path = parent.JoinWith(child, separator.ToCharString());
            return NormalizeSeparators(path, separator);
        }


        public static string JoinPath(
            DirectoryInfo?     parentDirectory,
            string?            child,
            DirectorySeparator separator = DirectorySeparator.Universal
        ) {
            return JoinPath(parentDirectory?.FullName, child, separator);
        }


        public static string JoinPath(params string?[]? parts) {
            return JoinPath(parts, default(DirectorySeparator));
        }


        public static string JoinPath(IEnumerable<string?>? parts, DirectorySeparator separator = DirectorySeparator.Universal) {
            return parts?.Aggregate((pathSoFar, nextPart) => JoinPath(pathSoFar, nextPart, separator)) ?? "";
        }
    }
}