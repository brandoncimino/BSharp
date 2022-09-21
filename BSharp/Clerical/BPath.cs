using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Clerical;

/// <summary>
/// TODO: There is almost certainly a cross-platform library to use for this that I can get from Nuget
/// TODO: Made this non-static to prepare for the ability to make an instantiable class like <see cref="Conjugal"/>
/// </summary>
[Obsolete($"Please use FowlFever.BSharp.Clerical.Clerk instead")]
public class BPath {
    internal static readonly RegexGroup ExtensionGroup = new RegexGroup(nameof(ExtensionGroup), @"(\.[^.]+?)+$");

    #region Character Sets

    #endregion

    #region Directory Separator Regex Patterns

    public static readonly Regex DirectorySeparatorPattern = new Regex(@"[\\\/]");
    public static readonly Regex OuterSeparatorPattern     = RegexPatterns.OuterMatch(DirectorySeparatorPattern);
    public static readonly Regex InnerSeparatorPattern     = RegexPatterns.InnerMatch(DirectorySeparatorPattern);

    #endregion

    #region Icons

    internal static readonly string ClosedFolderIcon = "📁";
    internal static readonly string FileIcon         = "📄";

    #endregion

    [Pure]
    public static string[] SplitPath(string? path) {
        return path?.Split(InnerSeparatorPattern) ?? Array.Empty<string>();
    }

    #region ⚠ OBSOLETE ⚠ Validation

    [Obsolete]
    private static class Validate {
        public static void PathString(string? maybePath) {
            // ValidationExtensions.ValidateMultiple(
            //     maybePath,
            //     p => p.MustNotBeBlank(),
            //     p => _ = Path.GetFullPath(p!),
            //     p => _ = new FileInfo(p!),
            //     PathChars
            //     // p => p != null ? PathParts(SplitPath(p)) : null
            // );
        }

        public static string PathPart(string? part, bool isFirst) {
            // ValidationExtensions.ValidateMultiple(
            //     part,
            //     p => p.MustNotBeBlank(),
            //     p => _ = p?.Matches(InnerSeparatorPattern)                == true ? throw new ArgumentException($"[{part}] contains inner separators!") : true,
            //     p => _ = p?.Matches($"{DirectorySeparatorPattern}{{2,}}") == true ? throw new ArgumentException($"[{part}] contains repeating separators!") : true,
            //     p => FileNameChars(p, isFirst)
            // );

            return part!;
        }

        public static IList<string> PathParts(IEnumerable<string?>? parts) {
            return parts?.Select((it, i) => PathPart(it, i == 0)).ToList()!;
        }
    }

    #endregion

    #region Extensions

    /// <summary>
    /// This method is similar to <see cref="Path.GetExtension"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c>
    /// </summary>
    /// <remarks>This uses the <see cref="ExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    [Pure]
    public static string[] GetExtensions(string? path) {
        return GetFullExtension(path)
               ?.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(it => it.PrependIfMissing("."))
               .ToArray()
               ?? Array.Empty<string>();
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    public static string? GetFullExtension(string? path) {
        return Path.GetFileName(path)
                   ?
                   .Match(ExtensionGroup.Regex)
                   .Groups[ExtensionGroup.Name]
                   .Value;
    }

    #endregion

    #region Separators

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

    #endregion

    #region Joining

    private enum JoinSeparatorOption {
        Simple, TrimAll, TrimOne,
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
    public static string JoinPath(params string?[] parts) {
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
        parts = Validate.PathParts(parts?.SelectMany(SplitPath));
        return parts?.Aggregate((pathSoFar, nextPart) => _JoinPathInternal(pathSoFar, nextPart, separator, JoinSeparatorOption.TrimOne)) ?? "";
    }

    #endregion
}