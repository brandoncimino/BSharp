using System;
using System.Reflection;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Reflection;

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal partial class InnerPretty {
        private const string MethodIcon      = "Ⓜ";
        private const string LambdaIcon      = "λ";
        private const string ConstructorIcon = "🏗"; //alternatives include: 🏭, 👩‍🏭 (if that worked in this font)

        private static MethodStyle GetDelegateStyle(Delegate del) {
            return del switch {
                { } when del.IsCompilerGenerated() => MethodStyle.Lambda,
                { Method.IsConstructor: true }     => MethodStyle.Constructor,
                _                                  => MethodStyle.MethodReference
            };
        }

        public static string PrettifyDelegate(Delegate del, PrettificationSettings settings) {
            return GetDelegateStyle(del) switch {
                MethodStyle.Lambda          => _Prettify_Lambda_Simple(del, settings),
                MethodStyle.Constructor     => _Prettify_Constructor(del, settings),
                MethodStyle.MethodReference => _Prettify_MethodReference(del, settings),
                _                           => throw BEnum.InvalidEnumArgumentException(nameof(GetDelegateStyle), GetDelegateStyle(del))
            };
        }

        private static string _Prettify_MethodReference(Delegate methodReference, PrettificationSettings settings) {
            settings = settings with { TypeLabelStyle = TypeNameStyle.None };
            var methodInfo = PrettifyMethodInfo(methodReference.Method, settings);
            return $"{MethodIcon} {methodInfo}";
        }

        private static string _Prettify_Lambda(Delegate del, PrettificationSettings settings) {
            var typeStr = del.GetType().Prettify(settings);
            var nameStr = del.GetMethodInfo().Name;
            return $"{LambdaIcon} {typeStr} => {nameStr}";
        }

        private static string _Prettify_Constructor(Delegate del, PrettificationSettings settings) {
            var typeStr         = del.GetType().Prettify(settings);
            var constructedType = del.Method.ReturnType.Prettify();
            return $"{ConstructorIcon} {typeStr} => new {constructedType}";
        }

        private static string _Prettify_Lambda_Simple(Delegate del, PrettificationSettings settings) {
            var argString    = InnerPretty.PrettifyParameters(del.Method, settings);
            var resultString = del.Method.ReturnType;
            return $"{LambdaIcon} ({argString}) => {resultString}";
        }
    }
}