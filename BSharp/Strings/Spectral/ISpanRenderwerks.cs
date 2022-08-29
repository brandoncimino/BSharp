using System;
using System.Runtime.CompilerServices;

using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Converts <see cref="ReadOnlySpan{T}"/>s to <see cref="IRenderable"/>s.
/// </summary>
public interface ISpanRenderwerks {
    public IRenderable GetRenderable<T, TLabel>(
        ReadOnlySpan<T>                            span,
        TLabel                                     label,
        [CallerArgumentExpression("span")] string? expression = default
    );

    public IRenderable GetRenderable<T>(
        ReadOnlySpan<T>                            span,
        [CallerArgumentExpression("span")] string? expression = default
    );
}