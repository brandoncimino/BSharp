namespace FowlFever.BSharp.Strings.Markup;

public enum MarkupLanguage {
    Html, Markdown, AsciiDoc,
}

public interface IMarkupMarkdown {
    string MarkupMarkdown();
}

public interface IMarkupHtml {
    string MarkupHtml();
}

public interface IMarkupAsciidoc {
    string MarkupAsciidoc();
}