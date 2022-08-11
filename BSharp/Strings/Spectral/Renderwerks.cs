using System;
using System.Runtime.CompilerServices;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Converts things to <see cref="IRenderable"/>s.
/// </summary>
public interface IRenderwerks {
    public Func<object?, string> ToStringFunction { get; }

    /// <inheritdoc cref="GetRenderable{T,TLabel}"/>
    public sealed IRenderable GetRenderable<T>(
        T                                           value,
        string?                                     label      = default,
        Palette?                                    palette    = default,
        [CallerArgumentExpression("value")] string? expression = default
    ) {
        return GetRenderable<T, string>(value, label, palette, expression);
    }

    /// <summary>
    /// Creates an <see cref="IRenderable"/> for a <typeparamref name="T"/> <paramref name="value"/>.
    /// </summary>
    /// <param name="value">the <typeparamref name="T"/> that will be rendered</param>
    /// <param name="label">used for things like <see cref="Panel.Header"/>s</param>
    /// <param name="palette">a collection of <see cref="Style"/>s</param>
    /// <param name="expression">a fallback for <paramref name="label"/>. <i>(Should <b>not</b> be set manually - see <see cref="CallerArgumentExpressionAttribute"/>)</i></param>
    /// <typeparam name="T">the type of the <paramref name="value"/></typeparam>
    /// <typeparam name="TLabel">the type of the <paramref name="label"/></typeparam>
    /// <returns>a new <see cref="IRenderable"/></returns>
    /// <remarks>
    /// Implementation should be done in <see cref="GetRenderable_Hook{T,TLabel}"/>.
    /// </remarks>
    public sealed IRenderable GetRenderable<T, TLabel>(
        T                                           value,
        TLabel?                                     label,
        Palette?                                    palette    = default,
        [CallerArgumentExpression("value")] string? expression = default
    ) {
        return value switch {
            IRenderable r => r,
            _             => GetRenderable_Hook(value, label, palette, expression)
        };
    }

    protected IRenderable GetRenderable_Hook<T, TLabel>(
        T        value,
        TLabel?  label,
        Palette? palette,
        string?  expression
    );
}

public class Panelwerks : IRenderwerks {
    public Func<object?, string> ToStringFunction { get; } = Prettification.Prettify<object>;

    IRenderable IRenderwerks.GetRenderable_Hook<T, TLabel>(T value, TLabel? label, Palette? palette, string? expression)
        where TLabel : default {
        var labelStr = ToStringFunction(label);
        var header   = labelStr.IfBlank(expression);
        var content  = ToStringFunction(value).PadRight(header?.Length ?? 0);
        var spectre = new Panel(content.EscapeMarkup()) {
            Header      = new PanelHeader(header.EscapeMarkup()),
            Expand      = false,
            Border      = BoxBorder.Rounded,
            BorderStyle = palette.OrDefault().Borders,
        };

        return spectre;
    }
}