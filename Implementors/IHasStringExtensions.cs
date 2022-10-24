using System.Diagnostics.CodeAnalysis;

namespace FowlFever.Implementors;

public static class HasStringExtensions {
    public static string             OrEmpty<T>(this                         T? self) where T : IHas<string?>? => self?.Value ?? "";
    public static ReadOnlySpan<char> AsSpan<T>(this                          T? self) where T : IHas<string?>? => self?.Value;
    public static bool               IsEmpty<T>([NotNullWhen(   false)] this T? self) where T : IHas<string?>? => string.IsNullOrEmpty(self?.Value);
    public static bool               IsNotEmpty<T>([NotNullWhen(true)] this  T? self) where T : IHas<string?>? => !self.IsEmpty();
    public static bool               IsBlank<T>([NotNullWhen(   false)] this T? self) where T : IHas<string?>? => string.IsNullOrWhiteSpace(self?.Value);
    public static bool               IsNotBlank<T>([NotNullWhen(true)] this  T? self) where T : IHas<string?>? => !self.IsBlank();
    public static int                GetLength<T>(this                       T  self) where T : IHas<string?>  => self.Value?.Length ?? 0;
}