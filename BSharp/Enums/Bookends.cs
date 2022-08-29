using FowlFever.Conjugal.Affixing;

namespace FowlFever.BSharp.Enums;

public static class Bookends {
    public static readonly Circumfix Parentheses      = new("(", ")");
    public static readonly Circumfix SquareBrackets   = new("[", "]");
    public static readonly Circumfix SquigglyBrackets = new("{", "}");
    public static readonly Circumfix Diamond          = new("<", ">");
    public static readonly Ambifix   Quotes           = new("\"");
    public static readonly Ambifix   SingleQuotes     = new("'");
    public static readonly Ambifix   GraveAccents     = new("`");
    public static readonly Ambifix   Asterisks        = new("*");
    public static readonly Circumfix AngleBrackets    = new("⟨", "⟩");
    public static readonly Ambifix   Underscores      = new("_");
}