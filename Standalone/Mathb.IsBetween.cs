using BrandonUtils.Standalone.Enums;

// ReSharper disable ConvertConditionalTernaryExpressionToSwitchExpression

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        #region IsBetween

        public static bool IsBetween(this short   value, short   min, short   max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this int     value, int     min, int     max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this long    value, long    min, long    max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this float   value, float   min, float   max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this double  value, double  min, double  max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this decimal value, decimal min, decimal max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this ushort  value, ushort  min, ushort  max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this uint    value, uint    min, uint    max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this ulong   value, ulong   min, ulong   max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this byte    value, byte    min, byte    max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);
        public static bool IsBetween(this sbyte   value, sbyte   min, sbyte   max, Clusivity clusivity = Clusivity.Inclusive) => clusivity == Clusivity.Inclusive ? value >= min && value <= max : clusivity == Clusivity.Exclusive ? value > min && value < max : throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity);

        #endregion

        #region IsInRange

        public static bool IsInRange(this short   value, short   min, short   max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this int     value, int     min, int     max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this long    value, long    min, long    max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this float   value, float   min, float   max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this double  value, double  min, double  max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this decimal value, decimal min, decimal max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this ushort  value, ushort  min, ushort  max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this uint    value, uint    min, uint    max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this ulong   value, ulong   min, ulong   max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this byte    value, byte    min, byte    max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);
        public static bool IsInRange(this sbyte   value, sbyte   min, sbyte   max, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(min, max, clusivity);

        #endregion
    }
}