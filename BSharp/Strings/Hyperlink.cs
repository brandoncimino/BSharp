using System;

using FowlFever.BSharp.Strings.Markup;
using FowlFever.BSharp.Strings.Spectral;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// A class for storing hyperlinks in a "url / display text" style.
/// </summary>
/// <param name="DisplayText">the actual <see cref="string"/> that a user will see</param>
/// <param name="Url">the destination url</param>
/// <remarks>
/// TODO: Add a custom editor for this!
/// </remarks>
[Serializable]
public record Hyperlink(string Url) : IMarkupHtml, IMarkupAsciidoc, IMarkupMarkdown, IHasRenderable {
    private string? _displayText;
    public string DisplayText {
        get => _displayText.IfBlank(Url);
        init => _displayText = value;
    }
    private readonly Style _style = new(Color.Blue, decoration: Decoration.Underline, link: Url);
    public Style Style {
        get => _style.Link(Url);
        init => _style = value;
    }

    //TODO: a variable to store an icon (like the twitter logo or something)

    public string      MarkupMarkdown() => $"[{DisplayText}]({Url})";
    public string      MarkupHtml()     => $"<a href=\"{Url}\">{DisplayText}</a>";
    public string      MarkupAsciidoc() => $"link::{Url}[{DisplayText}]";
    public IRenderable GetRenderable()  => _displayText.EscapeSpectre(Style);
}