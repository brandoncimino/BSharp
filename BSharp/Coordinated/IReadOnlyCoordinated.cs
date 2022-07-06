using System;

namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// Represents a collection that can be accessed via 2D <see cref="Coord"/>s.
/// </summary>
/// <typeparam name="T">the type of each element</typeparam>
/// <remarks>
/// Unless the inheritor also implements <see cref="IOffsetCoordinated{T}"/>, it can be assumed that the origin point for the collection is <c>(0,0)</c>.
/// </remarks>
public interface IReadOnlyCoordinated<T> : IGriderable<T> {
    T this[Coord              coord] { get; }
    T this[(Index x, Index y) coord] { get; }
    T this[Index              x, Index y] { get; }
    public int                     Width      { get; }
    public int                     Height     { get; }
    public (int width, int height) Dimensions { get; }
}

public static class CoordinatedExtensions {
    public static Coord ValidateCoord<T>(this IReadOnlyCoordinated<T> source, Coord coord) {
        if (coord.X    >= source.Width
            || coord.X < 0
            || coord.Y >= source.Height
            || coord.Y < 0
           ) {
            throw new IndexOutOfRangeException($"The {coord.GetType().Name} {coord} was out-of-bounds for the {source.GetType().Name}{source.Dimensions})!");
        }

        return coord;
    }
}