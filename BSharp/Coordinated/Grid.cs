using System;
using System.Collections;
using System.Collections.Generic;

namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// An <see cref="ICoordinated{T}"/> implementation that:
/// <ul>
/// <li>Is indexed by <c>(0,0)</c></li>
/// <li>Can have its cells updated</li>
/// <li>Cannot be resized</li>
/// </ul>
/// </summary>
/// <remarks>
/// <b>Performance</b>
/// <br/>
/// </remarks>
/// <typeparam name="T"></typeparam>
public abstract record Grid<T> : ICoordinated<T> {
    #region Abstract

    public abstract T this[Coord coord] { get; set; }
    public abstract int Width  { get; }
    public abstract int Height { get; }

    #endregion

    #region Implemented

    public T this[(Index x, Index y) coord] {
        get => this[Coord.FromIndices(coord, Dimensions)];
        set => this[Coord.FromIndices(coord, Dimensions)] = value;
    }
    public T this[Index x, Index y] {
        get => this[Coord.FromIndices((x, y), Dimensions)];
        set => this[Coord.FromIndices((x, y), Dimensions)] = value;
    }

    public (int width, int height) Dimensions => (Width, Height);

    #endregion

    #region Factory Methods

    public static Grid<T> Of(T[,]     source)                        => new OfArray(source);
    public static Grid<T> Of(IList<T> source, int width, int height) => new OfMultiDimensional(new MultiDimensional<T>(source, width, height));

    #endregion

    #region Enumerators

    public IEnumerator<KeyValuePair<Coord, T>> GetEnumerator() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Width; y++) {
                yield return new KeyValuePair<Coord, T>((x, y), this[x, y]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    #endregion

    #region Implementations

    private sealed record OfArray(T[,] Source) : Grid<T> {
        public override T this[Coord coord] {
            get => Source[coord.X, coord.Y];
            set => Source[coord.X, coord.Y] = value;
        }
        public override int Width  => Source.GetLength(0);
        public override int Height => Source.GetLength(1);
    }

    private sealed record OfMultiDimensional(MultiDimensional<T> Source) : Grid<T> {
        public override T this[Coord coord] {
            get => Source[coord.X, coord.Y];
            set => Source[coord.X, coord.Y] = value;
        }
        public override int Width  => Source.Dimensions[0];
        public override int Height => Source.Dimensions[1];
    }

    private sealed record OfIndexes(Griderable Griderable) : Grid<Coord> {
        public override Coord this[Coord coord] {
            get => Griderable[coord];
            set => throw new NotSupportedException();
        }
        public override int Width  => Griderable.Width;
        public override int Height => Griderable.Height;
    }

    #endregion
}