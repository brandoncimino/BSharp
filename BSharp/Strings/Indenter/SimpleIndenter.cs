namespace FowlFever.BSharp.Strings.Indenter;

public class SimpleIndenter : IIndenter {
    private int _currentIndent;

    public int CurrentIndent {
        get => _currentIndent;
        private set => _currentIndent = value.Clamp(0);
    }

    public int    IndentSize                     { get; init; } = 2;
    public string IndentString                   => " ";
    public string Render()                       => "".PadRight(IndentSize * CurrentIndent);
    public void   Indent(int relativeIndent = 1) => CurrentIndent += relativeIndent;
}