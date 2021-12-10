using System;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// A class for storing hyperlinks in a "url / display text" style.
    /// </summary>
    /// <remarks>
    /// TODO: Add a custom editor for this!
    /// </remarks>
    [Serializable]
    public class Hyperlink {
        public enum MarkupLanguage {
            Markdown,
            HTML,
            AsciiDoc
        }

        public string DisplayText;
        public string URL;

        public Hyperlink(string displayText, string url) {
            DisplayText = displayText;
            URL         = url;
        }

        //TODO: a variable to store an icon (like the twitter logo or something)

        public string ToString(MarkupLanguage markupLanguage) {
            switch (markupLanguage) {
                case MarkupLanguage.Markdown:
                    return Markdown;
                case MarkupLanguage.HTML:
                    return HTML;
                case MarkupLanguage.AsciiDoc:
                    throw new NotImplementedException($"haven't coded the {MarkupLanguage.AsciiDoc} format yet!");
                default:
                    throw new ArgumentOutOfRangeException(nameof(markupLanguage), markupLanguage, null);
            }
        }

        public string Markdown => $"[{DisplayText}]({URL})";
        public string HTML     => $"<a href=\"{URL}\">{DisplayText}</a>";
        // public string AsciiDoc => $"link::{URL}[{DisplayText}]";
    }
}