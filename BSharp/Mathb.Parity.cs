using System.Diagnostics.Contracts;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp {
    public static partial class Mathb {
        #region Parity

        #region Enum Parity

        #region Integral types

        [Pure]
        public static Parity Parity(this byte value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this sbyte value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this int value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this uint value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this long value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this ulong value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this short value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        [Pure]
        public static Parity Parity(this ushort value) => value % 2 == 0 ? Enums.Parity.Even : Enums.Parity.Odd;

        #endregion

        #region Floating-point types

        [Pure]
        public static Parity? Parity(this float value) => (value % 2) switch {
            0 => Enums.Parity.Even,
            1 => Enums.Parity.Odd,
            _ => null
        };

        [Pure]
        public static Parity? Parity(this double value) => (value % 2) switch {
            0 => Enums.Parity.Even,
            1 => Enums.Parity.Odd,
            _ => null
        };

        [Pure]
        public static Parity? Parity(this decimal value) => (value % 2) switch {
            0 => Enums.Parity.Even,
            1 => Enums.Parity.Odd,
            _ => null
        };

        #endregion

        #endregion

        #region IsEven

        [Pure]
        public static bool IsEven(this int value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this uint value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this long value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this ulong value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this short value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this ushort value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this float value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this double value) => value.Parity() == Enums.Parity.Even;

        [Pure]
        public static bool IsEven(this decimal value) => value.Parity() == Enums.Parity.Even;

        #endregion

        #region IsNotEven

        [Pure]
        public static bool IsNotEven(this int value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this uint value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this long value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this ulong value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this short value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this ushort value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this float value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this double value) => value.Parity() != Enums.Parity.Even;

        [Pure]
        public static bool IsNotEven(this decimal value) => value.Parity() != Enums.Parity.Even;

        #endregion

        #endregion
    }
}