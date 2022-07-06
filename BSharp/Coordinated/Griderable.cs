using System;
using System.Collections;
using System.Collections.Generic;

namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// Similar to <see cref="FowlFever.BSharp.Collections.Indexes"/>, this offers a way to treat the indexes of an <see cref="ICoordinated{T}"/> as if they were a
/// collection that we didn't care about the contents of.
/// </summary>
/// <param name="Width"></param>
/// <param name="Height"></param>
public readonly record struct Griderable(int Width, int Height) : IReadOnlyCoordinated<Coord>, IEnumerable<Coord> {
    public Coord this[Coord              coord] => this.ValidateCoord(coord);
    public Coord this[(Index x, Index y) coord] => this[Coord.FromIndices(coord, Dimensions)];
    public Coord this[Index              x, Index y] => this[(x, y)];
    public (int width, int height) Dimensions => (Width, Height);

    public IEnumerator<Coord> GetEnumerator() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                yield return (x, y);
            }
        }
    }

    IEnumerator<KeyValuePair<Coord, Coord>> IEnumerable<KeyValuePair<Coord, Coord>>.GetEnumerator() {
        using var iterator = GetEnumerator();
        while (iterator.MoveNext()) {
            yield return new KeyValuePair<Coord, Coord>(iterator.Current, iterator.Current);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}