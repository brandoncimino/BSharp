using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
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
        public static readonly   Regex      OuterSeparatorPattern     = RegexPatterns.OuterMatch(DirectorySeparatorPattern);
        public static readonly   Regex      InnerSeparatorPattern     = RegexPatterns.InnerMatch(DirectorySeparatorPattern);
        internal static readonly string     OpenFolderIcon            = "📂";
        internal static readonly string     ClosedFolderIcon          = "📁";
        internal static readonly string     FileIcon                  = "📄";

        public static Failable ValidatePath(string? maybePath) {
            Action<string> action = Validate.PathString;
            return action!.Try(maybePath);
        }

        public static string[] SplitPath(this string path) {
            return path.Split(InnerSeparatorPattern);
        }

        private static class Validate {
            public static void PathString(string? maybePath) {
                ValidationExtensions.ValidateMultiple(
                    maybePath,
                    p => p.MustNotBeBlank(),
                    p => _ = Path.GetFullPath(p!),
                    p => _ = new FileInfo(p!),
                    PathChars,
                    p => PathParts(p?.SplitPath())
                );
            }

            public static string PathPart(string? part, bool isFirst) {
                ValidationExtensions.ValidateMultiple(
                    part,
                    p => p.MustNotBeBlank(),
                    p => _ = p?.Matches(InnerSeparatorPattern)                == true ? throw new ArgumentException($"[{part}] contains inner separators!") : true,
                    p => _ = p?.Matches($"{DirectorySeparatorPattern}{{2,}}") == true ? throw new ArgumentException($"[{part}] contains repeating separators!") : true,
                    p => FileNameChars(p, isFirst)
                );

                return part!;
            }

            public static IList<string> PathParts(IEnumerable<string?>? parts) {
                return parts?.Select((it, i) => PathPart(it, i == 0)).ToList()!;
            }

            private static void PathChars(string? maybePath) {
                if (maybePath?.ContainsAny(Path.GetInvalidPathChars()) == true) {
                    var badChars = maybePath.Intersect(Path.GetInvalidPathChars());
                    throw new ArgumentException($"[{maybePath}] contains invalid path characters: {badChars.Prettify()}");
                }
            }

            private static void FileNameChars(string? maybeFile, bool isFirst) {
                if (isFirst) {
                    return;
                }

                if (maybeFile?.Trim(OuterSeparatorPattern).ContainsAny(Path.GetInvalidFileNameChars()) == true) {
                    var badChars = maybeFile.Intersect(Path.GetInvalidFileNameChars());
                    throw new ArgumentException($"[{maybeFile}] contains invalid file name characters: [{badChars.JoinString(",")}]");
                }
            }
        }

        [ContractAnnotation("null => false")]
        public static bool IsValidPath(string? maybePath) {
            return ValidatePath(maybePath).Failed == false;
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
                       .Groups[ExtensionGroup.Name]
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
                (path?.Trim()
                     .TrimEnd(DirectorySeparatorPattern))
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

        private enum JoinSeparatorOption {
            Simple,
            TrimAll,
            TrimOne,
        }


        /// <summary>
        /// Combines multiple <see cref="string"/>s into a <see cref="Path"/> <see cref="string"/>.
        /// <p/>
        /// Also <see cref="NormalizeSeparators"/> using <paramref name="separator"/>.
        /// </summary>
        /// <remarks>
        /// Up to one <see cref="DirectorySeparator"/> is "collapsed", i.e. <c>JoinPath("a//", "/b") => "a//b"</c>
        /// <p/>
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
            return JoinPath(new[] { parent, child });
        }

        private static string _JoinPathInternal(
            string?             parent,
            string?             child,
            DirectorySeparator  separator,
            JoinSeparatorOption joinSeparatorOption
        ) {
            Validate.PathString(parent);
            Validate.PathString(child);
            switch (joinSeparatorOption) {
                case JoinSeparatorOption.TrimAll:
                    parent = parent!.TrimEnd(OuterSeparatorPattern);
                    child  = child!.TrimStart(OuterSeparatorPattern);
                    break;
                case JoinSeparatorOption.TrimOne:
                    parent = parent!.TrimEnd(OuterSeparatorPattern, 1);
                    child  = child!.TrimStart(OuterSeparatorPattern, 1);
                    break;
                case JoinSeparatorOption.Simple:
                    break;
                default:
                    throw BEnum.InvalidEnumArgumentException(nameof(joinSeparatorOption), joinSeparatorOption);
            }

            return NormalizeSeparators(string.Join(separator.ToCharString(), parent, child));
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
            parts = Validate.PathParts(parts?.SelectMany(it => it?.SplitPath()));
            return parts?.Aggregate((pathSoFar, nextPart) => _JoinPathInternal(pathSoFar, nextPart, separator, JoinSeparatorOption.TrimOne)) ?? "";
        }
    }
}