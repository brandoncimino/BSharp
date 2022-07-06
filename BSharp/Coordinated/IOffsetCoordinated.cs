namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// Represents an <see cref="ICoordinated{T}"/> or <see cref="ICoordinated{T}"/> that might not be indexed by <c>0,0</c>
/// </summary>
/// <typeparam name="T">the type of the entries in the collection</typeparam>
/// <remarks>
/// TODO: With .NET 6 generic static abstracts, extract <see cref="Coord"/> into a generic parameter constrained to the <c>IAddition</c> interface
/// </remarks>
public interface IOffsetCoordinated<T> {
    Coord MaxCoord { get; }
    Coord MinCoord { get; }
    Coord Center   { get; }
    Coord Offset   { get; }
}