using System;
using System.Diagnostics.Contracts;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        #region Pow

        [Pure]
        public static double Pow(this byte value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this sbyte value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this short value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this ushort value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this int value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this uint value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this long value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this ulong value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this double value, double exponent) => Math.Pow(value, exponent);

        [Pure]
        public static double Pow(this float value, double exponent) => Math.Pow(value, exponent);

        #endregion

        #region Squared

        [Pure]
        public static int Squared(this byte value) => value * value;

        [Pure]
        public static int Squared(this sbyte value) => value * value;

        [Pure]
        public static int Squared(this short value) => value * value;

        [Pure]
        public static int Squared(this ushort value) => value * value;

        [Pure]
        public static int Squared(this int value) => value * value;

        [Pure]
        public static uint Squared(this uint value) => value * value;

        [Pure]
        public static long Squared(this long value) => value * value;

        [Pure]
        public static ulong Squared(this ulong value) => value * value;

        [Pure]
        public static float Squared(this float value) => value * value;

        [Pure]
        public static double Squared(this double value) => value * value;

        [Pure]
        public static decimal Squared(this decimal value) => value * value;

        #endregion

        #region Cubed

        [Pure]
        public static int Cubed(this byte value) => value * value * value;

        [Pure]
        public static int Cubed(this sbyte value) => value * value * value;

        [Pure]
        public static int Cubed(this short value) => value * value * value;

        [Pure]
        public static int Cubed(this ushort value) => value * value * value;

        [Pure]
        public static int Cubed(this int value) => value * value * value;

        [Pure]
        public static uint Cubed(this uint value) => value * value * value;

        [Pure]
        public static long Cubed(this long value) => value * value * value;

        [Pure]
        public static ulong Cubed(this ulong value) => value * value * value;

        [Pure]
        public static float Cubed(this float value) => value * value * value;

        [Pure]
        public static double Cubed(this double value) => value * value * value;

        [Pure]
        public static decimal Cubed(this decimal value) => value * value * value;

        #endregion
    }
}