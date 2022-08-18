using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums;

/// <summary>
/// Represents <a href="https://en.wikipedia.org/wiki/Interval_(mathematics)#Including_or_excluding_endpoints">inclusion or exclusion</a>, aka <a href="https://en.wikipedia.org/wiki/Clusivity">clusivity</a>.
/// </summary>
[PublicAPI]
public enum Clusivity { Inclusive, Exclusive, }