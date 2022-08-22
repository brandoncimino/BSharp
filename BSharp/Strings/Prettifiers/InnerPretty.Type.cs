using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Settings;

[assembly: InternalsVisibleTo("BrandonUtils.BSharp.Strings")]

namespace FowlFever.BSharp.Strings.Prettifiers;

internal static partial class InnerPretty {
    /// <summary>
    /// Prettifies a type, which may have different rules depending on whether it's a <see cref="ValueTuple{T1}"/>, <see cref="Type.IsGenericType"/>, etc.
    /// </summary>
    /// <param name="type">the <see cref="Type"/> to make pretty</param>
    /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
    /// <returns>a pretty <see cref="string"/></returns>
    [Pure]
    public static string PrettifyType(this Type? type, PrettificationSettings? settings) {
        settings = settings.Resolve();

        if (type == null) {
            return settings.NullPlaceholder;
        }

        if (type.IsTupleType()) {
            return PrettifyTupleType(type, settings);
        }

        // if the type is generic, we need to trim the `n and replace it with the generic type arguments
        return type.IsGenericType ? PrettifyGenericType(type, settings) : type.NameOrKeyword();

        #region Local methods

        static string PrettifyGenericType(Type? genericType, PrettificationSettings settings) {
            if (genericType?.IsGenericType != true) {
                throw new ArgumentException($"{genericType} is not a generic type!", nameof(genericType));
            }

            // Make sure to use `.GetGenericArguments()` and not `.GenericTypeArguments`, because the latter will return an empty array for
            // a generic type definition like `List<>`
            var genArgs = genericType.GetGenericArguments();

            // Perform a special replacement on the Nullable<> type
            if (genericType.IsNullableValueType()) {
                return genArgs.Select(it => PrettifyNullableType(it, settings)).JoinString(", ");
            }

            return genericType.Name.Replace($"`{genArgs.Length}", PrettifyGenericTypeArguments(genArgs, settings));
        }

        static string PrettifyTupleType(Type tupleType, PrettificationSettings settings) {
            var genArgs = tupleType.GetGenericArguments().Select(it => it.PrettifyType(settings));
            return $"({genArgs.JoinString(", ")})";
        }

        static string PrettifyNullableType(Type type, PrettificationSettings settings) {
            return $"{type.PrettifyType(settings)}?";
        }

        static string PrettifyGenericTypeArguments(IEnumerable<Type> genericTypeArguments, PrettificationSettings settings) {
            var stylizedArgs = StylizeGenericTypeArguments(genericTypeArguments, settings);
            return $"<{stylizedArgs}>";
        }

        static string StylizeGenericTypeArguments(IEnumerable<Type?> genericTypeArguments, PrettificationSettings settings) {
            return settings.TypeLabelStyle switch {
                TypeNameStyle.None  => "",
                TypeNameStyle.Full  => genericTypeArguments.Select(it => it.PrettifyType(settings)).JoinString(", "),
                TypeNameStyle.Short => genericTypeArguments.Select(_ => "").JoinString(","),
                _                   => throw BEnum.InvalidEnumArgumentException(nameof(settings.TypeLabelStyle), settings.TypeLabelStyle)
            };
        }

        #endregion
    }

    [Pure] public static ReadOnlySpan<char> PrettifyType<T>(this T               obj,  PrettificationSettings? settings = default) => (obj?.GetType() ?? typeof(T)).PrettifyType(settings);
    [Pure] public static ReadOnlySpan<char> PrettifyType<T>(this Span<T>         span, PrettificationSettings? settings = default) => span.SpanType().PrettifyType();
    [Pure] public static ReadOnlySpan<char> PrettifyType<T>(this ReadOnlySpan<T> span, PrettificationSettings? settings = default) => span.SpanType().PrettifyType();

    [Experimental]
    public static string PrettifyNullable(this NullabilityInfo info, PrettificationSettings? settings = default) {
        settings = settings.Resolve();

        var bookend = info.GetType().IsTupleType() ? Bookend.Parentheses : Bookend.Diamonds;

        var sb       = new StringBuilder();
        var baseName = info.Type.Name.AsSpan().BeforeLast('`');
        sb.Append(baseName);
        sb.AppendJoin(
            info.GenericTypeArguments.Select(it => it.PrettifyNullable(settings)),
            ", ",
            bookend.Prefix().ToString(),
            bookend.Suffix().ToString()
        );

        if (info.ReadState == NullabilityState.Nullable || info.WriteState == NullabilityState.Nullable) {
            sb.Append('?');
        }

        return sb.ToString();
    }

    private static string WithTypeLabel(
        this string?           thing,
        Type                   labelType,
        PrettificationSettings settings,
        string                 joiner = " "
    ) {
        return new[] { labelType.GetTypeLabel(settings), thing }.NonEmpty().JoinString(joiner);
    }
}