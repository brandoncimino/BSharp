using System;

using FowlFever.BSharp.Strings.Markup;

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
public record Hyperlink(string DisplayText, string Url) : IMarkupHtml, IMarkupAsciidoc, IMarkupMarkdown, IMarkupSpectre {
    //TODO: a variable to store an icon (like the twitter logo or something)

    public string MarkupMarkdown() => $"[{DisplayText}]({Url})";
    public string MarkupHtml()     => $"<a href=\"{Url}\">{DisplayText}</a>";
    public string MarkupAsciidoc() => $"link::{Url}[{DisplayText}]";

    /// <summary>
    /// Formats a hyperlink for use with <see cref="Spectre.Console"/>'s <see cref="Markup"/>.
    /// <p/>
    /// From <a href="https://github.com/spectreconsole/spectre.console/blob/main/examples/Console/Links/Program.cs">Spectre.Console examples</a>:
    /// <code><![CDATA[
    /// AnsiConsole.MarkupLine("[link=https://patriksvensson.se]Click to visit my blog[/]!");
    /// ]]></code>
    /// </summary>
    /// <returns>a <see cref="Markup"/> string</returns>
    public string MarkupSpectre() => $"[link={Url.EscapeMarkup()}]{DisplayText.EscapeMarkup()}[/]";

    public IRenderable GetRenderable() {
        return new Spectre.Console.Markup(MarkupSpectre());
    }
}