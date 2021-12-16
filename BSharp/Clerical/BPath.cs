using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Clerical {
    /// <summary>
    /// TODO: There is almost certainly a cross-platform library to use for this that I can get from Nuget
    /// </summary>
    [PublicAPI]
    public static class BPath {
        internal static readonly RegexGroup ExtensionGroup            = new RegexGroup(nameof(ExtensionGroup), @"(\.[^.]+?)+$");
        internal static readonly char[]     Separators                = Enum.GetValues(typeof(DirectorySeparator)).Cast<DirectorySeparator>().Select(DirectorySeparatorExtensions.ToChar).ToArray();
        public static readonly   Regex      DirectorySeparatorPattern = new Regex(@"[\\\/]");
        internal static readonly string     OpenFolderIcon            = "📂";
        internal static readonly string     ClosedFolderIcon          = "📁";
        internal static readonly string     FileIcon                  = "📄";

        public static Failable ValidatePath(string? maybePath) {
            Action action = () => {
                ValidatePathCharacters(maybePath);
                _ = Path.GetFullPath(maybePath!);
            };
            return action.Try();
        }

        public static Failable ValidateFileName(string? maybeFileName) {
            Action action = () => {
                ValidateFileNameCharacters(maybeFileName);
                _ = Path.GetFullPath(maybeFileName!);
            };
            return action.Try();
        }

        private static void ValidatePathCharacters(string? maybePath) {
            if (maybePath == null) {
                throw new ArgumentNullException($"The string [{maybePath.ToString(Prettification.DefaultNullPlaceholder)}] wasn't a valid path: it was blank!");
            }

            if (maybePath.ContainsAny(Path.GetInvalidPathChars())) {
                var badCharacters = maybePath.Intersect(Path.GetInvalidPathChars());
                throw new ArgumentException($"The string [{maybePath}] isn't a valid path: it contains the illegal characters {badCharacters.Prettify()}!");
            }
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

        [Pure]
        public static string GetFullExtension(string path) {
            return Path.GetFileName(path)
                       .Match(ExtensionGroup.Regex)
                       .Groups[ExtensionGroup.GroupName]
                       .Value;
        }

        [Pure]
        public static string GetFileNameWithoutExtensions(string path) {
            var fileName    = Path.GetFileName(path);
            var firstPeriod = fileName.IndexOf(".", StringComparison.Ordinal);
            return firstPeriod < 0 ? fileName : fileName.Substring(0, firstPeriod);
        }

        /// <param name="path">a <see cref="Path"/> <see cref="string"/></param>
        /// <returns>true if the <see cref="string"/> <b>ends</b> with <b>any</b> <see cref="DirectorySeparator"/></returns>
        [Pure]
        public static bool EndsWithSeparator(string path) {
            return path.EndsWith(DirectorySeparatorPattern);
        }

        /// <param name="path">a <see cref="Path"/> <see cref="string"/></param>
        /// <returns>true if the <see cref="string"/> <b>starts</b> with <b>any</b> <see cref="DirectorySeparator"/></returns>
        [Pure]
        public static bool StartsWithSeparator(string path) {
            return path.StartsWith(DirectorySeparatorPattern);
        }

        [Pure]
        public static string EnsureTrailingSeparator(string? path, DirectorySeparator separator = DirectorySeparator.Universal) {
            return NormalizeSeparators(
                path?.Trim()
                    .TrimEnd(DirectorySeparatorPattern)
                    .Suffix(separator.ToCharString()),
                separator
            );
        }

        /// <summary>
        /// Removes any <see cref="DirectorySeparator"/>s from the <b>beginning</b> of <paramref name="path"/>.
        /// </summary>
        /// <param name="path">the original <see cref="Path"/> <see cref="string"/></param>
        /// <returns>the original <see cref="string"/> with all of the leading <see cref="DirectorySeparator"/>s removed</returns>
        [Pure]
        public static string StripLeadingSeparator(string? path) {
            return path.TrimStart(DirectorySeparatorPattern) ?? "";
        }

        /// <summary>
        /// Replaces all <see cref="DirectorySeparator"/>s in <paramref name="path"/> with the desired <paramref name="separator"/>.
        /// </summary>
        /// <param name="path">the original <see cref="string"/></param>
        /// <param name="separator">the desired <see cref="DirectorySeparator"/></param>
        /// <returns>the original <see cref="string"/> with all <see cref="DirectorySeparator"/>s replaced with the desired <paramref name="separator"/></returns>
        [Pure]
        public static string NormalizeSeparators(string? path, DirectorySeparator separator = DirectorySeparator.Universal) {
            return path.IsBlank() ? "" : DirectorySeparatorPattern.Replace(path!.Trim(), separator.ToCharString());
        }


        /// <summary>
        /// Combines multiple <see cref="string"/>s into a <see cref="Path"/> <see cref="string"/>, ensuring that <b>exactly 1</b>
        /// <see cref="DirectorySeparator"/> exists between them.
        /// <p/>
        /// Also <see cref="NormalizeSeparators"/> using <paramref name="separator"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="StringUtils.IsEmpty"/> strings <b>are included in the path</b> as <c>""</c>, i.e. <c>JoinPath("a",null) => "a/"</c>
        /// </remarks>
        /// <param name="parent">the first part of the path</param>
        /// <param name="child">the second part of the path</param>
        /// <param name="separator">the desired <see cref="DirectorySeparator"/></param>
        /// <returns>a new <see cref="Path"/> <see cref="string"/></returns>
        [Pure]
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

        /**
         * <inheritdoc cref="JoinPath(string?,string?,FowlFever.BSharp.Clerical.DirectorySeparator)"/>
         */
        [Pure]
        public static string JoinPath(
            string? parent,
            string? child
        ) {
            return JoinPath(parent, child, default(DirectorySeparator));
        }

        /// <summary>
        /// <inheritdoc cref="JoinPath(string?,string?,FowlFever.BSharp.Clerical.DirectorySeparator)"/>
        /// </summary>
        /// <param name="parentDirectory">the first part of the <see cref="Path"/></param>
        /// <param name="child">the second part of the <see cref="Path"/></param>
        /// <param name="separator">the desired <see cref="DirectorySeparator"/></param>
        /// <returns>a new <see cref="Path"/> <see cref="string"/></returns>
        [Pure]
        public static string JoinPath(
            DirectoryInfo?     parentDirectory,
            string?            child,
            DirectorySeparator separator = DirectorySeparator.Universal
        ) {
            return JoinPath(parentDirectory?.FullName, child, separator);
        }

        /// <summary>
        /// <inheritdoc cref="JoinPath(string?,string?,FowlFever.BSharp.Clerical.DirectorySeparator)"/>
        /// </summary>
        /// <param name="parts">the <see cref="string"/>s used to build the <see cref="Path"/></param>
        /// <returns>a new <see cref="Path"/> <see cref="string"/></returns>
        [Pure]
        public static string JoinPath(params string?[]? parts) {
            return JoinPath(parts, default(DirectorySeparator));
        }

        /// <summary>
        /// <inheritdoc cref="JoinPath(string?,string?,FowlFever.BSharp.Clerical.DirectorySeparator)"/>
        /// </summary>
        /// <param name="parts">the <see cref="string"/>s used to build the <see cref="Path"/></param>
        /// <param name="separator">the desired <see cref="DirectorySeparator"/></param>
        /// <returns>a new <see cref="Path"/> <see cref="string"/></returns>
        [Pure]
        public static string JoinPath(IEnumerable<string?>? parts, DirectorySeparator separator = DirectorySeparator.Universal) {
            return parts?.Aggregate((pathSoFar, nextPart) => JoinPath(pathSoFar, nextPart, separator)) ?? "";
        }
    }
}