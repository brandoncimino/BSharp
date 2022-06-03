namespace FowlFever.BSharp.Strings.Indenter;

/// <summary>
/// Tracks and formats an indentation.
/// </summary>
public interface IIndenter {
    int    CurrentIndent { get; }
    int    IndentSize    { get; }
    string IndentString  { get; }
    string Render();
    void   Indent(int relativeIndent = 1);
}

public interface IStackIndenter : IIndenter {
    void Indent(string indent);
}

public static class IndenterExtensions {
    public static void Outdent(this IIndenter self, int relativeOutdent = 1) => self.Indent(relativeOutdent * -1);
}