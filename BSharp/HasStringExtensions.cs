using FowlFever.Implementors;

namespace FowlFever.BSharp;

/// <summary>
/// Special extensions for <see cref="IHas{T}"/> of <see cref="string"/>.
/// </summary>
public static class HasStringExtensions {
    public static bool IsEmpty<SELF>(this SELF? str)
        where SELF : IHas<string?> =>
        str is not { Value.Length: > 0 };

    /// <inheritdoc cref="HasExtensions.OrDefault{SELF}"/>
    public static string? OrDefault<SELF>(this SELF? self)
        where SELF : IHas<string?> => self?.Value ?? default;

    public static string OrEmpty<SELF>(this SELF? self)
        where SELF : IHas<string?> => self?.Value ?? "";
}