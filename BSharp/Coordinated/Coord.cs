using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Coordinated;

/// <summary>
/// A simple <see cref="X"/>,<see cref="Y"/> pair used as an index for <see cref="ICoordinated{T}"/> collections.
///
/// TODO: Maybe convert this to a pair of <see cref="Indexes"/>?
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
public readonly record struct Coord(int X, int Y) {
    public static readonly Coord Zero   = new(0, 0);
    public static readonly Coord Origin = Zero;

    public (ComparisonResult x, ComparisonResult y) CompareEach(Coord other) => (X, Y).CompareEach((other.X, other.Y));
    public ComparisonResult?                        CompareAll(Coord  other) => (X, Y).CompareAll((other.X, other.Y));

    public Coord UpperBound(Coord other) => new(
        X.Max(other.X),
        Y.Max(other.Y)
    );

    public Coord LowerBound(Coord other) => new(
        X.Min(other.X),
        Y.Min(other.Y)
    );

    #region Casts

    public static implicit operator Coord((int x, int y) coord) => new(coord.x, coord.y);

    #endregion

    #region Static utilities

    public static Coord FromIndices((Index x, Index y) indices, (int width, int height) dimensions) {
        var xOff = indices.x.GetOffset(dimensions.width);
        var yOff = indices.y.GetOffset(dimensions.height);
        return new Coord(xOff, yOff);
    }

    public static Coord UpperBound(IEnumerable<Coord> coords)                                => coords.Aggregate((a, b) => a.UpperBound(b));
    public static Coord LowerBound(IEnumerable<Coord> coords)                                => coords.Aggregate((a, b) => a.LowerBound(b));
    public static Coord UpperBound(Coord              a, Coord b, params Coord[] additional) => UpperBound(additional.Append(a).Append(b));
    public static Coord LowerBound(Coord              a, Coord b, params Coord[] additional) => LowerBound(additional.Append(a).Append(b));

    public static (Coord lower, Coord upper) OuterBounds(IEnumerable<Coord> coords)                                => coords.DoubleAggro((a, b) => a.LowerBound(b), (a, b) => a.UpperBound(b));
    public static (Coord lower, Coord upper) OuterBounds(Coord              a, Coord b, params Coord[] additional) => OuterBounds(additional.Append(a).Append(b));

    #endregion
}