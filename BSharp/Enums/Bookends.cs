using FowlFever.Conjugal.Affixing;

namespace FowlFever.BSharp.Enums;

public enum Bookend {
    Parentheses,
    SquareBrackets,
    SquigglyBrackets,
    Quotes,
    SingleQuotes,
    Graves,
    Diamonds,
}

public static class BookendsExtensions {
    public static char Prefix(this Bookend bookend) => bookend switch {
        Bookend.Parentheses      => '(',
        Bookend.SquareBrackets   => '[',
        Bookend.SquigglyBrackets => '{',
        Bookend.Quotes           => '"',
        Bookend.SingleQuotes     => '\'',
        Bookend.Graves           => '`',
        Bookend.Diamonds         => '<',
        _                        => throw BEnum.UnhandledSwitch(bookend),
    };

    public static char Suffix(this Bookend bookend) => bookend switch {
        Bookend.Parentheses      => ')',
        Bookend.SquareBrackets   => ']',
        Bookend.SquigglyBrackets => '}',
        Bookend.Quotes           => '"',
        Bookend.SingleQuotes     => '\'',
        Bookend.Graves           => '`',
        Bookend.Diamonds         => '>',
        _                        => throw BEnum.UnhandledSwitch(bookend),
    };

    public static string Circumfix(this Bookend bookend, string around) {
        return around.Circumfix(bookend.Prefix().ToString(), bookend.Suffix().ToString());
    }
}