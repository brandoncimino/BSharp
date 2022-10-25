using System.Collections.Immutable;

namespace FowlFever.BSharp;

public readonly partial record struct ValueArray<T> {
    #region Conversions

    public static implicit operator ValueArray<T>(ImmutableArray<T> parts) => new(parts);
    public static implicit operator ValueArray<T>(T                 part)  => new(ImmutableArray.Create(part));
    public static implicit operator ValueArray<T>(T[]               parts) => new(ImmutableArray.Create(parts));

    #endregion
}