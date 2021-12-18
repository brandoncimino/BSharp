using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp;

public record Grid<T> {
    public record Cell((int x, int y) Coordinate, T Value);

    public (int x, int y) Dimensions { get; init; }
    public int            X          => Dimensions.x;
    public int            Y          => Dimensions.y;

    private IDictionary<(int x, int y), T> Cells;

    public Grid(
        (int x, int y)                  dimensions,
        IDictionary<(int x, int y), T>? cells = default
    ) {
        Dimensions = dimensions;

        cells?.Keys.ForEach(it => ValidateCoordinate(it));
        Cells = cells ?? new Dictionary<(int x, int y), T>();
    }

    private Grid(IEnumerable<KeyValuePair<(int x, int y), T>> cells) : this(MaxDimensions(cells.Select(it => it.Key))) { }

    public Grid(T?[,] cells) : this(cells.Dimensions(), GetSparseDictionary(cells)) { }

    #region Helpers

    private static (int x, int y) MaxDimensions(IEnumerable<(int x, int y)> coordinates) {
        return coordinates.Min();
    }

    private static Dictionary<(int x, int y), T> GetSparseDictionary(T?[,] source) {
        var dic = new Dictionary<(int, int), T>();
        for (int x = 0; x < source.X(); x++) {
            for (int y = 0; y < source.Y(); y++) {
                var cell = source[x, y];
                if (cell != null) {
                    dic[(x, y)] = cell;
                }
            }
        }

        return dic;
    }

    #endregion

    #region Validation

    private (int x, int y) ValidateCoordinate((int x, int y) coord) {
        var (x, y) = coord;
        if (
            x < 0 ||
            x > X ||
            y < 0 ||
            y > Y
        ) {
            throw new IndexOutOfRangeException($"The coordinate {coord} is out-of-bounds for a {GetType().Prettify()} of size [{Dimensions}]!");
        }

        return (x, y);
    }

    #endregion

    #region Indexers

    public T? this[int x, int y] {
        get => this[(x, y)];
        set => this[(x, y)] = value;
    }

    public T? this[(int x, int y) coord] {
        get => Cells.ContainsKey(ValidateCoordinate(coord)) ? Cells[coord] : default;
        set {
            ValidateCoordinate(coord);
            if (value == null) {
                Cells.Remove(coord);
            }
            else {
                Cells[coord] = value;
            }
        }
    }

    #endregion

    #region Conversions

    public T[,] ToArray() {
        var ar = new T[X, Y];
        foreach (var cell in Cells) {
            var (x, y) = cell.Key;
            ar[x, y]   = cell.Value;
        }

        return ar;
    }

    #endregion

    #region Linq Methods

    public Grid<TNew> Select<TNew>(GridExtensions.GridFunction<T, TNew> selector) {
        return new Grid<TNew>(Cells.ToDictionary((coord, cell) => selector(coord.x, coord.y, cell)));
    }

    #endregion
}