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

    [System.Diagnostics.Contracts.Pure]
    public static short Clamp(this short value, short min, short max = short.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static int Clamp(this int value, int min, int max = int.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static long Clamp(this long value, long min, long max = long.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static float Clamp(this float value, float min, float max = float.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static double Clamp(this double value, double min, double max = double.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static decimal Clamp(this decimal value, decimal min, decimal max = decimal.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static ushort Clamp(this ushort value, ushort min, ushort max = ushort.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static uint Clamp(this uint value, uint min, uint max = uint.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static ulong Clamp(this ulong value, ulong min, ulong max = ulong.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static byte Clamp(this byte value, byte min, byte max = byte.MaxValue) => value <= min ? min : value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static sbyte Clamp(this sbyte value, sbyte min, sbyte max = sbyte.MaxValue) => value <= min ? min : value >= max ? max : value;

    #region Clamp01

    [System.Diagnostics.Contracts.Pure]
    public static float Clamp01(this float value) => value.Clamp(0, 1);

    [System.Diagnostics.Contracts.Pure]
    public static double Clamp01(this double value) => value.Clamp(0, 1);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Clamp01(this decimal value) => value.Clamp(0, 1);

    #endregion

    #region Limit

    [System.Diagnostics.Contracts.Pure]
    public static short Limit(this short value, short max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static int Limit(this int value, int max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static long Limit(this long value, long max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static float Limit(this float value, float max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static double Limit(this double value, double max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static decimal Limit(this decimal value, decimal max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static ushort Limit(this ushort value, ushort max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static uint Limit(this uint value, uint max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static ulong Limit(this ulong value, ulong max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static byte Limit(this byte value, byte max) => value >= max ? max : value;

    [System.Diagnostics.Contracts.Pure]
    public static sbyte Limit(this sbyte value, sbyte max) => value >= max ? max : value;

    #endregion

    #endregion

    #region Min

    #region Min (two inputs)

    [System.Diagnostics.Contracts.Pure]
    public static short Min(this short value, short other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static int Min(this int value, int other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static long Min(this long value, long other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static float Min(this float value, float other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static double Min(this double value, double other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Min(this decimal value, decimal other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static ushort Min(this ushort value, ushort other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static uint Min(this uint value, uint other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static ulong Min(this ulong value, ulong other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static byte Min(this byte value, byte other) => Math.Min(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static sbyte Min(this sbyte value, sbyte other) => Math.Min(value, other);

    #region IComparable

    [System.Diagnostics.Contracts.Pure]
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

    [System.Diagnostics.Contracts.Pure]
    public static short Min(this (short, short ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static int Min(this (int, int ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static long Min(this (long, long ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static float Min(this (float, float ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static double Min(this (double, double ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Min(this (decimal, decimal) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static ushort Min(this (ushort, ushort ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static uint Min(this (uint, uint ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static ulong Min(this (ulong, ulong ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static byte Min(this (byte, byte ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static sbyte Min(this (sbyte, sbyte ) tuple) => Math.Min(tuple.Item1, tuple.Item2);

    #endregion

    #region Min (Memberwise Tuple2)

    [System.Diagnostics.Contracts.Pure]
    public static (T, T2) Min<T, T2>((T, T2) a, (T, T2) b)
        where T : IComparable<T>
        where T2 : IComparable<T2> =>
        (a.Item1.Min(b.Item1), a.Item2.Min(b.Item2));

    #endregion

    #endregion

    #endregion

    #region Max

    #region Max (two inputs)

    [System.Diagnostics.Contracts.Pure]
    public static short Max(this short value, short other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static int Max(this int value, int other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static long Max(this long value, long other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static float Max(this float value, float other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static double Max(this double value, double other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Max(this decimal value, decimal other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static ushort Max(this ushort value, ushort other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static uint Max(this uint value, uint other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static ulong Max(this ulong value, ulong other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static byte Max(this byte value, byte other) => Math.Max(value, other);

    [System.Diagnostics.Contracts.Pure]
    public static sbyte Max(this sbyte value, sbyte other) => Math.Max(value, other);

    #region IComparable

    [System.Diagnostics.Contracts.Pure]
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

    [System.Diagnostics.Contracts.Pure]
    public static short Max(this (short, short ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static int Max(this (int, int ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static long Max(this (long, long ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static float Max(this (float, float ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static double Max(this (double, double ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Max(this (decimal, decimal) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static ushort Max(this (ushort, ushort ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static uint Max(this (uint, uint ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static ulong Max(this (ulong, ulong ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static byte Max(this (byte, byte ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    [System.Diagnostics.Contracts.Pure]
    public static sbyte Max(this (sbyte, sbyte ) tuple) => Math.Max(tuple.Item1, tuple.Item2);

    #endregion

    #region Memberwise Tuple2

    [System.Diagnostics.Contracts.Pure]
    public static (T, T2) Max<T, T2>((T, T2) a, (T, T2) b)
        where T : IComparable<T>
        where T2 : IComparable<T2> =>
        (a.Item1.Max(b.Item1), a.Item2.Max(b.Item2));

    #endregion

    #endregion

    #endregion

    #region Rounding

    #region Floor

    [System.Diagnostics.Contracts.Pure]
    public static float Floor(this float value) => (float)Math.Floor(value);

    [System.Diagnostics.Contracts.Pure]
    public static double Floor(this double value) => Math.Floor(value);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Floor(this decimal value) => Math.Floor(value);

    [System.Diagnostics.Contracts.Pure]
    public static int FloorToInt(this float value) => (int)Math.Floor(value);

    [System.Diagnostics.Contracts.Pure]
    public static int FloorToInt(this double value) => (int)Math.Floor(value);

    [System.Diagnostics.Contracts.Pure]
    public static int FloorToInt(this decimal value) => (int)Math.Floor(value);

    #endregion

    #region Ceiling

    [System.Diagnostics.Contracts.Pure]
    public static float Ceiling(this float value) => (float)Math.Ceiling(value);

    [System.Diagnostics.Contracts.Pure]
    public static double Ceiling(this double value) => Math.Ceiling(value);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Ceiling(this decimal value) => Math.Ceiling(value);

    [System.Diagnostics.Contracts.Pure]
    public static int CeilingToInt(this float value) => (int)Math.Ceiling(value);

    [System.Diagnostics.Contracts.Pure]
    public static int CeilingToInt(this double value) => (int)Math.Ceiling(value);

    [System.Diagnostics.Contracts.Pure]
    public static int CeilingToInt(this decimal value) => (int)Math.Ceiling(value);

    #endregion

    #region Round

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this float value) => Math.Round(value);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this double value) => Math.Round(value);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Round(this decimal value) => Math.Round(value);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this float value, MidpointRounding mode) => Math.Round(value, mode);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this double value, MidpointRounding mode) => Math.Round(value, mode);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Round(this decimal value, MidpointRounding mode) => Math.Round(value, mode);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this float value, RoundingDirection direction) => Rounder.Get(direction).Round(value);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this double value, RoundingDirection direction) => Rounder.Get(direction).Round(value);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Round(this decimal value, RoundingDirection direction) => Rounder.Get(direction).Round(value);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this float value, int digits) => Math.Round(value, digits);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this double value, int digits) => Math.Round(value, digits);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Round(this decimal value, int decimals) => Math.Round(value, decimals);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this float value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);

    [System.Diagnostics.Contracts.Pure]
    public static decimal Round(this decimal value, int decimals, MidpointRounding mode) => Math.Round(value, decimals, mode);

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this float value, int digits, RoundingDirection direction) => value.Round(direction).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static double Round(this double value, int digits, RoundingDirection direction) => value.Round(direction).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static decimal Round(this decimal value, int decimals, RoundingDirection direction) => value.Round(direction).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this float value) => Math.Round(value).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this double value) => Math.Round(value).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this decimal value) => Math.Round(value).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this float value, MidpointRounding mode) => Math.Round(value, mode).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this double value, MidpointRounding mode) => Math.Round(value, mode).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this decimal value, MidpointRounding mode) => Math.Round(value, mode).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this float value, RoundingDirection direction) => Rounder.Get(direction).RoundToInt(value);

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this double value, RoundingDirection direction) => Rounder.Get(direction).RoundToInt(value);

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this decimal value, RoundingDirection direction) => Rounder.Get(direction).RoundToInt(value);

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this float value, int digits) => Math.Round(value, digits).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this double value, int digits) => Math.Round(value, digits).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this decimal value, int decimals) => Math.Round(value, decimals).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this float value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static int RoundToInt(this decimal value, int decimals, MidpointRounding mode) => Math.Round(value, decimals, mode).ToInt();

    [System.Diagnostics.Contracts.Pure]
    public static double RoundToInt(this float value, int digits, RoundingDirection direction) => Rounder.Get(direction).RoundToInt(value);

    [System.Diagnostics.Contracts.Pure]
    public static double RoundToInt(this double value, int digits, RoundingDirection direction) => Rounder.Get(direction).RoundToInt(value);

    [System.Diagnostics.Contracts.Pure]
    public static decimal RoundToInt(this decimal value, int decimals, RoundingDirection direction) => Rounder.Get(direction).RoundToInt(value);

    #endregion

    #region WouldRound

    [System.Diagnostics.Contracts.Pure]
    public static RoundingDirection WouldRound(this float value) => value.Fraction() >= 0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;

    [System.Diagnostics.Contracts.Pure]
    public static RoundingDirection WouldRound(this double value) => value.Fraction() >= 0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;

    [System.Diagnostics.Contracts.Pure]
    public static RoundingDirection WouldRound(this decimal value) => value.Fraction() >= (decimal)0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;

    #endregion

    #endregion
}