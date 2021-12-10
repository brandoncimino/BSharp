using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Enums {
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
        public static T[] GetValues<T>() where T : struct, Enum {
            return GetValues<T>(typeof(T));
        }


        public static T[] GetValues<T>(Type enumType) where T : struct, Enum {
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
        ) where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, (int)(object)enumValue, typeof(T));
        }


        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            string? argumentName,
            T?      enumValue
        ) where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, -1, typeof(T));
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
                }.SelectValues(it => it.Prettify())
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
        ) where T : struct, Enum {
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
        ) where T : struct, Enum {
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

        public static T Min<T>() where T : struct, Enum {
            return GetValues<T>().Min();
        }

        public static T Max<T>() where T : struct, Enum {
            return GetValues<T>().Max();
        }

        #endregion
    }
}