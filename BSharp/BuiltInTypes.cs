using System;
using System.Reflection;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp;

/// <summary>
/// Enumerates the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in types</a>, like <see cref="int"/> and <see cref="object"/>. 
/// </summary>
public enum BuiltInType {
    Byte,
    SByte,
    Short,
    UShort,
    Int,
    UInt,
    Long,
    ULong,
    Float,
    Double,
    Decimal,
    Boolean,
    Object,
    String,
    Void,
    Dynamic
}

/// <summary>
/// Methods for retrieving <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in types</a> and their keywords.
/// </summary>
/// <remarks>
/// 📎 Not all <see cref="BuiltInType"/> are <see cref="System.Type.IsPrimitive"/>! The <see cref="System.Type.IsPrimitive"/> types are the numeric types with the exception of <see cref="decimal"/>.
/// </remarks>
public static class BuiltInTypes {
    /// <summary>
    /// Gets the <see cref="System.Type"/> that corresponds to this <see cref="BuiltInType"/>.
    /// </summary>
    /// <param name="builtInType">this <see cref="BuiltInType"/></param>
    /// <returns>the corresponding <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a></returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unknown <see cref="BuiltInType"/> is provided</exception>
    public static Type GetRuntimeType(this BuiltInType builtInType) {
        return builtInType switch {
            BuiltInType.Byte    => typeof(byte),
            BuiltInType.SByte   => typeof(sbyte),
            BuiltInType.Short   => typeof(short),
            BuiltInType.UShort  => typeof(ushort),
            BuiltInType.Int     => typeof(int),
            BuiltInType.UInt    => typeof(uint),
            BuiltInType.Long    => typeof(long),
            BuiltInType.ULong   => typeof(ulong),
            BuiltInType.Float   => typeof(float),
            BuiltInType.Double  => typeof(double),
            BuiltInType.Decimal => typeof(decimal),
            BuiltInType.Boolean => typeof(bool),
            BuiltInType.Object  => typeof(object),
            BuiltInType.String  => typeof(string),
            BuiltInType.Void    => typeof(void),
            BuiltInType.Dynamic => typeof(object),
            _                   => throw BEnum.InvalidEnumArgumentException(builtInType),
        };
    }

    /// <param name="builtInType">this <see cref="BuiltInType"/></param>
    /// <returns>the keyword that corresponds to this <see cref="BuiltInType"/></returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">if an unknown <see cref="BuiltInType"/> is provided</exception>
    public static string GetKeyword(this BuiltInType builtInType) {
        return builtInType switch {
            BuiltInType.Byte    => "byte",
            BuiltInType.SByte   => "sbyte",
            BuiltInType.Short   => "short",
            BuiltInType.UShort  => "ushort",
            BuiltInType.Int     => "int",
            BuiltInType.UInt    => "uint",
            BuiltInType.Long    => "long",
            BuiltInType.ULong   => "ulong",
            BuiltInType.Float   => "float",
            BuiltInType.Double  => "double",
            BuiltInType.Decimal => "decimal",
            BuiltInType.Boolean => "bool",
            BuiltInType.Object  => "object",
            BuiltInType.String  => "string",
            BuiltInType.Void    => "void",
            BuiltInType.Dynamic => "dynamic",
            _                   => throw BEnum.InvalidEnumArgumentException(builtInType),
        };
    }

    #region Keywords

    /// <summary>
    /// If this <see cref="Type"/> is a <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a>, return the corresponding <see cref="BuiltInType"/> enum value.
    /// Otherwise, returns <c>null</c>.
    /// </summary>
    /// <param name="type">this <see cref="Type"/></param>
    /// <returns>the corresponding <see cref="BuiltInType"/>, if found; otherwise, <c>null</c></returns>
    public static BuiltInType? AsBuiltInType(Type type) {
        if (type == typeof(byte)) {
            return BuiltInType.Byte;
        }
        else if (type == typeof(sbyte)) {
            return BuiltInType.SByte;
        }
        else if (type == typeof(short)) {
            return BuiltInType.Short;
        }
        else if (type == typeof(ushort)) {
            return BuiltInType.UShort;
        }
        else if (type == typeof(int)) {
            return BuiltInType.Int;
        }
        else if (type == typeof(uint)) {
            return BuiltInType.UInt;
        }
        else if (type == typeof(long)) {
            return BuiltInType.Long;
        }
        else if (type == typeof(ulong)) {
            return BuiltInType.ULong;
        }
        else if (type == typeof(float)) {
            return BuiltInType.Float;
        }
        else if (type == typeof(double)) {
            return BuiltInType.Double;
        }
        else if (type == typeof(decimal)) {
            return BuiltInType.Decimal;
        }
        else if (type == typeof(bool)) {
            return BuiltInType.Boolean;
        }
        else if (type == typeof(object)) {
            return BuiltInType.Object;
        }
        else if (type == typeof(string)) {
            return BuiltInType.String;
        }
        else if (type == typeof(void)) {
            return BuiltInType.Void;
        }
        else if (type == typeof(object)) {
            return BuiltInType.Object;
        }
        else {
            return default;
        }
    }

    /// <summary>
    /// Retrieves the keyword that corresponds to a <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a>.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>✔ <see cref="Void"/> evaluates to <c>"void"</c>.</li>
    /// <li>❌ <see cref="Nullable{T}"/> types do <b>NOT</b> return a keyword.</li>
    /// <li>❌ <see cref="Enum"/> and <see cref="Type.IsEnum"/> types do <b>NOT</b> return a keyword.</li>
    /// </ul>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// var type  = 5.GetType();                            // => Int32
    /// var found = TryGetKeyword(type, out var keyword);   // => true
    /// Console.WriteLine(keyword);                         // => "int"
    /// ]]></code>
    /// </example>
    /// <param name="type">the <see cref="Type"/> that might have a keyword</param>
    /// <returns>the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a> keyword, if found; otherwise, <c>null</c></returns>
    [Pure]
    public static string? GetKeyword(this Type type) => AsBuiltInType(type)?.GetKeyword();

    /// <summary>
    /// Retrieves the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a> that corresponds to the given keyword <i>(<c>int</c>)</i>, <see cref="MemberInfo.Name"/> <i>(<c>Int32</c>)</i>, or <see cref="Type.FullName"/> <i>(<c>System.Int32</c>)</i>.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>✔ <c>"true"</c> and <c>"false"</c> evaluate to <see cref="bool"/></li>
    /// <li>✔ <c>"void"</c> evaluates to <see cref="System.Void"/>, though the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in types</a> reference specifically excludes it.</li>
    /// <li>✔ <c>"dynamic"</c> evaluates to <see cref="object"/> in accordance with the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in types</a> reference.</li>
    /// <li>❌ <see cref="Nullable{T}"/> types are <b>NOT</b> supported <i>(i.e. <c>"int?"</c> will return <c>null</c>)</i></li>
    /// <li>❌ <c>"enum"</c> will <b>NOT</b> return <see cref="Enum"/>.</li> 
    /// </ul>
    /// </remarks>
    /// <example>
    /// <list>
    /// <listheader><term><paramref name="nameOrKeyword"/></term><description><see cref="GetRuntimeType"/></description></listheader>
    /// <item><term><c>"ushort"</c></term><description><see cref="ushort"/></description></item>
    /// <item><term><c>"Decimal"</c></term><description><see cref="Decimal"/></description></item>
    /// <item><term><c>"System.Char"</c></term><description><see cref="char"/></description></item>
    /// <item><term><c>"true"</c></term><description><see cref="bool"/></description></item>
    /// <item><term><c>"enum"</c></term><description><c>null</c></description></item>
    /// <item><term><c>"int?"</c></term><description><c>null</c></description></item>
    /// </list>
    /// </example>
    /// <param name="nameOrKeyword">a <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a> keyword, <see cref="MemberInfo.Name"/>, or <see cref="Type.FullName"/>.
    /// </param>
    /// <returns>the corresponding <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types">built-in type</a>, if found; otherwise, <c>null</c></returns>
    [Pure]
    public static Type? GetBuiltInType(string nameOrKeyword) {
        return nameOrKeyword switch {
            "bool" or "true" or "false" or nameof(Boolean) or $"System.{nameof(Boolean)}" => typeof(bool),
            "byte" or nameof(Byte) or $"System.{nameof(Byte)}"                            => typeof(byte),
            "sbyte" or nameof(SByte) or $"System.{nameof(SByte)}"                         => typeof(sbyte),
            "short" or nameof(Int16) or $"System.{nameof(Int16)}"                         => typeof(short),
            "ushort" or nameof(UInt16) or $"System.{nameof(UInt16)}"                      => typeof(ushort),
            "int" or nameof(Int32) or $"System.{nameof(Int32)}"                           => typeof(int),
            "uint" or nameof(UInt32) or $"System.{nameof(UInt32)}"                        => typeof(uint),
            "long" or nameof(Int64) or $"System.{nameof(Int64)}"                          => typeof(long),
            "ulong" or nameof(UInt64) or $"System.{nameof(UInt64)}"                       => typeof(ulong),
            "float" or nameof(Single) or $"System.{nameof(Single)}"                       => typeof(float),
            "double" or nameof(Double) or $"System.{nameof(Double)}"                      => typeof(double),
            "decimal" or nameof(Decimal) or $"System.{nameof(Decimal)}"                   => typeof(decimal),
            "char" or nameof(Char) or $"System.{nameof(Char)}"                            => typeof(char),
            "string" or nameof(String) or $"System.{nameof(String)}"                      => typeof(string),
            "object" or nameof(Object) or $"System.{nameof(Object)}"                      => typeof(object),
            "void" or "Void" or "System.Void"                                             => typeof(void),
            "dynamic"                                                                     => typeof(object),
            _                                                                             => default,
        };
    }

    #endregion
}