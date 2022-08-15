using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Json;
using FowlFever.BSharp.Strings.Prettifiers;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings;

public enum TypeNameStyle {
    None, Short, Full,
}

public static class TypeNameStyleExtensions {
    /// <summary>
    /// Returns the <see cref="InnerPretty.PrettifyType"/> form of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="obj">an arbitrary <typeparamref name="T"/> instance</param>
    /// <param name="style"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Pure]
    [SuppressMessage("ReSharper", "EntityNameCapturedOnly.Global", Justification = "Flagrantly untrue")]
    [Experimental]
    public static string TypeName<T>(this T obj, TypeNameStyle style = TypeNameStyle.Full) {
        style.Rejecting(TypeNameStyle.None);
        return MethodBase.GetCurrentMethod()!.GetNullability(nameof(obj)).PrettifyNullable();
    }

    public static string GetTypeLabel(this Type? type, PrettificationSettings? settings) {
        settings = settings.Resolve();

        var style = type?.IsEnum == true ? settings.EnumLabelStyle : settings.TypeLabelStyle;

        settings?.TraceWriter.Verbose(() => $"Using style: {style} (type: {settings.TypeLabelStyle}, enum: {settings.EnumLabelStyle})");

        if (type == null || style == TypeNameStyle.None) {
            return "";
        }

        var str = type.PrettifyType(settings);

        if (type.IsArray || type.IsEnum || (type != typeof(string) && type.IsEnumerable())) {
            return str;
        }

        return $"[{str}]";
    }

    public static TypeNameStyle Reduce(this TypeNameStyle style, int steps = 1) {
        var newStep = (int)style - steps;
        newStep = newStep.Clamp(0, BEnum.GetValues<TypeNameStyle>().Cast<int>().Max());
        return (TypeNameStyle)newStep;
    }
}