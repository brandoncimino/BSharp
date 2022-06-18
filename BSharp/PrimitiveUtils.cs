using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Exceptions;

using JetBrains.Annotations;

namespace FowlFever.BSharp {
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
        private static readonly IReadOnlyCollection<Type> IntegralTypes = new HashSet<Type>() {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
        };

        /// <summary>
        /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">Floating-Point Numeric Types</a>
        /// </summary>
        private static readonly IReadOnlyCollection<Type> FloatingPointTypes = new HashSet<Type>() {
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        /// <summary>
        /// Both <see cref="IntegralTypes"/> and <see cref="FloatingPointTypes"/>.
        /// </summary>
        public static readonly IReadOnlyCollection<Type> NumericTypes = new HashSet<Type>(IntegralTypes.Union(FloatingPointTypes));

        /// <summary>
        /// Special <see cref="NumericTypes"/> that are automatically cast to <see cref="Int32"/> when used in <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators">arithmetic</a>.
        /// </summary>
        public static readonly IReadOnlyCollection<Type> PseudoIntTypes = new HashSet<Type>() {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort)
        };

        /// <summary>
        /// Returns whether or not the given <see cref="value"/> is one of the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">integral numeric types</a> or <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">floating-point numeric types</a>.
        /// </summary>
        /// <param name="value">some random junk</param>
        /// <returns>true if the value is assignable to any of the <see cref="NumericTypes"/></returns>
        [ContractAnnotation("null => stop")]
        public static bool IsNumber(this object value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            return NumericTypes.Contains(value.GetType());
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

        #region Icons

        private const string TrueIcon  = "✅";
        private const string FalseIcon = "❌";

        /// <param name="value">a <see cref="bool"/></param>
        /// <returns>either <see cref="TrueIcon"/> or <see cref="FalseIcon"/></returns>
        public static string Icon(this bool value) {
            return value ? TrueIcon : FalseIcon;
        }

        #endregion
    }
}