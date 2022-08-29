using FowlFever.Conjugal.Affixing;

namespace FowlFever.BSharp.Enums;

public static class Bookends {
    public static readonly Circumfix Parentheses      = new("(", ")");
    public static readonly Circumfix SquareBrackets   = new("[", "]");
    public static readonly Circumfix SquigglyBrackets = new("{", "}");
    /// <summary>
    /// The less-than (<c>&lt;</c>) and greater-than (<c>&gt;</c>) symbols as used in programming contexts, like XML nodes and generic type parameters.
    /// </summary>
    /// <remarks>
    /// In order of size: <see cref="AngleBrackets"/> &gt; <see cref="Diamond"/> &gt; <see cref="Guillemets"/>: <c><![CDATA[⟨<«»>⟩]]></c>
    /// <p/>
    /// Wiktionary: <a href="https://en.wiktionary.org/wiki/diamond_bracket">diamond bracket</a>, <a href="https://en.wiktionary.org/wiki/broket">broket</a>
    /// </remarks>
    public static readonly Circumfix Diamond = new("<", ">");
    /// <inheritdoc cref="Diamond"/>
    public static Circumfix GreaterThanLessThan => Diamond;
    public static readonly Ambifix   Quotes        = new("\"");
    public static readonly Ambifix   SingleQuotes  = new("'");
    public static readonly Ambifix   GraveAccents  = new("`");
    public static readonly Ambifix   Asterisks     = new("*");
    public static readonly Circumfix AngleBrackets = new("⟨", "⟩");
    public static readonly Ambifix   Underscores   = new("_");
    /// <summary>
    /// Used as a quotation mark in some languages.
    /// </summary>
    /// <remarks>
    /// In order of size: <see cref="AngleBrackets"/> &gt; <see cref="Diamond"/> &gt; <see cref="Guillemets"/>: <c><![CDATA[⟨<«»>⟩]]></c>
    /// <p/>
    /// <b>Wikipedia:</b> <a href="https://en.wikipedia.org/wiki/Guillemet">guillemet</a>
    /// <br/>
    /// <b>Unicode:</b>
    /// <ul>
    /// <li><c>U+00AB «</c> LEFT-POINTING DOUBLE ANGLE QUOTATION MARK (<c><![CDATA[&laquo;]]></c>)</li>
    /// <li><c>U+00BB »</c> RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK (<c><![CDATA[&raquo;]]></c>)</li>
    /// </ul>
    /// </remarks>
    public static readonly Circumfix Guillemets = new("«", "»");
}