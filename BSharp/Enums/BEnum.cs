using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums {
    [PublicAPI]
    public static class BEnum {
        [ContractAnnotation("null => stop")]
        private static Type MustBeEnumType(this Type enumType) {
            return enumType?.IsEnum == true ? enumType : throw new ArgumentException($"{enumType.PrettifyType(default)} is not an enum type!");
        }

        [ContractAnnotation("null => stop")]
        private static Type MustMatchTypeArgument<T>(this Type enumType) {
            return enumType == typeof(T) ? enumType : throw new ArgumentException($"The {nameof(enumType)} {enumType.Prettify()} was not the same as the type argument <{nameof(T)}> {typeof(T).Prettify()}!");
        }

        /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
        /// <returns>an array containing the <see cref="Type.GetEnumValues"/> of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">if <typeparamref name="T"/> is not an <see cref="Enum"/> type</exception>
        public static T[] GetValues<T>()
            where T : Enum {
            return GetValues<T>(typeof(T));
        }

        public static T[] GetValues<T>(Type enumType)
            where T : Enum {
            return enumType.MustBeEnumType()
                           .MustMatchTypeArgument<T>()
                           .GetEnumValues()
                           .Cast<T>()
                           .ToArray();
        }

        /// <summary>
        /// Creates an <see cref="System.ComponentModel.InvalidEnumArgumentException"/> using generics to infer the enum's <see cref="Type"/>.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="enumValue"></param>
        /// <param name="allowedValues"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            string? argumentName,
            T       enumValue,
            [InstantHandle]
            IEnumerable<T>? allowedValues = default
        )
            where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, (int)(object)enumValue, typeof(T));
        }

        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            string? argumentName,
            T?      enumValue
        )
            where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, -1, typeof(T));
        }

        /// <summary>
        /// Composes a nice <see cref="System.ComponentModel.InvalidEnumArgumentException"/> specifically for use when a
        /// <c>switch</c> statement didn't have a branch to account for <paramref name="actualValue"/>.
        /// </summary>
        /// <param name="actualValue">the <typeparamref name="T"/> value that didn't have a switch branch</param>
        /// <param name="parameterName">the parameter that caused this exception</param>
        /// <param name="methodName">the method that caused this exception</param>
        /// <typeparam name="T">an <see cref="Enum"/> <see cref="Type"/></typeparam>
        /// <returns>a nice <see cref="System.ComponentModel.InvalidEnumArgumentException"/></returns>
        public static InvalidEnumArgumentException UnhandledSwitch<T>(
            T? actualValue,
            [CallerArgumentExpression("actualValue")]
            string? parameterName = default,
            [CallerMemberName]
            string? methodName = default
        )
            where T : Enum {
            var rejection = new RejectionException(
                actualValue,
                parameterName,
                methodName,
                $"{typeof(T)}.{actualValue.OrNullPlaceholder()} was not handled by any switch branch!"
            );

            return new InvalidEnumArgumentException(rejection.Message);
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

            var dic = new Dictionary<object, object>() {
                    ["Enum type"]      = enumType,
                    ["Parameter name"] = paramName,
                    ["Allowed values"] = allowedValues,
                    ["Checked values"] = checkedValues,
                    ["Bad values"]     = badValues
                }.SelectValues(it => Prettification.Prettify(it))
                 .WhereValues(it => it.IsNotBlank());

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

        #region Flags

        /// <param name="enumType">this <see cref="Type"/></param>
        /// <returns><c>true</c> if the <see cref="FlagsAttribute"/> <see cref="MemberInfo.IsDefined"/></returns>
        public static bool IsEnumFlags(this Type enumType) => enumType.IsDefined(typeof(FlagsAttribute));

        /// <summary>
        /// Iterates through the individual <see cref="Enum.HasFlag"/> values of an <see cref="Enum"/> value with the <see cref="FlagsAttribute"/>.
        /// </summary>
        /// <param name="flags">a value of an <see cref="Enum"/> that <see cref="IsEnumFlags"/></param>
        /// <typeparam name="T">the <see cref="IsEnumFlags"/> type</typeparam>
        /// <returns>each individual <see cref="Enum.HasFlag"/> value from <paramref name="flags"/></returns>
        /// <exception cref="RejectionException">if <typeparamref name="T"/> isn't <see cref="IsEnumFlags"/></exception>
        public static IEnumerable<T> EachFlag<T>(this T flags)
            where T : Enum {
            Must.Be(typeof(T), IsEnumFlags);
            return GetValues<T>()
                .Where(it => flags.HasFlag(it));
        }

        #endregion
    }
}