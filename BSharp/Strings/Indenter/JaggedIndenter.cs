using System.Collections.Generic;
using System.Linq;

namespace FowlFever.BSharp.Strings.Indenter;

public class JaggedIndenter : IStackIndenter {
    public           string     IndentString { get; init; } = " ";
    private readonly Stack<int> IndentStack = new();
    public           int        CurrentIndent => IndentStack.Count;
    public           int        IndentSize    { get; init; } = 2;
    public           string     Render()      => IndentString.RepeatToLength(IndentStack.Sum());

    public void Indent(int relativeIndent = 1) {
        var abs = relativeIndent.Abs();
        for (int i = 0; i < abs; i++) {
            if (relativeIndent < 0) {
                IndentStack.Pop();
            }
            else {
                IndentStack.Push(IndentSize);
            }
        }
    }

    public void Indent(string indentation) => IndentStack.Push(indentation.Length);
}