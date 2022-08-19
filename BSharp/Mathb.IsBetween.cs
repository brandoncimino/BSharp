using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

public static partial class Mathb {
    #region IsBetween

    [Pure]
    public static bool IsBetween(
        this int         value,
        (int, Clusivity) min,
        (int, Clusivity) max
    ) {
        var minBound = Bound.Min(min.Item1, min.Item2);
        var maxBound = Bound.Max(max.Item1, max.Item2);
        return value.IsBetween(minBound, maxBound);
    }

    public static bool IsBetween(
        this int          value,
        int               min,
        (int, Clusivity ) max
    ) {
        return value >= min && Bound.Max(max.Item1, max.Item2).Contains(value);
    }

    [Pure]
    public static bool IsBetween(
        this int      value,
        MinBound<int> min,
        MaxBound<int> max
    ) {
        return new Brange<int>(min, max).Contains(value);
    }

    [Pure]
    public static bool IsBetween(
        this short value,
        short      min,
        short      max,
        Clusivity  clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this int  value,
        int       min,
        int       max,
        Clusivity clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this long value,
        long      min,
        long      max,
        Clusivity clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this float value,
        float      min,
        float      max,
        Clusivity  clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this double value,
        double      min,
        double      max,
        Clusivity   clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this decimal value,
        decimal      min,
        decimal      max,
        Clusivity    clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this ushort value,
        ushort      min,
        ushort      max,
        Clusivity   clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this uint value,
        uint      min,
        uint      max,
        Clusivity clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this ulong value,
        ulong      min,
        ulong      max,
        Clusivity  clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this byte value,
        byte      min,
        byte      max,
        Clusivity clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    [Pure]
    public static bool IsBetween(
        this sbyte value,
        sbyte      min,
        sbyte      max,
        Clusivity  clusivity = Clusivity.Inclusive
    ) => clusivity switch {
        Clusivity.Inclusive => value >= min && value <= max,
        Clusivity.Exclusive => value > min  && value < max,
        _                   => throw BEnum.InvalidEnumArgumentException(nameof(clusivity), clusivity)
    };

    #region Tuple version

    [Pure] public static bool IsBetween(this short   value, (short min, short max)     range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this int     value, (int min, int max)         range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this long    value, (long min, long max)       range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this float   value, (float min, float max)     range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this double  value, (double min, double max)   range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this decimal value, (decimal min, decimal max) range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this ushort  value, (ushort min, ushort max)   range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this uint    value, (uint min, uint max)       range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this ulong   value, (ulong min, ulong max)     range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this byte    value, (byte min, byte max)       range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);
    [Pure] public static bool IsBetween(this sbyte   value, (sbyte min, sbyte max)     range, Clusivity clusivity = Clusivity.Inclusive) => value.IsBetween(range.min, range.max, clusivity);

    #endregion

    #endregion
}