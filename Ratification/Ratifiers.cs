namespace Ratified;

public static class Ratifiers {
    public static readonly IRatifier<string, IEnumerable<char>> ContainsAny = Ratifier<string>.Create<IEnumerable<char>>(static (str, chars) => chars.Any(str.Contains));
    public static          IRatifier<string>                    MakeContainsAny(IEnumerable<char> chars) => ContainsAny.Bind(chars);
}