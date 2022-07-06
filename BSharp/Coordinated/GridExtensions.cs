using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// Extension methods that operate on 2D <see cref="Array"/>s as well as the specialized <see cref="ICoordinated{T}"/> type.
/// </summary>
public static class GridExtensions {
    public static Coord UpperBound(this IEnumerable<Coord> coords) {
        return coords.Aggregate((a, b) => a.UpperBound(b));
    }

    // public static Grid<T> ToGrid<T>(this T[,] source) => new(source);
    //
    // public static Grid<T> ToGrid<T>(this IEnumerable<KeyValuePair<Coord, T>> source) {
    //     return source switch {
    //         Grid<T> grid => grid,
    //         _            => new Grid<T>(source)
    //     };
    // }

    public enum MissingCellAction { PadRight, PadLeft, Error }

    // public static Grid<T> ToGrid<T>(this IEnumerable<IEnumerable<T>> source, MissingCellAction missingCellAction = MissingCellAction.PadRight) {
    //     var              asJagged = source.AsJagged();
    //     ICollection<int> ar       = new int[] { 1, 2, 3 };
    // }

    public static IList<IList<T>> AsJagged<T>(this IEnumerable<IEnumerable<T>> source) {
        return source.Select(it => it.AsList())
                     .AsList();
    }

    public static (int x, int y) GetExtents<T>(this T[][] source) {
        var x = source.Max(it => it.Length);
        var y = source.Length;
        return (x, y);
    }
}