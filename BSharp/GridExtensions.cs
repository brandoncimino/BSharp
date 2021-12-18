using System;

namespace FowlFever.BSharp;

/// <summary>
/// Extension methods that operate on 2D <see cref="Array"/>s as well as the specialized <see cref="Grid{T}"/> type.
/// </summary>
public static class GridExtensions {
    public static (int x, int y) Dimensions<T>(this T[,] grid) {
        return (grid.X(), grid.Y());
    }

    /// <param name="grid">a 2D <see cref="Array"/></param>
    /// <typeparam name="T">the <see cref="Type"/> of the elements in <paramref name="grid"/></typeparam>
    /// <returns><paramref name="grid"/>.<see cref="Array.GetLength">GetLength</see>(0)</returns>
    /// <seealso cref="Y{T}"/>
    /// <seealso cref="Dimensions{T}"/>
    public static int X<T>(this T[,] grid) {
        return grid.GetLength(0);
    }

    /// <param name="grid">a 2D <see cref="Array"/></param>
    /// <typeparam name="T">the <see cref="Type"/> of the elements in <paramref name="grid"/></typeparam>
    /// <returns><paramref name="grid"/>.<see cref="Array.GetLength">GetLength</see>(1)</returns>
    /// <seealso cref="X{T}"/>
    /// <seealso cref="Dimensions{T}"/>
    public static int Y<T>(this T[,] grid) {
        return grid.GetLength(1);
    }

    public delegate TOut GridFunction<in TIn, out TOut>(int x, int y, TIn cell);

    public static TOut[,] Select<TIn, TOut>(this TIn[,] source, GridFunction<TIn, TOut> transformation) {
        var tf = new TOut[source.X(), source.Y()];

        for (int x = 0; x < source.X(); x++) {
            for (int y = 0; y < source.Y(); y++) {
                tf[x, y] = transformation(x, y, source[x, y]);
            }
        }

        return tf;
    }

    // public static TOut[,] Select<TIn, TOut>(this TIn[,] source, Func<int, int, TIn, TOut> transformation) {
    //     return source.Select(new GridFunction<TIn, TOut>(transformation));
    // }

    public static TOut[,] Select<TIn, TOut>(this TIn[,] source, Func<TIn, TOut> transformation) {
        var tf = new TOut[source.X(), source.Y()];
        for (int x = 0; x < source.X(); x++) {
            for (int y = 0; y < source.Y(); y++) {
                tf[x, y] = transformation(source[x, y]);
            }
        }

        return tf;
    }

    public static Grid<T> ToGrid<T>(this T[,] source) {
        return new Grid<T>(source);
    }
}