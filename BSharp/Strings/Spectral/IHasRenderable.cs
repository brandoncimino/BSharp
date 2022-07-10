using System.Collections.Generic;

using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Implements the <see cref="IRenderable"/> interface via a <see cref="GetRenderable"/> method that delegates to the <see cref="IRenderable.Measure"/> and <see cref="IRenderable.Render"/> methods.
/// </summary>
public interface IHasRenderable : IRenderable {
    IRenderable                      GetRenderable();
    Measurement IRenderable.         Measure(RenderContext context, int maxWidth) => GetRenderable().Measure(context, maxWidth);
    IEnumerable<Segment> IRenderable.Render(RenderContext  context, int maxWidth) => GetRenderable().Render(context, maxWidth);
}