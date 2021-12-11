using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        [Pure]
        public static double ShiftDecimal(this double value, int decimalPlaces) => value == 0 ? 0 : value * Math.Pow(10, decimalPlaces);

        [Pure]
        public static double ShiftDecimal(this int value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);

        [Pure]
        public static double ShiftDecimal(this uint value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);

        [Pure]
        public static double ShiftDecimal(this short value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);

        [Pure]
        public static double ShiftDecimal(this ushort value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);

        [Pure]
        public static float ShiftDecimal(this float value, int decimalPlaces) => value == 0 ? 0 : value * Math.Pow(10, decimalPlaces).ToFloat();

        [Pure]
        public static double ToPercentage(this double value) => value.ShiftDecimal(2);

        [Pure]
        public static float ToPercentage(this float value) => value.ShiftDecimal(2);
    }
}