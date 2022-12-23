using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Attributes;
using FowlFever.Implementors;

namespace FowlFever.BSharp;

/// <summary>
/// Extension methods for operating on basic <see cref="Type.IsPrimitive"/> types.
/// </summary>
public static partial class PrimitiveUtils {
    #region Truthiness

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

    /// <summary>
    /// Attempts to avoid:
    /// <ul>
    /// <li>Boxing <paramref name="self"/> as <see cref="object"/></li>
    /// <li>Needing to specify 2 type parameters (i.e. <c><![CDATA[(stringObject).As<object, string>()]]></c>)</li>
    /// </ul>
    ///
    /// The "optional" type parameter (<typeparamref name="T2"/>) can be specified by <c>default(T2)</c>, making the theoretical syntax:
    /// <code><![CDATA[
    /// object x = "abc";
    /// string s = x.As(default(string));
    /// ]]></code>
    /// </summary>
    /// <param name="self">this <typeparamref name="T"/> instance</param>
    /// <param name="_">any instance of <typeparamref name="T2"/>, such as <c>default(T2)</c></param>
    /// <typeparam name="T">the original <see cref="Type"/></typeparam>
    /// <typeparam name="T2">the destination <see cref="Type"/></typeparam>
    /// <returns><paramref name="self"/> as <typeparamref name="T2"/></returns>
    [Experimental]
    public static T2? As2<T, T2>(this T self, T2 _)
        where T2 : class {
        return self as T2;
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