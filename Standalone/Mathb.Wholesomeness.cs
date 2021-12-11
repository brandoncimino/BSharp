using System;
using System.Diagnostics.Contracts;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        #region IsWhole

        [Pure]
        public static bool IsWhole(this double value) => value % 1 == 0;

        [Pure]
        public static bool IsWhole(this float value) => value % 1 == 0;

        [Pure]
        public static bool IsWhole(this decimal value) => value % 1 == 0;

        #endregion

        #region MustBeWhole

        [Pure]
        public static double MustBeWhole(this double value) => value.IsWhole() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be a whole number!");

        [Pure]
        public static float MustBeWhole(this float value) => value.IsWhole() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be a whole number!");

        [Pure]
        public static decimal MustBeWhole(this decimal value) => value.IsWhole() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be a whole number!");

        #endregion

        #region AsInt

        [Pure]
        public static int? AsInt(this double value) => value.IsWhole() && value <= int.MaxValue ? value.ToInt() : default;

        [Pure]
        public static int? AsInt(this float value) => value.IsWhole() && value <= int.MaxValue ? value.ToInt() : default;

        [Pure]
        public static int? AsInt(this decimal value) => value.IsWhole() && value <= int.MaxValue ? value.ToInt() : default;

        [Pure]
        public static int? AsInt(this uint value) => value <= int.MaxValue ? value.ToInt() : default;

        [Pure]
        public static int? AsInt(this long value) => value >= int.MinValue && value <= int.MaxValue ? value.ToInt() : default;

        [Pure]
        public static int? AsInt(this ulong value) => value <= int.MaxValue ? value.ToInt() : default;

        #endregion

        #region MustBeInt

        [Pure]
        public static int MustBeInt(this double value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

        [Pure]
        public static int MustBeInt(this float value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

        [Pure]
        public static int MustBeInt(this decimal value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

        [Pure]
        public static int MustBeInt(this uint value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

        [Pure]
        public static int MustBeInt(this long value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

        [Pure]
        public static int MustBeInt(this ulong value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

        #endregion

        #region AsLong

        [Pure]
        public static long? AsLong(this double value) => value.IsWhole() && value <= long.MaxValue ? value.ToLong() : default;

        [Pure]
        public static long? AsLong(this float value) => value.IsWhole() && value <= long.MaxValue ? value.ToLong() : default;

        [Pure]
        public static long? AsLong(this decimal value) => value.IsWhole() && value <= long.MaxValue ? value.ToLong() : default;

        [Pure]
        public static long? AsLong(this ulong value) => value <= long.MaxValue ? value.ToLong() : default;

        #endregion

        #region MustBeLong

        [Pure]
        public static long MustBeLong(this double value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");

        [Pure]
        public static long MustBeLong(this float value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");

        [Pure]
        public static long MustBeLong(this decimal value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");

        [Pure]
        public static long MustBeLong(this ulong value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");

        #endregion

        #region Finality

        [Pure]
        public static bool IsInfinite(this double value) => double.IsInfinity(value);

        [Pure]
        public static bool IsInfinite(this float value) => float.IsInfinity(value);

        [Pure]
        public static bool IsPositiveInfinity(this double value) => double.IsPositiveInfinity(value);

        [Pure]
        public static bool IsPositiveInfinity(this float value) => float.IsPositiveInfinity(value);

        [Pure]
        public static bool IsNegativeInfinity(this double value) => double.IsNegativeInfinity(value);

        [Pure]
        public static bool IsNegativeInfinity(this float value) => float.IsNegativeInfinity(value);

        #endregion
    }
}