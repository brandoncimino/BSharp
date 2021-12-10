using System;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal partial class InnerPretty {
        internal static string PrettifyEnum(Enum enm, PrettificationSettings settings) {
            settings = Prettification.ResolveSettings(settings);
            return enm.ToString().WithTypeLabel(enm.GetType(), settings, ".");
        }
    }
}