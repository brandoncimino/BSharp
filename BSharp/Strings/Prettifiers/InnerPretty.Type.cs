using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Reflection;

using JetBrains.Annotations;

[assembly: InternalsVisibleTo("BrandonUtils.BSharp.Strings")]

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal static partial class InnerPretty {
        /// <summary>
        /// Prettifies a type, which may have different rules depending on whether it's a <see cref="ValueTuple{T1}"/>, <see cref="Type.IsGenericType"/>, etc.
        /// </summary>
        /// <param name="type">the <see cref="Type"/> to make pretty</param>
        /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
        /// <returns>a pretty <see cref="string"/></returns>
        [Pure]
        public static string PrettifyType(this Type? type, PrettificationSettings? settings) {
            settings ??= Prettification.DefaultPrettificationSettings;

            if (type == null) {
                return settings.NullPlaceholder;
            }

            if (type.IsTupleType()) {
                return PrettifyTupleType(type, settings);
            }

            // if the type is generic, we need to trim the `n and replace it with the generic type arguments
            return type.IsGenericType ? PrettifyGenericType(type, settings) : type.NameOrKeyword();
        }


        private static string PrettifyGenericType(Type? genericType, PrettificationSettings? settings) {
            if (genericType?.IsGenericType != true) {
                throw new ArgumentException($"{genericType} is not a generic type!", nameof(genericType));
            }

            // Make sure to use `.GetGenericArguments()` and not `.GenericTypeArguments`, because the latter will return an empty array for
            // a generic type definition like `List<>`
            var genArgs = genericType.GetGenericArguments();
            return genericType.Name.Replace($"`{genArgs.Length}", PrettifyGenericTypeArguments(genArgs, settings));
        }


        private static string PrettifyTupleType(Type tupleType, PrettificationSettings? settings) {
            var genArgs = tupleType.GetGenericArguments().Select(it => it.PrettifyType(settings));
            return $"({genArgs.JoinString(", ")})";
        }


        private static string PrettifyGenericTypeArguments(IEnumerable<Type> genericTypeArguments, PrettificationSettings? settings) {
            var stylizedArgs = StylizeGenericTypeArguments(genericTypeArguments, settings);
            return $"<{stylizedArgs}>";
        }


        private static string StylizeGenericTypeArguments(IEnumerable<Type?> genericTypeArguments, PrettificationSettings? settings) {
            settings ??= Prettification.DefaultPrettificationSettings;
            return settings.TypeLabelStyle.Value switch {
                TypeNameStyle.None  => "",
                TypeNameStyle.Full  => genericTypeArguments.Select(it => it.PrettifyType(settings)).JoinString(", "),
                TypeNameStyle.Short => genericTypeArguments.Select(_ => "").JoinString(","),
                _                   => throw BEnum.InvalidEnumArgumentException(nameof(settings.TypeLabelStyle.Value), settings.TypeLabelStyle.Value)
            };
        }


        internal static string WithTypeLabel(this string? thing, Type labelType, PrettificationSettings settings, string joiner = "") {
            return new[] { labelType.GetTypeLabel(settings), thing }.NonNull().JoinString(joiner);
        }
    }
}