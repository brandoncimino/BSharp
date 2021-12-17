using System;
using System.Reflection;

using FowlFever.BSharp.Reflection;

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal partial class InnerPretty {
        private const string MethodIcon = "Ⓜ";
        private const string LambdaIcon = "λ";


        public static string PrettifyDelegate(Delegate del, PrettificationSettings settings) {
            return del.IsCompilerGenerated() ? _Prettify_Lambda(del, settings) : _Prettify_MethodReference(del, settings);
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
    }
}