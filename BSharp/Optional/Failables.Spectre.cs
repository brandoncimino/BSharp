using System;
using System.Linq;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Optional;

public static partial class Failables {
    #region Spectre.Console Support

    public static IRenderable GetRenderable(this IFailable failable) {
        var panel = new Panel(GetRenderableExcuse(failable) ?? Text.Empty) {
            Header = new PanelHeader(failable.GetIcon().EscapeMarkup())
        };
        return panel;
    }

    private static Func<ExceptionSettings> ImmutableExceptionSettings() {
        return () => new ExceptionSettings() {
            Format = ExceptionFormats.ShortenEverything,
            Style = {
                Dimmed = new Style(foreground: Color.DarkSlateGray1, decoration: Decoration.Italic)
            }
        };
    }

    [UsedImplicitly]
    public static ExceptionSettings DefaultExceptionSettings = ImmutableExceptionSettings().Invoke();

    public static IRenderable? GetRenderableExcuse(this IFailable failable, ExceptionSettings? settings = default) {
        settings ??= DefaultExceptionSettings;
        return failable.Excuse?.GetRenderable(settings);
    }

    public static IRenderable GetRenderable(this RapSheet rapSheet) {
        var tree = new Tree(rapSheet.GetHeadline().EscapeMarkup());
        tree.AddNodes(rapSheet.Charges.Select(GetRenderable));
        return tree;
    }

    #endregion
}