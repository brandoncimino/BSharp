using System;

using FowlFever.BSharp.Enums;

using JetBrains.Annotations;

namespace FowlFever.BSharp;

/// <summary>
/// Contains cute extension methods for primitive types, allowing things like <c>5.4f.Clamp01()</c>
/// </summary>
/// <remarks>
/// GET IT! <see cref="Mathb"/>! Like <c>"Mathf"</c>!
/// </remarks>
[PublicAPI]
public static partial class Mathb {
    #region Clamp

    //TODO: alternatively, the Clamp methods could accept a nullable types - but maybe that would be better with a custom "Brange" struct?

    [Pure] public static short   Clamp(this short   value, short   min, short   max = short.MaxValue)   => Math.Clamp(value, min, max);
    [Pure] public static int     Clamp(this int     value, int     min, int     max = int.MaxValue)     => Math.Clamp(value, min, max);
    [Pure] public static long    Clamp(this long    value, long    min, long    max = long.MaxValue)    => Math.Clamp(value, min, max);
    [Pure] public static float   Clamp(this float   value, float   min, float   max = float.MaxValue)   => Math.Clamp(value, min, max);
    [Pure] public static double  Clamp(this double  value, double  min, double  max = double.MaxValue)  => Math.Clamp(value, min, max);
    [Pure] public static decimal Clamp(this decimal value, decimal min, decimal max = decimal.MaxValue) => Math.Clamp(value, min, max);
    [Pure] public static ushort  Clamp(this ushort  value, ushort  min, ushort  max = ushort.MaxValue)  => Math.Clamp(value, min, max);
    [Pure] public static uint    Clamp(this uint    value, uint    min, uint    max = uint.MaxValue)    => Math.Clamp(value, min, max);
    [Pure] public static ulong   Clamp(this ulong   value, ulong   min, ulong   max = ulong.MaxValue)   => Math.Clamp(value, min, max);
    [Pure] public static byte    Clamp(this byte    value, byte    min, byte    max = byte.MaxValue)    => Math.Clamp(value, min, max);
    [Pure] public static sbyte   Clamp(this sbyte   value, sbyte   min, sbyte   max = sbyte.MaxValue)   => Math.Clamp(value, min, max);

    /// <summary>
    /// Limits an <see cref="Index"/> so that it is valid for a collection of <paramref name="length"/> items: if the <see cref="Index.GetOffset"/> would be out-of-bounds,
    /// returns either <see cref="Index.Start"/> or <see cref="Index.End"/> as appropriate.
    /// </summary>
    /// <param name="index">this <see cref="Index"/></param>
    /// <param name="length">the number of items in a theoretically indexable collection</param>
    /// <returns><see cref="Index.Start"/>, <see cref="Index.End"/>, or the original <paramref name="index"/></returns>
    [Pure]
    public static Index Clamp(this Index index, int length) {
        var offset = index.GetOffset(length);
        return offset switch {
            < 0                     => Index.Start,
            _ when offset >= length => Index.End,
            _                       => index
        };
    }

    /// <summary>
    /// <see cref="Clamp(Index, int)"/>s both the <see cref="Range.Start"/> and <see cref="Range.End"/>.
    /// </summary>
    /// <param name="range">this <see cref="Range"/></param>
    /// <param name="length">the number of items in a theoretically indexable collection</param>
    /// <returns>(<see cref="Range.Start"/>..<see cref="Range.End"/>), <see cref="Clamp(Index, int)"/>ped, as a new <see cref="Range"/></returns>
    [Pure]
    public static Range Clamp(this Range range, int length) {
        return range.Start.Clamp(length)..range.End.Clamp(length);
    }

    #region Clamp01

    [Pure] public static float   Clamp01(this float   value) => value.Clamp(0, 1);
    [Pure] public static double  Clamp01(this double  value) => value.Clamp(0, 1);
    [Pure] public static decimal Clamp01(this decimal value) => value.Clamp(0, 1);

    #endregion

    #region Limit

    //TODO: Should the `Limit` methods work with negative numbers? For example: -10.Limit(5) => -5 or -10.Limit(-5) => -5

    [Pure, NonNegativeValue] public static short   Limit(this short   value, short   max) => Math.Clamp(value, default, Math.Abs(max));
    [Pure, NonNegativeValue] public static int     Limit(this int     value, int     max) => Math.Clamp(value, default, Math.Abs(max));
    [Pure, NonNegativeValue] public static long    Limit(this long    value, long    max) => Math.Clamp(value, default, Math.Abs(max));
    [Pure, NonNegativeValue] public static float   Limit(this float   value, float   max) => Math.Clamp(value, default, Math.Abs(max));
    [Pure, NonNegativeValue] public static double  Limit(this double  value, double  max) => Math.Clamp(value, default, Math.Abs(max));
    [Pure, NonNegativeValue] public static decimal Limit(this decimal value, decimal max) => Math.Clamp(value, default, Math.Abs(max));
    [Pure, NonNegativeValue] public static ushort  Limit(this ushort  value, ushort  max) => Math.Clamp(value, default, max);
    [Pure, NonNegativeValue] public static uint    Limit(this uint    value, uint    max) => Math.Clamp(value, default, max);
    [Pure, NonNegativeValue] public static ulong   Limit(this ulong   value, ulong   max) => Math.Clamp(value, default, max);
    [Pure, NonNegativeValue] public static byte    Limit(this byte    value, byte    max) => Math.Clamp(value, default, max);
    [Pure, NonNegativeValue] public static sbyte   Limit(this sbyte   value, sbyte   max) => Math.Clamp(value, default, max);

    #endregion

    #endregion

    #region Min

    #region Min (two inputs)

    [Pure] public static short   Min(this short   value, short   other) => Math.Min(value, other);
    [Pure] public static int     Min(this int     value, int     other) => Math.Min(value, other);
    [Pure] public static long    Min(this long    value, long    other) => Math.Min(value, other);
    [Pure] public static float   Min(this float   value, float   other) => Math.Min(value, other);
    [Pure] public static double  Min(this double  value, double  other) => Math.Min(value, other);
    [Pure] public static decimal Min(this decimal value, decimal other) => Math.Min(value, other);
    [Pure] public static ushort  Min(this ushort  value, ushort  other) => Math.Min(value, other);
    [Pure] public static uint    Min(this uint    value, uint    other) => Math.Min(value, other);
    [Pure] public static ulong   Min(this ulong   value, ulong   other) => Math.Min(value, other);
    [Pure] public static byte    Min(this byte    value, byte    other) => Math.Min(value, other);
    [Pure] public static sbyte   Min(this sbyte   value, sbyte   other) => Math.Min(value, other);

    #region IComparable

    [Pure]
    public static T Min<T>(this T a, T b)
        where T : IComparable<T> {
        return a.CompareTo(b) switch {
            < 0 => a,
            0   => a,
            > 0 => b
        };
    }

    #endregion

    #endregion

    #region Min (Tuple)

    #region Min (Tuple2)

    [Pure] public static short   Min(this (short, short )    tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static int     Min(this (int, int )        tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static long    Min(this (long, long )      tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static float   Min(this (float, float )    tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static double  Min(this (double, double )  tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static decimal Min(this (decimal, decimal) tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static ushort  Min(this (ushort, ushort )  tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static uint    Min(this (uint, uint )      tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static ulong   Min(this (ulong, ulong )    tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static byte    Min(this (byte, byte )      tuple) => Math.Min(tuple.Item1, tuple.Item2);
    [Pure] public static sbyte   Min(this (sbyte, sbyte )    tuple) => Math.Min(tuple.Item1, tuple.Item2);

    #endregion

    #region Min (Memberwise Tuple2)

    [Pure]
    public static (T, T2) Min<T, T2>((T, T2) a, (T, T2) b)
        where T : IComparable<T>
        where T2 : IComparable<T2> =>
        (a.Item1.Min(b.Item1), a.Item2.Min(b.Item2));

    #endregion

    #endregion

    #endregion

    #region Max

    #region Max (two inputs)

    [Pure] public static short   Max(this short   value, short   other) => Math.Max(value, other);
    [Pure] public static int     Max(this int     value, int     other) => Math.Max(value, other);
    [Pure] public static long    Max(this long    value, long    other) => Math.Max(value, other);
    [Pure] public static float   Max(this float   value, float   other) => Math.Max(value, other);
    [Pure] public static double  Max(this double  value, double  other) => Math.Max(value, other);
    [Pure] public static decimal Max(this decimal value, decimal other) => Math.Max(value, other);
    [Pure] public static ushort  Max(this ushort  value, ushort  other) => Math.Max(value, other);
    [Pure] public static uint    Max(this uint    value, uint    other) => Math.Max(value, other);
    [Pure] public static ulong   Max(this ulong   value, ulong   other) => Math.Max(value, other);
    [Pure] public static byte    Max(this byte    value, byte    other) => Math.Max(value, other);
    [Pure] public static sbyte   Max(this sbyte   value, sbyte   other) => Math.Max(value, other);

    #region IComparable

    [Pure]
    public static T Max<T>(this T a, T b)
        where T : IComparable<T> {
        return a.CompareTo(b) switch {
            < 0 => b,
            0   => a,
            > 0 => a,
        };
    }

    #endregion

    #endregion

    #region Max (Tuple)

    #region Max (Tuple2)

    [Pure] public static short   Max(this (short, short )    tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static int     Max(this (int, int )        tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static long    Max(this (long, long )      tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static float   Max(this (float, float )    tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static double  Max(this (double, double )  tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static decimal Max(this (decimal, decimal) tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static ushort  Max(this (ushort, ushort )  tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static uint    Max(this (uint, uint )      tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static ulong   Max(this (ulong, ulong )    tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static byte    Max(this (byte, byte )      tuple) => Math.Max(tuple.Item1, tuple.Item2);
    [Pure] public static sbyte   Max(this (sbyte, sbyte )    tuple) => Math.Max(tuple.Item1, tuple.Item2);

    #endregion

    #region Memberwise Tuple2

    [Pure]
    public static (T, T2) Max<T, T2>((T, T2) a, (T, T2) b)
        where T : IComparable<T>
        where T2 : IComparable<T2> =>
        (a.Item1.Max(b.Item1), a.Item2.Max(b.Item2));

    #endregion

    #endregion

    #endregion

    #region Rounding

    #region Floor

    [Pure] public static float   Floor(this      float   value) => (float)Math.Floor(value);
    [Pure] public static double  Floor(this      double  value) => Math.Floor(value);
    [Pure] public static decimal Floor(this      decimal value) => Math.Floor(value);
    [Pure] public static int     FloorToInt(this float   value) => (int)Math.Floor(value);
    [Pure] public static int     FloorToInt(this double  value) => (int)Math.Floor(value);
    [Pure] public static int     FloorToInt(this decimal value) => (int)Math.Floor(value);

    #endregion

    #region Round

    /// <inheritdoc cref="Math.Round(double)"/>
    [Pure]
    public static double Round(this double value) => Math.Round(value);

    /// <inheritdoc cref="Math.Round(decimal)"/>
    [Pure]
    public static decimal Round(this decimal value) => Math.Round(value);

    /// <inheritdoc cref="Math.Round(double, MidpointRounding)"/>
    [Pure]
    public static double Round(this double value, MidpointRounding mode) => Math.Round(value, mode);

    /// <inheritdoc cref="Math.Round(decimal, MidpointRounding)"/>
    [Pure]
    public static decimal Round(this decimal value, MidpointRounding mode) => Math.Round(value, mode);

    [Pure] public static double  Round(this float   value, Rounder direction) => direction.Round(value);
    [Pure] public static double  Round(this double  value, Rounder direction) => direction.Round(value);
    [Pure] public static decimal Round(this decimal value, Rounder direction) => direction.Round(value);

    /// <inheritdoc cref="Math.Round(double, int)"/>
    [Pure]
    public static double Round(this double value, int digits) => Math.Round(value, digits);

    /// <inheritdoc cref="Math.Round(decimal, int)"/>
    [Pure]
    public static decimal Round(this decimal value, int decimals) => Math.Round(value, decimals);

    /// <inheritdoc cref="Math.Round(double, int, MidpointRounding)"/>
    [Pure]
    public static double Round(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);

    /// <inheritdoc cref="Math.Round(decimal, int, MidpointRounding)"/>
    [Pure]
    public static decimal Round(this decimal value, int decimals, MidpointRounding mode) => Math.Round(value, decimals, mode);

    [Pure] public static int     Round(this      float   value, int digits,   Rounder direction) => value.Round(direction).ToInt();
    [Pure] public static int     Round(this      double  value, int digits,   Rounder direction) => value.Round(direction).ToInt();
    [Pure] public static int     Round(this      decimal value, int decimals, Rounder direction) => value.Round(direction).ToInt();
    [Pure] public static int     RoundToInt(this float   value)                                    => Math.Round(value).ToInt();
    [Pure] public static int     RoundToInt(this double  value)                                    => Math.Round(value).ToInt();
    [Pure] public static int     RoundToInt(this decimal value)                                    => Math.Round(value).ToInt();
    [Pure] public static int     RoundToInt(this float   value, Rounder rounder)                   => rounder.RoundToInt(value);
    [Pure] public static int     RoundToInt(this double  value, Rounder rounder)                   => rounder.RoundToInt(value);
    [Pure] public static int     RoundToInt(this decimal value, Rounder rounder)                   => rounder.RoundToInt(value);
    [Pure] public static int     RoundToInt(this float   value, int     digits)                    => Math.Round(value, digits).ToInt();
    [Pure] public static int     RoundToInt(this double  value, int     digits)                    => Math.Round(value, digits).ToInt();
    [Pure] public static int     RoundToInt(this decimal value, int     decimals)                  => Math.Round(value, decimals).ToInt();
    [Pure] public static double  RoundToInt(this float   value, int     digits,   Rounder rounder) => rounder.RoundToInt(value);
    [Pure] public static double  RoundToInt(this double  value, int     digits,   Rounder rounder) => rounder.RoundToInt(value);
    [Pure] public static decimal RoundToInt(this decimal value, int     decimals, Rounder rounder) => rounder.RoundToInt(value);

    #endregion

    #region WouldRound

    [Pure] public static RoundingDirection WouldRound(this float   value) => value.Fraction() >= 0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;
    [Pure] public static RoundingDirection WouldRound(this double  value) => value.Fraction() >= 0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;
    [Pure] public static RoundingDirection WouldRound(this decimal value) => value.Fraction() >= (decimal)0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;

    #endregion

    #endregion

    #region DivRem

    /// <summary>
    /// An extension method for <see cref="Math.DivRem(int,int,out int)"/> that uses a modern <see cref="ValueTuple{T1,T2}"/> return value instead of an <c>out</c> parameter. 
    /// </summary>
    /// <param name="dividend">the "top" of the fraction</param>
    /// <param name="divisor">the "bottom" of the fraction</param>
    /// <returns>the (quotient, remainder) of the division</returns>
    public static (int quotient, int remainder) DivRem(this int dividend, int divisor) => (Math.DivRem(dividend, divisor, out var remainder), remainder);

    /// <inheritdoc cref="DivRem(int,int)"/>
    public static (long quotient, long remainder) DivRem(this long dividend, long divisor) => (Math.DivRem(dividend, divisor, out var remainder), remainder);

    /// <summary>
    /// Finds the index we'd end at if we traversed a collection of <paramref name="totalCount"/> items, turning around whenever we reach the end.
    ///
    /// TODO: Currently this will <b>repeat</b> the bounce point, i.e. for a count of <c>3</c> and 7 steps, we'd get <c>[0,1,2,3,3,2,1]</c> 
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="numberOfSteps"></param>
    /// <returns></returns>
    private static int CalculatePingPong(int totalCount, int numberOfSteps) {
        var quotient  = Math.DivRem(numberOfSteps, totalCount, out var remainder);
        var direction = Convert.ToBoolean(quotient % 2);
        var index     = new Index(remainder, direction);
        return index.GetOffset(totalCount);
    }

    #endregion
}