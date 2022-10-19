using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;
using FowlFever.BSharp.Strings.Settings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums {
    [PublicAPI]
    public static partial class BEnum {
        /// <remarks>
        /// TODO: Replace the return with a more efficient, nullable-safe version of <see cref="EnumSet"/>, maybe
        /// TODO: Is it more efficient to use <see cref="IImmutableSet{T}"/> or <see cref="ImmutableArray{T}"/>? Probably <see cref="ImmutableArray{T}"/>, right? Since it's a <see cref="ValueType"/>?
        ///     It may be worth while to create a wrapper for <see cref="ImmutableArray{T}"/> that implements <see cref="IImmutableSet{T}"/>.
        /// TODO: It's <i>definitely</i> more efficient to use <see cref="ImmutableArray{T}"/>, because then we don't have to allocate a second collection (we can re-use the one from <see cref="Enum.GetValues"/>)
        ///     (*Actually I don't think we can do that)
        /// </remarks>
        private static class BenumCache<T>
            where T : Enum {
            private static readonly Type StructEnumType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            public static readonly ImmutableArray<T> AllValues = Enum.GetValues(StructEnumType)
                                                                     .Cast<T>()
                                                                     .ToImmutableArray();

            public static readonly ImmutableArray<T> UniqueValues = Enum.GetValues(StructEnumType)
                                                                        .Cast<T>()
                                                                        .Distinct()
                                                                        .ToImmutableArray();
        }

        /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
        /// <returns>an array containing the <see cref="Type.GetEnumValues"/> of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">if <typeparamref name="T"/> is not an <see cref="Enum"/> type</exception>
        /// <remarks>
        /// This is similar to the <a href="https://docs.microsoft.com/en-us/dotnet/api/system.enum.getvalues?view=net-6.0#system-enum-getvalues-1">.NET 5+ Enum.GetValues<![CDATA[<T>]]>()</a> method (<i><see cref="M:System.Enum.GetValues``1"/></i>),
        /// with some improvements:
        /// <ul>
        /// <li>It can be used in .NET Standard 2+</li>
        /// <li>The results are cached</li>
        /// <li>It doesn't require the <see cref="ValueType">T : struct</see> constraint <i>(which is redundant with the <see cref="System.Enum">T : Enum</see> constraint)</i></li>
        /// <li>The results are returned as an <see cref="IImmutableSet{T}"/> instead of an array</li> 
        /// </ul>
        /// </remarks>
        public static ImmutableArray<T> GetValues<T>() where T : Enum {
            return BenumCache<T>.UniqueValues;
        }

        public static ImmutableArray<T> GetAllValues<T>() where T : Enum {
            return BenumCache<T>.AllValues;
        }

        /// <summary>
        /// Creates an <see cref="InvalidEnumArgumentException{T}(T, string?)"/>, inferring the <paramref name="argumentName"/> using the <see cref="CallerArgumentExpressionAttribute"/>.
        /// </summary>
        /// <param name="enumValue">the bad <typeparamref name="T"/> value</param>
        /// <param name="argumentName">see <see cref="CallerArgumentExpressionAttribute"/></param>
        /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
        /// <returns>a new <see cref="InvalidEnumArgumentException"/></returns>
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            T enumValue,
            [CallerArgumentExpression("enumValue")]
            string? argumentName = default
        )
            where T : Enum {
            return InvalidEnumArgumentException(argumentName, enumValue);
        }

        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            T? enumValue,
            [CallerArgumentExpression("enumValue")]
            string? argumentName = default
        )
            where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, enumValue.HasValue ? (int)(object)enumValue : -1, typeof(T?));
        }

        /// <summary>
        /// Creates an <see cref="System.ComponentModel.InvalidEnumArgumentException"/> using generics to infer the enum's <see cref="Type"/>.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="enumValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>You should generally use <see cref="InvalidEnumArgumentException{T}(T, string?)"/> instead, which takes advantage of <see cref="CallerArgumentExpressionAttribute"/>.</remarks>
        /// <returns></returns>
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            string? argumentName,
            T       enumValue
        )
            where T : Enum? {
            return new InvalidEnumArgumentException(argumentName, -1, typeof(T));
        }

        /// <summary>
        /// Composes a nice <see cref="System.ComponentModel.InvalidEnumArgumentException"/> specifically for use when a
        /// <c>switch</c> statement didn't have a branch to account for <paramref name="actualValue"/>.
        /// </summary>
        /// <param name="actualValue">the <typeparamref name="T"/> value that didn't have a switch branch</param>
        /// <param name="details">optional additional details</param>
        /// <param name="parameterName">the parameter that caused this exception</param>
        /// <param name="rejectedBy">the method that caused this exception</param>
        /// <typeparam name="T">an <see cref="Enum"/> <see cref="Type"/></typeparam>
        /// <returns>a nice <see cref="System.ComponentModel.InvalidEnumArgumentException"/></returns>
        public static InvalidEnumArgumentException UnhandledSwitch<T>(
            T?      actualValue,
            string? details = default,
            [CallerArgumentExpression("actualValue")]
            string? parameterName = default,
            [CallerMemberName] string? rejectedBy = default
        )
            where T : Enum? {
            var rejection = new RejectionException(
                actualValue,
                details,
                parameterName,
                rejectedBy,
                $"{typeof(T)}.{actualValue.OrNullPlaceholder()} was not handled by any switch branch!"
            );

            return new InvalidEnumArgumentException(rejection.Message);
        }

        /// <inheritdoc cref="UnhandledSwitch{T}"/>
        public static InvalidEnumArgumentException UnhandledSwitch<T, T2>(
            (T, T2) actualCombo,
            string? details = default,
            [CallerArgumentExpression("actualCombo")]
            string? parameterName = default,
            [CallerMemberName] string? rejectedBy = default
        )
            where T : Enum
            where T2 : Enum {
            var rejection = new RejectionException(
                actualCombo,
                details,
                parameterName,
                rejectedBy,
                $"{actualCombo.PrettifyTuple(PrettificationSettings.Default)} was not handled by any switch branch!"
            );

            return new InvalidEnumArgumentException(rejection.Message);
        }

        /// <summary>
        /// Indicates that the given <typeparamref name="T"/> <paramref name="actualValue"/> is <b>intentionally</b> not supported.
        ///
        /// This is in contrast with <see cref="UnhandledSwitch{T}"/>, which implies that the <typeparamref name="T"/> value didn't exist at the time.
        /// </summary>
        /// <inheritdoc cref="UnhandledSwitch{T}"/>
        public static InvalidEnumArgumentException NotSupported<T>(
            T?      actualValue,
            string? details = default,
            [CallerArgumentExpression("actualValue")]
            string? parameterName = default,
            [CallerMemberName] string? rejectedBy = default
        )
            where T : Enum? {
            var rejection = new RejectionException(
                actualValue,
                details,
                parameterName,
                rejectedBy,
                $"{typeof(T)}.{actualValue.OrNullPlaceholder()} isn't supported for {rejectedBy}{details.IfNotBlank(it => $": {it}")}!"
            );

            return new InvalidEnumArgumentException(rejection.Message);
        }

        /// <summary>
        /// Throws a <see cref="NotSupported{T}"/> exception if <paramref name="actual"/> equals <see cref="unwanted"/>.
        /// </summary>
        /// <param name="actual">this <typeparamref name="T"/> value</param>
        /// <param name="unwanted">the bad <typeparamref name="T"/> value</param>
        /// <param name="details">optional additional details</param>
        /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
        /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
        /// <typeparam name="T">the type of <see cref="Enum"/></typeparam>
        /// <exception cref="InvalidEnumArgumentException">if <paramref name="actual"/> equals <see cref="unwanted"/></exception>
        /// <returns><paramref name="actual"/></returns>
        public static T Rejecting<T>(
            this T                                       actual,
            T                                            unwanted,
            string?                                      details       = default,
            [CallerArgumentExpression("actual")] string? parameterName = default,
            [CallerMemberName]                   string? rejectedBy    = default
        )
            where T : Enum {
            if (actual.Equals(unwanted)) {
                throw NotSupported(actual, details, parameterName, rejectedBy);
            }

            return actual;
        }

        #region Enum not in set

        private static string BuildEnumNotInSetMessage(
            string?             paramName,
            Type                enumType,
            IEnumerable<object> checkedValues,
            IEnumerable<object> allowedValues
        ) {
            var badValues = checkedValues.Except(allowedValues);

            var msg = $"{enumType.PrettifyType(default)} values {badValues.Prettify()} aren't among the allowed values!";

            var dic = new Dictionary<string, object?> {
                          ["Enum type"]      = enumType,
                          ["Parameter name"] = paramName,
                          ["Allowed values"] = allowedValues,
                          ["Checked values"] = checkedValues,
                          ["Bad values"]     = badValues
                      }
                      .Select(kvp => Kvp.Of(kvp.Key, kvp.Value.Prettify<object>()))
                      .Where(kvp => kvp.Value.IsNotBlank())
                      .ToDictionary();

            var prettyDic = dic.Prettify(HeaderStyle.None);

            return new object[] {
                msg,
                prettyDic.SplitLines().Indent(),
            }.JoinLines();
        }

        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            string?        argumentName,
            IEnumerable<T> checkedValues,
            IEnumerable<T> allowedValues
        )
            where T : struct, Enum {
            return new InvalidEnumArgumentException(
                BuildEnumNotInSetMessage(
                    argumentName,
                    typeof(T),
                    checkedValues.Cast<object>(),
                    allowedValues.Cast<object>()
                )
            );
        }

        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            string?         argumentName,
            IEnumerable<T?> checkedValues,
            IEnumerable<T?> allowedValues
        )
            where T : struct, Enum {
            return new InvalidEnumArgumentException(
                BuildEnumNotInSetMessage(
                    argumentName,
                    typeof(T?),
                    checkedValues.Cast<object>(),
                    allowedValues.Cast<object>()
                )
            );
        }

        #endregion

        #region Max / Min

        public static T Min<T>()
            where T : struct, Enum {
            return GetValues<T>().Min();
        }

        public static T Max<T>()
            where T : struct, Enum {
            return GetValues<T>().Max();
        }

        #endregion

        /// <param name="enumType">this <see cref="Type"/></param>
        /// <returns><c>true</c> if the <see cref="FlagsAttribute"/> <see cref="MemberInfo.IsDefined"/></returns>
        public static bool IsEnumFlags(this Type enumType) => enumType.IsDefined(typeof(FlagsAttribute));

        /// <summary>
        /// Iterates through the individual <see cref="Enum.HasFlag"/> values of an <see cref="Enum"/> value with the <see cref="FlagsAttribute"/>.
        /// <p/>
        /// Only includes values that <see cref="IsCanonFlag{T}"/>, meaning that "combination" values like <see cref="MemberTypes.All"/> will be excluded.
        /// </summary>
        /// <param name="flags">a value of an <see cref="Enum"/> that <see cref="IsEnumFlags"/></param>
        /// <typeparam name="T">the <see cref="IsEnumFlags"/> type</typeparam>
        /// <returns>each individual <see cref="Enum.HasFlag"/> value from <paramref name="flags"/></returns>
        /// <exception cref="RejectionException">if <typeparamref name="T"/> isn't <see cref="IsEnumFlags"/></exception>
        public static IEnumerable<T> EachFlag<T>(this T flags)
            where T : struct, Enum {
            return GetValues<T>()
                   .Where(IsCanonFlag)
                   .Where(it => flags.HasFlag(it));
        }

        /// <param name="flag">a <see cref="FlagsAttribute"/> <see cref="Enum"/> value</param>
        /// <typeparam name="T">the type of <see cref="Enum"/></typeparam>
        /// <returns><c>true</c> if the <paramref name="flag"/> refers to an explicitly defined, non-combination <see cref="FlagsAttribute"/></returns>
        public static bool IsCanonFlag<T>([NotNullWhen(true)] this T flag)
            where T : Enum? {
            return flag != null
                   && Convert.ToInt64(flag).IsPowerOf2()
                   && typeof(T).IsEnumDefined(flag);
        }

        /// <summary>
        /// Adds a flag to an <see cref="Enum"/> with the <see cref="FlagsAttribute"/>.
        /// </summary>
        /// <param name="flags">an <see cref="Enum"/> value with the <see cref="FlagsAttribute"/></param>
        /// <param name="additionalFlags">additional values to be combined with the existing <paramref name="flags"/></param>
        /// <typeparam name="T">the <see cref="Enum"/> type</typeparam>
        /// <returns>the combined flags</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithFlag<T>(this T flags, T additionalFlags)
            where T : struct, Enum => flags.SetFlag(additionalFlags, true);

        /// <param name="flags">an <see cref="Enum"/> value with the <see cref="FlagsAttribute"/></param>
        /// <param name="unwantedFlags">additional values to be <b>removed</b> from the existing <paramref name="flags"/></param>
        /// <typeparam name="T">the <see cref="Enum"/> type</typeparam>
        /// <returns>the remaining flags</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithoutFlag<T>(this T flags, T unwantedFlags)
            where T : struct, Enum => flags.SetFlag(unwantedFlags, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="flagToSet"></param>
        /// <param name="on"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// Taken from <a href="https://github.com/dotnet/runtime/issues/14084#issuecomment-803638941">github user mburbea</a>.
        /// <p/>
        /// <p/>
        /// I really, <i>really</i> don't like the way this code is written - it's written for computer architecture rather than logical programming.
        /// <p/>
        /// <b>It ends in an <c>else</c> statement</b>
        /// <p>
        /// The code should have an additional branch that fails if the "size" thingy isn't any of the values we expect.
        /// My guess is, not having it helps the <see cref="MethodImplAttributes.AggressiveInlining"/> option, since as <a href="https://github.com/dotnet/runtime/issues/14084#issuecomment-557362093">mikernet</a> stated, <i>"The conditional branches are eliminated by the JIT."</i>
        /// I imagine it would be less possible for the compiler to "eliminate the conditional branches" if the method has code both in and outside of the <c>if</c> statements.
        /// <br/>
        /// (📎 "JIT" means <a href="https://en.wikipedia.org/wiki/Just-in-time_compilation">just-in-time compilation</a>)  
        /// </p>
        /// <br/>
        /// <b>It repeats <see cref="Unsafe.SizeOf{T}"/> in each branch instead of evaluating it once and using a <c>switch</c> statement</b>
        /// <p>
        /// My best / only guess is that this also helps with <see cref="MethodImplAttributes.AggressiveInlining"/>.
        /// </p> 
        /// <br/>
        /// <b>It doesn't ensure that <typeparamref name="T"/> is a <see cref="FlagsAttribute"/> <see cref="Enum"/></b>
        /// <p>
        /// This is, theoretically, a limitation of the generic <see cref="Enum"/> type constraint.
        /// I could overcome this by validating the <see cref="IsEnumFlags"/> variable, but:
        /// <ul>
        /// <li>The entire purpose of these hideous methods is "efficiency", which an assertion would probably undermine</li>
        /// <li>The basic <see cref="Enum.HasFlag"/> method also doesn't check for the <see cref="FlagsAttribute"/></li>
        /// </ul>
        /// </p>
        /// <p/>
        /// Or, most likely, people who care about saving <see cref="Byte"/>s don't care about being good programmers.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T SetFlag<T>(this T flags, T flagToSet, bool on)
            where T : struct, Enum {
            if (Unsafe.SizeOf<T>() == 1) {
                var x = (byte)((Unsafe.As<T, byte>(ref flags)    & ~Unsafe.As<T, byte>(ref flagToSet))
                               | (-Unsafe.As<bool, byte>(ref on) & Unsafe.As<T, byte>(ref flagToSet)));
                return Unsafe.As<byte, T>(ref x);
            }

            if (Unsafe.SizeOf<T>() == 2) {
                var x = (short)((Unsafe.As<T, short>(ref flags)   & ~Unsafe.As<T, short>(ref flagToSet))
                                | (-Unsafe.As<bool, byte>(ref on) & Unsafe.As<T, short>(ref flagToSet)));
                return Unsafe.As<short, T>(ref x);
            }

            if (Unsafe.SizeOf<T>() == 4) {
                var x = (Unsafe.As<T, uint>(ref flags)          & ~Unsafe.As<T, uint>(ref flagToSet))
                        | ((uint)-Unsafe.As<bool, byte>(ref on) & Unsafe.As<T, uint>(ref flagToSet));
                return Unsafe.As<uint, T>(ref x);
            }
            else {
                var x = (Unsafe.As<T, ulong>(ref flags)                & ~Unsafe.As<T, ulong>(ref flagToSet))
                        | ((ulong)-(long)Unsafe.As<bool, byte>(ref on) & Unsafe.As<T, ulong>(ref flagToSet));
                return Unsafe.As<ulong, T>(ref x);
            }
        }
    }
}