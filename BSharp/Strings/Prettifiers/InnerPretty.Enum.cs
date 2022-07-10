using System;

using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings.Prettifiers {
    internal partial class InnerPretty {
        internal static string PrettifyEnum(Enum enm, PrettificationSettings settings) {
            settings = Prettification.ResolveSettings(settings);
            return enm.ToString().WithTypeLabel(enm.GetType(), settings, ".");
        }
    }
}