using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Strings {
    [Serializable]
    public class Author {
        public List<string> NameParts;
        public Hyperlink    Website;

        public string FullName => NameParts.Where(n => !string.IsNullOrWhiteSpace(n)).JoinString(" ");

        public string Citation(Hyperlink.MarkupLanguage markupLanguage = Hyperlink.MarkupLanguage.HTML) {
            return $"{FullName} ({Website.ToString(markupLanguage)})";
        }

        public Author(string firstName, string lastName, string websiteDisplay, string websiteUrl) {
            NameParts = new List<string> { firstName, lastName };
            Website   = new Hyperlink(websiteDisplay, websiteUrl);
        }
    }
}