namespace FowlFever.BSharp.Strings.Settings;

public static class SettingsExtensions {
    public static PrettificationSettings Resolve(this PrettificationSettings? settings) => settings ?? PrettificationSettings.Default;
    public static FillerSettings         Resolve(this FillerSettings?         settings) => settings ?? FillerSettings.Default;
}