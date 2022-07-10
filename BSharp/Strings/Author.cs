using System;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings.Markup;

namespace FowlFever.BSharp.Strings;

[Obsolete("this just doesn't do that much to be useful")]
[Serializable]
public record Author(string Name, params Hyperlink[] Websites) {
    public string Citation(MarkupLanguage markupLanguage, string? websiteJoiner = " // ") {
        return $"{Name} ({Websites.Select(it => it.ToMarkup(markupLanguage)).JoinString(websiteJoiner)}";
    }
}