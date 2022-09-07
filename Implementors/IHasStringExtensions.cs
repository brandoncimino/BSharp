namespace FowlFever.Implementors;

public static class HasStringExtensions {
    public static string             OrEmpty<T>(this T? self) where T : IHas<string?>? => self?.Value ?? "";
    public static ReadOnlySpan<char> AsSpan<T>(this  T? self) where T : IHas<string?>? => self?.Value;
}