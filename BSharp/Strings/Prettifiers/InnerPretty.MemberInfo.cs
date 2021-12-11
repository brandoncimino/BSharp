using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Collections;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string PrettifyMemberInfo(MemberInfo memberInfo, PrettificationSettings settings) {
            var typeName   = memberInfo.DeclaringType?.PrettifyType(settings).Suffix(".");
            var memberName = memberInfo.Name;
            return $"{typeName}{memberName}".WithTypeLabel(memberInfo.GetType(), settings);
        }

        public static string PrettifyMethodInfo(MethodInfo methodInfo, PrettificationSettings settings) {
            return $"{PrettifyMemberInfo(methodInfo, settings)}({PrettifyParameters(methodInfo, settings)})";
        }


        private static string PrettifyParameters(MethodBase methodInfo, PrettificationSettings settings) {
            return methodInfo.GetParameters()
                             .Select(it => PrettifyParameterInfo(it, settings))
                             .JoinString(", ");
        }


        public static string PrettifyParameterInfo(ParameterInfo parameterInfo, PrettificationSettings settings) {
            return WithTypeLabel(parameterInfo.Name, parameterInfo.ParameterType, settings);
        }
    }
}