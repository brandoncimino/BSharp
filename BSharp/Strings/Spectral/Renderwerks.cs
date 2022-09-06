using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Static methods to produce <see cref="IRenderable"/>s.
/// </summary>
public static class Renderwerks {
    public static          Palette? Palette              { get; set; }
    private static         string   GetString<T>(T? obj) => obj.Prettify();
    public static readonly Text     NullPlaceholder = new(Prettification.DefaultNullPlaceholder.EscapeMarkup());

    public static IRenderable GetRenderable<T>(T value, Palette? palette = default) {
        var pal = palette.OrFallback(Palette);
        var renderable = value switch {
            string s              => default,
            IRenderable r         => r,
            Exception e           => e.GetRenderable(),
            Uri uri               => uri.GetRenderable(pal),
            FileSystemInfo f      => f.GetRenderable(pal),
            IEnumerable<object> e => GetRenderable(e, pal),
            IEnumerable e         => GetRenderable(e.Cast<object>()),
            _                     => null
        };

        if (renderable != null) {
            return renderable;
        }

        var valueStr = GetString(value);
        var style    = FindStyle<T>(pal);

        return valueStr.EscapeSpectre(style);
    }

    private static Stylist FindStyle<T>(Palette? palette) {
        var pal  = palette.OrFallback(Palette);
        var type = typeof(T);

        if (type == typeof(string)) {
            return pal.Strings;
        }

        if (type.IsNumber()) {
            return pal.Numbers;
        }

        return default;
    }

    /// <summary>
    /// Similar to <see cref="GetRenderable{T}(T,System.Nullable{FowlFever.BSharp.Strings.Spectral.Palette})"/>, but accepts a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">a <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="palette">an optional <see cref="Strings.Spectral.Palette"/> to use over the default <see cref="Palette"/></param>
    /// <typeparam name="T">the type of entries in the <paramref name="span"/></typeparam>
    /// <returns>a new <see cref="IRenderable"/></returns>
    public static IRenderable GetRenderable<T>(ReadOnlySpan<T> span, Palette? palette = default) => GetRenderable(span.ToArray().AsEnumerable(), palette);

    /// <summary>
    /// Similar to <see cref="GetRenderable{T}(T,System.Nullable{FowlFever.BSharp.Strings.Spectral.Palette})"/>, but accepts a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="span">a <see cref="Span{T}"/></param>
    /// <param name="palette">an optional <see cref="FowlFever.BSharp.Strings.Spectral.Palette"/> to use over the default <see cref="Palette"/></param>
    /// <typeparam name="T">the type of entries in the <paramref name="span"/></typeparam>
    /// <returns>a new <see cref="IRenderable"/></returns>
    public static IRenderable GetRenderable<T>(Span<T> span, Palette? palette = default) => GetRenderable((ReadOnlySpan<T>)span, palette);

    public static IRenderable GetRenderable<T>(IEnumerable<T> stuff, Palette? palette = default) {
        var pal = palette.OrFallback(Palette);
        return new OneLineList(stuff.Select(it => GetRenderable(it, pal)), pal);
    }

    #region Labelled

    public static (IRenderable label, IRenderable value) GetLabelled<T, TLabel>(
        T?                                          value,
        TLabel?                                     label,
        Palette?                                    palette     = default,
        [CallerArgumentExpression("value")] string? _expression = default
    ) {
        var rLabel = label == null ? GetRenderable(_expression, palette) : GetRenderable(label);
        return (rLabel, GetRenderable(value));
    }

    public static (IRenderable label, IRenderable value) GetLabelled<T>(
        T?                                          value,
        Palette?                                    palette     = default,
        [CallerArgumentExpression("value")] string? _expression = default
    ) {
        return GetLabelled(value, default(object), palette, _expression);
    }

    public static (IRenderable label, IRenderable value) GetLabelled<T, TLabel>(
        ReadOnlySpan<T>                            span,
        TLabel?                                    label       = default,
        Palette?                                   palette     = default,
        [CallerArgumentExpression("span")] string? _expression = default
    ) {
        return GetLabelled(span.ToArray(), label, palette, _expression);
    }

    public static (IRenderable label, IRenderable value) GetLabelled<T>(
        ReadOnlySpan<T>                            span,
        Palette?                                   palette     = default,
        [CallerArgumentExpression("span")] string? _expression = default
    ) {
        return GetLabelled(span, default(object), palette, _expression);
    }

    #endregion
}