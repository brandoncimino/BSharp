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
            //TODO: Remove this roundabout logic once we have a proper way to copy settings, such as using a fancy-schmancy new record
            var labelStyleModified = settings.TypeLabelStyle.HasValue == false;
            if (labelStyleModified) {
                settings.TypeLabelStyle.Set(TypeNameStyle.None);
            }

            var methodInfo = PrettifyMethodInfo(methodReference.Method, settings);

            if (labelStyleModified) {
                settings.TypeLabelStyle.Unset();
            }

            return $"{MethodIcon} {methodInfo}";
        }


        private static string _Prettify_Lambda(Delegate del, PrettificationSettings settings) {
            var typeStr = del.GetType().Prettify(settings);
            var nameStr = del.GetMethodInfo().Name;
            return $"{LambdaIcon} {typeStr} => {nameStr}";
        }
    }
}