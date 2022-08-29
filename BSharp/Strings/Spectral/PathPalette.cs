using System;
using System.IO;
using System.Linq;

using FowlFever.BSharp.Enums;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// <see cref="Stylist"/>s used by the <see cref="TextPath"/> object.
/// </summary>
public readonly record struct PathPalette() {
    public Stylist LeafStyle      { get; init; } = default;
    public Stylist RootStyle      { get; init; } = default;
    public Stylist SeparatorStyle { get; init; } = default;
    public Stylist StemStyle      { get; init; } = default;

    public string? FolderIcon  { get; init; } = "📁";
    public string? FileIcon    { get; init; } = "📄";
    public string? WebIcon     { get; init; } = "🌐";
    public string? GenericIcon { get; init; } = default;

    public enum PathIcon {
        Generic,
        Folder,
        File,
        Web
    }

    public string? this[PathIcon iconType] => iconType switch {
        PathIcon.Folder  => FolderIcon,
        PathIcon.File    => FileIcon,
        PathIcon.Web     => WebIcon,
        PathIcon.Generic => GenericIcon,
        _                => throw BEnum.UnhandledSwitch(iconType)
    };
}

public static class RenderablePathExtensions {
    public static IRenderable GetRenderable(this Uri uri, PathPalette palette = default) {
        return palette.GetRenderablePath(uri.ToString(), palette.FindIcon(uri));
    }

    public static IRenderable GetRenderable(this FileSystemInfo path, PathPalette palette = default) {
        return palette.GetRenderablePath(
            path.ToString(),
            path switch {
                FileInfo      => PathPalette.PathIcon.File,
                DirectoryInfo => PathPalette.PathIcon.Folder,
                _             => PathPalette.PathIcon.Generic
            }
        );
    }

    private static string? FindIcon(this PathPalette palette, Uri uri) {
        return uri switch {
            { IsFile: true }              => uri.ToString().LastOrDefault().IsDirectorySeparator() ? palette.FolderIcon : palette.FileIcon,
            { Scheme: "http" or "https" } => palette.WebIcon,
            _                             => palette.GenericIcon,
        };
    }

    public static IRenderable GetRenderablePath(this PathPalette palette, string text, PathPalette.PathIcon icon) {
        return palette.GetRenderablePath(text, palette[icon]);
    }

    public static IRenderable GetRenderablePath(this PathPalette palette, string text, string? icon) {
        var textPath = new TextPath(text) {
            LeafStyle      = palette.LeafStyle,
            RootStyle      = palette.RootStyle,
            SeparatorStyle = palette.SeparatorStyle,
            StemStyle      = palette.SeparatorStyle,
        };

        if (icon.IsBlank()) {
            return textPath;
        }

        return new Columns(icon.EscapeSpectre(), textPath) {
            Expand  = false,
            Padding = new Padding(1),
        };
    }
}