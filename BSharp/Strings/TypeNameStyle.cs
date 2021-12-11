using System;
using System.Linq;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings.Json;
using FowlFever.BSharp.Strings.Prettifiers;

namespace FowlFever.BSharp.Strings {
    public enum TypeNameStyle {
        None  = 0,
        Short = 1,
        Full  = 2,
    }

    public static class TypeNameStyleExtensions {
        public static string GetTypeLabel(this Type? type, PrettificationSettings? settings) {
            settings = Prettification.ResolveSettings(settings);

            var style = type?.IsEnum == true ? settings.EnumLabelStyle : settings.TypeLabelStyle;

            settings?.TraceWriter.Verbose(() => $"Using style: {style} (type: {settings.TypeLabelStyle}/{settings.TypeLabelStyle.Value}, enum: {settings.EnumLabelStyle}/{settings.EnumLabelStyle.Value})");

            if (type == null || style == TypeNameStyle.None) {
                return "";
            }

            var str = type.PrettifyType(settings);

            if (type.IsArray || type.IsEnum || type.IsEnumerable()) {
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
}