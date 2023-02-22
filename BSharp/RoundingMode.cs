using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// Specifies how <a href="https://en.wikipedia.org/wiki/Rounding">rounding</a> should be performed.
/// </summary>
public enum RoundingMode {
    /// <summary>
    /// Midpoint values (i.e. <c>.5</c>) will be rounded to the nearest <b>even</b> number <i>(with 0 considered even)</i>.
    /// </summary>
    /// <example>
    /// <code>
    ///  3.5  ↗  4
    ///  2.5  ↘  2
    ///  1.5  ↗  2
    ///  0.5  ↘  0
    ///  0    →  0
    /// -0.5  ↗  0
    /// -1.5  ↘ -2
    /// -2.5  ↗ -2
    /// </code>
    /// </example>
    /// <remarks>
    /// Corresponds to <see cref="MidpointRounding.ToEven"/> and is the default rounding mode used by <see cref="Math.Round(double)"/>, <see cref="Convert.ToInt32(double)"/>, etc.
    /// <p/>
    /// See also: <a href="https://en.wikipedia.org/wiki/Rounding#Rounding_half_to_even">Rounding half to even</a>
    /// </remarks>
    HalfEven = MidpointRounding.ToEven,
    /// <summary>
    /// Midpoint values (i.e. <c>.5</c>) will be rounded to the value with the largest <see cref="Math.Abs(decimal)">absolute value</see>.
    /// </summary>
    /// <example>
    /// <code>
    ///  1.5 ↗  2
    ///   .5 ↗  1
    ///  0   →  0
    /// - .5 ↘ -1
    /// -1.5 ↘ -2
    /// </code>
    /// </example>
    /// <remarks>
    /// Corresponds to <see cref="MidpointRounding.AwayFromZero"/>.
    /// <p/>
    /// See also: <a href="https://en.wikipedia.org/wiki/Rounding#Rounding_half_away_from_zero">Rounding half away from zero</a>
    /// </remarks>
    HalfAwayFromZero = MidpointRounding.AwayFromZero,
    /// <summary>
    /// The fractional portion of the value will be dropped, no matter what it is, bringing the value closer to 0.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    ///  1    →  1
    ///   .9  ↘  0
    ///   .5  ↘  0
    ///   .1  ↘  0
    ///    0  →  0
    /// - .1  ↗  0
    /// - .5  ↗  0
    /// - .9  ↗  0
    /// -1.5  ↗ -1
    /// -1.9  ↗ -1
    /// ]]></code>
    /// </example>
    /// <remarks>
    /// Equivalent to <see cref="Math.Truncate(decimal)"/>, <a href="https://learn.microsoft.com/en-us/dotnet/api/system.midpointrounding#system-midpointrounding-tozero">MidpointRounding.ToZero</a>, and <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.IFloatingPoint-1.Truncate">IFloatingPoint&lt;T&gt;.Truncate()</a>.
    /// <p/>
    /// This is also the same result as you would get by performing a hard-cast to an integral type, as in <c>(int)1.9</c>.
    /// <p/>
    /// See also: <a href="https://en.wikipedia.org/wiki/Rounding#Rounding_toward_zero">Rounding toward zero</a>, <a href="https://en.wikipedia.org/wiki/Truncation">truncation</a> 
    /// </remarks>
    Truncate = 2, // MidpointRounding.ToZero, // equivalent to IFloatingPoint<T>.Truncate()
    /// <summary>
    /// Values will be increased to the closest integer ≤ themselves.
    /// </summary>
    /// <remarks>
    /// Equivalent to <see cref="Math.Floor(decimal)"/>, <a href="https://learn.microsoft.com/en-us/dotnet/api/system.midpointrounding#system-midpointrounding-tonegativeinfinity">MidpointRounding.ToNegativeInfinity</a>, and <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.IFloatingPoint-1.Floor">IFloatingPoint&lt;TSelf&gt;.Floor()</a>.
    /// <p/>
    /// See also: <a href="https://en.wikipedia.org/wiki/Rounding#Rounding_down">Rounding down</a>, <a href="https://en.wikipedia.org/wiki/Floor_and_ceiling_functions">floor</a>
    /// </remarks>
    Floor = 3, // MidpointRounding.ToNegativeInfinity,
    /// <summary>
    /// Values will be increased to the closest integer ≥ themselves.
    /// </summary>
    /// <remarks>
    /// Equivalent to <see cref="Math.Ceiling(decimal)"/>, <a href="https://learn.microsoft.com/en-us/dotnet/api/system.midpointrounding#system-midpointrounding-topositiveinfinity">MidpointRounding.ToPositiveInfinity</a>, and <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.IFloatingPoint-1.Ceiling">IFloatingPoint&lt;TSelf&gt;.Ceiling()</a>.
    /// <p/>
    /// See also: <a href="https://en.wikipedia.org/wiki/Rounding#Rounding_up">Rounding up</a>, <a href="https://en.wikipedia.org/wiki/Floor_and_ceiling_functions">ceiling</a>
    /// </remarks>
    Ceiling = 4 // MidpointRounding.ToPositiveInfinity,
}

public static class RoundingModeExtensions {
    /// <summary>
    /// Converts a <see cref="MidpointRounding"/> to a <see cref="RoundingMode"/>.
    /// </summary>
    /// <param name="midpointRounding">a <permission cref="MidpointRounding"> value</permission></param>
    /// <returns>an equivalent <see cref="RoundingMode"/></returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RoundingMode ToRoundingMode(this MidpointRounding midpointRounding) => (RoundingMode)midpointRounding;

    private static readonly float[] Power10_Floats = {
        1e0f, 1e1f, 1e2f, 1e3f, 1e4f, 1e5f, 1e6f
    };

    private static readonly double[] Power10_Doubles = {
        1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8,
        1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15
    };

    private static readonly decimal[] Power10_Decimals = {
        1e0m, 1e1m, 1e2m, 1e3m, 1e4m, 1e5m, 1e6m, 1e7m, 1e8m, 1e9m,
        1e10m, 1e11m, 1e12m, 1e13m, 1e14m, 1e15m, 1e16m, 1e17m, 1e18m, 1e19m,
        1e20m, 1e21m, 1e22m, 1e23m, 1e24m, 1e25m, 1e26m, 1e27m, 1e28m
    };

    /// <summary>
    /// Applies a <see cref="RoundingMode"/> to a <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">floating-point number</a>.
    /// </summary>
    /// <param name="roundingMode">the <see cref="RoundingMode"/> to apply</param>
    /// <param name="value">a <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">floating-point number</a></param>
    /// <returns>an integral number close to this floating-point one</returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown <see cref="RoundingMode"/> is provided</exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this RoundingMode roundingMode, float value) {
        return roundingMode switch {
            RoundingMode.HalfEven         => MathF.Round(value),
            RoundingMode.HalfAwayFromZero => MathF.Round(value, MidpointRounding.AwayFromZero),
            RoundingMode.Truncate         => MathF.Truncate(value),
            RoundingMode.Floor            => MathF.Floor(value),
            RoundingMode.Ceiling          => MathF.Ceiling(value),
            _                             => throw BEnum.UnhandledSwitch(roundingMode)
        };
    }

    /// <inheritdoc cref="Round(FowlFever.BSharp.RoundingMode,float)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this RoundingMode roundingMode, double value) {
        return roundingMode switch {
            RoundingMode.HalfEven         => Math.Round(value),
            RoundingMode.HalfAwayFromZero => Math.Round(value, MidpointRounding.AwayFromZero),
            RoundingMode.Truncate         => Math.Truncate(value),
            RoundingMode.Floor            => Math.Floor(value),
            RoundingMode.Ceiling          => Math.Ceiling(value),
            _                             => throw BEnum.UnhandledSwitch(roundingMode)
        };
    }

    /// <inheritdoc cref="Round(FowlFever.BSharp.RoundingMode,float)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this RoundingMode roundingMode, decimal value) {
        return roundingMode switch {
            RoundingMode.HalfEven         => decimal.Round(value),
            RoundingMode.HalfAwayFromZero => decimal.Round(value, MidpointRounding.AwayFromZero),
            RoundingMode.Truncate         => decimal.Truncate(value),
            RoundingMode.Floor            => decimal.Floor(value),
            RoundingMode.Ceiling          => decimal.Ceiling(value),
            _                             => throw BEnum.UnhandledSwitch(roundingMode)
        };
    }

    /// <inheritdoc cref="Round(FowlFever.BSharp.RoundingMode,float)"/>
    /// <param name="digits">the number of decimal places to round to</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static float Round(this RoundingMode roundingMode, float value, int digits) {
        return roundingMode switch {
            RoundingMode.HalfEven         => MathF.Round(value, digits, MidpointRounding.ToEven),
            RoundingMode.HalfAwayFromZero => MathF.Round(value, digits, MidpointRounding.AwayFromZero),
            RoundingMode.Truncate         => MathF.Truncate(value * Power10_Floats[digits]) / Power10_Floats[digits],
            RoundingMode.Floor            => MathF.Floor(value    * Power10_Floats[digits]) / Power10_Floats[digits],
            RoundingMode.Ceiling          => MathF.Ceiling(value  * Power10_Floats[digits]) / Power10_Floats[digits],
            _                             => throw BEnum.UnhandledSwitch(roundingMode)
        };
    }

    /// <inheritdoc cref="Round(FowlFever.BSharp.RoundingMode,float,int)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this RoundingMode roundingMode, double value, int digits) {
        return roundingMode switch {
            RoundingMode.HalfEven         => Math.Round(value, digits, MidpointRounding.ToEven),
            RoundingMode.HalfAwayFromZero => Math.Round(value, digits, MidpointRounding.AwayFromZero),
            RoundingMode.Truncate         => Math.Truncate(value * Power10_Doubles[digits]) / Power10_Doubles[digits],
            RoundingMode.Floor            => Math.Floor(value    * Power10_Doubles[digits]) / Power10_Doubles[digits],
            RoundingMode.Ceiling          => Math.Ceiling(value  * Power10_Doubles[digits]) / Power10_Doubles[digits],
            _                             => throw BEnum.UnhandledSwitch(roundingMode)
        };
    }

    /// <inheritdoc cref="Round(FowlFever.BSharp.RoundingMode,float,int)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this RoundingMode roundingMode, decimal value, int digits) {
        return roundingMode switch {
            RoundingMode.HalfEven         => decimal.Round(value, digits, MidpointRounding.ToEven),
            RoundingMode.HalfAwayFromZero => decimal.Round(value, digits, MidpointRounding.AwayFromZero),
            RoundingMode.Truncate         => decimal.Truncate(value * Power10_Decimals[digits]) / Power10_Decimals[digits],
            RoundingMode.Floor            => decimal.Floor(value    * Power10_Decimals[digits]) / Power10_Decimals[digits],
            RoundingMode.Ceiling          => decimal.Ceiling(value  * Power10_Decimals[digits]) / Power10_Decimals[digits],
            _                             => throw BEnum.UnhandledSwitch(roundingMode)
        };
    }
}