using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp;

/// <summary>
/// Extension methods for operating on basic <see cref="Type.IsPrimitive"/> types.
/// </summary>
public static class PrimitiveUtils {
    #region Truthiness

    /// <summary>
    /// Extension method for <see cref="Convert.ToInt32(bool)"/>, which converts a <see cref="bool"/> to either 0 or 1.
    /// </summary>
    /// <param name="boolean">a <see cref="bool"/></param>
    /// <returns><c>0</c> if the <see cref="bool"/> is <c>false</c>; <c>1</c> if the <see cref="bool"/> is <c>true</c></returns>
    [Pure]
    public static int ToInt(this bool boolean) {
        return Convert.ToInt32(boolean);
    }

    #region Int -> Bool

    /// <summary>
    /// Extension method for <see cref="Convert.ToBoolean(int)"/>, i.e. <c>0 ? false : true</c>
    /// </summary>
    /// <param name="integer">an <see cref="int"/> value</param>
    /// <returns><see langword="false"/> if the <see cref="integer"/> is <c>0</c>; otherwise, <see langword="true"/></returns>
    [Pure]
    public static bool Truthy(this int integer) {
        return Convert.ToBoolean(integer);
    }

    /// <summary>
    /// Negation of <see cref="Convert.ToBoolean(bool)"/>, i.e. <c>0 ? true : false</c>
    /// </summary>
    /// <param name="integer"><inheritdoc cref="Truthy(int)"/></param>
    /// <returns><see langword="true"/> if the <see cref="integer"/> is <c>0</c>; otherwise, <see langword="false"/></returns>
    [Pure]
    public static bool Falsey(this int integer) {
        return !Convert.ToBoolean(integer);
    }

    /**
         * <inheritdoc cref="Truthy(int)"/>
         */
    [Pure]
    public static bool Boolean(this int integer) {
        return Convert.ToBoolean(integer);
    }

    #endregion

    #region String -> Bool

    /// <remarks>This matches the way that truthiness is evaluated in Powershell, where whitespace is considered <c>true</c>.</remarks>
    /// <returns><see cref="string.IsNullOrWhiteSpace"/></returns>
    [Pure]
    public static bool Truthy(this string str) {
        return string.IsNullOrEmpty(str);
    }

    /// <remarks><inheritdoc cref="Truthy(string)"/></remarks>
    /// <returns>negation of <see cref="string.IsNullOrWhiteSpace"/></returns>
    [Pure]
    public static bool Falsey(this string str) {
        return !string.IsNullOrEmpty(str);
    }

    #endregion

    #endregion

    #region Type-checking

    /// <summary>
    /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">Integral Numeric Types</a>
    /// </summary>
    private static readonly ImmutableHashSet<Type> IntegralTypes = ImmutableHashSet.Create(
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong)
    );

    /// <summary>
    /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">Floating-Point Numeric Types</a>
    /// </summary>
    private static readonly ImmutableHashSet<Type> FloatingPointTypes = ImmutableHashSet.Create(
        typeof(float),
        typeof(double),
        typeof(decimal)
    );

    /// <summary>
    /// Both <see cref="IntegralTypes"/> and <see cref="FloatingPointTypes"/>.
    /// </summary>
    public static readonly ImmutableHashSet<Type> NumericTypes = IntegralTypes.Union(FloatingPointTypes);

    /// <summary>
    /// Special <see cref="NumericTypes"/> that are automatically cast to <see cref="Int32"/> when used in <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators">arithmetic</a>.
    /// </summary>
    public static readonly ImmutableHashSet<Type> PseudoIntTypes = ImmutableHashSet.Create(
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort)
    );

    /// <summary>
    /// Returns whether or not the this <see cref="Type"/> is one of the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">integral numeric types</a> or <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">floating-point numeric types</a>.
    ///
    /// TODO: Add an optional parameter to allow more expansive checking against types like <see cref="IHas{T}"/>
    /// </summary>
    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>true if this <see cref="Type"/> is any of the <see cref="NumericTypes"/></returns>
    public static bool IsNumber(this Type type) {
        return NumericTypes.Contains(type);
    }

    #endregion

    #region Kind of a joke

    /// <summary>
    /// This is...kind of a joke
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static bool NOT(this bool value) {
        return !value;
    }

    #endregion

    #region Convert.To{x}

    public static short ToShort<T>(this T obj)
        where T : struct => Convert.ToInt16(obj);

    public static ushort ToUShort<T>(this T obj)
        where T : struct => Convert.ToUInt16(obj);

    public static int ToInt<T>(this T obj)
        where T : struct => Convert.ToInt32(obj);

    public static uint ToUInt<T>(this T obj)
        where T : struct => Convert.ToUInt32(obj);

    public static long ToLong<T>(this T obj)
        where T : struct => Convert.ToInt64(obj);

    public static ulong ToULong<T>(this T obj)
        where T : struct => Convert.ToUInt64(obj);

    public static float ToFloat<T>(this T obj)
        where T : struct => Convert.ToSingle(obj);

    public static double ToDouble<T>(this T obj)
        where T : struct => Convert.ToDouble(obj);

    public static decimal ToDecimal<T>(this T obj)
        where T : struct => Convert.ToDecimal(obj);

    public static byte ToByte<T>(this T obj)
        where T : struct => Convert.ToByte(obj);

    public static sbyte ToSByte<T>(this T obj)
        where T : struct => Convert.ToSByte(obj);

    public static bool ToBool<T>(this T obj)
        where T : struct => Convert.ToBoolean(obj);

    #endregion

    /// <summary>
    /// A nicer way to chain <c>x as y</c>.
    /// <p/>
    /// For a chain-friendly hard-cast, see <see cref="Must.MustBe{T}(object,string?,string?)"/>.
    /// </summary>
    public static T? As<T>(this object self)
        where T : class {
        return self as T;
    }

    /// <summary>
    /// Casts <paramref name="self"/> as an <see cref="object"/> in a way that works well with method-chaining.
    /// </summary>
    /// <param name="self">this <typeparamref name="T"/> instance</param>
    /// <typeparam name="T">the type of <paramref name="self"/></typeparam>
    /// <returns>this as an <see cref="object"/></returns>
    [return: NotNullIfNotNull("self")]
    public static object? AsObject<T>(this T? self) => self;

    #region Icons

    private const string TrueIcon  = "✅";
    private const string FalseIcon = "❌";

    /// <param name="value">a <see cref="bool"/></param>
    /// <returns>either <see cref="TrueIcon"/> or <see cref="FalseIcon"/></returns>
    public static string Icon(this bool value) {
        return value ? TrueIcon : FalseIcon;
    }

    #endregion

    #region Powers of Two

    public static bool IsPowerOf2(this short  value) => (value != 0) && ((value & (value - 1)) == 0);
    public static bool IsPowerOf2(this ushort value) => (value != 0) && ((value & (value - 1)) == 0);
    public static bool IsPowerOf2(this int    value) => (value != 0) && ((value & (value - 1)) == 0);
    public static bool IsPowerOf2(this uint   value) => (value != 0) && ((value & (value - 1)) == 0);
    public static bool IsPowerOf2(this long   value) => (value != 0) && ((value & (value - 1)) == 0);
    public static bool IsPowerOf2(this ulong  value) => (value != 0) && ((value & (value - 1)) == 0);

    #endregion
}