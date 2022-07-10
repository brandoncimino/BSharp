namespace FowlFever.BSharp.Strings.Markup;

public enum MarkupLanguage {
    Html,
    Markdown,
    AsciiDoc,
    SpectreConsole,
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

public interface IMarkupSpectre {
    string MarkupSpectre();
}