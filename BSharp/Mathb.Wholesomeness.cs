using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp;

public static partial class Mathb {
    #region IsWhole

    /// <remarks>Either passes through to the .NET 7+ method <a href="https://learn.microsoft.com/en-us/dotnet/api/system.double.isinteger?view=net-7.0#system-double-isinteger(system-double)">double.IsInteger(value)</a> or copies the logic from that method's implementation.</remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Logic is copied from the .NET 7 source code")]
    public static bool IsInteger(this double value) {
#if NET7_0_OR_GREATER
        return double.IsInteger(value);
#else
        return double.IsFinite(value) && (value == Math.Truncate(value));
#endif
    }

    /// <inheritdoc cref="IsInteger(double)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhole(this double value) => value.IsInteger();

    /// <remarks>Either passes through to the .NET 7+ method <a href="https://learn.microsoft.com/en-us/dotnet/api/system.single.isinteger?view=net-7.0#system-single-isinteger(system-single)">float.IsInteger(value)</a> or copies the logic from that method.</remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Logic is copied from the .NET 7 source code")]
    public static bool IsInteger(this float value) {
#if NET7_0_OR_GREATER
        return double.IsInteger(value);
#else
        return float.IsFinite(value) && (value == Math.Truncate(value));
#endif
    }

    /// <inheritdoc cref="IsInteger(float)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhole(this float value) => value.IsInteger();

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhole(this decimal value) => value % 1 == 0;

    #endregion

    #region MustBeWhole

    [Pure] public static double  MustBeWhole(this double  value) => value.IsWhole() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be a whole number!");
    [Pure] public static float   MustBeWhole(this float   value) => value.IsWhole() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be a whole number!");
    [Pure] public static decimal MustBeWhole(this decimal value) => value.IsWhole() ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be a whole number!");

    #endregion

    #region AsInt

    [Pure] public static int? AsInt(this double  value) => value.IsWhole() && value <= int.MaxValue ? value.ToInt() : default;
    [Pure] public static int? AsInt(this float   value) => value.IsWhole() && value <= int.MaxValue ? value.ToInt() : default;
    [Pure] public static int? AsInt(this decimal value) => value.IsWhole() && value <= int.MaxValue ? value.ToInt() : default;
    [Pure] public static int? AsInt(this uint    value) => value <= int.MaxValue ? value.ToInt() : default;
    [Pure] public static int? AsInt(this long    value) => value is >= int.MinValue and <= int.MaxValue ? value.ToInt() : default;
    [Pure] public static int? AsInt(this ulong   value) => value <= int.MaxValue ? value.ToInt() : default;

    #endregion

    #region MustBeInt

    [Pure] public static int MustBeInt(this double  value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");
    [Pure] public static int MustBeInt(this float   value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");
    [Pure] public static int MustBeInt(this decimal value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");
    [Pure] public static int MustBeInt(this uint    value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");
    [Pure] public static int MustBeInt(this long    value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");
    [Pure] public static int MustBeInt(this ulong   value) => value.AsInt() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int32)} values!");

    #endregion

    #region AsLong

    [Pure] public static long? AsLong(this double  value) => value.IsWhole() && value <= long.MaxValue ? value.ToLong() : default;
    [Pure] public static long? AsLong(this float   value) => value.IsWhole() && value <= long.MaxValue ? value.ToLong() : default;
    [Pure] public static long? AsLong(this decimal value) => value.IsWhole() && value <= long.MaxValue ? value.ToLong() : default;
    [Pure] public static long? AsLong(this ulong   value) => value <= long.MaxValue ? value.ToLong() : default;

    #endregion

    #region MustBeLong

    [Pure] public static long MustBeLong(this double  value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");
    [Pure] public static long MustBeLong(this float   value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");
    [Pure] public static long MustBeLong(this decimal value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");
    [Pure] public static long MustBeLong(this ulong   value) => value.AsLong() ?? throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be a whole number in the range of valid {nameof(Int64)} values!");

    #endregion
}