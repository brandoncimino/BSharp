using System;

using BrandonUtils.Standalone.Enums;

using JetBrains.Annotations;

// using UnityEngine;

namespace BrandonUtils.Standalone {
    /// <summary>
    /// Contains cute extension methods for primitive types, allowing things like <c>5.4f.Clamp01()</c>
    ///
    /// TODO: The reference to UnityEngine should be removed, and this should be moved into <see cref="Standalone"/>.
    /// </summary>
    /// <remarks>
    /// GET IT! <see cref="Mathb"/>! Like <c>"Mathf"</c>!
    /// </remarks>
    [PublicAPI]
    public static partial class Mathb {
        #region Clamp

        public static short   Clamp(this short   value, short   min, short   max) => value <= min ? min : value >= max ? max : value;
        public static int     Clamp(this int     value, int     min, int     max) => value <= min ? min : value >= max ? max : value;
        public static long    Clamp(this long    value, long    min, long    max) => value <= min ? min : value >= max ? max : value;
        public static float   Clamp(this float   value, float   min, float   max) => value <= min ? min : value >= max ? max : value;
        public static double  Clamp(this double  value, double  min, double  max) => value <= min ? min : value >= max ? max : value;
        public static decimal Clamp(this decimal value, decimal min, decimal max) => value <= min ? min : value >= max ? max : value;
        public static ushort  Clamp(this ushort  value, ushort  min, ushort  max) => value <= min ? min : value >= max ? max : value;
        public static uint    Clamp(this uint    value, uint    min, uint    max) => value <= min ? min : value >= max ? max : value;
        public static ulong   Clamp(this ulong   value, ulong   min, ulong   max) => value <= min ? min : value >= max ? max : value;
        public static byte    Clamp(this byte    value, byte    min, byte    max) => value <= min ? min : value >= max ? max : value;
        public static sbyte   Clamp(this sbyte   value, sbyte   min, sbyte   max) => value <= min ? min : value >= max ? max : value;

        #region Clamp01

        public static float   Clamp01(this float   value) => value.Clamp(0, 1);
        public static double  Clamp01(this double  value) => value.Clamp(0, 1);
        public static decimal Clamp01(this decimal value) => value.Clamp(0, 1);

        #endregion

        #endregion

        #region Min

        #region Min (two inputs)

        public static short   Min(this short   value, short   other) => Math.Min(value, other);
        public static int     Min(this int     value, int     other) => Math.Min(value, other);
        public static long    Min(this long    value, long    other) => Math.Min(value, other);
        public static float   Min(this float   value, float   other) => Math.Min(value, other);
        public static double  Min(this double  value, double  other) => Math.Min(value, other);
        public static decimal Min(this decimal value, decimal other) => Math.Min(value, other);
        public static ushort  Min(this ushort  value, ushort  other) => Math.Min(value, other);
        public static uint    Min(this uint    value, uint    other) => Math.Min(value, other);
        public static ulong   Min(this ulong   value, ulong   other) => Math.Min(value, other);
        public static byte    Min(this byte    value, byte    other) => Math.Min(value, other);
        public static sbyte   Min(this sbyte   value, sbyte   other) => Math.Min(value, other);

        #endregion

        #region Min (Tuple)

        #region Min (Tuple2)

        public static short   Min(this (short, short )    tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static int     Min(this (int, int )        tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static long    Min(this (long, long )      tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static float   Min(this (float, float )    tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static double  Min(this (double, double )  tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static decimal Min(this (decimal, decimal) tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static ushort  Min(this (ushort, ushort )  tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static uint    Min(this (uint, uint )      tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static ulong   Min(this (ulong, ulong )    tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static byte    Min(this (byte, byte )      tuple) => Math.Min(tuple.Item1, tuple.Item2);
        public static sbyte   Min(this (sbyte, sbyte )    tuple) => Math.Min(tuple.Item1, tuple.Item2);

        #endregion

        #endregion

        #endregion

        #region Max

        #region Max (two inputs)

        public static short   Max(this short   value, short   other) => Math.Max(value, other);
        public static int     Max(this int     value, int     other) => Math.Max(value, other);
        public static long    Max(this long    value, long    other) => Math.Max(value, other);
        public static float   Max(this float   value, float   other) => Math.Max(value, other);
        public static double  Max(this double  value, double  other) => Math.Max(value, other);
        public static decimal Max(this decimal value, decimal other) => Math.Max(value, other);
        public static ushort  Max(this ushort  value, ushort  other) => Math.Max(value, other);
        public static uint    Max(this uint    value, uint    other) => Math.Max(value, other);
        public static ulong   Max(this ulong   value, ulong   other) => Math.Max(value, other);
        public static byte    Max(this byte    value, byte    other) => Math.Max(value, other);
        public static sbyte   Max(this sbyte   value, sbyte   other) => Math.Max(value, other);

        #endregion

        #region Max (Tuple)

        #region Max (Tuple2)

        public static short   Max(this (short, short )    tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static int     Max(this (int, int )        tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static long    Max(this (long, long )      tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static float   Max(this (float, float )    tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static double  Max(this (double, double )  tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static decimal Max(this (decimal, decimal) tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static ushort  Max(this (ushort, ushort )  tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static uint    Max(this (uint, uint )      tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static ulong   Max(this (ulong, ulong )    tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static byte    Max(this (byte, byte )      tuple) => Math.Max(tuple.Item1, tuple.Item2);
        public static sbyte   Max(this (sbyte, sbyte )    tuple) => Math.Max(tuple.Item1, tuple.Item2);

        #endregion

        #endregion

        #endregion

        #region Rounding

        #region Floor

        public static float   Floor(this      float   value) => (float)Math.Floor(value);
        public static double  Floor(this      double  value) => Math.Floor(value);
        public static decimal Floor(this      decimal value) => Math.Floor(value);
        public static int     FloorToInt(this float   value) => (int)Math.Floor(value);
        public static int     FloorToInt(this double  value) => (int)Math.Floor(value);
        public static int     FloorToInt(this decimal value) => (int)Math.Floor(value);

        #endregion

        #region Ceiling

        [Pure]
        public static float Ceiling(this float value) => (float)Math.Ceiling(value);

        [Pure]
        public static double Ceiling(this double value) => Math.Ceiling(value);

        [Pure]
        public static decimal Ceiling(this decimal value) => Math.Ceiling(value);

        [Pure]
        public static int CeilingToInt(this float value) => (int)Math.Ceiling(value);

        [Pure]
        public static int CeilingToInt(this double value) => (int)Math.Ceiling(value);

        [Pure]
        public static int CeilingToInt(this decimal value) => (int)Math.Ceiling(value);

        #endregion

        #region Round

        [Pure]
        public static double Round(this float value) => Math.Round(value);

        [Pure]
        public static double Round(this double value) => Math.Round(value);

        [Pure]
        public static decimal Round(this decimal value) => Math.Round(value);

        [Pure]
        public static double Round(this float value, MidpointRounding mode) => Math.Round(value, mode);

        [Pure]
        public static double Round(this double value, MidpointRounding mode) => Math.Round(value, mode);

        [Pure]
        public static decimal Round(this decimal value, MidpointRounding mode) => Math.Round(value, mode);

        [Pure]
        public static double Round(this float value, int digits) => Math.Round(value, digits);

        [Pure]
        public static double Round(this double value, int digits) => Math.Round(value, digits);

        [Pure]
        public static decimal Round(this decimal value, int decimals) => Math.Round(value, decimals);

        [Pure]
        public static double Round(this float value, int digits, MidpointRounding mode) => Math.Round(value, digits);

        [Pure]
        public static double Round(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits);

        [Pure]
        public static decimal Round(this decimal value, int decimals, MidpointRounding mode) => Math.Round(value, decimals);

        [Pure]
        public static int RoundToInt(this float value) => Math.Round(value).ToInt();

        [Pure]
        public static int RoundToInt(this double value) => Math.Round(value).ToInt();

        [Pure]
        public static int RoundToInt(this decimal value) => Math.Round(value).ToInt();

        [Pure]
        public static int RoundToInt(this float value, MidpointRounding mode) => Math.Round(value, mode).ToInt();

        [Pure]
        public static int RoundToInt(this double value, MidpointRounding mode) => Math.Round(value, mode).ToInt();

        [Pure]
        public static int RoundToInt(this decimal value, MidpointRounding mode) => Math.Round(value, mode).ToInt();

        [Pure]
        public static int RoundToInt(this float value, int digits) => Math.Round(value, digits).ToInt();

        [Pure]
        public static int RoundToInt(this double value, int digits) => Math.Round(value, digits).ToInt();

        [Pure]
        public static int RoundToInt(this decimal value, int decimals) => Math.Round(value, decimals).ToInt();

        [Pure]
        public static int RoundToInt(this float value, int digits, MidpointRounding mode) => Math.Round(value, digits).ToInt();

        [Pure]
        public static int RoundToInt(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits).ToInt();

        [Pure]
        public static int RoundToInt(this decimal value, int decimals, MidpointRounding mode) => Math.Round(value, decimals).ToInt();

        #endregion

        #region WouldRound

        public static RoundingDirection WouldRound(this float   value) => value.Fraction() >= 0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;
        public static RoundingDirection WouldRound(this double  value) => value.Fraction() >= 0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;
        public static RoundingDirection WouldRound(this decimal value) => value.Fraction() >= (decimal)0.5 ? RoundingDirection.Ceiling : RoundingDirection.Floor;

        #endregion

        #endregion
    }
}