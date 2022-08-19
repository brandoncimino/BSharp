using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums;

/// <summary>
/// Represents <a href="https://en.wikipedia.org/wiki/Interval_(mathematics)#Including_or_excluding_endpoints">inclusion or exclusion</a>, aka <a href="https://en.wikipedia.org/wiki/Clusivity">clusivity</a>.
/// </summary>
[PublicAPI]
public enum Clusivity : byte { Inclusive, Exclusive, }

[PublicAPI]
public static class ClusivityExtensions {
    [Pure]
    public static char MaxSymbol(this Clusivity clusivity) {
        return clusivity switch {
            Clusivity.Inclusive => ']',
            Clusivity.Exclusive => ')',
            _                   => throw BEnum.UnhandledSwitch(clusivity)
        };
    }

    [Pure]
    public static char MinSymbol(this Clusivity clusivity) {
        return clusivity switch {
            Clusivity.Inclusive => '[',
            Clusivity.Exclusive => '(',
            _                   => throw BEnum.UnhandledSwitch(clusivity),
        };
    }

    [Pure]
    public static string FormatRange<T, T2>(T min, Clusivity minClusivity, T2 max, Clusivity maxClusivity) {
        return $"{minClusivity.MinSymbol()}{min}, {max}{maxClusivity.MaxSymbol()}";
    }
}