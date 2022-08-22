using System.Runtime.CompilerServices;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Converts things to <see cref="IRenderable"/>s.
/// </summary>
public interface IRenderwerks {
    /// <summary>
    /// Creates an <see cref="IRenderable"/> for a <typeparamref name="T"/> <paramref name="value"/>.
    /// </summary>
    /// <param name="value">the <typeparamref name="T"/> that will be rendered</param>
    /// <param name="label">used for things like <see cref="Panel.Header"/>s</param>
    /// <param name="expression">a fallback for <paramref name="label"/>. <i>(Should <b>not</b> be set manually - see <see cref="CallerArgumentExpressionAttribute"/>)</i></param>
    /// <typeparam name="T">the type of the <paramref name="value"/></typeparam>
    /// <typeparam name="TLabel">the type of the <paramref name="label"/></typeparam>
    /// <returns>a new <see cref="IRenderable"/></returns>
    public IRenderable GetRenderable<T, TLabel>(
        T                                           value,
        TLabel                                      label,
        [CallerArgumentExpression("value")] string? expression = default
    );

    /// <inheritdoc cref="GetRenderable{T,TLabel}"/>
    public IRenderable GetRenderable<T>(
        T                                           value,
        [CallerArgumentExpression("value")] string? expression = default
    ) {
        return GetRenderable(value, expression, expression);
    }
}