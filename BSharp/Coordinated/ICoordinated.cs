using System;

namespace FowlFever.BSharp.Coordinated;

/// <inheritdoc/>
/// <remarks>
/// <see cref="ICoordinated{T}"/> adds setters to <see cref="IReadOnlyCoordinated{T}"/>.
/// </remarks>
public interface ICoordinated<T> : IReadOnlyCoordinated<T> {
    new T this[Coord              coord] { get; set; }
    new T this[(Index x, Index y) coord] { get; set; }
    new T this[Index              x, Index y] { get; set; }
}